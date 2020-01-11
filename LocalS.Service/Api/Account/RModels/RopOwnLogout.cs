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
        public string AppId { get; set; }
        public Dictionary<string, string> LoginPms { get; set; }

    }
}
