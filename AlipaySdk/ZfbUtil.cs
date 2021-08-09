using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlipaySdk
{
    public class ZfbUtil
    {
        private ZfbAppInfoConfig _config = null;
        private IAopClient _client = null;
        public ZfbUtil(ZfbAppInfoConfig config)
        {
            this._config = config;

            string URL = "https://openapi.alipay.com/gateway.do";  //沙箱环境与正式环境不同 这里要用沙箱的 支付宝地址
            //	APPID即创建应用后生成
            string APPID = _config.AppId;
            //开发者应用私钥，由开发者自己生成  开发者私钥到底是什么玩意  原来开发者私钥就是商户应用私钥
            string APP_PRIVATE_KEY = _config.AppPrivateKey;// "MIIEowIBAAKCAQEAnITKqwr6QtAvquzyGMbrJA7p2gQ6q4AUCSFlv4/l0vFB9kmrNQH6y1IVAhiKlaBrtxIqviQ5d6o+HhgW7StlPJ3RRqoHf22yoyqBK4Kv6cIOBaJY0Oz08trCVkgdyThIbWoTi1lDRb28XpTwRh1UryxTLOVzZgaeNeFqev6xpQziWcdFjvxUQEhZLOIARX2nPski9LDJWES8lxAUFvQ0R9TE+KnfNmxataJKEqj4w5yLoD4+cR/UbC+vJgSeURsPoVyp+0SzZMOeWOHpyGv1U1D77Vxy8ThECGAvJksVpS7s3PI7eoN6dte7CPU666aR15ujMpD8cHhAHgk6D27VjwIDAQABAoIBAGVa3H7siekQNX4DGDcRQR3FhovGp1N9ifvro57sCRCTaHqbdAHMeWKGkdIcoEmKa5ZObl1YydoC5VzJjrcgndsl+2o+GsLa/44HrUPGyjGel0PwwIDyoSBOpwAGjTtLdiNcSLG5KQYVetDos7tDrR4OnwH4x8Suzm7nvPNO0SymavKGSeqlX9GrTNlUG5vzsZwiDb2cUsqPB836+nSDuOiW2gc1oDfQi7CdG1qYBlVxUIS+zKHg+PoRIkpIWAqKvwZOecMrApDMGN6ng/hl5bKx5QJ8d/ReeNb0UskpekXXibgZy8vkcxfZSI4YjzA3+taUuTrLRCKkUcyk/bDyPLECgYEAzHb4bzU1r9iOJh0uiaSEe04zV4Ys1XX2PCJZVyVFvVCpT3jT2OASjaukpcrVYEt2vgJfq48ZLnFzZJ81wkjxR/j/49kbPZMNZlOV9uA00H7vJgUwW2IUhhtRsCEHpPM64HyLG6k3Bm1pLdNYlpxwIDt3gUCJwrNy4UvZahzPVccCgYEAw/geRcP9K53MlRrOedTfWC5O+j/hVaRxDU0n8pE2tIQc/wCxEanA0+DprnDJ8cwTW2QMivPY8X8DwiFa2kDJ44tNZcJj6epzqu9zqTfp2MpSrCTPmQwLqP9t/9Q20Xk+8Msmvk2Yrp6hM5GeweYNct4FdDvAKSgMHiTmwSOcYfkCgYAJPa9Ix2zfv7fc/SCnU+ow8H3djNDl7OjuGtdS2vpl7glY8CsS+D2ebY2JeLtgGMkLGWxdgqAuuy3t9EwntchB6n0WPS+//q9yWoDCoauBaNtCKqXe23X2AbIVdci0qdGVZ8uZunIkNjm0uoKcfAGNU2K5UsNnK1kb2aO/6gFs7QKBgDIKhrhr7pcXqWkdukHUAOBEmvg+Ha0/23p5DE1dlWmNHtZi99Q507qHAUUBGiA7a2n351gIIoqwU2ZcHBYFW0hWhwIIHHlb3AN3N8KrO3SXXXsFv1kmgUe7Sfx81S6yVkcoqREJQYa9jQ5dDfwXYbHGTgA7Tbt0tXtxEteY31MJAoGBAIMtfVcenwj48WpcE9gUWfPtqxGF0uj69pz6jpqBnbOWSSPSzYW0wBTmS0eFE4rrOgzBA5XOvy3ER5KgR7MPTEaqUq/CnqEqctkLQhf026ExElJ0mPegfy10DgjhTjeCBXQ3pl+eoDsn/fmrL4pOlwyW2D45RiOBLc2nh3nno38I";
            //参数返回格式，只支持json
            string FORMAT = "json";
            //请求和签名使用的字符编码格式，支持GBK和UTF-8
            string CHARSET = "UTF-8";
            //支付宝公钥，由支付宝生成     到蚂蚁金服复制
            string ALIPAY_PUBLIC_KEY = _config.ZfbPublicKey;// "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDDI6d306Q8fIfCOaTXyiUeJHkrIvYISRcc73s3vF1ZT7XN8RNPwJxo8pWaJMmvyTn9N4HQ632qJBVHf8sxHi/fEsraprwCtzvzQETrNRwVxLO5jVmRGi60j8Ue1efIlzPXV9je9mkjzOmdssymZkh2QhUrCmZYI/FCEa3/cNMW0QIDAQAB";
            _client = new DefaultAopClient(URL, APPID, APP_PRIVATE_KEY, FORMAT, "1.0", "RSA2", ALIPAY_PUBLIC_KEY, CHARSET, false);
            //实例化具体API对应的request类,类名称和接口名称对应,当前调用接口名称如：

            LogUtil.Info("APPID:" + APPID);
            LogUtil.Info("APP_PRIVATE_KEY:" + APP_PRIVATE_KEY);
            LogUtil.Info("ALIPAY_PUBLIC_KEY:" + ALIPAY_PUBLIC_KEY);
        }

        public UnifiedOrderResult UnifiedOrder(UnifiedOrder order)
        {
            var rt = new UnifiedOrderResult();

            //const string URL = "https://openapi.alipay.com/gateway.do";  //沙箱环境与正式环境不同 这里要用沙箱的 支付宝地址
            ////	APPID即创建应用后生成
            //string APPID = _config.AppId;
            ////开发者应用私钥，由开发者自己生成  开发者私钥到底是什么玩意  原来开发者私钥就是商户应用私钥
            //string APP_PRIVATE_KEY = _config.AppPrivateKey;// "MIIEowIBAAKCAQEAnITKqwr6QtAvquzyGMbrJA7p2gQ6q4AUCSFlv4/l0vFB9kmrNQH6y1IVAhiKlaBrtxIqviQ5d6o+HhgW7StlPJ3RRqoHf22yoyqBK4Kv6cIOBaJY0Oz08trCVkgdyThIbWoTi1lDRb28XpTwRh1UryxTLOVzZgaeNeFqev6xpQziWcdFjvxUQEhZLOIARX2nPski9LDJWES8lxAUFvQ0R9TE+KnfNmxataJKEqj4w5yLoD4+cR/UbC+vJgSeURsPoVyp+0SzZMOeWOHpyGv1U1D77Vxy8ThECGAvJksVpS7s3PI7eoN6dte7CPU666aR15ujMpD8cHhAHgk6D27VjwIDAQABAoIBAGVa3H7siekQNX4DGDcRQR3FhovGp1N9ifvro57sCRCTaHqbdAHMeWKGkdIcoEmKa5ZObl1YydoC5VzJjrcgndsl+2o+GsLa/44HrUPGyjGel0PwwIDyoSBOpwAGjTtLdiNcSLG5KQYVetDos7tDrR4OnwH4x8Suzm7nvPNO0SymavKGSeqlX9GrTNlUG5vzsZwiDb2cUsqPB836+nSDuOiW2gc1oDfQi7CdG1qYBlVxUIS+zKHg+PoRIkpIWAqKvwZOecMrApDMGN6ng/hl5bKx5QJ8d/ReeNb0UskpekXXibgZy8vkcxfZSI4YjzA3+taUuTrLRCKkUcyk/bDyPLECgYEAzHb4bzU1r9iOJh0uiaSEe04zV4Ys1XX2PCJZVyVFvVCpT3jT2OASjaukpcrVYEt2vgJfq48ZLnFzZJ81wkjxR/j/49kbPZMNZlOV9uA00H7vJgUwW2IUhhtRsCEHpPM64HyLG6k3Bm1pLdNYlpxwIDt3gUCJwrNy4UvZahzPVccCgYEAw/geRcP9K53MlRrOedTfWC5O+j/hVaRxDU0n8pE2tIQc/wCxEanA0+DprnDJ8cwTW2QMivPY8X8DwiFa2kDJ44tNZcJj6epzqu9zqTfp2MpSrCTPmQwLqP9t/9Q20Xk+8Msmvk2Yrp6hM5GeweYNct4FdDvAKSgMHiTmwSOcYfkCgYAJPa9Ix2zfv7fc/SCnU+ow8H3djNDl7OjuGtdS2vpl7glY8CsS+D2ebY2JeLtgGMkLGWxdgqAuuy3t9EwntchB6n0WPS+//q9yWoDCoauBaNtCKqXe23X2AbIVdci0qdGVZ8uZunIkNjm0uoKcfAGNU2K5UsNnK1kb2aO/6gFs7QKBgDIKhrhr7pcXqWkdukHUAOBEmvg+Ha0/23p5DE1dlWmNHtZi99Q507qHAUUBGiA7a2n351gIIoqwU2ZcHBYFW0hWhwIIHHlb3AN3N8KrO3SXXXsFv1kmgUe7Sfx81S6yVkcoqREJQYa9jQ5dDfwXYbHGTgA7Tbt0tXtxEteY31MJAoGBAIMtfVcenwj48WpcE9gUWfPtqxGF0uj69pz6jpqBnbOWSSPSzYW0wBTmS0eFE4rrOgzBA5XOvy3ER5KgR7MPTEaqUq/CnqEqctkLQhf026ExElJ0mPegfy10DgjhTjeCBXQ3pl+eoDsn/fmrL4pOlwyW2D45RiOBLc2nh3nno38I";
            ////参数返回格式，只支持json
            //const string FORMAT = "json";
            ////请求和签名使用的字符编码格式，支持GBK和UTF-8
            //const string CHARSET = "UTF-8";
            ////支付宝公钥，由支付宝生成     到蚂蚁金服复制
            //string ALIPAY_PUBLIC_KEY = _config.AlipayPublicKey;// "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDDI6d306Q8fIfCOaTXyiUeJHkrIvYISRcc73s3vF1ZT7XN8RNPwJxo8pWaJMmvyTn9N4HQ632qJBVHf8sxHi/fEsraprwCtzvzQETrNRwVxLO5jVmRGi60j8Ue1efIlzPXV9je9mkjzOmdssymZkh2QhUrCmZYI/FCEa3/cNMW0QIDAQAB";
            //IAopClient client = new DefaultAopClient(URL, APPID, APP_PRIVATE_KEY, FORMAT, "1.0", "RSA2", ALIPAY_PUBLIC_KEY, CHARSET, false);



            //实例化具体API对应的request类,类名称和接口名称对应,当前调用接口名称如：
            AlipayTradePrecreateRequest request = new AlipayTradePrecreateRequest();//创建API对应的request类
            //SDK已经封装掉了公共参数，这里只需要传入业务参数
            request.SetNotifyUrl(_config.PayResultNotifyUrl);

            //此次只是参数展示，未进行字符串转义，实际情况下请转义
            request.BizContent = Newtonsoft.Json.JsonConvert.SerializeObject(order);

            //"{" +
            //                    "    \"out_trade_no\":\"20150320010101001\"," +
            //                    "    \"total_amount\":\"88.88\"," +
            //                    "    \"subject\":\"Iphone6 16G\"," +
            //                    "    \"store_id\":\"NJ_001\"," +
            //                    "    \"timeout_express\":\"90m\"}";

            LogUtil.Info("BizContent:" + request.BizContent);


            AlipayTradePrecreateResponse response = _client.Execute(request);
            //调用成功，则处理业务逻辑
            if (!response.IsError)
            {
                rt.CodeUrl = response.QrCode;
            }
            else
            {
                LogUtil.Info("Error:" + Newtonsoft.Json.JsonConvert.SerializeObject(response));
            }

            LogUtil.Info("CodeUrl:" + rt.CodeUrl);

            return rt;
        }


        public string OrderQuery(string out_trade_no)
        {
            AlipayTradeQueryRequest request = new AlipayTradeQueryRequest();
            request.BizContent = "{\"out_trade_no\":\"" + out_trade_no + "\"}";
            AlipayTradeQueryResponse response = _client.Execute(request);

            LogUtil.Info("OrderQuery->response:" + response.ToJsonString());

            return response.Body;
        }

        public AlipayTradeRefundResponse PayRefund(string out_trade_no, decimal refund_amount, string refund_reason)
        {
            AlipayTradeRefundRequest request = new AlipayTradeRefundRequest();

            var bizContent = new
            {
                out_trade_no = out_trade_no,
                refund_amount = refund_amount,
                refund_reason = refund_reason
            };

            request.BizContent = bizContent.ToJsonString();

            AlipayTradeRefundResponse response = _client.Execute(request);

            LogUtil.Info("PayRefund->response:" + response.ToJsonString());

            return response;
        }

    }
}
