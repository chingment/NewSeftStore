using LocalS.BLL;
using LocalS.BLL.UI;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Admin
{
    public class SysMenuService : BaseService
    {
        private List<TreeNode> GetMenuTree(string id, List<SysMenu> sysMenus)
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
                treeNode.Depth = p_sysMenu.Depth;

                if (p_sysMenu.Depth == 0)
                {
                    treeNode.ExtAttr = new { CanDelete = false, CanEdit = false, CanAdd = true };
                }
                else
                {
                    if (p_sysMenu.Depth >= 3)
                    {
                        treeNode.ExtAttr = new { CanDelete = true, CanEdit = true, CanAdd = false };
                    }
                    else
                    {
                        treeNode.ExtAttr = new { CanDelete = true, CanEdit = true, CanAdd = true };
                    }
                }

                var children = GetMenuTree(p_sysMenu.Id, sysMenus);
                if (children != null)
                {
                    if (children.Count > 0)
                    {
                        treeNode.Children = new List<TreeNode>();
                        treeNode.Children.AddRange(children);
                    }
                }

                treeNodes.Add(treeNode);
            }

            return treeNodes;
        }

        public CustomJsonResult GetList(string operater, Enumeration.BelongSite belongSite, RupSysMenuGetList rup)
        {
            var result = new CustomJsonResult();

            var sysMenus = CurrentDb.SysMenu.Where(m => m.BelongSite == belongSite).OrderBy(m => m.Priority).ToList();

            var topMenu = sysMenus.Where(m => m.Depth == 0).FirstOrDefault();

            var menuTree = GetMenuTree(topMenu.PId, sysMenus);

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
                ret.PId = sysMenu.Id;
                ret.PName = sysMenu.Name;
                ret.PTitle = sysMenu.Title;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

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

                var pSysMenu = CurrentDb.SysMenu.Where(m => m.Id == rop.PId).FirstOrDefault();
                if (pSysMenu == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到上级节点");
                }

                var sysMenu = new SysMenu();
                sysMenu.Id = IdWorker.Build(IdType.NewGuid);
                sysMenu.Name = rop.Name;
                sysMenu.Title = rop.Title;
                sysMenu.Icon = rop.Icon;
                sysMenu.Path = rop.Path;
                sysMenu.Component = rop.Path;
                sysMenu.Description = rop.Description;
                sysMenu.PId = rop.PId;
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
                ret.Id = sysMenu.Id;
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
                    ret.PId = p_sysMenu.Id;
                    ret.PName = p_sysMenu.Name;
                    ret.PTitle = p_sysMenu.Title;
                }
                else
                {
                    ret.PName = "/";
                    ret.PTitle = "/";
                }
            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, Enumeration.BelongSite belongSite, RopSysMenuEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var sysMenu = CurrentDb.SysMenu.Where(m => m.Id == rop.Id).FirstOrDefault();
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

        public CustomJsonResult Sort(string operater, Enumeration.BelongSite belongSite, RopSysMenuSort rop)
        {

            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var sysMenus = CurrentDb.SysMenu.Where(m => rop.Ids.Contains(m.Id)).ToList();

                for (int i = 0; i < sysMenus.Count; i++)
                {
                    int priority = rop.Ids.IndexOf(sysMenus[i].Id);
                    LogUtil.Info("id:" + sysMenus[i].Id + "," + priority.ToString());
                    sysMenus[i].Priority = priority;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }
    }
}
