using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dc
{
    class ServerMain
    {
        static void Main(string[] args)
        {
            Master.Instance.Setup();
            Master.Instance.Start();
        }
    }
}
