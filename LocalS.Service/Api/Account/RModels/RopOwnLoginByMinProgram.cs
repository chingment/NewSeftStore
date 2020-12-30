using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RopOwnLoginByMinProgram
    {
        public string MerchId { get; set; }
        public string AppId { get; set; }
        public string OpenId { get; set; }
        public UserInfoEpModel UserInfoEp { get; set; }
        public PhoneNumberEpModel PhoneNumberEp { get; set; }
        public string ReffSign { get; set; }
        public class UserInfoEpModel
        {
            public string Code { get; set; }
            public string Iv { get; set; }
            public string EncryptedData { get; set; }
        }
        public class PhoneNumberEpModel
        {
            public string encryptedData { get; set; }
            public string iv { get; set; }
            public string session_key { get; set; }
        }
    }
}
