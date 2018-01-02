using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 邮件管理器
    /// @author hannibal
    /// @time 2016-9-11
    /// </summary>
    public class MailDataManager : Singleton<MailDataManager>
    {
        private List<MailTitleInfo> m_mail_titles = new List<MailTitleInfo>();
        
        public void Setup()
        {

        }

        public void Destroy()
        {
            m_mail_titles.Clear();
        }

        public void AddNewMails(List<MailTitleInfo> list)
        {
            m_mail_titles.AddRange(list);
        }

        public List<MailTitleInfo> mail_titles
        {
            get { return m_mail_titles; }
        }
    }
}
