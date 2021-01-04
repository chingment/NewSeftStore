using LocalS.BLL;
using LocalS.Service.UI;
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
    public class SysRoleService : BaseService
    {
        public CustomJsonResult GetList(string operater, Enumeration.BelongSite belongSite, RupSysRoleGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SysRole
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.BelongSite == belongSite
                         select new { u.Id, u.Name, u.Description, u.CreateTime, u.Priority });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderBy(r => r.Priority).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

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


                var children = GetMenuTree(treeNode.Id, sysMenus);
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

        private List<TreeNode> GetMenuTree(Enumeration.BelongSite belongSite)
        {
            var sysMenus = CurrentDb.SysMenu.Where(m => m.BelongSite == belongSite).OrderBy(m => m.Priority).ToList();

            var topMenu = sysMenus.Where(m => m.Depth == 0).FirstOrDefault();

            return GetMenuTree(topMenu.Id, sysMenus);
        }

        public CustomJsonResult InitAdd(string operater, Enumeration.BelongSite belongSite)
        {
            var result = new CustomJsonResult();
            var ret = new RetSysRoleInitAdd();
            ret.Menus = GetMenuTree(belongSite);
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;
        }

        public CustomJsonResult Add(string operater, Enumeration.BelongSite belongSite, RopSysRoleAdd rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var isExists = CurrentDb.SysRole.Where(m => m.Name == rop.Name && m.BelongSite == belongSite).FirstOrDefault();
                if (isExists != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该名称已经存在");
                }

                var sysRole = new SysRole();
                sysRole.Id = IdWorker.Build(IdType.NewGuid);
                sysRole.Name = rop.Name;
                sysRole.Description = rop.Description;
                sysRole.PId = IdWorker.Build(IdType.EmptyGuid);
                sysRole.BelongSite = belongSite;
                sysRole.Dept = 0;
                sysRole.CreateTime = DateTime.Now;
                sysRole.Creator = operater;
                CurrentDb.SysRole.Add(sysRole);

                if (rop.MenuIds != null)
                {
                    foreach (var menuId in rop.MenuIds)
                    {
                        CurrentDb.SysRoleMenu.Add(new SysRoleMenu { Id = IdWorker.Build(IdType.NewGuid), RoleId = sysRole.Id, MenuId = menuId, Creator = operater, CreateTime = DateTime.Now });
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult InitEdit(string operater, Enumeration.BelongSite belongSite, string roleId)
        {
            var result = new CustomJsonResult();

            var ret = new RetSysRoleInitEdit();
            var role = CurrentDb.SysRole.Where(m => m.Id == roleId).FirstOrDefault();

            ret.Id = role.Id;
            ret.Name = role.Name;
            ret.Description = role.Description;
            ret.Menus = GetMenuTree(belongSite);

            var roleMenus = from c in CurrentDb.SysMenu
                            where
                                (from o in CurrentDb.SysRoleMenu where o.RoleId == roleId select o.MenuId).Contains(c.Id)
                            select c;


            ret.MenuIds = (from p in roleMenus select p.Id).ToList();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, Enumeration.BelongSite belongSite, RopSysRoleEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var sysRole = CurrentDb.SysRole.Where(m => m.Id == rop.Id).FirstOrDefault();
                if (sysRole == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }

                sysRole.Description = rop.Description;
                sysRole.MendTime = DateTime.Now;
                sysRole.Mender = operater;

                var roleMenus = CurrentDb.SysRoleMenu.Where(r => r.RoleId == rop.Id).ToList();

                foreach (var roleMenu in roleMenus)
                {
                    CurrentDb.SysRoleMenu.Remove(roleMenu);
                }


                if (rop.MenuIds != null)
                {
                    foreach (var menuId in rop.MenuIds)
                    {
                        CurrentDb.SysRoleMenu.Add(new SysRoleMenu { Id = IdWorker.Build(IdType.NewGuid), RoleId = rop.Id, MenuId = menuId, Creator = operater, CreateTime = DateTime.Now });
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }
    }
}
