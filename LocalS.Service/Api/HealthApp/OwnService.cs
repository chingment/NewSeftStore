﻿using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using Lumos.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class OwnService : BaseService
    {
        public CustomJsonResult AuthUrl(RopOwnAuthUrl rop)
        {
            var app_Config = BizFactory.Senviv.GetWxAppInfoConfigByMerchIdOrDeviceId(rop.MerchId, rop.DeviceId);

            if (app_Config == null)
            {
                return new CustomJsonResult(ResultType.Failure, "2802", "配置未生效");
            }

            var ret = new
            {
                url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect", app_Config.AppId, rop.RedriectUrl)
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }


        public CustomJsonResult AuthInfo(RopOwnAuthInfo rop)
        {

            var result = new CustomJsonResult();

            var app_Config = BizFactory.Senviv.GetWxAppInfoConfigByMerchIdOrDeviceId(rop.MerchId, rop.DeviceId);

            if (app_Config == null)
            {
                return new CustomJsonResult(ResultType.Failure, "2802", "配置未生效");
            }

            var r_Api_Oauth2 = SdkFactory.Wx.GetWebOauth2AccessToken(app_Config, rop.Code);

            if (r_Api_Oauth2.errcode != null)
            {
                return new CustomJsonResult(ResultType.Failure, "2803", "解释身份信息失败");
            }

            var r_Api_UseInfo = SdkFactory.Wx.GetUserInfo(r_Api_Oauth2.access_token, r_Api_Oauth2.openid);

            if (r_Api_UseInfo == null)
            {
                return new CustomJsonResult(ResultType.Failure, "2804", "解释身份信息失败");
            }

            string merchId = app_Config.Exts["MerchId"];

            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.WxPaOpenId == r_Api_UseInfo.openid).FirstOrDefault();
            if (d_ClientUser == null)
            {
                d_ClientUser = new SysClientUser();
                d_ClientUser.Id = IdWorker.Build(IdType.NewGuid);
                d_ClientUser.UserName = IdWorker.Build(IdType.NewGuid);
                d_ClientUser.PasswordHash = PassWordHelper.HashPassword("sfsfsffds");
                d_ClientUser.SecurityStamp = IdWorker.Build(IdType.NewGuid);
                d_ClientUser.NickName = r_Api_UseInfo.nickname;
                d_ClientUser.Avatar = r_Api_UseInfo.headimgurl;
                d_ClientUser.MerchId = merchId;
                d_ClientUser.WxPaAppId = app_Config.AppId;
                d_ClientUser.WxPaOpenId = r_Api_UseInfo.openid;
                d_ClientUser.BelongType = Enumeration.BelongType.Client;
                d_ClientUser.RegisterTime = DateTime.Now;
                d_ClientUser.CreateTime = DateTime.Now;
                d_ClientUser.Creator = d_ClientUser.Id;
                CurrentDb.SysClientUser.Add(d_ClientUser);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_ClientUser.NickName = r_Api_UseInfo.nickname;
                d_ClientUser.Avatar = r_Api_UseInfo.headimgurl;
                d_ClientUser.MendTime = DateTime.Now;
                d_ClientUser.Mender = d_ClientUser.Id;
                CurrentDb.SaveChanges();
            }

            string token_key = IdWorker.Build(IdType.NewGuid);

            var token_val = new { userId = d_ClientUser.Id, merchId = d_ClientUser.MerchId };

            var session = new Session();
            session.Set<Object>(string.Format("token:{0}", token_key), token_val, new TimeSpan(24, 0, 0));

            var ret = new { token = token_key };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

        }

        public CustomJsonResult InitInfo(string operater, string userId)
        {

            var ret = new
            {
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);


        }

        public CustomJsonResult AuthTokenCheck(string token)
        {

            var session = new Session();

            var token_val = session.Get<Dictionary<string, string>>(string.Format("token:{0}", token));

            if (token_val == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2Sign, "");
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }

    }
}