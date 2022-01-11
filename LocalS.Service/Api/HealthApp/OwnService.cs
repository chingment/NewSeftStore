using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
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
            var app_Config = BizFactory.Senviv.GetWxAppInfoConfigByMerchIdAndDeviceId(rop.MerchId, rop.DeviceId);

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

            var app_Config = BizFactory.Senviv.GetWxAppInfoConfigByMerchIdAndDeviceId(rop.MerchId, rop.DeviceId);

            if (app_Config == null)
            {
                return new CustomJsonResult(ResultType.Failure, "2802", "配置未生效");
            }

            var oauth2_Result = SdkFactory.Wx.GetWebOauth2AccessToken(app_Config, rop.Code);

            if (oauth2_Result.errcode != null)
            {
                return new CustomJsonResult(ResultType.Failure, "2803", "解释身份信息失败");
            }

            var userInfo_Result = SdkFactory.Wx.GetUserInfo(oauth2_Result.access_token, oauth2_Result.openid);

            if (userInfo_Result == null)
            {
                return new CustomJsonResult(ResultType.Failure, "2804", "解释身份信息失败");
            }

            string merchId = "88273829";
            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.WxOpenId == userInfo_Result.openid).FirstOrDefault();
            if (d_SenvivUser == null)
            {
                d_SenvivUser = new Entity.SenvivUser();
                d_SenvivUser.Id = IdWorker.Build(IdType.NewGuid);
                d_SenvivUser.MerchId = merchId;
                d_SenvivUser.WxOpenId = userInfo_Result.openid;
                d_SenvivUser.NickName = userInfo_Result.nickname;
                d_SenvivUser.Avatar = userInfo_Result.headimgurl;
                d_SenvivUser.CreateTime = DateTime.Now;
                d_SenvivUser.Creator = d_SenvivUser.Id;
                CurrentDb.SenvivUser.Add(d_SenvivUser);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_SenvivUser.NickName = userInfo_Result.nickname;
                d_SenvivUser.Avatar = userInfo_Result.headimgurl;
                d_SenvivUser.MendTime = DateTime.Now;
                d_SenvivUser.Mender = d_SenvivUser.Id;
                CurrentDb.SaveChanges();
            }

            string token_key = IdWorker.Build(IdType.NewGuid);
            var token_val = new { userId = d_SenvivUser.Id };
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
