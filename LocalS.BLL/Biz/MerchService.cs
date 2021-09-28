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
    public class MerchService : BaseService
    {
        public WxAppInfoConfig GetWxMpAppInfoConfig(string merchId)
        {

            var m_Config = new WxAppInfoConfig();

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            if (d_Merch == null)
                return null;


            m_Config.AppId = d_Merch.WxMpAppId;
            m_Config.AppSecret = d_Merch.WxMpAppSecret;
            m_Config.PayMchId = d_Merch.WxPayMchId;
            m_Config.PayKey = d_Merch.WxPayKey;
            m_Config.PayResultNotifyUrl = d_Merch.WxPayResultNotifyUrl;
            m_Config.SslCert_Path = d_Merch.WxPayCertPath;
            m_Config.SslCert_Password = d_Merch.WxPayCertPassword;
            return m_Config;
        }

        public WxAppInfoConfig GetWxMpAppInfoConfigByAppId(string appId)
        {

            var m_Config = new WxAppInfoConfig();

            var d_Merch = CurrentDb.Merch.Where(m => m.WxMpAppId == appId).FirstOrDefault();
            if (d_Merch == null)
                return null;


            m_Config.AppId = d_Merch.WxMpAppId;
            m_Config.AppSecret = d_Merch.WxMpAppSecret;
            m_Config.PayMchId = d_Merch.WxPayMchId;
            m_Config.PayKey = d_Merch.WxPayKey;
            m_Config.PayResultNotifyUrl = d_Merch.WxPayResultNotifyUrl;
            m_Config.SslCert_Path = d_Merch.WxPayCertPath;
            m_Config.SslCert_Password = d_Merch.WxPayCertPassword;

            return m_Config;
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

            var m_Config = new ZfbAppInfoConfig();

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            if (d_Merch == null)
                return null;


            m_Config.AppId = d_Merch.ZfbMpAppId;
            m_Config.AppPrivateKey = d_Merch.ZfbMpAppPrivateSecret;
            m_Config.ZfbPublicKey = d_Merch.ZfbPublicSecret;
            m_Config.PayResultNotifyUrl = d_Merch.ZfbResultNotifyUrl;

            return m_Config;
        }

        public TgPayInfoConfg GetTgPayInfoConfg(string merchId)
        {

            var m_Confg = new TgPayInfoConfg();

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            if (d_Merch == null)
                return null;


            m_Confg.Account = d_Merch.TgPayAccount;
            m_Confg.Key = d_Merch.TgPayKey;
            m_Confg.PayResultNotifyUrl = d_Merch.TgPayResultNotifyUrl;

            return m_Confg;
        }

        public XrtPayInfoConfg GetXrtPayInfoConfg(string merchId)
        {
            var m_Config = new XrtPayInfoConfg();

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();
            if (d_Merch == null)
                return null;


            m_Config.Mch_id = d_Merch.XrtPayMchId;
            m_Config.Key = d_Merch.XrtPayKey;
            m_Config.PayResultNotifyUrl = d_Merch.XrtPayResultNotifyUrl;

            return m_Config;
        }

        public string GetMerchName(string merchId)
        {

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();

            if (d_Merch == null)
                return null;

            return d_Merch.Name;
        }

        public string GetClientName(string merchId, string userId)
        {
            string userName = "匿名";
            var d_SysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();
            if (d_SysUser != null)
            {
                if (!string.IsNullOrEmpty(d_SysUser.FullName))
                {
                    return d_SysUser.FullName;
                }

                if (!string.IsNullOrEmpty(d_SysUser.NickName))
                {
                    return d_SysUser.NickName;
                }

                if (!string.IsNullOrEmpty(d_SysUser.UserName))
                {
                    return d_SysUser.UserName;
                }
            }

            return userName;
        }

        public string GetOperaterUserName(string merchId, string userId)
        {
            string userName = "匿名";
            var d_SysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();
            if (d_SysUser != null)
            {
                if (!string.IsNullOrEmpty(d_SysUser.FullName))
                {
                    return d_SysUser.FullName;
                }

                if (!string.IsNullOrEmpty(d_SysUser.NickName))
                {
                    return d_SysUser.NickName;
                }

                if (!string.IsNullOrEmpty(d_SysUser.UserName))
                {
                    return d_SysUser.UserName;
                }
            }

            return userName;
        }

        public string GetStoreName(string merchId, string storeId)
        {

            var d_Store = CurrentDb.Store.Where(m => m.Id == storeId && m.MerchId == merchId).FirstOrDefault();

            if (d_Store == null)
                return null;

            return d_Store.Name;
        }

        public string GetShopName(string merchId, string shopId)
        {

            var d_Shop = CurrentDb.Shop.Where(m => m.Id == shopId && m.MerchId == merchId).FirstOrDefault();

            if (d_Shop == null)
                return null;

            return d_Shop.Name;
        }

        public string GetIotApiSecret(string merchId)
        {
            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();

            if (d_Merch == null)
                return null;

            return d_Merch.IotApiSecret;
        }
    }
}
