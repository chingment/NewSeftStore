using MyAlipaySdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWeiXinSdk;
using TgPaySdk;
using XrtPaySdk;

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

        //public WxAppInfoConfig GetWxPaAppInfoConfig(string merchId)
        //{

        //    var config = new WxAppInfoConfig();

        //    var merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
        //    if (merch == null)
        //        return null;


        //    config.AppId = merch.WxPaAppId;
        //    config.AppSecret = merch.WxPaAppSecret;
        //    config.PayMchId = merch.WxPayMchId;
        //    config.PayKey = merch.WxPayKey;
        //    config.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;

        //    return config;
        //}

        public ZfbAppInfoConfig GetZfbMpAppInfoConfig(string merchId)
        {

            var config = new ZfbAppInfoConfig();

            var merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            if (merch == null)
                return null;


            config.AppId = merch.ZfbMpAppId;
            config.AppPrivateKey = merch.ZfbMpAppPrivateSecret;
            config.ZfbPublicKey = merch.ZfbPublicSecret;
            config.PayResultNotifyUrl = merch.ZfbResultNotifyUrl;

            return config;
        }

        public TgPayInfoConfg GetTgPayInfoConfg(string merchId)
        {

            var config = new TgPayInfoConfg();

            var merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            if (merch == null)
                return null;


            config.Account = merch.TgPayAccount;
            config.Key = merch.TgPayKey;
            config.PayResultNotifyUrl = merch.TgPayResultNotifyUrl;

            return config;
        }

        public XrtPayInfoConfg GetXrtPayInfoConfg(string merchId)
        {
            var config = new XrtPayInfoConfg();

            var merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            if (merch == null)
                return null;


            config.Mch_id = merch.XrtPayMchId;
            config.Key = merch.XrtPayKey;
            config.PayResultNotifyUrl = merch.XrtPayResultNotifyUrl;

            return config;
        }


        public string GetMerchName(string merchId)
        {

            var merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();

            if (merch == null)
                return null;

            return merch.Name;
        }

        public string GetMachineName(string merchId, string machineId)
        {
            string machineName = "";
            var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == machineId).FirstOrDefault();
            if (merchMachine != null)
            {
                machineName = merchMachine.Name;
            }
            return machineName;
        }

        public string GetClientName(string merchId, string clientUserId)
        {
            string clientUserName = "匿名";
            var clientUser = CurrentDb.SysUser.Where(m => m.Id == clientUserId).FirstOrDefault();
            if (clientUser != null)
            {
                clientUserName = clientUser.NickName;
            }

            return clientUserName;
        }

        public string GetStoreName(string merchId, string storeId)
        {

            var store = CurrentDb.Store.Where(m => m.Id == storeId && m.MerchId == merchId).FirstOrDefault();

            if (store == null)
                return null;

            return store.Name;
        }
    }
}
