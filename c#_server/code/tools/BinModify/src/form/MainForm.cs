using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{
    public partial class MainForm : Form
    {
        private QueryInfo m_query_info = null;
        private List<BinHeader> m_list_bin_header = new List<BinHeader>();
        private List<List<BinContent>> m_list_bin_contentes = new List<List<BinContent>>();

        public MainForm()
        {
            InitializeComponent();
            this.Init();
        }

        private void Init()
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            m_bin_type.SelectedItem = "blob";
            m_sql_type.SelectedItem = "数字";

            //db列表
            foreach (var db_info in ServerConfig.info.db_list)
            {
                m_db_list.Items.Add(db_info.name);
            }

            //字段配置列表
            foreach(var field_info in ServerConfig.info.field_list)
            {
                ///先验证数据的有效性
                bool is_valid = false;
                do
                {
                    string str_fields = field_info.fields;
                    if (string.IsNullOrEmpty(str_fields)) break;

                    string[] arr_fields = str_fields.Split(',');
                    if (arr_fields == null || arr_fields.Length == 0) break;

                    int i = 0;
                    for (; i < arr_fields.Length; ++i)
                    {
                        string str_field = arr_fields[i];
                        if (string.IsNullOrEmpty(str_field)) break;

                        string[] arr_field = str_field.Split('|');
                        if (arr_field == null || arr_field.Length == 0 || arr_field.Length >= 3) break;

                        if (arr_field.Length == 2)
                        {
                            string str_type = arr_field[0];
                            string str_num = arr_field[1];
                            if (string.IsNullOrEmpty(str_num) || !StringUtils.IsInteger(str_num)) break;
                            if (str_type != "int" && str_type != "uint") break;//只有int才带长度
                        }
                        else
                        {
                            string str_type = arr_field[0];
                            if (string.IsNullOrEmpty(str_type) || (str_type != "int" && str_type != "uint" && str_type != "float" && str_type != "double" && str_type != "string")) break;
                        }
                    }
                    if(i == arr_fields.Length)
                    {
                        is_valid = true;
                    }
                } while (false);
                if (!is_valid)
                {
                    ShowErrorMsg("确保配置字段有效,有效关键字[int,uint,float,double,string]:" + field_info.name);
                    continue;
                }

                ///再添加
                m_field_config_list.Items.Add(field_info.name);
            }
        }
        #region 选择字段
        private ServerNetInfo.DBItems GetSelectDB()
        {
            int select_indx = m_db_list.SelectedIndex;
            if (select_indx < 0 || select_indx >= ServerConfig.info.db_list.Count) return null;

            ServerNetInfo.DBItems db_info = ServerConfig.info.db_list[select_indx];
            return db_info;
        }

        private string GetSelectTable()
        {
            string table_name = m_table_name.SelectedItem.ToString();
            return table_name;
        }

        private string GetSelectField()
        {
            string field_name = m_field_name.SelectedItem.ToString();
            return field_name;
        }

        /// <summary>
        /// 数据库改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DBSelectedIndexChanged(object sender, EventArgs e)
        {
            ServerNetInfo.DBItems db_info = this.GetSelectDB();
            if (db_info == null) return;

            System.Data.DataTable dt = DatabaseManager.Instance.GetTables((eDBType)db_info.id);
            if (dt == null) return;
            //--列出表名  
            m_table_name.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                m_table_name.Items.Add(dt.Rows[i][2].ToString());
            }
            m_table_name.SelectedIndex = -1;
            m_field_name.SelectedIndex = -1;
            m_key_type.SelectedIndex = -1;
            ClearFind();
        }
        /// <summary>
        /// 表改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableSelectedIndexChanged(object sender, EventArgs e)
        {
            ServerNetInfo.DBItems db_info = this.GetSelectDB();
            if (db_info == null) return;

            string table_name = GetSelectTable();
            if (string.IsNullOrEmpty(table_name)) return;

            Dictionary<string, Type> list = DatabaseManager.Instance.GetFields((eDBType)db_info.id, table_name);
            if (list == null || list.Count == 0) return;
            //--列出字段
            m_field_name.Items.Clear();
            m_key_type.Items.Clear();
            foreach (var obj in list)
            {
                m_field_name.Items.Add(obj.Key);
                m_key_type.Items.Add(obj.Key);
            }
            m_field_name.SelectedIndex = -1;
            m_key_type.SelectedIndex = -1;
            ClearFind();
        }
        private void OnFieldNameSelectedValueChanged(object sender, EventArgs e)
        {
            ClearFind();
        }
        private void OnBinTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            ClearFind();
        }
        #endregion

        #region 编辑字段

        private void OnFieldConfigItemCheck(object sender, ItemCheckEventArgs e)
        {
            ItemCheckEventArgs ice = e as ItemCheckEventArgs;
            if (ice.CurrentValue == CheckState.Checked) return;//取消选中就不用进行以下操作  
            for (int i = 0; i < ((CheckedListBox)sender).Items.Count; i++)
            {
                ((CheckedListBox)sender).SetItemChecked(i, false);//将所有选项设为不选中  
            }
            ice.NewValue = CheckState.Checked;//刷新  
        }

        private void OnFieldConfigSelectedIndexChanged(object sender, EventArgs e)
        {
            int index = m_field_config_list.SelectedIndex;
            if (index < 0 || index >= ServerConfig.info.field_list.Count) return;

            m_field_list.Items.Clear();
            m_list_bin_header.Clear();

            ServerNetInfo.FieldItems field_info = ServerConfig.info.field_list[index];
            string str_fields = field_info.fields;
            if (string.IsNullOrEmpty(str_fields)) return;

            string[] arr_fields = str_fields.Split(',');
            if (arr_fields == null || arr_fields.Length == 0) return;

            //显示当前配置表字段
            for(int i = 0; i < arr_fields.Length; ++i)
            {
                string str_field = arr_fields[i];
                if(string.IsNullOrEmpty(str_field))continue;

                string[] arr_field = str_field.Split('|');
                if (arr_field == null || arr_field.Length == 0 || arr_field.Length >= 3) continue;

                string str_name = "";
                int length = 0;
                if (arr_field.Length == 2)
                {
                    str_name = arr_field[0] + "_" + arr_field[1];
                    length = int.Parse(arr_field[1]);
                }
                else
                    str_name = arr_field[0];

                m_field_list.Items.Add(str_name);
                m_list_bin_header.Add(new BinHeader(arr_field[0], length));
            }

            //查询结果标题
            m_find_result.Columns.Clear();
            for (int i = 0; i < m_list_bin_header.Count; ++i)
            {
                BinHeader header_info = m_list_bin_header[i];
                string str_name = header_info.type;
                if(header_info.length > 0)str_name += "_" + header_info.length;
                this.m_find_result.Columns.Add(i.ToString(), str_name);
            }
            for (int i = 0; i < this.m_find_result.Columns.Count; i++)
            {
                this.m_find_result.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }


            ClearFind();
        }
        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditConfig(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Explorer.exe", ".\\");
        }
        #endregion

        #region 查询+显示
        private void OnSqlKeySelectedIndexChanged(object sender, EventArgs e)
        {
            ClearFind();
        }
        private void OnSqlValueTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            ClearFind();
        }
        private void OnSqlKeyValueTextChanged(object sender, EventArgs e)
        {
            ClearFind();
        }

        private void OnFindResult(object sender, EventArgs e)
        {
            try
            {
                //校验和收集数据
                QueryInfo query_info = CheckValidAndCollect();
                if (query_info == null) return;

                //查询
                ByteArray result_by = null;
                ProcessQuery(query_info, (by) => { result_by = by; this.ShowQueryResult(query_info, by); });
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK);
            }
        }
        /// <summary>
        /// 判断有效性，收集输入信息
        /// </summary>
        /// <returns></returns>
        private QueryInfo CheckValidAndCollect()
        {
            QueryInfo query_info = new QueryInfo();

            ServerNetInfo.DBItems db_info = this.GetSelectDB();
            if (db_info == null)
            {
                ShowErrorMsg("未选择数据库");
                return null;
            }
            query_info.db_type = (eDBType)(db_info.id);

            string table_name = GetSelectTable();
            if (string.IsNullOrEmpty(table_name))
            {
                ShowErrorMsg("未选择表");
                return null;
            }
            query_info.table_name = table_name;

            string field_name = GetSelectField();
            if (string.IsNullOrEmpty(field_name))
            {
                ShowErrorMsg("未选择字段");
                return null;
            }
            query_info.field_name = field_name;
            
            //bin类型
            switch(m_bin_type.SelectedItem.ToString())
            {
                case "binary": query_info.bin_type = MySql.Data.MySqlClient.MySqlDbType.Binary; break;
                case "varbinary": query_info.bin_type = MySql.Data.MySqlClient.MySqlDbType.VarBinary; break;
                case "bit": query_info.bin_type = MySql.Data.MySqlClient.MySqlDbType.Bit; break;
                case "tinyblob": query_info.bin_type = MySql.Data.MySqlClient.MySqlDbType.TinyBlob; break;
                case "blob": query_info.bin_type = MySql.Data.MySqlClient.MySqlDbType.Blob; break;
                case "mediumblob": query_info.bin_type = MySql.Data.MySqlClient.MySqlDbType.MediumBlob; break;
                case "longblob": query_info.bin_type = MySql.Data.MySqlClient.MySqlDbType.LongBlob; break;
                default: query_info.bin_type = MySql.Data.MySqlClient.MySqlDbType.Blob; break;
            }

            ///固定长度
            query_info.fixed_length = 0;
            int index = m_field_config_list.SelectedIndex;
            if (index < 0 || index >= ServerConfig.info.field_list.Count)
            {
                ShowErrorMsg("未选中bin字段配置");
                return null;
            }
            query_info.fixed_length = ServerConfig.info.field_list[index].fixed_length;

            if (m_field_list.Items.Count <= 0)
            {
                ShowErrorMsg("未设置bin结构");
                return null;
            }

            if (string.IsNullOrEmpty(m_key_type.SelectedItem.ToString().Trim()))
            {
                ShowErrorMsg("未选择查询关键字");
                return null;
            }
            query_info.sql_key_name = m_key_type.SelectedItem.ToString();

            string key = m_txt_key.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                ShowErrorMsg("未输入关键字值");
                return null;
            }
            query_info.sql_key_value = key;

            if (m_sql_type.SelectedIndex < 0)
            {
                ShowErrorMsg("未选择关键字类型");
                return null;
            }
            query_info.sql_key_type = m_sql_type.SelectedIndex;
            if (query_info.sql_key_type != 0 && query_info.sql_key_type != 1)
            {
                ShowErrorMsg("选择的关键字类型不对");
                return null;
            }

            return query_info;
        }
        /// <summary>
        /// 执行查询
        /// </summary>
        private void ProcessQuery(QueryInfo query_info, Action<ByteArray> callback)
        {
            string sql = "select `" + query_info.field_name + "` from `" + query_info.table_name + "` where `" + query_info.sql_key_name + "` = ";
            if (query_info.sql_key_type == 0)
            {//数字
                sql += long.Parse(query_info.sql_key_value) + ";";
            }
            else if (query_info.sql_key_type == 1)
            {//字符串
                sql += "'" + query_info.sql_key_value + "';";
            }
            else
            {
                return;
            }
            DatabaseManager.Instance.GetDB(query_info.db_type).Query(sql, (reader) =>
            {
                ByteArray by = new ByteArray(1024 * 8, int.MaxValue);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//内容
                        long len = reader.GetBytes(0, 0, null, 0, int.MaxValue);
                        reader.GetBytes(0, 0, by.Buffer, 0, (int)len);
                        by.WriteEmpty((int)len);
                    }
                }
                callback(by);
            });
        }
        /// <summary>
        /// 显示查询结果
        /// </summary>
        private void ShowQueryResult(QueryInfo query_info, ByteArray by)
        {
            if(by.Available == 0)
            {
                m_query_info = null;
                MessageBox.Show("查询数据为空", "信息", MessageBoxButtons.OK);
                return;
            }
            m_query_info = query_info;

            int total_length_one_row = 0;
            foreach (var bin_header in m_list_bin_header)
            {
                switch (bin_header.type)
                {
                    case "int":
                    case "uint":
                        total_length_one_row += bin_header.length;
                    break;
                    case "float": total_length_one_row += 4; break;
                    case "double": total_length_one_row += 8; break;
                    case "string": /**undo*/; break;
                    default: continue;
                }
            }

            ///解析
            m_list_bin_contentes.Clear();
            while (by.Available > 0)
            {
                if ((query_info.fixed_length > 0 && by.Available < query_info.fixed_length) || (by.Available < total_length_one_row))
                {
                    MessageBox.Show("数据异常，长度不够", "警告", MessageBoxButtons.OK);
                    break;
                }

                int start_pos = by.Head;

                List<BinContent> list = new List<BinContent>();
                foreach (var bin_header in m_list_bin_header)
                {
                    string value = "";
                    byte[] by_val = new byte[8];
                    switch (bin_header.type)
                    {
                        case "int": by.Read(ref by_val, bin_header.length); value = BitConverter.ToInt64(by_val, 0).ToString(); break;
                        case "uint": by.Read(ref by_val, bin_header.length); value = BitConverter.ToUInt64(by_val, 0).ToString(); break;
                        case "float": value = by.ReadFloat().ToString(); break;
                        case "double": value = by.ReadDouble().ToString(); break;
                        case "string": value = by.ReadString(); break;
                        default: continue;
                    }
                    list.Add(new BinContent(bin_header.type, value));
                }
                m_list_bin_contentes.Add(list);

                if (query_info.fixed_length > 0)
                {
                    by.SetHead(start_pos + query_info.fixed_length);
                }
            }

            ///显示数据
            m_find_result.Rows.Clear();
            for (int row = 0; row < m_list_bin_contentes.Count; ++row)
            {
                int index = this.m_find_result.Rows.Add();
                List<BinContent> list = m_list_bin_contentes[row];
                for(int col = 0; col < list.Count; col++)
                {
                    m_find_result.Rows[index].Cells[col].Value = list[col].value;
                }
            }
        }
        #endregion

        #region 保存数据
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveResult(object sender, EventArgs e)
        {
            if (m_query_info == null)
            {
                ShowErrorMsg("请先执行查询");
                return;
            }
            if(MessageBox.Show("是否保存修改?", "提示", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    ProcessSave(m_query_info);
                    MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    ShowErrorMsg("保存失败:" + ex.Message);
                }
            }
        }
        private void ProcessSave(QueryInfo query_info)
        {
            //先收集数据
            List<List<string>> list_result = new List<List<string>>();
            for (int row = 0; row < m_find_result.Rows.Count; ++row)
            {
                List<string> list = new List<string>();
                for (int col = 0; col < m_find_result.Columns.Count; col++)
                {
                    string value = m_find_result.Rows[row].Cells[col].Value as string;
                    list.Add(value);
                }
                list_result.Add(list);
            }

            //序列化
            ByteArray by = new ByteArray(1024 * 8, int.MaxValue);
            for (int row = 0; row < list_result.Count; ++row)
            {
                int start_pos = by.Tail;
                List<string> list_row_result = list_result[row];
                for (int col = 0; col < m_list_bin_header.Count; ++col)
                {
                    BinHeader header_info = m_list_bin_header[col];
                    string value = list_row_result[col];
                    switch (header_info.type)
                    {
                        case "int":
                            if (value.Length == 0) value = "0";
                            long l = long.Parse(value);
                            byte[] b = BitConverter.GetBytes(l);
                            by.Write(b, header_info.length);
                            break;
                        case "uint":
                            if (value.Length == 0) value = "0";
                            ulong ul = ulong.Parse(value);
                            b = BitConverter.GetBytes(ul);
                            by.Write(b, header_info.length);
                            break;
                        case "float":
                            if (value.Length == 0) value = "0";
                            float f = float.Parse(value);
                            b = BitConverter.GetBytes(f);
                            by.Write(b, header_info.length);
                            break;
                        case "double":
                            if (value.Length == 0) value = "0";
                            double d = double.Parse(value);
                            b = BitConverter.GetBytes(d);
                            by.Write(b, header_info.length);
                            break;
                        case "string":
                            by.WriteString(value);
                            break;
                        default: continue;
                    }
                }
                if (query_info.fixed_length > 0)
                {
                    by.WriteEmpty(query_info.fixed_length - (by.Tail - start_pos));
                }
            }

            //保存到db
            string key_sql = query_info.sql_key_value;
            if (query_info.sql_key_type == 1) key_sql = "'" + key_sql + "'";

            string sql = "replace into `" + query_info.table_name + "`" +
            "(`" + query_info.sql_key_name + "`,`" + query_info.field_name + "`) " +
            "values (" +
            key_sql + "," +
            "@bin_data)";

            List<MySqlParameter> param = new List<MySqlParameter>();
            MySqlParameter p = Database.MakeMysqlParam("@bin_data", query_info.bin_type, by.GetBuffer(), by.Available);
            param.Add(p);
            DatabaseManager.Instance.GetDB(query_info.db_type).Execute(sql, param);
        }

        private void ShowErrorMsg(string msg)
        {
            MessageBox.Show(msg, "错误", MessageBoxButtons.OK);
        }

        private void ClearFind()
        {
            m_query_info = null;
            m_find_result.Rows.Clear();
        }
        #endregion

    }
    /// <summary>
    /// 收集的查询信息
    /// </summary>
    class QueryInfo
    {
        public eDBType db_type;
        public string table_name;
        public string field_name;
        public int fixed_length;        // 如果是0，不固定长度
        public string sql_key_name;     // 查询关键字
        public string sql_key_value;    // 查询值
        public int sql_key_type;        // 关键字类型，只支持两种类型的查询: 0-数字,1-字符串
        public MySql.Data.MySqlClient.MySqlDbType bin_type;
    }
    
    struct BinHeader
    {
        public string type;
        public int length;
        public BinHeader(string _type, int _length)
        {
            type = _type;
            length = _length;
        }
    }
    struct BinContent
    {
        public string type;
        public string value;
        public BinContent(string _type, string _value)
        {
            type = _type;
            value = _value;
        }
    }
}
