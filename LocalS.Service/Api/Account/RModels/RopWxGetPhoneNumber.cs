using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RopWxGetPhoneNumber
    {
        public string encryptedData { get; set; }
        public string iv { get; set; }

        public string session_key { get; set; }
    }
}
