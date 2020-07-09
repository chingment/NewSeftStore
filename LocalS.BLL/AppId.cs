using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public static class AppId
    {
        public const string ACCOUNT = "account.17fanju.com";
        public const string ADMIN = "admin.17fanju.com";
        public const string MERCH = "merch.17fanju.com";
        public const string AGENT = "agent.17fanju.com";
        public const string WXMINPRAGROM = "mp.17fanju.com";
        public const string STORETERM = "com.uplink.selfstore";
        public const string SVCCHAT = "svcchat.17fanju.com";


        public static string GetName(string appId)
        {
            string name = "";
            switch (appId)
            {
                case AppId.ACCOUNT:
                    name = "个人中心";
                    break;
                case AppId.MERCH:
                    name = "商户运营系统";
                    break;
                case AppId.AGENT:
                    name = "商户代理系统";
                    break;
                case AppId.ADMIN:
                    name = "后台管理系统";
                    break;
                case AppId.WXMINPRAGROM:
                    name = "小程序";
                    break;
                case AppId.STORETERM:
                    name = "终端设备";
                    break;
                case AppId.SVCCHAT:
                    name = "音视频应用";
                    break;
            }

            return name;
        }
    }
}
