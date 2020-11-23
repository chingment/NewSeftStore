using LocalS.BLL;
using LocalS.Service.UI;
using Lumos;
using Lumos.DbRelay;
using Lumos.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWeiXinSdk;
using System.Runtime.InteropServices;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using System.Transactions;
using LocalS.Entity;
using Lumos.Redis;

namespace LocalS.Service.Api.Account
{
    public class OwnService : BaseDbContext
    {
        [DllImport(@"BioVein.x64.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_MatchFeature([MarshalAs(UnmanagedType.LPArray)] byte[] featureDataMatch, [MarshalAs(UnmanagedType.LPArray)]  byte[] featureDataReg, byte RegCnt, byte flag, byte securityLevel, int[] diff, [MarshalAs(UnmanagedType.LPArray)] byte[] AIDataBuf, int[] AIDataLen);

        private List<MenuNode> GetMenus(Enumeration.BelongSite belongSite, string userId, string mctMode = "")
        {
            List<MenuNode> menuNodes = new List<MenuNode>();

            var sysMenus = CurrentDb.SysMenu.Where(m => m.BelongSite == belongSite && m.Depth != 0).OrderBy(m => m.Priority).ToList();

            if (belongSite == Enumeration.BelongSite.Admin || belongSite == Enumeration.BelongSite.Merch)
            {
                sysMenus = (from menu in CurrentDb.SysMenu where (from rolemenu in CurrentDb.SysRoleMenu where (from sysUserRole in CurrentDb.SysUserRole where sysUserRole.UserId == userId select sysUserRole.RoleId).Contains(rolemenu.RoleId) select rolemenu.MenuId).Contains(menu.Id) && menu.BelongSite == belongSite select menu).Where(m => m.Depth != 0).OrderBy(m => m.Priority).ToList();

                if (belongSite == Enumeration.BelongSite.Merch)
                {
                    if (!string.IsNullOrEmpty(mctMode))
                    {
                        mctMode = (mctMode.Replace("M", "")).Replace("S", "");

                        if (mctMode == "F")
                        {
                            sysMenus = sysMenus.Where(m => m.BelongMctMode != "K").ToList();
                        }
                    }
                }

            }

            foreach (var sysMenu in sysMenus)
            {
                MenuNode menuNode = new MenuNode();
                menuNode.Id = sysMenu.Id;
                menuNode.PId = sysMenu.PId;
                menuNode.Path = sysMenu.Path;
                menuNode.Name = sysMenu.Name;
                menuNode.Icon = sysMenu.Icon;
                menuNode.Title = sysMenu.Title;
                menuNode.Component = sysMenu.Component;
                menuNode.IsSidebar = sysMenu.IsSidebar;
                menuNode.IsNavbar = sysMenu.IsNavbar;
                menuNode.IsRouter = sysMenu.IsRouter;
                menuNodes.Add(menuNode);
            }

            return menuNodes;

        }
        private List<RoleModel> GetRoles(Enumeration.BelongSite belongSite, string userId)
        {
            List<RoleModel> models = new List<RoleModel>();


            var roleIds = CurrentDb.SysUserRole.Where(m => m.UserId == userId).Select(m => m.RoleId).ToArray();

            if (roleIds == null || roleIds.Length == 0)
            {
                return models;
            }

            var roles = CurrentDb.SysRole.Where(m => belongSite == Enumeration.BelongSite.Merch && roleIds.Contains(m.Id)).ToList();

            foreach (var role in roles)
            {
                RoleModel model = new RoleModel();
                model.Id = role.Id;
                model.Name = role.Name;

                models.Add(model);
            }

            return models;

        }

        public CustomJsonResult LoginByAccount(RopOwnLoginByAccount rop)
        {
            string machineId = "";

            if (string.IsNullOrEmpty(rop.AppId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未指定登录应用");
            }

            if (rop.AppId == AppId.STORETERM)
            {
                if (rop.LoginPms == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，缺少参数loginPms");
                }

                if (!rop.LoginPms.ContainsKey("machineId") || string.IsNullOrEmpty(rop.LoginPms["machineId"].ToString()))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，缺少参数loginPms.machineId");
                }

                machineId = rop.LoginPms["machineId"].ToString();

            }

            var result = new CustomJsonResult();

            var sysUser = CurrentDb.SysUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();

            if (sysUser == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，账号不存在");
            }


            if (!PassWordHelper.VerifyHashedPassword(sysUser.PasswordHash, rop.Password))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，密码不正确");
            }

            if (sysUser.IsDisable)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，账号已被禁用");
            }

            string token = IdWorker.Build(IdType.NewGuid);

            var tokenInfo = new TokenInfo();
            tokenInfo.UserId = sysUser.Id;
            tokenInfo.UserName = sysUser.UserName;

            if (rop.AppId == AppId.MERCH)
            {
                #region MERCH

                var merchUser = CurrentDb.SysMerchUser.Where(m => m.Id == sysUser.Id).FirstOrDefault();
                if (merchUser == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户不属于该站点");
                }

                var merch = CurrentDb.Merch.Where(m => m.Id == merchUser.MerchId).FirstOrDefault();
                if (merch == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该商户不存在");
                }

                tokenInfo.BelongId = merchUser.MerchId;
                tokenInfo.BelongType = Enumeration.BelongType.Merch;
                tokenInfo.MctMode = merch.MctMode;
                MqFactory.Global.PushEventNotify(sysUser.Id, rop.AppId, merchUser.MerchId, "", machineId, EventCode.Login, "登录成功", new LoginLogModel { LoginAccount = sysUser.UserName, LoginFun = Enumeration.LoginFun.Account, LoginResult = Enumeration.LoginResult.LoginSuccess, LoginWay = rop.LoginWay, LoginIp = rop.Ip });

                SSOUtil.SetTokenInfo(token, tokenInfo, new TimeSpan(1, 0, 0));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", new { Token = token });

                #endregion
            }
            else if (rop.AppId == AppId.STORETERM)
            {
                #region STORETERM

                var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();
                if (machine == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未登记");
                }

                if (string.IsNullOrEmpty(machine.CurUseMerchId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未绑定商家");
                }

                if (string.IsNullOrEmpty(machine.CurUseStoreId))
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未绑定店铺");
                }

                var storeTermUser = CurrentDb.SysMerchUser.Where(m => m.Id == sysUser.Id).FirstOrDefault();
                if (storeTermUser == null)
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户不属于该站点");
                }

                if (machine.CurUseMerchId != storeTermUser.MerchId)
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "帐号与商户不对应");
                }

                MqFactory.Global.PushEventNotify(sysUser.Id, rop.AppId, machine.CurUseMerchId, machine.CurUseStoreId, machineId, EventCode.Login, "登录成功", new LoginLogModel { LoginAccount = sysUser.UserName, LoginFun = Enumeration.LoginFun.Account, LoginResult = Enumeration.LoginResult.LoginSuccess, LoginWay = rop.LoginWay, LoginIp = rop.Ip });


                SSOUtil.SetTokenInfo(token, tokenInfo, new TimeSpan(1, 0, 0));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", new { Token = token, UserName = sysUser.UserName, FullName = sysUser.FullName });
                #endregion
            }
            else if (rop.AppId == AppId.SVCCHAT || rop.AppId == AppId.SVCCHAT_Test)
            {
                #region SVCCHAT
                var merchUser = CurrentDb.SysMerchUser.Where(m => m.Id == sysUser.Id).FirstOrDefault();
                if (merchUser == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户不属于该站点");
                }

                if (!merchUser.ImIsUse)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户没有权限");
                }


                SSOUtil.SetTokenInfo(token, tokenInfo, new TimeSpan(1, 0, 0));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", new { Token = token, UserName = sysUser.UserName, Avatar = sysUser.Avatar, NickName = sysUser.NickName, ImUserName = merchUser.ImUserName, ImPassword = merchUser.ImPassword });

                #endregion 
            }
            else if (rop.AppId == AppId.ADMIN)
            {
                #region ADMIN
                var sysAdminUser = CurrentDb.SysAdminUser.Where(m => m.Id == sysUser.Id).FirstOrDefault();
                if (sysAdminUser == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户不属于该站点");
                }

                tokenInfo.BelongId = sysAdminUser.Id;
                tokenInfo.BelongType = Enumeration.BelongType.Admin;

                SSOUtil.SetTokenInfo(token, tokenInfo, new TimeSpan(1, 0, 0));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", new { Token = token });

                #endregion 
            }


            return result;
        }

        public CustomJsonResult LoginByMinProgram(RopOwnLoginByMinProgram rop)
        {
            var result = new CustomJsonResult();
            var ret = new RetOwnLoginByMinProgram();

            WxUserInfo wxUserInfo = null;

            var merch = CurrentDb.Merch.Where(m => m.Id == rop.MerchId && m.WxMpAppId == rop.AppId).FirstOrDefault();

            if (merch == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "微信信息认证失败");
            }

            var wxAppInfoConfig = new WxAppInfoConfig();

            wxAppInfoConfig.AppId = merch.WxMpAppId;
            wxAppInfoConfig.AppSecret = merch.WxMpAppSecret;
            wxAppInfoConfig.PayMchId = merch.WxPayMchId;
            wxAppInfoConfig.PayKey = merch.WxPayKey;
            wxAppInfoConfig.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;
            wxAppInfoConfig.NotifyEventUrlToken = merch.WxPaNotifyEventUrlToken;

            var wxUserInfoByMinProram = SdkFactory.Wx.GetUserInfoByApiToken(wxAppInfoConfig, rop.OpenId);

            if (wxUserInfoByMinProram == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "微信用户信息认证失败");
            }

            wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.OpenId == rop.OpenId).FirstOrDefault();

            if (wxUserInfo == null)
            {
                string sysClientUserId = IdWorker.Build(IdType.NewGuid);

                var sysClientUser = new SysClientUser();

                sysClientUser.Id = sysClientUserId;
                sysClientUser.UserName = string.Format("wx{0}", Guid.NewGuid().ToString().Replace("-", ""));
                sysClientUser.PasswordHash = PassWordHelper.HashPassword("888888");
                sysClientUser.SecurityStamp = Guid.NewGuid().ToString();
                sysClientUser.RegisterTime = DateTime.Now;
                sysClientUser.NickName = wxUserInfoByMinProram.nickname;
                sysClientUser.Sex = wxUserInfoByMinProram.sex == 1 ? "男" : "女";
                sysClientUser.Province = wxUserInfoByMinProram.province;
                sysClientUser.City = wxUserInfoByMinProram.city;
                sysClientUser.Country = wxUserInfoByMinProram.country;
                sysClientUser.Avatar = wxUserInfoByMinProram.headimgurl;
                sysClientUser.PhoneNumber = rop.PhoneNumber;
                sysClientUser.IsVip = false;
                sysClientUser.CreateTime = DateTime.Now;
                sysClientUser.Creator = sysClientUserId;
                sysClientUser.BelongType = Enumeration.BelongType.Client;
                sysClientUser.MerchId = rop.MerchId;
                CurrentDb.SysClientUser.Add(sysClientUser);
                CurrentDb.SaveChanges();

                wxUserInfo = new WxUserInfo();
                wxUserInfo.Id = IdWorker.Build(IdType.NewGuid);
                wxUserInfo.MerchId = rop.MerchId;
                wxUserInfo.AppId = rop.AppId;
                wxUserInfo.ClientUserId = sysClientUser.Id;
                wxUserInfo.OpenId = rop.OpenId;
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
                    sysClientUser.NickName = wxUserInfoByMinProram.nickname;
                    sysClientUser.Sex = wxUserInfoByMinProram.sex == 1 ? "男" : "女";
                    sysClientUser.Province = wxUserInfoByMinProram.province;
                    sysClientUser.City = wxUserInfoByMinProram.city;
                    sysClientUser.Country = wxUserInfoByMinProram.country;
                    sysClientUser.Avatar = wxUserInfoByMinProram.headimgurl;
                    sysClientUser.PhoneNumber = rop.PhoneNumber;
                }
                CurrentDb.SaveChanges();
            }


            var tokenInfo = new TokenInfo();
            ret.Token = IdWorker.Build(IdType.NewGuid);
            ret.OpenId = wxUserInfo.OpenId;

            tokenInfo.UserId = wxUserInfo.ClientUserId;

            SSOUtil.SetTokenInfo(ret.Token, tokenInfo, new TimeSpan(24 * 7, 0, 0));

            MqFactory.Global.PushEventNotify(wxUserInfo.Id, rop.AppId, wxUserInfo.MerchId, "", "", EventCode.Login, "登录成功");

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);

            return result;
        }

        //public CustomJsonResult LoginByMinProgram(RopOwnLoginByMinProgram rop)
        //{
        //    var result = new CustomJsonResult();
        //    var ret = new RetOwnLoginByMinProgram();

        //    WxUserInfo wxUserInfo = null;

        //    if (string.IsNullOrEmpty(rop.OpenId))
        //    {
        //        var merch = CurrentDb.Merch.Where(m => m.Id == rop.MerchId && m.WxMpAppId == rop.AppId).FirstOrDefault();

        //        if (merch == null)
        //        {
        //            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商户信息认证失败");
        //        }

        //        var wxAppInfoConfig = new WxAppInfoConfig();

        //        wxAppInfoConfig.AppId = merch.WxMpAppId;
        //        wxAppInfoConfig.AppSecret = merch.WxMpAppSecret;
        //        wxAppInfoConfig.PayMchId = merch.WxPayMchId;
        //        wxAppInfoConfig.PayKey = merch.WxPayKey;
        //        wxAppInfoConfig.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;
        //        wxAppInfoConfig.NotifyEventUrlToken = merch.WxPaNotifyEventUrlToken;


        //        var wxUserInfoByMinProram = SdkFactory.Wx.GetUserInfoByMinProramJsCode(wxAppInfoConfig, rop.EncryptedData, rop.Iv, rop.Code);

        //        if (wxUserInfoByMinProram == null)
        //        {
        //            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "获取微信用户信息失败");
        //        }

        //        wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.OpenId == wxUserInfoByMinProram.openId).FirstOrDefault();
        //        if (wxUserInfo == null)
        //        {
        //            string sysClientUserId = IdWorker.Build(IdType.NewGuid);

        //            var sysClientUser = new SysClientUser();

        //            sysClientUser.Id = sysClientUserId;
        //            sysClientUser.UserName = string.Format("wx{0}", Guid.NewGuid().ToString().Replace("-", ""));
        //            sysClientUser.PasswordHash = PassWordHelper.HashPassword("888888");
        //            sysClientUser.SecurityStamp = Guid.NewGuid().ToString();
        //            sysClientUser.RegisterTime = DateTime.Now;
        //            sysClientUser.NickName = wxUserInfoByMinProram.nickName;
        //            sysClientUser.Sex = wxUserInfoByMinProram.gender;
        //            sysClientUser.Province = wxUserInfoByMinProram.province;
        //            sysClientUser.City = wxUserInfoByMinProram.city;
        //            sysClientUser.Country = wxUserInfoByMinProram.country;
        //            sysClientUser.Avatar = wxUserInfoByMinProram.avatarUrl;
        //            sysClientUser.IsVip = false;
        //            sysClientUser.CreateTime = DateTime.Now;
        //            sysClientUser.Creator = sysClientUserId;
        //            sysClientUser.BelongType = Enumeration.BelongType.Client;
        //            sysClientUser.MerchId = rop.MerchId;
        //            CurrentDb.SysClientUser.Add(sysClientUser);
        //            CurrentDb.SaveChanges();

        //            wxUserInfo = new WxUserInfo();
        //            wxUserInfo.Id = IdWorker.Build(IdType.NewGuid);
        //            wxUserInfo.MerchId = rop.MerchId;
        //            wxUserInfo.AppId = rop.AppId;
        //            wxUserInfo.ClientUserId = sysClientUser.Id;
        //            wxUserInfo.OpenId = wxUserInfoByMinProram.openId;
        //            wxUserInfo.CreateTime = DateTime.Now;
        //            wxUserInfo.Creator = sysClientUserId;
        //            CurrentDb.WxUserInfo.Add(wxUserInfo);
        //            CurrentDb.SaveChanges();
        //        }
        //        else
        //        {
        //            var sysClientUser = CurrentDb.SysClientUser.Where(m => m.Id == wxUserInfo.ClientUserId).FirstOrDefault();
        //            if (sysClientUser != null)
        //            {
        //                sysClientUser.NickName = wxUserInfoByMinProram.nickName;
        //                sysClientUser.Sex = wxUserInfoByMinProram.gender;
        //                sysClientUser.Province = wxUserInfoByMinProram.province;
        //                sysClientUser.City = wxUserInfoByMinProram.city;
        //                sysClientUser.Country = wxUserInfoByMinProram.country;
        //                sysClientUser.Avatar = wxUserInfoByMinProram.avatarUrl;
        //            }
        //            CurrentDb.SaveChanges();
        //        }
        //    }
        //    else
        //    {
        //        wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.OpenId == rop.OpenId).FirstOrDefault();
        //    }

        //    var tokenInfo = new TokenInfo();
        //    ret.Token = IdWorker.Build(IdType.NewGuid);
        //    ret.OpenId = wxUserInfo.OpenId;

        //    tokenInfo.UserId = wxUserInfo.ClientUserId;

        //    SSOUtil.SetTokenInfo(ret.Token, tokenInfo, new TimeSpan(1, 0, 0));

        //    MqFactory.Global.PushEventNotify(wxUserInfo.Id, rop.AppId, wxUserInfo.MerchId, "", "", EventCode.Login, "登录成功");

        //    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);

        //    return result;
        //}

        public CustomJsonResult LoginByFingerVein(RopOwnLoginByFingerVein rop)
        {

            string machineId = "";
            Enumeration.BelongType belongType = Enumeration.BelongType.Unknow;
            string belongId = "";
            if (string.IsNullOrEmpty(rop.AppId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未指定登录应用");
            }

            if (rop.AppId == AppId.STORETERM)
            {
                if (rop.LoginPms == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，缺少参数loginPms");
                }

                if (!rop.LoginPms.ContainsKey("machineId") || string.IsNullOrEmpty(rop.LoginPms["machineId"].ToString()))
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，缺少参数loginPms.machineId");
                }
                else
                {
                    machineId = rop.LoginPms["machineId"].ToString();
                }

                var machine = BizFactory.Machine.GetOne(machineId);
                if (machine == null)
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未登记");
                }

                if (string.IsNullOrEmpty(machine.MerchId))
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未绑定商家");
                }

                if (string.IsNullOrEmpty(machine.StoreId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未绑定店铺");
                }

                belongType = Enumeration.BelongType.Merch;
                belongId = machine.MerchId;
            }
            else
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂不支持");
            }


            var result = new CustomJsonResult();


            LogUtil.Info("静指脉数据1：" + rop.VeinData);

            string userId = "";
            bool isMachSuccess = false;

            try
            {
                var sysUserFingerVeins = CurrentDb.SysUserFingerVein.Where(m => m.BelongId == belongId && m.BelongType == belongType).ToList();
                byte[] matchFeature = Convert.FromBase64String(rop.VeinData);
                foreach (var sysUserFingerVein in sysUserFingerVeins)
                {
                    if (sysUserFingerVein.VeinData != null)
                    {
                        int[] diff2 = new int[1];
                        byte[] AIDataBuf = new byte[matchFeature.Length];
                        int[] AIDataLen = new int[1];
                        var re1t = FV_MatchFeature(matchFeature, sysUserFingerVein.VeinData, (byte)0x03, (byte)0x03, (byte)4, diff2, AIDataBuf, AIDataLen);
                        if (re1t == 0)
                        {
                            userId = sysUserFingerVein.UserId;
                            isMachSuccess = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("静指脉匹配异常", ex);
            }

            if (!isMachSuccess)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，静指脉验证失败");
            }


            if (isMachSuccess)
            {
                var sysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();

                if (sysUser == null)
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，账号不存在");
                }

                if (sysUser.IsDisable)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，账号已被禁用");
                }

                string token = IdWorker.Build(IdType.NewGuid);

                var tokenInfo = new TokenInfo();
                tokenInfo.UserId = userId;
                tokenInfo.UserName = sysUser.UserName;
                tokenInfo.BelongId = belongId;
                tokenInfo.BelongType = belongType;

                if (rop.AppId == AppId.STORETERM)
                {
                    #region StoreTerm

                    var storeTermUser = CurrentDb.SysMerchUser.Where(m => m.Id == userId).FirstOrDefault();
                    if (storeTermUser == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户不属于该站点");
                    }

                    if (belongId != storeTermUser.MerchId)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "帐号与商户不对应");
                    }
                    #endregion
                }


                SSOUtil.SetTokenInfo(token, tokenInfo, new TimeSpan(1, 0, 0));

                MqFactory.Global.PushEventNotify(userId, rop.AppId, "", "", machineId, EventCode.Login, "登录成功", new LoginLogModel { LoginAccount = sysUser.UserName, LoginFun = Enumeration.LoginFun.FingerVein, LoginResult = Enumeration.LoginResult.LoginSuccess, LoginWay = rop.LoginWay });


                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", new { UserName = sysUser.UserName, FullName = sysUser.FullName });
            }

            return result;
        }

        public CustomJsonResult Logout(string operater, string userId, RopOwnLogout rop)
        {
            var result = new CustomJsonResult();

            string userName = null;


            var sysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();
            if (sysUser != null)
            {
                userName = sysUser.UserName;

            }



            SSOUtil.Quit(rop.Token);

            MqFactory.Global.PushEventNotify(operater, rop.AppId, rop.BelongId, "", "", EventCode.Logout, "退出成功", new LoginLogModel { LoginAccount = userName, LoginResult = Enumeration.LoginResult.LogoutSuccess, LoginWay = rop.LoginWay });

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "退出成功");

            return result;
        }

        public CustomJsonResult GetInfo(string operater, string userId, RupOwnGetInfo rup)
        {
            var result = new CustomJsonResult();
            var ret = new RetOwnGetInfo();

            var sysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();

            ret.UserName = sysUser.UserName;
            ret.FullName = sysUser.FullName;
            ret.Avatar = sysUser.Avatar;
            ret.Introduction = sysUser.Introduction;
            ret.Email = sysUser.Email;
            ret.PhoneNumber = sysUser.PhoneNumber;
            ret.FingerVeinCount = sysUser.FingerVeinCount;
            if (rup != null)
            {
                switch (rup.WebSite)
                {
                    case "admin":
                        ret.Menus = GetMenus(Enumeration.BelongSite.Admin, userId);
                        ret.Roles = GetRoles(Enumeration.BelongSite.Admin, userId);
                        break;
                    case "agent":
                        ret.Menus = GetMenus(Enumeration.BelongSite.Agent, userId);
                        ret.Roles = GetRoles(Enumeration.BelongSite.Agent, userId);
                        break;
                    case "account":
                        ret.Menus = GetMenus(Enumeration.BelongSite.Account, userId);
                        ret.Roles = GetRoles(Enumeration.BelongSite.Account, userId);
                        break;
                    case "merch":
                        var tokenInfo = SSOUtil.GetTokenInfo(rup.Token);
                        ret.MctMode = tokenInfo.MctMode;
                        ret.Menus = GetMenus(Enumeration.BelongSite.Merch, userId, tokenInfo.MctMode);
                        ret.Roles = GetRoles(Enumeration.BelongSite.Merch, userId);
                        break;
                }
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult CheckPermission(string operater, string userId, string token, RupOwnCheckPermission rup)
        {
            var result = new CustomJsonResult();

            Enumeration.BelongSite webSite = Enumeration.BelongSite.Unknow;
            switch (rup.WebSite)
            {
                case "admin":
                    webSite = Enumeration.BelongSite.Admin;
                    break;
                case "agent":
                    webSite = Enumeration.BelongSite.Agent;
                    break;
                case "account":
                    webSite = Enumeration.BelongSite.Account;
                    break;
                case "merch":
                    webSite = Enumeration.BelongSite.Merch;
                    break;
            }

            SSOUtil.Postpone(token);

            switch (rup.Type)
            {
                case "1":
                    string path = rup.Content;
                    if (rup.Content == "/")
                    {
                        path = "/home";
                    }

                    var menus = GetMenus(webSite, userId);
                    var hasMenu = menus.Where(m => m.Path == path).FirstOrDefault();
                    if (hasMenu == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2NoRight, "没有权限访问页面");
                    }

                    break;
            }
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "检查成功");

            return result;
        }

        public CustomJsonResult UploadFingerVeinData(string operater, string userId, RopUploadFingerVeinData rop)
        {
            LogUtil.Info("VeinData:" + rop.VeinData);

            var result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.VeinData))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "静指脉数据为空");
            }

            var machine = BizFactory.Machine.GetOne(rop.MachineId);

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未登记");
            }

            if (string.IsNullOrEmpty(machine.MerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户");
            }

            using (TransactionScope ts = new TransactionScope())
            {

                var sysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();
                if (sysUser == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该用户");
                }

                if (sysUser.FingerVeinCount > 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已录入，请先删除");
                }


                bool isMachSuccess = false;
                try
                {
                    var sysUserFingerVeins = CurrentDb.SysUserFingerVein.Where(m => m.BelongId == machine.MerchId && m.BelongType == Enumeration.BelongType.Merch).ToList();
                    byte[] matchFeature = Convert.FromBase64String(rop.VeinData);
                    foreach (var item in sysUserFingerVeins)
                    {
                        int[] diff2 = new int[1];
                        byte[] AIDataBuf = new byte[matchFeature.Length];
                        int[] AIDataLen = new int[1];
                        var re1t = FV_MatchFeature(matchFeature, item.VeinData, (byte)0x03, (byte)0x03, (byte)4, diff2, AIDataBuf, AIDataLen);
                        if (re1t == 0)
                        {
                            isMachSuccess = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error("静指脉匹配异常", ex);
                }

                if (isMachSuccess)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "采集失败，该静脉指信息已被上传");
                }

                var sysUserFingerVein = new SysUserFingerVein();
                sysUserFingerVein.Id = IdWorker.Build(IdType.NewGuid);
                sysUserFingerVein.UserId = userId;
                sysUserFingerVein.BelongType = Enumeration.BelongType.Merch;
                sysUserFingerVein.BelongId = machine.MerchId;
                sysUserFingerVein.VeinData = Convert.FromBase64String(rop.VeinData);
                sysUserFingerVein.CreateTime = DateTime.Now;
                sysUserFingerVein.Creator = operater;
                CurrentDb.SysUserFingerVein.Add(sysUserFingerVein);
                CurrentDb.SaveChanges();

                sysUser.FingerVeinCount = 1;


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "录入成功");
            }

            return result;
        }

        public CustomJsonResult DeleteFingerVeinData(string operater, string userId)
        {

            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var sysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();
                if (sysUser == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该用户");
                }

                sysUser.FingerVeinCount = 0;


                var sysUserFingerVeins = CurrentDb.SysUserFingerVein.Where(m => m.UserId == userId).ToList();

                foreach (var sysUserFingerVein in sysUserFingerVeins)
                {
                    CurrentDb.SysUserFingerVein.Remove(sysUserFingerVein);
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");
            }

            return result;
        }


        public CustomJsonResult GetWxApiCode2Session(RopWxApiCode2Session rop)
        {
            var result = new CustomJsonResult();

            var config = BizFactory.Merch.GetWxMpAppInfoConfig(rop.MerchId);

            if (config == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "配置信息失败");
            }

            var ret = SdkFactory.Wx.GetJsCode2Session(config, rop.Code);

            if (ret == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解释信息失败");
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { openid = ret.openid, session_key = ret.session_key });

            return result;
        }


        public CustomJsonResult GetWxPhoneNumber(RopWxGetPhoneNumber rop)
        {
            var result = new CustomJsonResult();

            var ret = SdkFactory.Wx.GetWxPhoneNumber(rop.encryptedData, rop.iv, rop.session_key);

            if (ret == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解释数据失败");
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
