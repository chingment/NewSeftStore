using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Lumos;
using Newtonsoft.Json;

namespace TongGuanPaySdk
{


    public class ApiDoPost
    {
        public ApiDoPost()
        {

        }

        public T DoPost<T>(IApiPostRequest<T> request)
        {

            string str_PostData = JsonConvert.SerializeObject(request.PostData);

            WebUtils webUtils = new WebUtils();
            LogUtil.Info(string.Format("TongGuanPaySdk-PostUrl->{0}", request.ApiUrl));
            LogUtil.Info(string.Format("TongGuanPaySdk-PostData->{0}", str_PostData));
            string responseString = webUtils.DoPost(request.ApiUrl, str_PostData);

            LogUtil.Info(string.Format("TongGuanPaySdk-PostResult->{0}", responseString));
            T rsp = JsonConvert.DeserializeObject<T>(responseString);
            return rsp;
        }

    }
}
