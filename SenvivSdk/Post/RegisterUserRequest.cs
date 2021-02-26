using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class RegisterUserRequest : IApiPostRequest<RegisterUserResult>
    {
        private string _postData = null;

        public RegisterUserRequest(string username, string password, string nickname)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("username", username);
            dic.Add("password", password);
            dic.Add("nickname", nickname);

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
                return "users";
            }
        }
    }
}
