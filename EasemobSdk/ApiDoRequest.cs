                     using Lumos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasemobSdk
{
    public class ApiDoRequest
    {
        private string responseString = null;

        public string GetSeviceUrl()
        {
            return "http://a1.easemob.com/1106200520157173/selfstore";
        }


        public ApiDoRequest()
        {

        }

        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }


        private string _accessToken = null;
        public void setAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }


        public CustomJsonResult<T> DoPost<T>(IApiPostRequest<T> request)
        {
            var result = new CustomJsonResult<T>();

            try
            {
                string requestUrl = GetSeviceUrl() + "/" + request.ApiUrl;

                WebUtils webUtils = new WebUtils();
                LogUtil.Info(string.Format("EasemobSdk-PostUrl->{0}", requestUrl));
                LogUtil.Info(string.Format("EasemobSdk-PostData->{0}", request.PostData));
                webUtils.setAccessToken(_accessToken);
                var doPost = webUtils.DoPost(requestUrl, request.PostData);
                if (doPost == null)
                {
                    result.Result = ResultType.Failure;
                    result.Code = ResultCode.Failure;
                    result.Message = "请求数据失败";
                    return result;
                }

                if (doPost.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    result.Result = ResultType.Failure;
                    result.Code = ResultCode.Failure;
                    result.Message = "请求数据失败：" + doPost.StatusCode;
                    return result;
                }

                this.responseString = doPost.ResponseString;

                LogUtil.Info(string.Format("EasemobSdk-PostResult->{0}", responseString));
                T data = JsonConvert.DeserializeObject<T>(responseString);

                result.Result = ResultType.Success;
                result.Code = ResultCode.Success;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Result = ResultType.Exception;
                result.Code = ResultCode.Exception;
                result.Message = "请求数据发生异常";
            }

            return result;
        }
    }
}
