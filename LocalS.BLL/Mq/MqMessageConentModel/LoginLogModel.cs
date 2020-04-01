using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{
    public class LoginLogModel
    {
        public string LoginAccount { get; set; }
        public Enumeration.LoginResult LoginResult { get; set; }
        public Enumeration.LoginWay LoginWay { get; set; }
        public Enumeration.LoginFun LoginFun { get; set; }
        public string LoginIp { get; set; }
        public string RemarkByDev { get; set; }
    }
}
