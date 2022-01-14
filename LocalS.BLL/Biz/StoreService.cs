using LocalS.Entity;
using Lumos;
using MyAlipaySdk;
using MyWeiXinSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgPaySdk;
using XrtPaySdk;

namespace LocalS.BLL.Biz
{
    public class StoreService : BaseService
    {
        public StoreModel GetOne(string id)
        {
            var m_Store = new StoreModel();

            var d_Store = CurrentDb.Store.Where(m => m.Id == id).FirstOrDefault();

            if (d_Store == null)
                return null;

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == d_Store.MerchId).FirstOrDefault();

            m_Store.StoreId = d_Store.Id;
            m_Store.Name = d_Store.Name;
            m_Store.MerchId = d_Store.MerchId;
            m_Store.MerchName = d_Merch.Name;
            m_Store.BriefDes = d_Store.BriefDes;
            m_Store.IsDelete = d_Store.IsDelete;
            m_Store.IsTestMode = d_Store.IsTestMode;
            return m_Store;
        }

        public WxAppConfig GetWxMpAppInfoConfig(string storeId)
        {

            var m_Config = new WxAppConfig();

            var d_Store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();
            if (d_Store == null)
                return null;


            m_Config.AppId = d_Store.WxMpAppId;
            m_Config.AppSecret = d_Store.WxMpAppSecret;
            m_Config.PayMchId = d_Store.WxPayMchId;
            m_Config.PayKey = d_Store.WxPayKey;
            m_Config.PayResultNotifyUrl = d_Store.WxPayResultNotifyUrl;
            m_Config.SslCert_Path = d_Store.WxPayCertPath;
            m_Config.SslCert_Password = d_Store.WxPayCertPassword;
            return m_Config;
        }

        public WxAppConfig GetWxMpAppInfoConfigByAppId(string appId)
        {

            var m_Config = new WxAppConfig();

            var d_Store = CurrentDb.Store.Where(m => m.WxMpAppId == appId).FirstOrDefault();
            if (d_Store == null)
                return null;


            m_Config.AppId = d_Store.WxMpAppId;
            m_Config.AppSecret = d_Store.WxMpAppSecret;
            m_Config.PayMchId = d_Store.WxPayMchId;
            m_Config.PayKey = d_Store.WxPayKey;
            m_Config.PayResultNotifyUrl = d_Store.WxPayResultNotifyUrl;
            m_Config.SslCert_Path = d_Store.WxPayCertPath;
            m_Config.SslCert_Password = d_Store.WxPayCertPassword;

            return m_Config;
        }

        public ZfbAppInfoConfig GetZfbMpAppInfoConfig(string storeId)
        {

            var m_Config = new ZfbAppInfoConfig();

            var d_Store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();
            if (d_Store == null)
                return null;


            m_Config.AppId = d_Store.ZfbMpAppId;
            m_Config.AppPrivateKey = d_Store.ZfbMpAppPrivateSecret;
            m_Config.ZfbPublicKey = d_Store.ZfbPublicSecret;
            m_Config.PayResultNotifyUrl = d_Store.ZfbResultNotifyUrl;

            return m_Config;
        }

        public TgPayInfoConfg GetTgPayInfoConfg(string storeId)
        {

            var m_Confg = new TgPayInfoConfg();

            var d_Store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();
            if (d_Store == null)
                return null;


            m_Confg.Account = d_Store.TgPayAccount;
            m_Confg.Key = d_Store.TgPayKey;
            m_Confg.PayResultNotifyUrl = d_Store.TgPayResultNotifyUrl;

            return m_Confg;
        }

        public XrtPayInfoConfg GetXrtPayInfoConfg(string storeId)
        {
            var m_Config = new XrtPayInfoConfg();

            var d_Store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();
            if (d_Store == null)
                return null;


            m_Config.Mch_id = d_Store.XrtPayMchId;
            m_Config.Key = d_Store.XrtPayKey;
            m_Config.PayResultNotifyUrl = d_Store.XrtPayResultNotifyUrl;

            return m_Config;
        }
    }
}
