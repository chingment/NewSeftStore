using Lumos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class YbInsUtil
    {
        public class userInfo
        {
            public string channelUserCode { get; set; }
            public string userName { get; set; }
            public string phone { get; set; }
            public string idName { get; set; }
            public string email { get; set; }
            public string channelCompanyCode { get; set; }
        }

        public static string GetSign(string channelUserCode, string userName, string phone, string idName = "", string email = "", string channelCompanyCode = "")
        {
            string publicKey = ConfigurationManager.AppSettings["custom:YbPublicKey"];
            RSAForJava rsa = new RSAForJava();

            userInfo u = new userInfo();

            u.channelUserCode = channelUserCode;
            u.userName = userName;
            u.phone = phone;
            u.idName = idName;
            u.email = email;
            u.channelCompanyCode = channelCompanyCode;

            string input = Newtonsoft.Json.JsonConvert.SerializeObject(u);


            String encry = rsa.EncryptByPublicKey(input, publicKey);


            return encry;
        }
    }
}
