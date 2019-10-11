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

        public static MyAlipaySdkProvider Alipay
        {
            get
            {
                return new MyAlipaySdkProvider();
            }
        }

    }
}
