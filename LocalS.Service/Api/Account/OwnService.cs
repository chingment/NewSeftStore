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

namespace LocalS.Service.Api.Account
{
    public class OwnService : BaseDbContext
    {
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

            switch (sysUser.BelongSite)
            {
                case Enumeration.BelongSite.Agent:
                    var agent = CurrentDb.Agent.Where(m => m.Id == sysUser.Id).FirstOrDefault();
                    if (agent != null)
                    {
                        tokenInfo.AgentId = agent.Id;
                    }
                    break;
            }


            LoginLog(sysUser.Id, sysUser.Id, Enumeration.LoginResult.Success, rop.LoginWay, rop.Ip, "", "登录成功");

            SSOUtil.SetTokenInfo(ret.Token, tokenInfo, new TimeSpan(1, 0, 0));

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);

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

            string path = rup.Path;
            if (rup.Path == "/")
            {
                path = "/home";
            }

            switch (rup.WebSite)
            {
                case "admin":
                    ret.Menus = GetMenus(Enumeration.BelongSite.Admin, userId);
                    var hasMenu = ret.Menus.Where(m => m.Path == path).FirstOrDefault();
                    if (hasMenu == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2NoRight, "没有权限访问页面");
                    }
                    break;
                case "agent":
                    ret.Menus = GetMenus(Enumeration.BelongSite.Agent, userId);
                    break;
                case "account":
                    ret.Menus = GetMenus(Enumeration.BelongSite.Account, userId);
                    break;
                case "merch":
                    ret.Menus = GetMenus(Enumeration.BelongSite.Merch, userId);
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

        public CustomJsonResult CheckPermission(string operater, string userId, string token, RupOwnCheckPermission rop)
        {
            var result = new CustomJsonResult();

            SSOUtil.Postpone(token);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "检查成功");

            return result;
        }
    }
}
