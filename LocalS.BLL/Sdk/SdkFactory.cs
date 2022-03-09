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

        public static ZfbSdkProvider Zfb
        {
            get
            {
                return new ZfbSdkProvider();
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

        public static EasemobSdkProvider Easemob
        {
            get
            {
                return new EasemobSdkProvider();
            }
        }

        public static Senviv4GProvider Senviv
        {
            get
            {
                return new Senviv4GProvider();
            }
        }
    }
}
