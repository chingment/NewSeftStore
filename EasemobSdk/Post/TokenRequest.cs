using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasemobSdk
{
    public class TokenRequest : IApiPostRequest<TokenResult>
    {

        private string _postData = null;

        public TokenRequest(string grant_type, string client_id, string client_secret)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("grant_type", grant_type);
            dic.Add("client_id", client_id);
            dic.Add("client_secret", client_secret);

            this._postData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
        }

        public string PostData
        {
            get
            {
                return this._postData;
            }
        }

        public string ApiUrl
        {

            get
            {
                return "token";
            }
        }
    }
}
