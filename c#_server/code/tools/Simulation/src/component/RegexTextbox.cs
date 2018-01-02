using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace dc.Compnent
{
    /// <summary>
    /// 带正则表达式的TextBox
    /// @author hannibal
    /// @time 2016-8-29
    /// </summary>
    public class RegTextBox : TextBox
    {
        private static ToolTip m_tooltip;   // 控件的tooltip
        #region 属性
        private string _regExp = string.Empty;
        ////// 获取或设置用于验证控件值的正则表达式
        ///[Description("获取或设置用于验证控件值的正则表达式"), Category("验证"),
        [DefaultValue("")]
        public string RegexExpression
        {
            get { return _regExp; }
            set { _regExp = value; }
        }

        private bool _allEmpty = false;
        ////// 获取或设置是否允许空值
        ///[Description("获取或设置是否允许空值"), Category("验证"), DefaultValue(true)]
        [DefaultValue(false)]
        public bool AllowEmpty
        {
            get { return _allEmpty; }
            set { _allEmpty = value; }
        }

        private bool _removeSpace = false;
        ////// 获取或设置验证的时候是否除去头尾空格
        ///[Description("获取或设置验证的时候是否除去头尾空格"), Category("验证"),
        [DefaultValue(true)]
        public bool RemoveSpace
        {
            get { return _removeSpace; }
            set { _removeSpace = value; }
        }

        private string _empMsg = "不能为空";
        private BackgroundWorker backgroundWorker1;
        ////// 获取或设置当控件的值为空的时候显示的信息
        ///[Description("获取或设置当控件的值为空的时候显示的信息"), Category("验证"),
        [DefaultValue("")]
        public string EmptyMessage
        {
            get { return _empMsg; }
            set { _empMsg = value; }
        }

        private string _errMsg = "输入格式错误";
        ////// 获取或设置当不满足正则表达式结果的时候显示的错误信息
        ///[Description("获取或设置当不满足正则表达式结果的时候显示的错误信息"), Category
        [DefaultValue("")]
        public string ErrorMessage
        {
            get { return _errMsg; }
            set { _errMsg = value; }
        }
        #endregion

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (m_tooltip != null) m_tooltip.Dispose(); // 如果tooltip已经存在则销毁

            if (!EmptyValidate(this.Text)) return;
            if (!RegexExpressionValidate(this.Text)) return;
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (!EmptyValidate(this.Text)) return;
            if (!RegexExpressionValidate(this.Text)) return;
        } 
        /// <summary>
        /// 非空验证
        /// </summary>
        /// <param name="ctl"></param>
        /// <returns></returns>
        private bool EmptyValidate(string txt)
        {
            if (!this.AllowEmpty)
            {
                if ((this.RemoveSpace && txt.Trim() == "") || txt == "")
                {
                    ShowErrorMessage(this.EmptyMessage);
                    this.SelectAll();
                    this.Focus();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 正则验证
        /// </summary>
        /// <param name="ctl"></param>
        /// <returns></returns>
        private bool RegexExpressionValidate(string txt)
        {
            if (!((this.RemoveSpace && txt.Trim() == "") || txt == ""))
            {
                if (!string.IsNullOrEmpty(this.RegexExpression) &&
                    !Regex.IsMatch((this.RemoveSpace ? txt.Trim() : txt),this.RegexExpression))
                {
                    ShowErrorMessage(this.ErrorMessage);
                    this.SelectAll();
                    this.Focus();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="ctl"></param>
        /// <param name="err"></param>
        private void ShowErrorMessage(string err)
        {
            if (m_tooltip != null) m_tooltip.Dispose(); // 如果tooltip已经存在则销毁
            m_tooltip = new ToolTip();
            m_tooltip.ToolTipIcon = ToolTipIcon.Warning;
            m_tooltip.IsBalloon = true;
            m_tooltip.ToolTipTitle = "提示";
            m_tooltip.AutoPopDelay = 5000;
            //得到信息显示的行数
            if (err == null) err = " ";
            int l = err.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
            m_tooltip.Show(err, this, new System.Drawing.Point(10, -47 - l * 18 + (this.Height - 21) / 2));
        }

        private void InitializeComponent()
        {
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}