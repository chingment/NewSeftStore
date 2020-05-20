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

        public CustomJsonResult<T> DoPost<T>(IApiPostRequest<T> request)
        {
            var result = new CustomJsonResult<T>();

            try
            {
                string requestUrl = GetSeviceUrl() + "/" + request.ApiUrl;

                WebUtils webUtils = new WebUtils();
                LogUtil.Info(string.Format("EasemobSdk-PostUrl->{0}", requestUrl));
                LogUtil.Info(string.Format("EasemobSdk-PostData->{0}", request.PostData));
                var response = webUtils.DoPost(requestUrl, request.PostData);
                if (response == null)
                {
                    result.Result = ResultType.Failure;
                    result.Code = ResultCode.Failure;
                    result.Message = "请求数据失败";
                    return result;
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    result.Result = ResultType.Failure;
                    result.Code = ResultCode.Failure;
                    result.Message = "请求数据失败：" + response.StatusCode;
                    return result;
                }

                Encoding encoding = webUtils.GetResponseEncoding(response);

                string responseStr = webUtils.GetResponseAsString(response, encoding);

                this.responseString = responseStr;

                LogUtil.Info(string.Format("EasemobSdk-PostResult->{0}", responseStr));
                T data = JsonConvert.DeserializeObject<T>(responseStr);

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
