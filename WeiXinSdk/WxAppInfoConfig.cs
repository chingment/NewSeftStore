using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeiXinSdk
{
    public class WxAppInfoConfig
    {
        private string _sslCert_Path = "E:\\Web\\cereson\\WebMobileByFuLi\\cert\\apiclient_cert.p12";
        private string _sslCert_Password = "1486589902";

        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string PayMchId { get; set; }
        public string PayKey { get; set; }
        public string PayResultNotifyUrl { get; set; }
        public string NotifyEventUrlToken { get; set; }
        public string Oauth2RedirectUrl { get; set; }
        public string SslCert_Path
        {
            get
            {
                return _sslCert_Path;
            }
            set
            {
                _sslCert_Path = value;
            }
        }
        public string SslCert_Password
        {
            get
            {
                return _sslCert_Password;
            }
            set
            {
                _sslCert_Password = value;
            }
        }

        public string MyMerchId { get; set; }

        public string MyStoreId { get; set; }
    }
}
