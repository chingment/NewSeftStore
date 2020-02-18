using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TgPaySdk
{
    public class TgPayUtil
    {
        private ApiDoPost _api = new ApiDoPost();
        private string notifyUrl = "";
        private string account = "";
        private string key = "";


        public TgPayUtil(TgPayInfoConfg config)
        {
            this.account = config.Account;
            this.key = config.Key;
            this.notifyUrl = config.PayResultNotifyUrl;
        }

        public string GetSign(Dictionary<string, string> dic)
        {

            //var vDic = (from objDic in dic orderby objDic.Key descending select objDic);
            //StringBuilder str1 = new StringBuilder();
            //foreach (KeyValuePair<string, string> kv in vDic)
            //{
            //    string pkey = kv.Key;
            //    string pvalue = kv.Value;
            //    str1.Append(pkey + "=" + pvalue + "&");
            //}

            //string c1 = str1.ToString();

            var arrKeys = dic.Keys.ToArray();
            Array.Sort(arrKeys, string.CompareOrdinal);//ASCII码从小到大排序

            string str = "";
            foreach (var key in arrKeys)
            {
                  str += key + "=" + dic[key] + "&";
                //str += key + "=" + System.Web.HttpUtility.UrlEncode(dic[key]) + "&";
            }


            str += "key=" + key;


            string str_sign = GetMD5(str);
            //string str_sign2 = GetStrMd5(str);
            return str_sign;
        }

        public static string GetMD5(string material)
        {
            if (string.IsNullOrEmpty(material))
                throw new ArgumentOutOfRangeException();



            byte[] result = Encoding.UTF8.GetBytes(material);    //tbPass为输入密码的文本框  
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string s_output = BitConverter.ToString(output).Replace("-", "");

            return s_output;
        }


        public AllQrcodePayRequestResult AllQrcodePay(string lowOrderId, string payMoney, string body, string lowCashier)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("account", account);
            dic.Add("notifyUrl", notifyUrl);

            if (!string.IsNullOrEmpty(lowOrderId))
            {
                dic.Add("lowOrderId", lowOrderId);
            }

            if (!string.IsNullOrEmpty(payMoney))
            {
                dic.Add("payMoney", payMoney);
            }

            if (!string.IsNullOrEmpty(body))
            {
                dic.Add("body", body);
            }

            //if (!string.IsNullOrEmpty(attach))
            //{
            //    dic.Add("attach", attach);
            //}

            if (!string.IsNullOrEmpty(lowCashier))
            {
                dic.Add("lowCashier", lowCashier);
            }

            dic.Add("sign", GetSign(dic));


            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            foreach (var key in dic)
            {
                dic1.Add(key.Key, System.Web.HttpUtility.UrlEncode(key.Value));
            }

            var request = new AllQrcodePayRequest(dic);

            var requestResult = _api.DoPost(request);


            return requestResult;
        }

        public string OrderQuery(string lowOrderId)
        {

            LogUtil.Info("OrderQuery:" + lowOrderId);

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("account", account);
            dic.Add("lowOrderId", lowOrderId);


            dic.Add("sign", GetSign(dic));


            //Dictionary<string, string> dic1 = new Dictionary<string, string>();
            //foreach (var key in dic)
            //{
            //    dic1.Add(key.Key, System.Web.HttpUtility.UrlEncode(key.Value));
            //}

            var request = new OrderQueryRequest(dic);

            var requestResult = _api.DoPost(request);

            return requestResult.ToJsonString();
        }
    }
}
