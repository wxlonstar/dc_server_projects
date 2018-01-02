using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dc
{
    /// <summary>
    /// 切换场景
    /// @author hannibal
    /// @time 2016-8-29
    /// </summary>
    public partial class ChangeSceneForm : Form
    {
        public ChangeSceneForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            this.m_txt_scene.Text = "";
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
       
        private void OnBtnOk(object sender, EventArgs e)
        {
            string scene_type = this.m_txt_scene.Text;
            if (scene_type.Trim().Length == 0) return;
            ServerMsgSend.SendEnterScene(ushort.Parse(scene_type));
            this.Close();
        }
        private void OnBtnCancel(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
