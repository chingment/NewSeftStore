using Lumos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsyInscarSdk
{
    public class Api
    {
        public string serverUrl = "http://insuredata.dameizhoubx.com";

        public string user_code = "kVU8KIi83270";
        public string key = "c93921d7a8c62f6e467f450147d747ce";

        public string GetServerUrl(string serverurl, string apiname)
        {
            return serverurl + "/" + apiname;
        }

        public BaseResult<T> Get<T>(IApiGetRequest<T> request)
        {
            string realServerUrl = GetServerUrl(this.serverUrl, request.ApiName);

            WebUtils webUtils = new WebUtils();

            request.UserCode = user_code;
            request.Key = key;


            string body = webUtils.DoGet(realServerUrl, request.GetUrlParameters(), null);

            var rsp = new BaseResult<T>();

            try
            {
                rsp = JsonConvert.DeserializeObject<BaseResult<T>>(body);
            }
            catch (Exception ex)
            {
                LogUtil.Error("解释 result 错误:" + ex.Message,ex);
            }
            return rsp;
        }

        public BaseResult<T> Post<T>(IApiPostRequest<T> request)
        {


            return null;
        }
    }
}
