using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TongGuanPaySdk
{
    public class TongGuanApi
    {
        private ApiDoPost _api = new ApiDoPost();
        private string notifyUrl = "https://demo.res.17fanju.com/Api/Order/PayResultNotify";
        private string account = "1571215372255";
        private string key = "bda1c3c86878b33258823d4d1dcc20ea";

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



            byte[] result = Encoding.Default.GetBytes(material);    //tbPass为输入密码的文本框  
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string s_output = BitConverter.ToString(output).Replace("-", "");

            return s_output;
        }

        public string GetStrMd5(string ConvertString) {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)));
            t2 = t2.Replace("-", "");
            return t2;
        }

        public AllQrcodePayRequestResult AllQrcodePay(string lowOrderId, string payMoney, string body, string lowCashier)
        {
            var result = new AllQrcodePayRequestResult();

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

            var request = new AllQrcodePayRequest(dic);

            var requestResult = _api.DoPost(request);


            return result;
        }
    }
}
