using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dc
{
    /// <summary>
    /// csv文件读取
    /// @author hannibal
    /// @time 2016-8-1
    /// </summary>
    public class CSVDocument
    {
        private List<string> m_header_list = null; //表头名称
        private List<List<string>> m_data_table = null;
        public int m_table_rows;    //行数量
        public int m_table_cols;    //列数量

        public CSVDocument()
        {
            m_data_table = new List<List<string>>();
            m_header_list = new List<string>();
        }
        /// <summary>
        /// 读取csv文件
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public bool Load(string file_name)
        {
            m_data_table.Clear();
            m_header_list.Clear();
            m_table_rows = 0;
            m_table_cols = 0;

            try
            {
                using (FileStream fs = new FileStream(file_name, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                    {
                        //记录每次读取的一行记录
                        string column_text = "";
                        bool is_row_skip = false;
                        bool is_first_row = true;
                        //逐行读取CSV中的数据
                        while ((column_text = sr.ReadLine()) != null)
                        {
                            if (column_text.Length >= 2 && column_text[0] == '/' && column_text[1] == '/')
                                continue;	//如果头两个字符 是  //  代表该行无效
                            if (column_text.Length >= 2 && column_text[0] == '\\' && column_text[1] == '\\')
                                continue;	//如果头两个字符 是  \\  代表该行无效
                            if (column_text.Length >= 2 && column_text[0] == ',' && column_text[1] == ',')
                                continue;   //如果头两个字符都是‘，’表示该行数据无效
                            if (column_text.Length > 2 && column_text[0] == '"' && column_text[1] == '/' && column_text[2] == '/')
                                continue;	//如果头三个字符是"// 也认为该行无效
                            if (column_text.Length >= 2 && column_text[0] == '/' && column_text[1] == '*')
                            {//使用/**/注释块
                                is_row_skip = true;
                                continue;	//如果头两个字符是"/* 也认为该行无效
                            }
                            if (is_row_skip && column_text.Length >= 2 && column_text[0] == '*' && column_text[1] == '/')
                            {
                                is_row_skip = false;
                                continue;	//如果头两个字符是"*/ 也认为该行无效
                            }
                            if (is_row_skip)
                                continue;
                            string[] column_array = readLine(column_text);
                            if (is_first_row)
                            {
                                is_first_row = false;
                                m_table_cols = column_array.Length;
                                //创建列
                                for (int j = 0; j < column_array.Length; j++)
                                {
                                    m_header_list.Add(column_array[j]);
                                }
                            }
                            else
                            {
                                List<string> tempLineStringList = new List<string>();
                                for (int j = 0; j < column_array.Length; j++)
                                {
                                    tempLineStringList.Add(column_array[j]);
                                }
                                m_data_table.Add(tempLineStringList);
                            }
                        }
                        m_table_rows = m_data_table.Count;
                    }
                }
                Log.Info("读取配置表:" + file_name);
                return true;
            }
            catch(Exception e)
            {
                Log.Exception(e);
            }
            return false;
        }
        private string[] readLine(string line)
        {
            var builder = new StringBuilder();
            var comma = false;
            var array = line.ToCharArray();
            var values = new List<string>();
            var length = array.Length;
            var index = 0;
            while (index < length)
            {
                var item = array[index++];
                switch (item)
                {
                    case ',':
                        if (comma)
                        {
                            builder.Append(item);
                        }
                        else
                        {
                            values.Add(builder.ToString());
                            builder.Remove(0, builder.Length);
                        }
                        break;
                    case '"':
                        comma = !comma;
                        break;
                    default:
                        builder.Append(item);
                        break;
                }
            }
            if (builder.Length > 0)
                values.Add(builder.ToString());
            return values.ToArray();
        }

        /// <summary>
        /// 返回CSV文档行数量
        /// </summary>
        public int TableRows() { return m_table_rows; }
        /// <summary>
        /// 返回CSV文档列数量
        /// </summary>
        public int TableColumns() { return m_table_cols; }
        /// <summary>
        /// 获取列名索引
        /// </summary>
        public int GetColumnIndex(string columnName)
        {
            return m_header_list.IndexOf(columnName);
        }
        static CSVColumElement m_DefaultElement = new CSVColumElement();
        /// <summary>
        /// 读取第rowIndex行columnIndex列的数据
        /// </summary>
        public CSVColumElement GetValue(int rowIndex, string columnName)
        {
            m_DefaultElement.Value = "";
            int columnIndex = m_header_list.IndexOf(columnName);
            if (rowIndex < 0 || m_data_table.Count <= rowIndex)
                return m_DefaultElement;
            List<string> tempStrList = m_data_table[rowIndex];
            if (columnIndex < 0 || tempStrList.Count <= columnIndex)
                return m_DefaultElement;
            m_DefaultElement.Value = tempStrList[columnIndex];
            return m_DefaultElement;
        }
        public void Clear()
        {
            if (m_header_list != null)
            {
                m_header_list.Clear();
                m_header_list = null;
            }
            if (m_data_table != null)
            {
                m_data_table.Clear();
                m_data_table = null;
            }
        }
    }
    public class CSVColumElement
    {
        private string m_elementValue = "";
        public string Value
        {
            get { return m_elementValue; }
            set { m_elementValue = value; }
        }
        public override string ToString() { return m_elementValue; }
        public bool ToBool() { return ToInt64() != 0 ? true : false; }
        public byte ToByte() { return (byte)ToInt64(); }
        public sbyte ToSByte() { return (sbyte)ToInt64(); }
        public char ToChar() { return (char)ToInt64(); }
        public short ToInt16() { return (short)ToInt64(); }
        public ushort ToUInt16() { return (ushort)ToInt64(); }
        public int ToInt32() { return (int)ToInt64(); }
        public uint ToUInt32() { return (uint)ToInt64(); }
        public float ToFloat() { return (float)ToDecimal(); }
        public double ToDouble() { return (double)ToDecimal(); }
        public long ToInt64()
        {
            try
            {
                return (m_elementValue.Length > 0) ? long.Parse(m_elementValue) : ((long)0);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public ulong ToUInt64()
        {
            try
            {
                return (m_elementValue.Length > 0) ? ulong.Parse(m_elementValue) : ((long)0);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public double ToDecimal()
        {
            try
            {
                return (m_elementValue.Length > 0) ? double.Parse(m_elementValue) : (0.0d);
            }
            catch (Exception)
            {
                return 0.0d;
            }
        }
    }
}
