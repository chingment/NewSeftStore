using LocalS.BLL;
using LocalS.Service.UI;
using Lumos;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Admin
{
    public class SysMenuService : BaseDbContext
    {
        private List<TreeNode> GetOrgTree(string id, List<SysMenu> sysMenus)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_sysMenus = sysMenus.Where(t => t.PId == id).ToList();

            foreach (var p_sysMenu in p_sysMenus)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_sysMenu.Id;
                treeNode.PId = p_sysMenu.PId;
                treeNode.Label = p_sysMenu.Title;
                treeNode.Description = p_sysMenu.Description;
                if (p_sysMenu.Depth == 0)
                {
                    treeNode.ExtAttr = new { CanDelete = false };
                }
                else
                {
                    treeNode.ExtAttr = new { CanDelete = true };
                }

                treeNode.Children.AddRange(GetOrgTree(p_sysMenu.Id, sysMenus));
                treeNodes.Add(treeNode);
            }

            return treeNodes;
        }

        public CustomJsonResult GetList(string operater, Enumeration.BelongSite belongSite, RupSysMenuGetList rup)
        {
            var result = new CustomJsonResult();

            var sysMenus = CurrentDb.SysMenu.Where(m => m.BelongSite == belongSite).OrderBy(m => m.Priority).ToList();

            var topMenu = sysMenus.Where(m => m.Depth == 0).FirstOrDefault();

            var menuTree = GetOrgTree(topMenu.PId, sysMenus);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", menuTree);

            return result;

        }

        public CustomJsonResult InitAdd(string operater, Enumeration.BelongSite belongSite, string pMenuId)
        {
            var result = new CustomJsonResult();

            var ret = new RetSysMenuInitAdd();

            var sysMenu = CurrentDb.SysMenu.Where(m => m.Id == pMenuId).FirstOrDefault();

            if (sysMenu != null)
            {
                ret.PMenuId = sysMenu.Id;
                ret.PMenuName = sysMenu.Name;
                ret.PMenuTitle = sysMenu.Title;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, Enumeration.BelongSite belongSite, RopSysMenuAdd rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var isExists = CurrentDb.SysMenu.Where(m => m.Name == rop.Name).FirstOrDefault();
                if (isExists != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该名称已经存在");
                }

                var pSysMenu = CurrentDb.SysMenu.Where(m => m.Id == rop.PMenuId).FirstOrDefault();
                if (pSysMenu == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到上级节点");
                }

                var sysMenu = new SysMenu();
                sysMenu.Id = GuidUtil.New();
                sysMenu.Name = rop.Name;
                sysMenu.Title = rop.Title;
                sysMenu.Icon = rop.Icon;
                sysMenu.Path = rop.Path;
                sysMenu.Component = rop.Path;
                sysMenu.Description = rop.Description;
                sysMenu.PId = rop.PMenuId;
                sysMenu.BelongSite = belongSite;
                sysMenu.Depth = pSysMenu.Depth + 1;
                sysMenu.IsNavbar = rop.IsNavbar;
                sysMenu.IsRouter = rop.IsRouter;
                sysMenu.IsSidebar = rop.IsSidebar;
                sysMenu.CreateTime = DateTime.Now;
                sysMenu.Creator = operater;
                CurrentDb.SysMenu.Add(sysMenu);

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult InitEdit(string operater, Enumeration.BelongSite belongSite, string orgId)
        {
            var result = new CustomJsonResult();

            var ret = new RetSysMenuInitEdit();

            var sysMenu = CurrentDb.SysMenu.Where(m => m.Id == orgId).FirstOrDefault();

            if (sysMenu != null)
            {
                ret.MenuId = sysMenu.Id;
                ret.Name = sysMenu.Name;
                ret.Title = sysMenu.Title;
                ret.Icon = sysMenu.Icon;
                ret.Path = sysMenu.Path;
                ret.IsNavbar = sysMenu.IsNavbar;
                ret.IsRouter = sysMenu.IsRouter;
                ret.IsSidebar = sysMenu.IsSidebar;
                ret.Description = sysMenu.Description;

                var p_sysMenu = CurrentDb.SysMenu.Where(m => m.Id == sysMenu.PId).FirstOrDefault();

                if (p_sysMenu != null)
                {
                    ret.PMenuId = p_sysMenu.Id;
                    ret.PMenuName = p_sysMenu.Name;
                    ret.PMenuTitle = p_sysMenu.Title;
                }
                else
                {
                    ret.PMenuName = "/";
                    ret.PMenuTitle = "/";
                }
            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, Enumeration.BelongSite belongSite, RopSysMenuEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var sysMenu = CurrentDb.SysMenu.Where(m => m.Id == rop.MenuId).FirstOrDefault();
                if (sysMenu == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }
                sysMenu.Name = rop.Name;
                sysMenu.Title = rop.Title;
                sysMenu.Path = rop.Path;
                sysMenu.Component = rop.Path;
                sysMenu.Icon = rop.Icon;
                sysMenu.IsSidebar = rop.IsSidebar;
                sysMenu.IsRouter = rop.IsRouter;
                sysMenu.IsNavbar = rop.IsNavbar;
                sysMenu.Description = rop.Description;
                sysMenu.MendTime = DateTime.Now;
                sysMenu.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }
    }
}
