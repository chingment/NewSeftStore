using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RopOwnLogout
    {
        public RopOwnLogout()
        {
            this.LoginPms = new Dictionary<string, string>();
        }

        public string AppId { get; set; }
        public string Token { get; set; }
        public Enumeration.LoginWay LoginWay { get; set; }
        public Dictionary<string, string> LoginPms { get; set; }

        public string BelongId { get; set; }

        public string Ip { get; set; }

    }
}
