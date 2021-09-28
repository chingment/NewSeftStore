using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeiXinSdk
{
    public class WxAppInfoConfig
    {
        //公众号，小程序 AppId
        public string AppId { get; set; }
        //公众号，小程序 AppSecret
        public string AppSecret { get; set; }
        //支付商户号
        public string PayMchId { get; set; }
        //支付商户号密钥
        public string PayKey { get; set; }
        //支付结果推送URL
        public string PayResultNotifyUrl { get; set; }
        //退款结果推送URL
        public string RefundResultNotifyUrl { get; set; }
        //App应用Token
        public string NotifyEventUrlToken { get; set; }
        //公众号身份验证跳转
        public string Oauth2RedirectUrl { get; set; }
        //支付证书路径
        public string SslCert_Path { get; set; }
        //支付证书密钥
        public string SslCert_Password { get; set; }

        //第三方已对接AccessToken，首先引用
        public string TrdAccessToken { get; set; }

    }
}
