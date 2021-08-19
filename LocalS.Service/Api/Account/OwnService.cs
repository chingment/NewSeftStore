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
using System.IO;
using System.Net;

namespace LocalS.Service.Api.Account
{
    public class OwnService : BaseService
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
                        //M多店铺
                        //S单店铺
                        //线上商城 F
                        //线下设备 K


                        mctMode = (mctMode.Replace("M", "")).Replace("S", "");

                        if (mctMode == "F")
                        {
                            sysMenus = sysMenus.Where(m => m.BelongMctMode != "K").ToList();
                        }


                    }
                }

            }

            //foreach (var sysMenu in sysMenus)
            //{
            //    MenuNode menuNode = new MenuNode();
            //    menuNode.Id = sysMenu.Id;
            //    menuNode.PId = sysMenu.PId;
            //    menuNode.Path = sysMenu.Path;
            //    menuNode.Name = sysMenu.Name;
            //    menuNode.Icon = sysMenu.Icon;
            //    menuNode.Title = sysMenu.Title;
            //    menuNode.Component = sysMenu.Component;
            //    menuNode.IsSidebar = sysMenu.IsSidebar;
            //    menuNode.IsNavbar = sysMenu.IsNavbar;
            //    menuNode.IsRouter = sysMenu.IsRouter;
            //    menuNodes.Add(menuNode);
            //}


            menuNodes = GetMenuTree("10000000000000000000000000000025", sysMenus);
            return menuNodes;

        }

        private List<MenuNode> GetMenus2(Enumeration.BelongSite belongSite, string userId, string mctMode = "")
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
                menuNode.Redirect = sysMenu.Redirect;
                menuNodes.Add(menuNode);
            }

            return menuNodes;

        }

        private List<MenuNode> GetMenuTree(string id, List<SysMenu> sysMenus)
        {
            List<MenuNode> treeNodes = new List<MenuNode>();

            var p_sysMenus = sysMenus.Where(t => t.PId == id).ToList();

            foreach (var p_sysMenu in p_sysMenus)
            {
                MenuNode menuNode = new MenuNode();
                menuNode.Id = p_sysMenu.Id;
                menuNode.PId = p_sysMenu.PId;
                menuNode.Path = p_sysMenu.Path;
                menuNode.Name = p_sysMenu.Name;
                menuNode.Icon = p_sysMenu.Icon;
                menuNode.Title = p_sysMenu.Title;
                menuNode.Component = p_sysMenu.Component;
                menuNode.IsSidebar = p_sysMenu.IsSidebar;
                menuNode.IsNavbar = p_sysMenu.IsNavbar;
                menuNode.IsRouter = p_sysMenu.IsRouter;
                menuNode.Redirect = p_sysMenu.Redirect;


                var children = GetMenuTree(p_sysMenu.Id, sysMenus);
                if (children != null)
                {
                    if (children.Count > 0)
                    {
                        menuNode.Children = new List<MenuNode>();
                        menuNode.Children = children;
                    }
                }

                treeNodes.Add(menuNode);
            }

            return treeNodes;
        }

        //private List<TreeNode> GetMenuTree(Enumeration.BelongSite belongSite)
        //{
        //    var sysMenus = CurrentDb.SysMenu.Where(m => m.BelongSite == belongSite).OrderBy(m => m.Priority).ToList();

        //    var topMenu = sysMenus.Where(m => m.Depth == 0).FirstOrDefault();

        //    return GetMenuTree(topMenu.Id, sysMenus);
        //}

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
            string deviceId = "";

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

                if (!rop.LoginPms.ContainsKey("deviceId") || string.IsNullOrEmpty(rop.LoginPms["deviceId"].ToString()))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，缺少参数loginPms.deviceId");
                }

                deviceId = rop.LoginPms["deviceId"].ToString();

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

                MqFactory.Global.PushEventNotify(sysUser.Id, AppId.MERCH, merchUser.MerchId, EventCode.login, "登录成功",0,"normal", new LoginLogModel { LoginAccount = sysUser.UserName, LoginFun = Enumeration.LoginFun.Account, LoginResult = Enumeration.LoginResult.LoginSuccess, LoginWay = rop.LoginWay, LoginIp = rop.Ip });

                SSOUtil.SetTokenInfo(token, tokenInfo, new TimeSpan(1, 0, 0));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", new { Token = token });

                #endregion
            }
            else if (rop.AppId == AppId.STORETERM)
            {
                #region STORETERM

                var device = CurrentDb.Device.Where(m => m.Id == deviceId).FirstOrDefault();
                if (device == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该设备未登记");
                }

                if (string.IsNullOrEmpty(device.CurUseMerchId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该设备未绑定商家");
                }

                if (string.IsNullOrEmpty(device.CurUseStoreId))
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该设备未绑定店铺");
                }

                var storeTermUser = CurrentDb.SysMerchUser.Where(m => m.Id == sysUser.Id).FirstOrDefault();
                if (storeTermUser == null)
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户不属于该站点");
                }

                if (device.CurUseMerchId != storeTermUser.MerchId)
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "帐号与商户不对应");
                }

                MqFactory.Global.PushOperateLog(sysUser.Id, AppId.STORETERM, deviceId, EventCode.login, "登录成功", new LoginLogModel { LoginAccount = sysUser.UserName, LoginFun = Enumeration.LoginFun.Account, LoginResult = Enumeration.LoginResult.LoginSuccess, LoginWay = rop.LoginWay, LoginIp = rop.Ip });


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

            var merch = CurrentDb.Merch.Where(m => m.Id == rop.MerchId && m.WxMpAppId == rop.AppId).FirstOrDefault();

            if (merch == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "微信信息认证失败[01]");
            }

            var wxAppInfoConfig = new WxAppInfoConfig();

            wxAppInfoConfig.AppId = merch.WxMpAppId;
            wxAppInfoConfig.AppSecret = merch.WxMpAppSecret;
            wxAppInfoConfig.PayMchId = merch.WxPayMchId;
            wxAppInfoConfig.PayKey = merch.WxPayKey;
            wxAppInfoConfig.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;
            wxAppInfoConfig.NotifyEventUrlToken = merch.WxPaNotifyEventUrlToken;

            UserInfoModelByMinProramJsCode wxUserInfo = null;
            WxPhoneNumber wxPhoneNumber = null;

            if (rop.UserInfoEp == null)
            {
                LogUtil.Info("rop.UserInfoEp=> is null");
            }

            if (rop.UserInfoEp != null)
            {
                wxUserInfo = SdkFactory.Wx.GetUserInfoByMinProramJsCode(wxAppInfoConfig, rop.UserInfoEp.EncryptedData, rop.UserInfoEp.Iv, rop.UserInfoEp.Code);
                if (wxUserInfo == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "微信用户信息认证失败[02]");
                }
            }

            if (rop.PhoneNumberEp != null)
            {
                LogUtil.Info("rop.PhoneNumberEp=>" + rop.PhoneNumberEp.ToJsonString());
                wxPhoneNumber = SdkFactory.Wx.GetWxPhoneNumber(rop.PhoneNumberEp.encryptedData, rop.PhoneNumberEp.iv, rop.PhoneNumberEp.session_key);
                LogUtil.Info("rop.wxPhoneNumber=>" + wxPhoneNumber.ToJsonString());
            }

            var d_clientUser = CurrentDb.SysClientUser.Where(m => m.WxMpOpenId == rop.OpenId).FirstOrDefault();

            if (d_clientUser == null)
            {
                d_clientUser = new SysClientUser();
                d_clientUser.Id = IdWorker.Build(IdType.NewGuid);


                var avatarUrl = CreateWxAvatar(d_clientUser.Id, wxUserInfo.avatarUrl);

                if (string.IsNullOrEmpty(avatarUrl))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "微信用户信息认证失败[03]");
                }

                d_clientUser.UserName = string.Format("wx{0}", Guid.NewGuid().ToString().Replace("-", ""));
                d_clientUser.PasswordHash = PassWordHelper.HashPassword("888888");
                d_clientUser.SecurityStamp = Guid.NewGuid().ToString();
                d_clientUser.RegisterTime = DateTime.Now;
                d_clientUser.NickName = wxUserInfo.nickName;
                d_clientUser.Sex = wxUserInfo.gender;
                d_clientUser.Province = wxUserInfo.province;
                d_clientUser.City = wxUserInfo.city;
                d_clientUser.Country = wxUserInfo.country;
                d_clientUser.Avatar = avatarUrl;

                if (wxPhoneNumber != null)
                {
                    d_clientUser.PhoneNumber = wxPhoneNumber.phoneNumber;
                }

                d_clientUser.MemberLevel = 0;
                d_clientUser.CreateTime = DateTime.Now;
                d_clientUser.Creator = d_clientUser.Id;
                d_clientUser.BelongType = Enumeration.BelongType.Client;
                d_clientUser.MerchId = rop.MerchId;
                d_clientUser.WxMpOpenId = rop.OpenId;
                d_clientUser.WxMpAppId = rop.AppId;
                d_clientUser.ReffSign = rop.ReffSign;
                CurrentDb.SysClientUser.Add(d_clientUser);
                CurrentDb.SaveChanges();

            }
            else
            {
                if (wxUserInfo != null)
                {
                    var avatarUrl = CreateWxAvatar(d_clientUser.Id, wxUserInfo.avatarUrl);

                    if (string.IsNullOrEmpty(avatarUrl))
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "微信用户信息认证失败[04]");
                    }

                    d_clientUser.NickName = wxUserInfo.nickName;
                    d_clientUser.Sex = wxUserInfo.gender;
                    d_clientUser.Province = wxUserInfo.province;
                    d_clientUser.City = wxUserInfo.city;
                    d_clientUser.Country = wxUserInfo.country;
                    d_clientUser.Avatar = avatarUrl;
                }

                if (wxPhoneNumber != null)
                {
                    d_clientUser.PhoneNumber = wxPhoneNumber.phoneNumber;
                }

                if (string.IsNullOrEmpty(d_clientUser.ReffSign))
                {
                    d_clientUser.ReffSign = rop.ReffSign;
                }

                d_clientUser.MendTime = DateTime.Now;
                d_clientUser.Mender = d_clientUser.Id;
                CurrentDb.SaveChanges();
            }


            if (string.IsNullOrEmpty(d_clientUser.PhoneNumber))
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2NoPhoneNumber, "未授权手机号码[05]", ret);

            var tokenInfo = new TokenInfo();
            ret.Token = IdWorker.Build(IdType.NewGuid);
            ret.OpenId = d_clientUser.WxMpOpenId;

            tokenInfo.UserId = d_clientUser.Id;

            SSOUtil.SetTokenInfo(ret.Token, tokenInfo, new TimeSpan(24 * 7, 0, 0));

            MqFactory.Global.PushOperateLog(d_clientUser.Id, AppId.WXMINPRAGROM, merch.MctStoreId, EventCode.login, "登录成功",new LoginLogModel { LoginAccount = d_clientUser.UserName, LoginFun = Enumeration.LoginFun.MpAuth, LoginResult = Enumeration.LoginResult.LoginSuccess, LoginWay = Enumeration.LoginWay.Wxmp, LoginIp = rop.Ip });

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);

            return result;
        }

        public CustomJsonResult BindPhoneNumberByWx(string clientUserId, RopOwnBindPhoneNumberByWx rop)
        {
            var result = new CustomJsonResult();

            var d_clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();
            if (d_clientUser != null)
            {
                d_clientUser.PhoneNumber = rop.PhoneNumber;
                d_clientUser.MendTime = DateTime.Now;
                d_clientUser.Mender = d_clientUser.Id;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");

            return result;
        }

        public CustomJsonResult LoginByFingerVein(RopOwnLoginByFingerVein rop)
        {

            string deviceId = "";
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

                if (!rop.LoginPms.ContainsKey("deviceId") || string.IsNullOrEmpty(rop.LoginPms["deviceId"].ToString()))
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，缺少参数loginPms.deviceId");
                }
                else
                {
                    deviceId = rop.LoginPms["deviceId"].ToString();
                }

                var device = BizFactory.Device.GetOne(deviceId);
                if (device == null)
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该设备未登记");
                }

                if (string.IsNullOrEmpty(device.MerchId))
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该设备未绑定商家");
                }

                if (string.IsNullOrEmpty(device.StoreId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该设备未绑定店铺");
                }

                belongType = Enumeration.BelongType.Merch;
                belongId = device.MerchId;
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


                    MqFactory.Global.PushOperateLog(userId, AppId.STORETERM, deviceId, EventCode.login, "登录成功", new LoginLogModel { LoginAccount = sysUser.UserName, LoginFun = Enumeration.LoginFun.FingerVein, LoginResult = Enumeration.LoginResult.LoginSuccess, LoginWay = rop.LoginWay });

                    #endregion
                }


                SSOUtil.SetTokenInfo(token, tokenInfo, new TimeSpan(1, 0, 0));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", new { UserName = sysUser.UserName, FullName = sysUser.FullName });
            }

            return result;
        }

        public CustomJsonResult Logout(string operater, string userId, RopOwnLogout rop)
        {
            var result = new CustomJsonResult();

            string userName = null;

            var tokenInfo = SSOUtil.GetTokenInfo(rop.Token);
            if (tokenInfo != null)
            {
                userName = tokenInfo.UserName;
            }

            SSOUtil.Quit(rop.Token);

            MqFactory.Global.PushOperateLog(operater, rop.AppId, rop.BelongId, EventCode.logout, "退出成功", new LoginLogModel { LoginAccount = userName, LoginResult = Enumeration.LoginResult.LogoutSuccess, LoginWay = rop.LoginWay, LoginIp = rop.Ip });

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
                        ret.Menus = GetMenus2(Enumeration.BelongSite.Admin, userId);
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

            List<string> permission = new List<string>();

            switch (rup.Type)
            {
                case "1":
                    string content = rup.Content;
                    LogUtil.Info("content1:" + content);
                    if (string.IsNullOrEmpty(content))
                    {
                        content = "MerchHome";
                    }
                    LogUtil.Info("content2:" + content);
                    var menus = GetMenus2(webSite, userId);
                    var hasMenu = menus.Where(m => m.Name == content).Count();
                    if (hasMenu == 0)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2NoRight, "没有权限访问页面");
                    }

                    permission = menus.Select(m => m.Name).ToList();

                    //string content = rup.Content;
                    //if (rup.Content == "/")
                    //{
                    //    path = "/home";
                    //}

                    //var menus = GetMenus2(webSite, userId);
                    //var hasMenu = menus.Where(m => m.Name == path || m.Redirect == path).Count();
                    //if (hasMenu == 0)
                    //{
                    //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2NoRight, "没有权限访问页面");
                    //}
                    //permission = menus.Select(m => m.Name).ToList();

                    break;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "检查成功", new { permission = permission });

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

            var l_Device = BizFactory.Device.GetOne(rop.DeviceId);

            if (l_Device == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未登记");
            }

            if (string.IsNullOrEmpty(l_Device.MerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定商户");
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
                    var sysUserFingerVeins = CurrentDb.SysUserFingerVein.Where(m => m.BelongId == l_Device.MerchId && m.BelongType == Enumeration.BelongType.Merch).ToList();
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
                sysUserFingerVein.BelongId = l_Device.MerchId;
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


            var config = BizFactory.Merch.GetWxMpAppInfoConfigByAppId(rop.AppId);

            if (config == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "配置信息失败");
            }

            var ret = SdkFactory.Wx.GetJsCode2Session(config, rop.Code);

            if (ret == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解释信息失败");
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { openid = ret.openid, session_key = ret.session_key, merchid = config.MyMerchId, storeid = config.MyStoreId });

            return result;
        }

        public CustomJsonResult GetConfig(RopGetConfig rop)
        {
            var result = new CustomJsonResult();


            var config = BizFactory.Merch.GetWxMpAppInfoConfigByAppId(rop.AppId);

            if (config == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "配置信息失败");
            }

            var ret = SdkFactory.Wx.GetJsCode2Session(config, rop.Code);

            if (ret == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解释信息失败");
            }

            var d_clientUser = CurrentDb.SysClientUser.Where(m => m.WxMpOpenId == ret.openid).FirstOrDefault();
            if (d_clientUser != null)
            {


            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { openId = ret.openid, sessionKey = ret.session_key, merchId = config.MyMerchId, storeId = config.MyStoreId });

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

        public CustomJsonResult ChangePassword(string operater, string userId, RopOwnChangePassword rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var user = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();

                if (string.IsNullOrEmpty(rop.Password))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "密码不能为空");
                }


                user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                user.MendTime = DateTime.Now;
                user.Mender = operater;


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }


            return result;
        }

        public CustomJsonResult GetWxACodeUnlimit(string operater, string userId, RopOwnGetWxACodeUnlimit rop)
        {
            var result = new CustomJsonResult();

            try
            {
                string id = Lumos.CommonUtil.ConvetMD5IN32B(rop.Data);

                var d_WxACode = CurrentDb.WxACode.Where(m => m.Id == id).FirstOrDefault();

                string domainPathUrl = "";
                if (d_WxACode == null)
                {

                    var merch = CurrentDb.Merch.Where(m => m.Id == rop.MerchId && m.WxMpAppId == rop.AppId).FirstOrDefault();

                    if (merch == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "生成小程序码失败[01]");
                    }

                    var wxAppInfoConfig = new WxAppInfoConfig();

                    wxAppInfoConfig.AppId = merch.WxMpAppId;
                    wxAppInfoConfig.AppSecret = merch.WxMpAppSecret;
                    wxAppInfoConfig.PayMchId = merch.WxPayMchId;
                    wxAppInfoConfig.PayKey = merch.WxPayKey;
                    wxAppInfoConfig.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;
                    wxAppInfoConfig.NotifyEventUrlToken = merch.WxPaNotifyEventUrlToken;

                    var result_WxACodeUnlimit = SdkFactory.Wx.GetWxACodeUnlimit(wxAppInfoConfig, id, "pages/main/main");

                    if (result_WxACodeUnlimit == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "生成小程序码失败[02]");
                    }

                    if (result_WxACodeUnlimit.errcode != "0")
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "生成小程序码失败[03]");
                    }

                    byte[] fileData = result_WxACodeUnlimit.buffer;

                    string domain = System.Configuration.ConfigurationManager.AppSettings["custom:FilesServerUrl"];
                    string rootPath = System.Configuration.ConfigurationManager.AppSettings["custom:FileServerUploadPath"];
                    string fileName = id;
                    string extension = ".png";
                    string savePath = "/upload/acode/";
                    string serverSavePath = rootPath + savePath + fileName + extension;
                    domainPathUrl = domain + savePath + fileName + extension;

                    DirectoryInfo dir = new DirectoryInfo(rootPath + savePath);
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }

                    LogUtil.Info("serverSavePath:" + serverSavePath);
                    LogUtil.Info("domainPathUrl:" + domainPathUrl);

                    FileStream fs = new FileStream(serverSavePath, FileMode.Create, FileAccess.Write);
                    fs.Write(fileData, 0, fileData.Length);
                    fs.Flush();
                    fs.Close();

                    d_WxACode = new WxACode();
                    d_WxACode.Id = id;
                    d_WxACode.MerchId = rop.MerchId;
                    d_WxACode.AppId = rop.AppId;
                    d_WxACode.OpenId = rop.OpenId;
                    d_WxACode.Type = rop.Type;
                    d_WxACode.Data = rop.Data;
                    d_WxACode.UserId = operater;
                    d_WxACode.ImgUrl = domainPathUrl;
                    d_WxACode.Creator = operater;
                    d_WxACode.CreateTime = DateTime.Now;
                    CurrentDb.WxACode.Add(d_WxACode);
                    CurrentDb.SaveChanges();
                }
                else
                {
                    domainPathUrl = d_WxACode.ImgUrl;
                }

                if (string.IsNullOrEmpty(domainPathUrl))
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "生成小程序码失败[04]");

                string avatar = "";
                string nickName = "";
                if (rop.IsGetAvatar)
                {
                    var d_SysClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
                    if (d_SysClientUser != null)
                    {
                        avatar = d_SysClientUser.Avatar;
                        nickName = d_SysClientUser.NickName;
                    }
                }

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "生成成功", new { WxACodeUrl = domainPathUrl, Avatar = avatar, NickName = nickName });
            }
            catch (Exception ex)
            {
                LogUtil.Error("生成小程序码失败[05]", ex);

                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "生成小程序码失败[05]");
            }
        }

        public string CreateWxAvatar(string userId, string avatarUrl)
        {
            string l_AvatarUrl = null;

            if (string.IsNullOrEmpty(avatarUrl))
                return l_AvatarUrl;

            try
            {
                string domain = System.Configuration.ConfigurationManager.AppSettings["custom:FilesServerUrl"];
                string rootPath = System.Configuration.ConfigurationManager.AppSettings["custom:FileServerUploadPath"];

                string savePath = "/Upload/avatar/";
                string fileName = string.Format("{0}.png", userId);

                var wreq = WebRequest.Create(avatarUrl);
                HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse();
                Stream s = wresp.GetResponseStream();
                var img = System.Drawing.Image.FromStream(s);

                string avatarSavePath = string.Format("{0}{1}{2}", rootPath, savePath, fileName);

                if (File.Exists(avatarSavePath))
                    File.Delete(avatarSavePath);

                img.Save(avatarSavePath, System.Drawing.Imaging.ImageFormat.Png);   //保存

                l_AvatarUrl = string.Format("{0}{1}{2}", domain, savePath, fileName);


            }
            catch (Exception ex)
            {
                LogUtil.Error("创建微信用户头像失败", ex);
            }

            return l_AvatarUrl;
        }
    }
}
