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
        public WxAppInfoConfig GetWxMpAppInfoConfig(string merchId)
        {

            var config = new WxAppInfoConfig();

            var merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            if (merch == null)
                return null;


            config.AppId = merch.WxMpAppId;
            config.AppSecret = merch.WxMpAppSecret;
            config.PayMchId = merch.WxPayMchId;
            config.PayKey = merch.WxPayKey;
            config.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;

            return config;
        }

        public WxAppInfoConfig GetWxPaAppInfoConfig(string merchId)
        {

            var config = new WxAppInfoConfig();

            var merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            if (merch == null)
                return null;


            config.AppId = merch.WxPaAppId;
            config.AppSecret = merch.WxPaAppSecret;
            config.PayMchId = merch.WxPayMchId;
            config.PayKey = merch.WxPayKey;
            config.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;

            return config;
        }


        public string GetMachineName(string merchId, string machineId)
        {
            string machineName = "";
            var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == machineId).FirstOrDefault();
            if (merchMachine == null)
            {
                machineName = merchMachine.Name;
            }
            return machineName;
        }

        public string GetClientName(string merchId, string clientUserId)
        {
            string clientUserName = "匿名";
            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();
            if (clientUser != null)
            {
                clientUserName = clientUser.NickName;
            }

            return clientUserName;
        }
    }
}
