using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using LocalS.BLL;
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
                return new CustomJsonResult(ResultType.Failure, "2801", "配置未生效");
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

            LogUtil.Info("rop.MerchId:1" + rop.MerchId);
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
                d_ClientUser.PasswordHash = PassWordHelper.HashPassword("sfsfsffds.3pg");
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
                d_ClientUser.MerchId = merchId;
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

        public CustomJsonResult Info(string operater, string userId)
        {
            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            var d_UserDevices = CurrentDb.SvUserDevice.Where(m => m.UserId == userId && m.BindStatus != E_SvUserDeviceBindStatus.NotBind).ToList();

            List<object> devices = new List<object>();

            foreach (var d_UserDevice in d_UserDevices)
            {
                var d_SvUser = CurrentDb.SvUser.Where(m => m.Id == d_UserDevice.SvUserId).FirstOrDefault();
                var signName = "";

                if (d_SvUser != null)
                {
                    signName = d_SvUser.FullName;
                }

                var bindStatus = new FieldModel();
                if (d_UserDevice.BindStatus == E_SvUserDeviceBindStatus.NotBind)
                {
                    bindStatus = new FieldModel(1, "未绑定");
                }
                else if (d_UserDevice.BindStatus == E_SvUserDeviceBindStatus.Binded)
                {
                    bindStatus = new FieldModel(2, "已绑定");
                }
                else if (d_UserDevice.BindStatus == E_SvUserDeviceBindStatus.UnBind)
                {
                    bindStatus = new FieldModel(3, "已解绑");
                }

                devices.Add(new
                {
                    Id = d_UserDevice.DeviceId,
                    SignName = signName,
                    BindTime = d_UserDevice.BindTime.ToUnifiedFormatDateTime(),
                    UnBindTime = d_UserDevice.UnBindTime.ToUnifiedFormatDateTime(),
                    BindStatus = bindStatus
                });
            }

            var ret = new
            {
                UserInfo = new
                {
                    Avatar = d_ClientUser.Avatar,
                    SignName = d_ClientUser.NickName
                },
                Devices = devices,
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

        public CustomJsonResult EgyContacts(string operater, string userId)
        {
            var d_SysUserContacts = CurrentDb.SysUserContact.Where(m => m.UserId == userId && m.Type == 1).ToList();

            List<object> items = new List<object>();

            foreach (var d_SysUserContact in d_SysUserContacts)
            {
                items.Add(new
                {
                    Id = d_SysUserContact.Id,
                    FullName = d_SysUserContact.FullName,
                    PhoneNumber = d_SysUserContact.PhoneNumber
                });
            }

            var ret = new
            {
                total = d_SysUserContacts.Count,
                items = items
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);


        }

        public CustomJsonResult Followers(string operater, string userId)
        {
            var query = (from u in CurrentDb.SysUserFollow
                         join s in CurrentDb.SysUser on u.UserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where u.IsDelete == false
                         && u.FollowUserId == userId
                         select new
                         {
                             u.UserId,
                             tt.Avatar,
                             tt.NickName,
                             tt.FullName,
                             u.CreateTime
                         });

            int total = query.Count();

            int pageIndex = 0;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    UserId = item.UserId,
                    Avatar = item.Avatar,
                    FullName = item.FullName,
                    NickName = item.NickName,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }

        public CustomJsonResult Idolers(string operater, string userId)
        {
            var query = (from u in CurrentDb.SysUserFollow
                         join s in CurrentDb.SysUser on u.FollowUserId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where u.IsDelete == false
                         && u.UserId == userId
                         select new
                         {
                             u.FollowUserId,
                             tt.Avatar,
                             tt.NickName,
                             tt.FullName,
                             u.CreateTime
                         });

            int total = query.Count();

            int pageIndex = 0;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    FollowUserId = item.FollowUserId,
                    Avatar = item.Avatar,
                    FullName = item.FullName,
                    NickName = item.NickName,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);
        }
    }
}
