using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XrtPaySdk
{
    public class XrtPayUtil
    {
        private ApiDoPost _api = new ApiDoPost();
        private string notifyUrl = "";
        private string mch_id = "";
        private string key = "";


        public XrtPayUtil(XrtPayInfoConfg config)
        {
            this.mch_id = config.Mch_id;
            this.key = config.Key;
            this.notifyUrl = config.PayResultNotifyUrl;
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

        public string GetSign(Dictionary<string, string> dic)
        {

            var arrKeys = dic.Keys.ToArray();
            Array.Sort(arrKeys, string.CompareOrdinal);//ASCII码从小到大排序

            string str = "";
            foreach (var key in arrKeys)
            {
                if (!string.IsNullOrEmpty(dic[key]))
                {
                    str += key + "=" + dic[key] + "&";
                }
            }


            str += "key=" + key;

            string str_sign = GetMD5(str);

            return str_sign;
        }

        public string GetNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public WxJsPayRequestResult WxPayBuildByJs(string sub_appid, string sub_openid, string out_trade_no, string total_fee, string body, string attach, string create_ip, string time_start, string time_expire)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("service", "pay.weixin.jspay");
            dic.Add("version", "");
            dic.Add("charset", "");
            dic.Add("sign_type", "");
            dic.Add("mch_id", this.mch_id);
            dic.Add("is_raw","1");
            dic.Add("out_trade_no", out_trade_no);
            dic.Add("device_info", "");
            dic.Add("body", body);
            dic.Add("sub_openid", sub_openid);
            dic.Add("sub_appid", sub_appid);
            dic.Add("attach", "");
            dic.Add("total_fee", total_fee); 
            dic.Add("mch_create_ip", create_ip);
            dic.Add("notify_url", this.notifyUrl);
            dic.Add("time_start", time_start);
            dic.Add("time_expire", time_expire);
            dic.Add("goods_tag", "");
            dic.Add("limit_credit_pay", "0");

            dic.Add("nonce_str", GetNonceStr());


            dic.Add("sign", GetSign(dic));


            Dictionary<string, string> post_dic = new Dictionary<string, string>();
            foreach (var key in dic)
            {
                if (!string.IsNullOrEmpty(key.Value))
                {
                    post_dic.Add(key.Key, key.Value);
                }
            }

            var request = new WxJsPayRequest(dic);

            var requestResult = _api.DoPost(request);


            return requestResult;
        }

        public WxNativePayRequestResult WxPayBuildByNt(string out_trade_no, string total_fee, string body, string attach, string create_ip, string time_start, string time_expire)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("service", "pay.weixin.native");
            dic.Add("version", "");
            dic.Add("charset", "");
            dic.Add("sign_type", "");
            dic.Add("mch_id", this.mch_id);
            dic.Add("out_trade_no", out_trade_no);
            dic.Add("device_info", "");
            dic.Add("body", body);
            dic.Add("attach", "");
            dic.Add("total_fee", total_fee);
            dic.Add("mch_create_ip", create_ip);
            dic.Add("notify_url", this.notifyUrl);
            dic.Add("time_start", time_start);
            dic.Add("time_expire", time_expire);
            dic.Add("goods_tag", "");
            dic.Add("product_id", "");
            dic.Add("nonce_str", GetNonceStr());


            dic.Add("sign", GetSign(dic));


            Dictionary<string, string> post_dic = new Dictionary<string, string>();
            foreach (var key in dic)
            {
                if (!string.IsNullOrEmpty(key.Value))
                {
                    post_dic.Add(key.Key, key.Value);
                }
            }

            var request = new WxNativePayRequest(dic);

            var requestResult = _api.DoPost(request);


            return requestResult;
        }

        public AliNativePayRequestResult ZfbPayBuildByNt(string out_trade_no, string total_fee, string body, string attach, string create_ip, string time_start, string time_expire)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("service", "pay.alipay.native");
            dic.Add("version", "");
            dic.Add("charset", "");
            dic.Add("sign_type", "");
            dic.Add("mch_id", this.mch_id);
            dic.Add("out_trade_no", out_trade_no);
            dic.Add("device_info", "");
            dic.Add("body", body);
            dic.Add("attach", "");
            dic.Add("total_fee", total_fee);
            dic.Add("mch_create_ip", create_ip);
            dic.Add("notify_url", this.notifyUrl);
            dic.Add("time_start", time_start);
            dic.Add("time_expire", time_expire);
            dic.Add("goods_tag", "");
            dic.Add("product_id", "");
            dic.Add("nonce_str", GetNonceStr());


            dic.Add("sign", GetSign(dic));


            Dictionary<string, string> post_dic = new Dictionary<string, string>();
            foreach (var key in dic)
            {
                if (!string.IsNullOrEmpty(key.Value))
                {
                    post_dic.Add(key.Key, key.Value);
                }
            }

            var request = new AliNativePayRequest(dic);

            var requestResult = _api.DoPost(request);


            return requestResult;
        }

        public string PayTransQuery(string out_trade_no)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("service", "unified.trade.query");
            dic.Add("version", "");
            dic.Add("charset", "");
            dic.Add("sign_type", "");
            dic.Add("mch_id", this.mch_id);
            dic.Add("out_trade_no", out_trade_no);
            dic.Add("nonce_str", GetNonceStr());

            dic.Add("sign", GetSign(dic));


            Dictionary<string, string> post_dic = new Dictionary<string, string>();
            foreach (var key in dic)
            {
                if (!string.IsNullOrEmpty(key.Value))
                {
                    post_dic.Add(key.Key, key.Value);
                }
            }

            var request = new PayTransQueryRequest(dic);

            var requestResult = _api.DoPost(request);

            return _api.ResponseString;
        }

        public PayRefundResult PayRefund(string out_trade_no,string out_refund_no, string total_fee, string refund_fee, string op_user_id, string refund_channel)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("service", "unified.trade.refund");
            dic.Add("version", "");
            dic.Add("charset", "");
            dic.Add("sign_type", "");
            dic.Add("mch_id", this.mch_id);
            dic.Add("out_trade_no", out_trade_no);
            dic.Add("out_refund_no", out_refund_no);
            dic.Add("device_info", "");
            dic.Add("total_fee", total_fee);
            dic.Add("refund_fee", refund_fee);
            dic.Add("op_user_id", op_user_id);
            dic.Add("refund_channel", refund_channel);
            dic.Add("nonce_str", GetNonceStr());


            dic.Add("sign", GetSign(dic));


            Dictionary<string, string> post_dic = new Dictionary<string, string>();
            foreach (var key in dic)
            {
                if (!string.IsNullOrEmpty(key.Value))
                {
                    post_dic.Add(key.Key, key.Value);
                }
            }

            var request = new PayRefundRequest(dic);

            var requestResult = _api.DoPost(request);


            return requestResult;
        }

        public string PayRefundQuery(string out_trade_no, string out_refund_no)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("service", "unified.trade.query");
            dic.Add("version", "");
            dic.Add("charset", "");
            dic.Add("sign_type", "");
            dic.Add("mch_id", this.mch_id);
            dic.Add("out_trade_no", out_trade_no);
            dic.Add("out_refund_no", out_refund_no);
            dic.Add("nonce_str", GetNonceStr());

            dic.Add("sign", GetSign(dic));


            Dictionary<string, string> post_dic = new Dictionary<string, string>();
            foreach (var key in dic)
            {
                if (!string.IsNullOrEmpty(key.Value))
                {
                    post_dic.Add(key.Key, key.Value);
                }
            }

            var request = new PayRefundQueryRequest(dic);

            var requestResult = _api.DoPost(request);

            return _api.ResponseString;
        }
    }
}
