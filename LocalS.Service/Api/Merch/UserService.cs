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
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class UserService : BaseDbContext
    {
        public string GetStatusText(bool isDisable)
        {
            string text = "";
            if (isDisable)
            {
                text = "禁用";
            }
            else
            {
                text = "正常";
            }

            return text;
        }

        public int GetStatusValue(bool isDisable)
        {
            int text = 0;
            if (isDisable)
            {
                text = 2;
            }
            else
            {
                text = 1;
            }

            return text;
        }

        public CustomJsonResult GetList(string operater, string agentId, RupUserGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SysAgentUser
                         where (rup.UserName == null || u.UserName.Contains(rup.UserName)) &&
                         (rup.FullName == null || u.FullName.Contains(rup.FullName)) &&
                         u.IsDelete == false &&
                         u.AgentId == agentId &&
                         u.IsMaster == false
                         select new { u.Id, u.UserName, u.FullName, u.Email, u.PhoneNumber, u.CreateTime, u.IsDelete, u.IsDisable });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    FullName = item.FullName,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    Status = new { text = GetStatusText(item.IsDisable), value = GetStatusValue(item.IsDisable) },
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        private List<TreeNode> GetOrgTree(string id, List<SysOrg> sysOrgs)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_sysOrgs = sysOrgs.Where(t => t.PId == id).ToList();

            foreach (var p_sysOrg in p_sysOrgs)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_sysOrg.Id;
                treeNode.PId = p_sysOrg.PId;
                treeNode.Value = p_sysOrg.Id;
                treeNode.Label = p_sysOrg.Name;
                treeNode.Children.AddRange(GetOrgTree(treeNode.Id, sysOrgs));
                treeNodes.Add(treeNode);
            }

            return treeNodes;
        }

        public List<TreeNode> GetOrgTree()
        {
            var sysOrgs = CurrentDb.SysOrg.OrderBy(m => m.Priority).ToList();
            return GetOrgTree(GuidUtil.Empty(), sysOrgs);
        }

        public List<TreeNode> GetRoleTree()
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var sysRoles = CurrentDb.SysRole.Where(m => m.BelongSite == Enumeration.BelongSite.Agent && m.IsSuper == false).OrderBy(m => m.Priority).ToList();

            foreach (var sysRole in sysRoles)
            {
                treeNodes.Add(new TreeNode { Id = sysRole.Id, PId = "", Label = sysRole.Name });
            }

            return treeNodes;
        }


        public CustomJsonResult InitAdd(string operater, string agentId)
        {
            var result = new CustomJsonResult();
            var ret = new RetUserInitAdd();

            // ret.Orgs = GetOrgTree();
            ret.Roles = GetRoleTree();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string agentId, RopUserAdd rop)
        {
            var result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.UserName))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "用户名不能为空");
            }

            if (string.IsNullOrEmpty(rop.Password))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "密码不能为空");
            }

            var isExistUserName = CurrentDb.SysUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();
            if (isExistUserName != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该用户名（{0}）已被使用", rop.UserName));
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var agentUser = new SysAgentUser();
                agentUser.Id = GuidUtil.New();
                agentUser.UserName = rop.UserName;
                agentUser.FullName = rop.FullName;
                agentUser.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                agentUser.Email = rop.Email;
                agentUser.PhoneNumber = rop.PhoneNumber;
                agentUser.BelongSite = Enumeration.BelongSite.Agent;
                agentUser.IsDelete = false;
                agentUser.IsDisable = false;
                agentUser.IsMaster = false;
                agentUser.AgentId = agentId;
                agentUser.Creator = operater;
                agentUser.CreateTime = DateTime.Now;
                agentUser.RegisterTime = DateTime.Now;
                agentUser.SecurityStamp = Guid.NewGuid().ToString().Replace("-", "");
                CurrentDb.SysAgentUser.Add(agentUser);

                if (rop.RoleIds != null)
                {
                    foreach (var roleId in rop.RoleIds)
                    {
                        if (!string.IsNullOrEmpty(roleId))
                        {
                            CurrentDb.SysUserRole.Add(new SysUserRole { Id = GuidUtil.New(), RoleId = roleId, UserId = agentUser.Id, Creator = operater, CreateTime = DateTime.Now });
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

            }

            return result;
        }

        public CustomJsonResult InitEdit(string operater, string agentId, string userId)
        {
            var result = new CustomJsonResult();

            var ret = new RetUserInitEdit();

            var agentUser = CurrentDb.SysAgentUser.Where(m => m.Id == userId).FirstOrDefault();

            ret.UserId = agentUser.Id;
            ret.UserName = agentUser.UserName;
            ret.PhoneNumber = agentUser.PhoneNumber;
            ret.Email = agentUser.Email;
            ret.FullName = agentUser.FullName;
            ret.IsDisable = agentUser.IsDisable;

            ret.Roles = GetRoleTree();
            ret.RoleIds = (from m in CurrentDb.SysUserRole where m.UserId == agentUser.Id select m.RoleId).ToList();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string agentId, RopUserEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var agentUser = CurrentDb.SysAgentUser.Where(m => m.AgentId == agentId && m.Id == rop.UserId).FirstOrDefault();

                if (!string.IsNullOrEmpty(rop.Password))
                {
                    agentUser.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }

                agentUser.FullName = rop.FullName;
                agentUser.Email = rop.Email;
                agentUser.PhoneNumber = rop.PhoneNumber;
                agentUser.IsDisable = rop.IsDisable;
                agentUser.MendTime = DateTime.Now;
                agentUser.Mender = operater;


                var sysUserRoles = CurrentDb.SysUserRole.Where(r => r.UserId == rop.UserId).ToList();

                foreach (var sysUserRole in sysUserRoles)
                {
                    CurrentDb.SysUserRole.Remove(sysUserRole);
                }

                if (rop.RoleIds != null)
                {
                    foreach (var roleId in rop.RoleIds)
                    {
                        if (!string.IsNullOrEmpty(roleId))
                        {
                            CurrentDb.SysUserRole.Add(new SysUserRole { Id = GuidUtil.New(), RoleId = roleId, UserId = rop.UserId, Creator = operater, CreateTime = DateTime.Now });
                        }
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
