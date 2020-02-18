﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Lumos;
using Newtonsoft.Json;

namespace TgPaySdk
{

    //http://tgjf.833006.biz
    //http://ipay.833006.net
    public class ApiDoPost
    {

        public string GetSeviceUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["custom:TgPayServerUrl"];
        }
        public ApiDoPost()
        {

        }

        public T DoPost<T>(IApiPostRequest<T> request)
        {

            string str_PostData = JsonConvert.SerializeObject(request.PostData);

            string requestUrl = GetSeviceUrl() + request.ApiUrl;

            WebUtils webUtils = new WebUtils();
            LogUtil.Info(string.Format("TgPaySdk-PostUrl->{0}", requestUrl));
            LogUtil.Info(string.Format("TgPaySdk-PostData->{0}", str_PostData));
            string responseString = webUtils.DoPost(requestUrl, str_PostData);

            LogUtil.Info(string.Format("TgPaySdk-PostResult->{0}", responseString));
            T rsp = JsonConvert.DeserializeObject<T>(responseString);
            return rsp;
        }

    }
}