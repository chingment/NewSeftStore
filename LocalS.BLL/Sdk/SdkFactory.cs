using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public static class SdkFactory
    {
        public static WxSdkProvider Wx
        {
            get
            {
                return new WxSdkProvider();
            }
        }

        public static AliPaySdkProvider AliPay
        {
            get
            {
                return new AliPaySdkProvider();
            }
        }

        public static TgPaySdkProvider TgPay
        {
            get
            {
                return new TgPaySdkProvider();
            }
        }

        public static XtyPaySdkProvider XrtPay
        {
            get
            {
                return new XtyPaySdkProvider();
            }
        }

    }
}
