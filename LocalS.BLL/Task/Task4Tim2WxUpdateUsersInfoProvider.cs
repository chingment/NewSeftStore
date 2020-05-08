using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyWeiXinSdk;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Task
{
    public class Task4Tim2WxUpdateUsersInfoProvider : BaseDbContext, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var wxAppInfoConfig = new WxAppInfoConfig();
                wxAppInfoConfig.AppId = "wxc6e80f8c575cf3f5";
                wxAppInfoConfig.AppSecret = "fee895c9923da26a4d42d9c435202b37";
                var openIds = SdkFactory.Wx.GetUserOpenIds(wxAppInfoConfig);

                LogUtil.Info(string.Format("获取微信openIds的数量", openIds.Count));

                var merchId = "d17df2252133478c99104180e8062230";
                if (openIds != null)
                {
                    foreach (var openId in openIds)
                    {
                        LogUtil.Info(string.Format("获取微信openId({0})的信息", openId));

                        var apiUserInfo = SdkFactory.Wx.GetUserInfoByApiToken(wxAppInfoConfig, openId);
                        if (apiUserInfo != null)
                        {
                            using (TransactionScope ts = new TransactionScope())
                            {
                                var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.OpenId == openId).FirstOrDefault();
                                if (wxUserInfo == null)
                                {
                                    string sysClientUserId = IdWorker.Build(IdType.NewGuid);
                                    var sysClientUser = new SysClientUser();
                                    sysClientUser.Id = sysClientUserId;
                                    sysClientUser.UserName = string.Format("wx{0}", Guid.NewGuid().ToString().Replace("-", ""));
                                    sysClientUser.PasswordHash = PassWordHelper.HashPassword("888888");
                                    sysClientUser.SecurityStamp = Guid.NewGuid().ToString();
                                    sysClientUser.RegisterTime = DateTime.Now;
                                    sysClientUser.NickName = apiUserInfo.nickname;
                                    sysClientUser.Sex = apiUserInfo.sex == 1 ? "男" : "女";
                                    sysClientUser.Province = apiUserInfo.province;
                                    sysClientUser.City = apiUserInfo.city;
                                    sysClientUser.Country = apiUserInfo.country;
                                    sysClientUser.Avatar = apiUserInfo.headimgurl;
                                    sysClientUser.IsVip = false;
                                    sysClientUser.CreateTime = DateTime.Now;
                                    sysClientUser.Creator = sysClientUserId;
                                    sysClientUser.BelongType = Enumeration.BelongType.Client;
                                    sysClientUser.MerchId = merchId;
                                    CurrentDb.SysClientUser.Add(sysClientUser);
                                    CurrentDb.SaveChanges();

                                    wxUserInfo = new WxUserInfo();
                                    wxUserInfo.Id = IdWorker.Build(IdType.NewGuid);
                                    wxUserInfo.MerchId = merchId;
                                    wxUserInfo.AppId = wxAppInfoConfig.AppId;
                                    wxUserInfo.ClientUserId = sysClientUser.Id;
                                    wxUserInfo.OpenId = openId;
                                    wxUserInfo.CreateTime = DateTime.Now;
                                    wxUserInfo.Creator = sysClientUserId;
                                    CurrentDb.WxUserInfo.Add(wxUserInfo);
                                    CurrentDb.SaveChanges();
                                }
                                else
                                {

                                    var sysClientUser = CurrentDb.SysClientUser.Where(m => m.Id == wxUserInfo.ClientUserId).FirstOrDefault();
                                    if (sysClientUser != null)
                                    {
                                        sysClientUser.Sex = apiUserInfo.sex == 1 ? "男" : "女";
                                        sysClientUser.Province = apiUserInfo.province;
                                        sysClientUser.City = apiUserInfo.city;
                                        sysClientUser.Country = apiUserInfo.country;
                                        sysClientUser.Avatar = apiUserInfo.headimgurl;
                                        CurrentDb.SaveChanges();
                                    }
                                }

                                ts.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("获取微信用户信息任务发生异常", ex);
            }

        }
    }
}
