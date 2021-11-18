using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RopOwnGetWxACodeUnlimit
    {
        public string Type { get; set; }
        public string Data { get; set; }
        public string StoreId { get; set; }
        public string AppId { get; set; }
        public string OpenId { get; set; }
        public bool IsGetAvatar { get; set; }
    }
}
