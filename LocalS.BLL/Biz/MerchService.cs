using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeiXinSdk;

namespace LocalS.BLL.Biz
{
    public class MerchService : BaseDbContext
    {
        public WxAppInfoConfig GetWxMpAppInfoConfig(string merchId, string appId)
        {

            var config = new WxAppInfoConfig();

            var merch = CurrentDb.Merch.Where(m => m.Id == merchId && m.WxMpAppId == appId).FirstOrDefault();
            if (merch == null)
                return null;


            config.AppId = merch.WxMpAppId;
            config.AppSecret = merch.WxMpAppSecret;
            config.PayMchId = merch.WxPayMchId;
            config.PayKey = merch.WxPayKey;
            config.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;

            return config;
        }

        public WxAppInfoConfig GetWxPaAppInfoConfig(string merchId, string appId)
        {

            var config = new WxAppInfoConfig();

            var merch = CurrentDb.Merch.Where(m => m.Id == merchId && m.WxPaAppId == appId).FirstOrDefault();
            if (merch == null)
                return null;


            config.AppId = merch.WxPaAppId;
            config.AppSecret = merch.WxPaAppSecret;
            config.PayMchId = merch.WxPayMchId;
            config.PayKey = merch.WxPayKey;
            config.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;

            return config;
        }
    }
}
