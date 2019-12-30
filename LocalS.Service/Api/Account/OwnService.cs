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

namespace LocalS.Service.Api.Account
{
    public class OwnService : BaseDbContext
    {
        [DllImport(@"BioVein.x64.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_MatchFeature([MarshalAs(UnmanagedType.LPArray)] byte[] featureDataMatch, [MarshalAs(UnmanagedType.LPArray)]  byte[] featureDataReg, byte RegCnt, byte flag, byte securityLevel, int[] diff, [MarshalAs(UnmanagedType.LPArray)] byte[] AIDataBuf, int[] AIDataLen);


        private void LoginLog(string operater, string userId, Enumeration.LoginResult loginResult, Enumeration.LoginWay loginWay, string ip, string location, string description)
        {
            var userLoginHis = new SysUserLoginHis();

            userLoginHis.Id = GuidUtil.New();
            userLoginHis.Ip = ip;
            userLoginHis.UserId = userId;
            userLoginHis.LoginWay = loginWay;
            userLoginHis.LoginTime = DateTime.Now;
            userLoginHis.Location = location;
            userLoginHis.Result = loginResult;
            userLoginHis.Description = description;
            userLoginHis.CreateTime = DateTime.Now;
            userLoginHis.Creator = operater;

            CurrentDb.SysUserLoginHis.Add(userLoginHis);
            CurrentDb.SaveChanges();
        }
        public CustomJsonResult LoginByAccount(RopOwnLoginByAccount rop)
        {
            if (rop.LoginWay == Enumeration.LoginWay.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未指定登录方式");
            }

            if (rop.LoginWay == Enumeration.LoginWay.StoreTerm)
            {
                if (rop.LoginPms == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败,LoginPms值不能为空");
                }

                if (!rop.LoginPms.ContainsKey("machineId") || string.IsNullOrEmpty(rop.LoginPms["machineId"].ToString()))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，缺少指定参数LoginPms.machineId");
                }
            }

            var result = new CustomJsonResult();
            var ret = new RetOwnLoginByAccount();

            var sysUser = CurrentDb.SysUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();

            if (sysUser == null)
            {
                LoginLog("", "", Enumeration.LoginResult.Failure, rop.LoginWay, rop.Ip, "", "登录失败，账号不存在");
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，账号不存在");
            }

            if (!PassWordHelper.VerifyHashedPassword(sysUser.PasswordHash, rop.Password))
            {
                LoginLog(sysUser.Id, sysUser.Id, Enumeration.LoginResult.Failure, rop.LoginWay, rop.Ip, "", "登录失败，密码不正确");
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，密码不正确");
            }

            if (sysUser.IsDisable)
            {
                LoginLog(sysUser.Id, sysUser.Id, Enumeration.LoginResult.Failure, rop.LoginWay, rop.Ip, "", "登录失败，账号已被禁用");
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，账号已被禁用");
            }

            ret.Token = GuidUtil.New();

            var tokenInfo = new TokenInfo();
            tokenInfo.UserId = sysUser.Id;


            switch (rop.LoginWay)
            {
                case Enumeration.LoginWay.Website:
                    #region Website

                    switch (sysUser.BelongSite)
                    {
                        case Enumeration.BelongSite.Agent:
                            var agentUser = CurrentDb.SysAgentUser.Where(m => m.Id == sysUser.Id).FirstOrDefault();
                            if (agentUser == null)
                            {
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户不属于该站点");
                            }

                            tokenInfo.AgentId = agentUser.AgentId;

                            break;
                        case Enumeration.BelongSite.Merch:

                            var merchUser = CurrentDb.SysMerchUser.Where(m => m.Id == sysUser.Id).FirstOrDefault();
                            if (merchUser == null)
                            {
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户不属于该站点");
                            }

                            tokenInfo.MerchId = merchUser.MerchId;

                            break;
                    }


                    #endregion
                    break;
                case Enumeration.LoginWay.StoreTerm:
                    #region StoreTerm

                    string machineId = rop.LoginPms["machineId"].ToString();
                    var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();
                    if (machine == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未登记");
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
                    ret.UserName = storeTermUser.UserName;
                    ret.FullName = storeTermUser.FullName;
                    #endregion
                    break;
            }




            LoginLog(sysUser.Id, sysUser.Id, Enumeration.LoginResult.Success, rop.LoginWay, rop.Ip, "", "登录成功");

            SSOUtil.SetTokenInfo(ret.Token, tokenInfo, new TimeSpan(1, 0, 0));

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);

            return result;
        }

        public CustomJsonResult LoginByMinProgram(RopOwnLoginByMinProgram rop)
        {
            var result = new CustomJsonResult();
            var ret = new RetOwnLoginByMinProgram();

            WxUserInfo wxUserInfo = null;

            if (string.IsNullOrEmpty(rop.OpenId))
            {
                var merch = CurrentDb.Merch.Where(m => m.Id == rop.MerchId && m.WxMpAppId == rop.AppId).FirstOrDefault();

                if (merch == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商户信息认证失败");
                }

                var wxAppInfoConfig = new WxAppInfoConfig();

                wxAppInfoConfig.AppId = merch.WxMpAppId;
                wxAppInfoConfig.AppSecret = merch.WxMpAppSecret;
                wxAppInfoConfig.PayMchId = merch.WxPayMchId;
                wxAppInfoConfig.PayKey = merch.WxPayKey;
                wxAppInfoConfig.PayResultNotifyUrl = merch.WxPayResultNotifyUrl;
                wxAppInfoConfig.NotifyEventUrlToken = merch.WxPaNotifyEventUrlToken;


                var wxUserInfoByMinProram = SdkFactory.Wx.GetUserInfoByMinProramJsCode(wxAppInfoConfig, rop.EncryptedData, rop.Iv, rop.Code);

                if (wxUserInfoByMinProram == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "获取微信用户信息失败");
                }

                wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.OpenId == wxUserInfoByMinProram.openId).FirstOrDefault();
                if (wxUserInfo == null)
                {
                    string sysClientUserId = GuidUtil.New();

                    var sysClientUser = new SysClientUser();

                    sysClientUser.Id = sysClientUserId;
                    sysClientUser.UserName = string.Format("wx{0}", Guid.NewGuid().ToString().Replace("-", ""));
                    sysClientUser.PasswordHash = PassWordHelper.HashPassword("888888");
                    sysClientUser.SecurityStamp = Guid.NewGuid().ToString();
                    sysClientUser.RegisterTime = DateTime.Now;
                    sysClientUser.NickName = wxUserInfoByMinProram.nickName;
                    sysClientUser.Sex = wxUserInfoByMinProram.gender;
                    sysClientUser.Province = wxUserInfoByMinProram.province;
                    sysClientUser.City = wxUserInfoByMinProram.city;
                    sysClientUser.Country = wxUserInfoByMinProram.country;
                    sysClientUser.Avatar = wxUserInfoByMinProram.avatarUrl;
                    sysClientUser.IsVip = false;
                    sysClientUser.CreateTime = DateTime.Now;
                    sysClientUser.Creator = sysClientUserId;
                    sysClientUser.BelongSite = Enumeration.BelongSite.Client;
                    sysClientUser.MerchId = rop.MerchId;
                    CurrentDb.SysClientUser.Add(sysClientUser);
                    CurrentDb.SaveChanges();

                    wxUserInfo = new WxUserInfo();
                    wxUserInfo.Id = GuidUtil.New();
                    wxUserInfo.MerchId = rop.MerchId;
                    wxUserInfo.AppId = rop.AppId;
                    wxUserInfo.ClientUserId = sysClientUser.Id;
                    wxUserInfo.OpenId = wxUserInfoByMinProram.openId;
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
                        sysClientUser.NickName = wxUserInfoByMinProram.nickName;
                        sysClientUser.Sex = wxUserInfoByMinProram.gender;
                        sysClientUser.Province = wxUserInfoByMinProram.province;
                        sysClientUser.City = wxUserInfoByMinProram.city;
                        sysClientUser.Country = wxUserInfoByMinProram.country;
                        sysClientUser.Avatar = wxUserInfoByMinProram.avatarUrl;
                    }
                    CurrentDb.SaveChanges();
                }
            }
            else
            {
                wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.OpenId == rop.OpenId).FirstOrDefault();
            }

            var tokenInfo = new TokenInfo();
            ret.Token = GuidUtil.New();
            ret.OpenId = wxUserInfo.OpenId;

            tokenInfo.UserId = wxUserInfo.ClientUserId;

            SSOUtil.SetTokenInfo(ret.Token, tokenInfo, new TimeSpan(1, 0, 0));

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);

            return result;
        }

        public CustomJsonResult LoginByFingerVein(RopOwnLoginByFingerVein rop)
        {

            if (rop.LoginWay == Enumeration.LoginWay.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未指定登录方式");
            }

            if (rop.LoginWay == Enumeration.LoginWay.StoreTerm)
            {
                if (rop.LoginPms == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败,LoginPms值不能为空");
                }

                if (!rop.LoginPms.ContainsKey("machineId") || string.IsNullOrEmpty(rop.LoginPms["machineId"].ToString()))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，缺少指定参数LoginPms.machineId");
                }
            }

            var result = new CustomJsonResult();

            var ret = new RetOwnLoginByAccount();

            LogUtil.Info("静指脉数据1：" + rop.VeinData);

            string userId = "";
            bool isMachSuccess = false;

            try
            {
                var sysUserFingerVeins = CurrentDb.SysUserFingerVein.ToList();
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
                var tokenInfo = new TokenInfo();
                tokenInfo.UserId = userId;

                switch (rop.LoginWay)
                {
                    case Enumeration.LoginWay.StoreTerm:
                        #region StoreTerm

                        string machineId = rop.LoginPms["machineId"].ToString();
                        var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();
                        if (machine == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未登记");
                        }

                        var storeTermUser = CurrentDb.SysMerchUser.Where(m => m.Id == userId).FirstOrDefault();
                        if (storeTermUser == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该用户不属于该站点");
                        }

                        if (machine.CurUseMerchId != storeTermUser.MerchId)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "帐号与商户不对应");
                        }
                        ret.UserName = storeTermUser.UserName;
                        ret.FullName = storeTermUser.FullName;
                        #endregion
                        break;
                }


                SSOUtil.SetTokenInfo(ret.Token, tokenInfo, new TimeSpan(1, 0, 0));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);
            }

            return result;
        }

        public List<MenuNode> GetMenus(Enumeration.BelongSite belongSite, string userId)
        {
            List<MenuNode> menuNodes = new List<MenuNode>();

            var sysMenus = CurrentDb.SysMenu.Where(m => m.BelongSite == belongSite && m.Depth != 0).OrderBy(m => m.Priority).ToList();

            if (belongSite == Enumeration.BelongSite.Admin || belongSite == Enumeration.BelongSite.Merch)
            {
                sysMenus = (from menu in CurrentDb.SysMenu where (from rolemenu in CurrentDb.SysRoleMenu where (from sysUserRole in CurrentDb.SysUserRole where sysUserRole.UserId == userId select sysUserRole.RoleId).Contains(rolemenu.RoleId) select rolemenu.MenuId).Contains(menu.Id) && menu.BelongSite == belongSite select menu).Where(m => m.Depth != 0).OrderBy(m => m.Priority).ToList();
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

        public List<RoleModel> GetRoles(Enumeration.BelongSite belongSite, string userId)
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
        //private List<TreeNode> GetMenuTree(string id, List<SysMenu> sysMenus)
        //{
        //    List<TreeNode> treeNodes = new List<TreeNode>();

        //    var p_sysMenus = sysMenus.Where(t => t.PId == id).ToList();

        //    foreach (var p_sysMenu in p_sysMenus)
        //    {
        //        TreeNode treeNode = new TreeNode();
        //        treeNode.Id = p_sysMenu.Id;
        //        treeNode.PId = p_sysMenu.PId;
        //        treeNode.Label = p_sysMenu.Title;
        //        treeNode.Children.AddRange(GetMenuTree(treeNode.Id, sysMenus));
        //        treeNodes.Add(treeNode);
        //    }

        //    return treeNodes;
        //}

        //public List<MenuNode> GetMenus(Enumeration.BelongSite belongSite)
        //{
        //    var sysMenus = CurrentDb.SysMenu.Where(m => m.BelongSite == belongSite).ToList();

        //    var topMenu = sysMenus.Where(m => m.Dept == 0).FirstOrDefault();

        //    return GetMenuTree(topMenu.Id, sysMenus);
        //}
        //private List<MenuNode> GetMenuTree(string id, List<SysMenu> sysMenus)
        //{
        //    List<MenuNode> menuNodes = new List<MenuNode>();

        //    var p_sysMenus = sysMenus.Where(t => t.PId == id).ToList();


        //    foreach (var p_sysMenu in p_sysMenus)
        //    {
        //        var menuNode = new MenuNode();
        //        menuNode.Id = p_sysMenu.Id;
        //        menuNode.PId = p_sysMenu.PId;
        //        menuNode.Path = p_sysMenu.Path == "/home" ? "/" : p_sysMenu.Path;
        //        menuNode.Component = null;
        //        menuNode.IsSidebar = p_sysMenu.IsSidebar;
        //        menuNode.IsNavbar = p_sysMenu.IsNavbar;
        //        menuNode.Icon = p_sysMenu.Icon;
        //        menuNode.Title = p_sysMenu.Title;
        //        var children = (from c in sysMenus where c.PId == p_sysMenu.Id select c).ToList();
        //        if (children.Count == 0)
        //        {
        //            if (p_sysMenu.Dept == 1)
        //            {
        //                menuNode.Name = null;
        //                menuNode.IsNavbar = false;
        //                menuNode.Children.Add(new MenuNode { Id = p_sysMenu.Id, PId = p_sysMenu.PId, IsNavbar = p_sysMenu.IsNavbar, IsSidebar = p_sysMenu.IsSidebar, Name = p_sysMenu.Name, Title = p_sysMenu.Title, Icon = p_sysMenu.Icon, Path = p_sysMenu.Path, Component = p_sysMenu.Component, Children = null });
        //            }
        //            else
        //            {
        //                menuNode.Children = null;
        //                menuNode.Component = p_sysMenu.Component;
        //            }
        //        }
        //        else
        //        {
        //            menuNode.Name = null;
        //            menuNode.Component = p_sysMenu.Component;
        //            menuNode.Children.AddRange(GetMenuTree(p_sysMenu.Id, sysMenus));
        //        }
        //        menuNodes.Add(menuNode);
        //    }

        //    return menuNodes;
        //}

        //private List<MenuNode> GetMenus(Enumeration.BelongSite belongSite)
        //{
        //    var menus = new List<MenuNode>();

        //    var sysMenus = CurrentDb.SysMenu.Where(m => m.BelongSite == belongSite).ToList();

        //    var sysMenusDept1 = from c in sysMenus where c.Dept == 1 select c;

        //    foreach (var sysMenuDept1 in sysMenusDept1)
        //    {
        //        var menu1 = new MenuNode();

        //        menu1.Path = sysMenuDept1.Path == "/home" ? "/" : sysMenuDept1.Path;
        //        menu1.Component = null;
        //        menu1.Hidden = !sysMenuDept1.IsSidebar;
        //        menu1.Meta = new MenuMeta { Title = sysMenuDept1.Title, Icon = sysMenuDept1.Icon };

        //        var sysMenusDept2 = (from c in sysMenus where c.PId == sysMenuDept1.Id select c).ToList();

        //        if (sysMenusDept2.Count == 0)
        //        {
        //            menu1.Name = null;
        //            menu1.Meta = null;
        //            menu1.Navbar = false;
        //            menu1.Redirect = sysMenuDept1.Path;

        //            menu1.Children.Add(new MenuChild { Navbar = sysMenuDept1.IsNavbar, Hidden = !sysMenuDept1.IsSidebar, Name = sysMenuDept1.Name, Path = sysMenuDept1.Path, Component = sysMenuDept1.Component, Meta = new MenuMeta { Title = sysMenuDept1.Title, Icon = sysMenuDept1.Icon } });
        //        }
        //        else
        //        {
        //            menu1.Name = sysMenuDept1.Name;
        //            foreach (var sysMenuDept2 in sysMenusDept2)
        //            {
        //                menu1.Children.Add(new MenuChild { Navbar = sysMenuDept2.IsNavbar, Hidden = !sysMenuDept2.IsSidebar, Name = sysMenuDept2.Name, Path = sysMenuDept2.Path, Component = sysMenuDept2.Component, Meta = new MenuMeta { Title = sysMenuDept2.Title, Icon = sysMenuDept2.Icon } });
        //            }
        //        }

        //        menus.Add(menu1);
        //    }

        //    return menus;
        //}
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
                    ret.Menus = GetMenus(Enumeration.BelongSite.Merch, userId);
                    ret.Roles = GetRoles(Enumeration.BelongSite.Merch, userId);
                    break;
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Logout(string operater, string userId, string token)
        {
            var result = new CustomJsonResult();


            SSOUtil.Quit(token);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "退出成功");

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

            var sysUserFingerVein = new SysUserFingerVein();
            sysUserFingerVein.Id = GuidUtil.New();
            sysUserFingerVein.UserId = userId;
            sysUserFingerVein.VeinData = Convert.FromBase64String(rop.VeinData);
            sysUserFingerVein.CreateTime = DateTime.Now;
            sysUserFingerVein.Creator = operater;
            CurrentDb.SysUserFingerVein.Add(sysUserFingerVein);
            CurrentDb.SaveChanges();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "录入成功");

            return result;
        }
    }
}
