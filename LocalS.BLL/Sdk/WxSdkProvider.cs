using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json;
using MyWeiXinSdk;
using MyWeiXinSdk.Tenpay;
using System.Security.Cryptography;
using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
using LocalS.Entity;

namespace LocalS.BLL
{
    public class WxSdkProvider : BaseDbContext, IPaySdkProvider<WxAppInfoConfig>
    {
        private string AES_decrypt(string encryptedData, string iv, string sessionKey)
        {
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                //设置解密器参数            
                aes.Mode = CipherMode.CBC;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                //格式化待处理字符串          
                byte[] byte_encryptedData = Convert.FromBase64String(encryptedData);
                byte[] byte_iv = Convert.FromBase64String(iv);
                byte[] byte_sessionKey = Convert.FromBase64String(sessionKey);
                aes.IV = byte_iv;
                aes.Key = byte_sessionKey;
                //根据设置好的数据生成解密器实例            
                ICryptoTransform transform = aes.CreateDecryptor();
                //解密          
                byte[] final = transform.TransformFinalBlock(byte_encryptedData, 0, byte_encryptedData.Length);
                //生成结果         
                string result = Encoding.UTF8.GetString(final);


                return result;
            }
            catch
            {
                return null;
            }

        }

        public string GetJsApiTicket(WxAppInfoConfig config)
        {

            string key = string.Format("Wx_AppId_{0}_JsApiTicket", config.AppId);

            var redis = new RedisClient<string>();
            var jsApiTicket = redis.KGetString(key);

            if (jsApiTicket == null)
            {
                WxApi c = new WxApi();

                string access_token = GetApiAccessToken(config);

                var wxApiJsApiTicket = new WxApiJsApiTicket(access_token);

                var wxApiJsApiTicketResult = c.DoGet(wxApiJsApiTicket);
                if (string.IsNullOrEmpty(wxApiJsApiTicketResult.ticket))
                {
                    LogUtil.Info(string.Format("获取微信JsApiTicket，key：{0}，已过期，Api重新获取失败", key));
                }
                else
                {
                    LogUtil.Info(string.Format("获取微信JsApiTicket，key：{0}，value：{1}，已过期，重新获取成功", key, wxApiJsApiTicketResult.ticket));

                    jsApiTicket = wxApiJsApiTicketResult.ticket;

                    redis.KSet(key, jsApiTicket, new TimeSpan(0, 30, 0));
                }
            }
            else
            {
                LogUtil.Info(string.Format("获取微信JsApiTicket，key：{0}，value：{1}", key, jsApiTicket));
            }

            return jsApiTicket;

        }

        public PayBuildWxJsPayInfoResult PayBuildWxJsPayInfo(WxAppInfoConfig config, string merch_id, string store_id, string machine_id, string open_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime? time_expire = null)
        {
            var result = new PayBuildWxJsPayInfoResult();

            var ret = new UnifiedOrderResult();

            TenpayUtil tenpayUtil = new TenpayUtil(config);

            UnifiedOrder unifiedOrder = new UnifiedOrder();
            unifiedOrder.openid = open_id;
            unifiedOrder.out_trade_no = order_sn;//商户订单号
            unifiedOrder.spbill_create_ip = create_ip;//终端IP
            unifiedOrder.total_fee = Convert.ToInt32(order_amount * 100);//标价金额
            unifiedOrder.body = body;//商品描述  
            unifiedOrder.trade_type = "JSAPI";
            //unifiedOrder.attach = attach.ToJsonString();
            if (time_expire != null)
            {
                unifiedOrder.time_expire = time_expire.Value.ToString("yyyyMMddHHmmss");
            }

            if (!string.IsNullOrEmpty(goods_tag))
            {
                unifiedOrder.goods_tag = goods_tag;
            }

            ret = tenpayUtil.UnifiedOrder(unifiedOrder);

            if (ret != null)
            {

                var pms = SdkFactory.Wx.GetJsApiPayParams(config, ret.PrepayId);
                if (pms != null)
                {
                    result = new PayBuildWxJsPayInfoResult();
                    result.NonceStr = pms.nonceStr;
                    result.Package = pms.package;
                    result.PaySign = pms.paySign;
                    result.SignType = pms.signType;
                    result.Timestamp = pms.timestamp;
                }

            }



            return result;

        }

        public PayBuildQrCodeResult PayBuildQrCode(WxAppInfoConfig config, E_PayCaller payCaller, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime? time_expire)
        {

            var result = new PayBuildQrCodeResult();

            TenpayUtil tenpayUtil = new TenpayUtil(config);

            UnifiedOrder unifiedOrder = new UnifiedOrder();
            unifiedOrder.openid = "";
            unifiedOrder.out_trade_no = order_sn;//商户订单号
            unifiedOrder.spbill_create_ip = create_ip;//终端IP
            unifiedOrder.total_fee = Convert.ToInt32(order_amount * 100);//标价金额
            unifiedOrder.body = body;//商品描述  
            unifiedOrder.trade_type = "NATIVE";
            if (time_expire != null)
            {
                unifiedOrder.time_expire = time_expire.Value.ToString("yyyyMMddHHmmss");
            }
            unifiedOrder.goods_tag = goods_tag;

            //unifiedOrder.attach = attach.ToJsonString();

            var ret = tenpayUtil.UnifiedOrder(unifiedOrder);

            if (ret != null)
            {
                result.CodeUrl = ret.CodeUrl;
            }

            return result;

        }

        public string GetWebAuthorizeUrl(WxAppInfoConfig config, string returnUrl)
        {
            return OAuthApi.GetAuthorizeUrl(config.AppId, config.Oauth2RedirectUrl + "?returnUrl=" + returnUrl);
        }

        public WxApiSnsOauth2AccessTokenResult GetWebOauth2AccessToken(WxAppInfoConfig config, string code)
        {
            return OAuthApi.GetWebOauth2AccessToken(config.AppId, config.AppSecret, code);
        }

        public string GetApiAccessToken(WxAppInfoConfig config)
        {
            string wxAccessToken = System.Configuration.ConfigurationManager.AppSettings["custom:WxTestAccessToken"];
            if (wxAccessToken != null)
            {
                return wxAccessToken;
            }

            string key = string.Format("Wx_AppId_{0}_AccessToken", config.AppId);

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，重新获取", key));

                WxApi c = new WxApi();

                WxApiAccessToken apiAccessToken = new WxApiAccessToken("client_credential", config.AppId, config.AppSecret);

                var apiAccessTokenResult = c.DoGet(apiAccessToken);

                if (string.IsNullOrEmpty(apiAccessTokenResult.access_token))
                {
                    LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，Api重新获取失败", key));
                }
                else
                {
                    LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}，已过期，重新获取成功", key, apiAccessTokenResult.access_token));

                    accessToken = apiAccessTokenResult.access_token;

                    redis.KSet(key, accessToken, new TimeSpan(0, 30, 0));
                }

            }
            else
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}", key, accessToken));
            }

            return accessToken;
        }

        public WxApiSnsUserInfoResult GetUserInfo(string accessToken, string openId)
        {
            return OAuthApi.GetUserInfo(accessToken, openId);
        }

        public UserInfoModelByMinProramJsCode GetUserInfoByMinProramJsCode(WxAppInfoConfig config, string encryptedData, string iv, string code)
        {
            try
            {
                var jsCode2Session = OAuthApi.GetWxApiJsCode2Session(config.AppId, config.AppSecret, code);
                string strData = AES_decrypt(encryptedData, iv, jsCode2Session.session_key);
                LogUtil.Info("UserInfo:" + strData);
                var obj = JsonConvert.DeserializeObject<UserInfoModelByMinProramJsCode>(strData);

                return obj;
            }
            catch
            {
                return null;
            }
        }

        public string CardCodeDecrypt(WxAppInfoConfig config, string encrypt_code)
        {
            return OAuthApi.CardCodeDecrypt(this.GetApiAccessToken(config), encrypt_code);
        }

        public WxApiUserInfoResult GetUserInfoByApiToken(WxAppInfoConfig config, string openId)
        {
            return OAuthApi.GetUserInfoByApiToken(this.GetApiAccessToken(config), openId);
        }

        public List<string> GetUserOpenIds(WxAppInfoConfig config)
        {
            return OAuthApi.GetUserOpenIds(this.GetApiAccessToken(config));
        }

        public CustomJsonResult<JsApiConfigParams> GetJsApiConfigParams(WxAppInfoConfig config, string url)
        {
            string jsApiTicket = GetJsApiTicket(config);

            JsApiConfigParams parms = new JsApiConfigParams(config.AppId, url, jsApiTicket);

            return new CustomJsonResult<JsApiConfigParams>(ResultType.Success, ResultCode.Success, "", parms);
        }

        public JsApiPayParams GetJsApiPayParams(WxAppInfoConfig config, string prepayId)
        {
            JsApiPayParams parms = new JsApiPayParams(config.AppId, config.PayKey, prepayId);

            return parms;
        }

        public string GetNotifyEventUrlToken(WxAppInfoConfig config)
        {
            return config.NotifyEventUrlToken;
        }

        public string GetCardApiTicket(WxAppInfoConfig config)
        {

            string key = string.Format("Wx_AppId_{0}_CardApiTicket", config.AppId);

            var redis = new RedisClient<string>();
            var jsApiTicket = redis.KGetString(key);

            if (jsApiTicket == null)
            {
                WxApi c = new WxApi();

                string access_token = GetApiAccessToken(config);

                var wxApiGetCardApiTicket = new WxApiGetCardApiTicket(access_token);

                var wxApiGetCardApiTicketResult = c.DoGet(wxApiGetCardApiTicket);
                if (string.IsNullOrEmpty(wxApiGetCardApiTicketResult.ticket))
                {
                    LogUtil.Info(string.Format("获取微信JsApiTicket，key：{0}，已过期，Api重新获取失败", key));
                }
                else
                {
                    LogUtil.Info(string.Format("获取微信JsApiTicket，key：{0}，value：{1}，已过期，重新获取成功", key, wxApiGetCardApiTicketResult.ticket));

                    jsApiTicket = wxApiGetCardApiTicketResult.ticket;

                    redis.KSet(key, jsApiTicket, new TimeSpan(0, 30, 0));
                }
            }
            else
            {
                LogUtil.Info(string.Format("获取微信JsApiTicket，key：{0}，value：{1}", key, jsApiTicket));
            }

            return jsApiTicket;

        }

        public string UploadMultimediaImage(WxAppInfoConfig config, string imageUrl)
        {
            return OAuthApi.UploadMultimediaImage(this.GetApiAccessToken(config), imageUrl);
        }

        public string PayQuery(WxAppInfoConfig config, string orderId)
        {
            CustomJsonResult result = new CustomJsonResult();
            TenpayUtil tenpayUtil = new TenpayUtil(config);
            string xml = tenpayUtil.OrderQuery(orderId);

            return xml;
        }

        public bool CheckPayNotifySign(WxAppInfoConfig config, string xml)
        {

            var dic1 = MyWeiXinSdk.CommonUtil.XmlToDictionary(xml);

            if (dic1["sign"] == null)
            {
                return false;
            }

            string wxSign = dic1["sign"].ToString();


            bool isFlag = true;
            string buff = "";
            foreach (KeyValuePair<string, object> pair in dic1)
            {
                if (pair.Value == null)
                {
                    isFlag = false;
                    break;
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }

            if (!isFlag)
            {
                return false;
            }

            buff = buff.Trim('&');


            //在string后加入API KEY
            buff += "&key=" + config.PayKey;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(buff));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            string mySign = sb.ToString().ToUpper();

            if (wxSign != mySign)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public PayTransResult Convert2PayResultByPayQuery(WxAppInfoConfig config, string content)
        {
            var result = new PayTransResult();

            var dic = MyWeiXinSdk.CommonUtil.XmlToDictionary(content);
            if (dic.ContainsKey("out_trade_no"))
            {
                result.PayTransId = dic["out_trade_no"].ToString();
            }

            if (dic.ContainsKey("transaction_id"))
            {
                result.PayPartnerPayTransId = dic["transaction_id"].ToString();
            }

            LogUtil.Info("解释微信支付协议，订单号：" + result.PayTransId);


            if (dic.ContainsKey("out_trade_no") && dic.ContainsKey("trade_state"))
            {
                string trade_state = dic["trade_state"].ToString();
                LogUtil.Info("解释微信支付协议，（trade_state）订单状态：" + trade_state);
                if (trade_state == "SUCCESS")
                {
                    result.IsPaySuccess = true;
                    result.PayWay = E_PayWay.Wx;
                }
            }


            return result;
        }

        public PayTransResult Convert2PayResultByNotifyUrl(WxAppInfoConfig config, string content)
        {
            var result = new PayTransResult();

            var dic = MyWeiXinSdk.CommonUtil.XmlToDictionary(content);

            if (dic.ContainsKey("out_trade_no"))
            {
                result.PayTransId = dic["out_trade_no"].ToString();
            }

            if (dic.ContainsKey("transaction_id"))
            {
                result.PayPartnerPayTransId = dic["transaction_id"].ToString();
            }

            LogUtil.Info("解释微信支付协议，订单号：" + result.PayTransId);



            if (dic.ContainsKey("result_code"))
            {
                string result_code = dic["result_code"].ToString();
                LogUtil.Info("解释微信支付协议，（result_code）订单状态：" + result_code);
                if (result_code == "SUCCESS")
                {
                    result.IsPaySuccess = true;
                    result.PayWay = E_PayWay.Wx;
                }
            }

            return result;
        }


        public void GiftvoucherActivityNotifyPick(string body, string opendId, string orderSn, string pickAddress, string pickCode, string productskusName, DateTime lastPickTime, string url)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + opendId + "\",");
            sb.Append("\"template_id\":\"M_D_LQGahaalSEt44QI22GY5ihB4zfusTYnvJrnNvN0\",");
            sb.Append("\"url\":\"" + url + "\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + body + "。\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + orderSn + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + productskusName + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword3\":{ \"value\":\"" + pickAddress + "\",\"color\":\"#FF3030\" },");
            sb.Append("\"keyword4\":{ \"value\":\"" + pickCode + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"请您在" + lastPickTime + "前去取货，否则取货码将会失效，谢谢。\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxAppInfoConfig config = new WxAppInfoConfig();
            config.AppId = "wxc6e80f8c575cf3f5";
            config.AppSecret = "fee895c9923da26a4d42d9c435202b37";

            string access_token = GetApiAccessToken(config);

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(access_token, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            c.DoPost(templateSend);
        }

        public PayRefundResult PayRefund(WxAppInfoConfig config, string payTranId, string payRefundId, decimal total_fee, decimal refund_fee, string refund_desc)
        {
            var result = new PayRefundResult();
            TenpayUtil tenpayUtil = new TenpayUtil(config);

            var orderPayRefund = tenpayUtil.OrderPayRefund(payTranId, payRefundId, Convert.ToInt32(total_fee * 100).ToString(), Convert.ToInt32(refund_fee * 100).ToString(), refund_desc);

            result.Status = orderPayRefund.Status;

            return result;
        }
        public string PayRefundQuery(WxAppInfoConfig config, string payRefundId)
        {
            return null;
        }


        //public string OrderPayReFund(string comCode, string orderId, string orderReFundSn, decimal totalFee, decimal refundFee, string refundDesc)
        //{
        //    wxConfig = GetWxConfig(comCode);
        //    CustomJsonResult result = new CustomJsonResult();
        //    TenpayUtil tenpayUtil = new TenpayUtil(wxConfig);
        //    string out_trade_no = orderId;
        //    string out_refund_no = orderReFundSn;
        //    string total_fee = Convert.ToInt32(totalFee * 100).ToString();
        //    string refund_fee = Convert.ToInt32(refundFee * 100).ToString();
        //    string xml = tenpayUtil.OrderPayReFund(out_trade_no, out_refund_no, total_fee, refund_fee, refundDesc);
        //    return xml;
        //}

        //public string OrderRefundQuery(string comCode, string out_refund_no)
        //{
        //    wxConfig = GetWxConfig(comCode);
        //    CustomJsonResult result = new CustomJsonResult();
        //    TenpayUtil tenpayUtil = new TenpayUtil(wxConfig);
        //    string xml = tenpayUtil.OrderRefundQuery(out_refund_no);
        //    return xml;
        //}
    }
}
