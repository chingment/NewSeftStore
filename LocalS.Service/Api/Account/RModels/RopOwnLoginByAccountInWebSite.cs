using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RopOwnLoginByAccountInWebSite
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RedirectUrl { get; set; }
        public Dictionary<string, string> LoginPms { get; set; }
        public string AppId { get; set; }

    }
}
