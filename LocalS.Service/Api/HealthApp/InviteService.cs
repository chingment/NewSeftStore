using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class InviteService : BaseService
    {
        public CustomJsonResult InitRpFollow(string operater, string userId, RopInviteInitRpFollow rop)
        {

            int step = 1;

            var d_Inviter = CurrentDb.SysUser.Where(m => m.Id == rop.Iv_uid).FirstOrDefault();

            var d_SysUserFollow = CurrentDb.SysUserFollow.Where(m => m.UserId == userId && m.FollowUserId == rop.Iv_uid).FirstOrDefault();
            if (d_SysUserFollow != null)
            {
                if (!d_SysUserFollow.IsDelete)
                {
                    step = 2;
                }
            }

            var ret = new
            {
                Inviter= new
                {
                    Avatar = d_Inviter.Avatar,
                    NickName = d_Inviter.NickName
                },
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
                Step = step
            };



            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult AgreeRpFollow(string operater, string userId, RopInviteAgreeRpFollow rop)
        {

            var app_Config = BizFactory.Senviv.GetWxAppConfigByUserId(userId);

            string merchId = app_Config.Exts["MerchId"];

            string wxPaOpenId = app_Config.Exts["WxPaOpenId"];


            var wx_UserInfo = SdkFactory.Wx.GetUserInfoByApiToken(app_Config, wxPaOpenId);

            if (wx_UserInfo == null)
            {
                return new CustomJsonResult(ResultType.Failure, "2801", "未关注公众号，请先关注");
            }

            if (wx_UserInfo.subscribe <= 0)
            {
                return new CustomJsonResult(ResultType.Failure, "2801", "未关注公众号，请先关注.");
            }

            var d_SysUserFollow = CurrentDb.SysUserFollow.Where(m => m.UserId == userId && m.FollowUserId == rop.Iv_uid).FirstOrDefault();

            if (d_SysUserFollow == null)
            {
                d_SysUserFollow = new Lumos.DbRelay.SysUserFollow();
                d_SysUserFollow.Id = IdWorker.Build(IdType.NewGuid);
                d_SysUserFollow.UserId = userId;
                d_SysUserFollow.FollowUserId = rop.Iv_uid;
                d_SysUserFollow.IsDelete = false;
                d_SysUserFollow.CreateTime = DateTime.Now;
                d_SysUserFollow.Creator = operater;
                CurrentDb.SysUserFollow.Add(d_SysUserFollow);
            }
            else
            {
                d_SysUserFollow.IsDelete = false;
                d_SysUserFollow.MendTime = DateTime.Now;
                d_SysUserFollow.Mender = operater;

            }

            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "关注成功");
        }
    }
}
