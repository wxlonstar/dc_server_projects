using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    public class DataManager : Singleton<DataManager>
    {
        public void Setup()
        {
            PlayerDataMgr.Instance.Setup();
            LoginDataMgr.Instance.Setup();
            MailDataManager.Instance.Setup();
            RelationDataManager.Instance.Setup();
            ChatDataManager.Instance.Setup();
            PingDataManager.Instance.Setup();
        }

        public void Destroy()
        {
            PlayerDataMgr.Instance.Destroy();
            LoginDataMgr.Instance.Destroy();
            MailDataManager.Instance.Destroy();
            RelationDataManager.Instance.Destroy();
            ChatDataManager.Instance.Destroy();
            PingDataManager.Instance.Destroy();
        }
    }
}
