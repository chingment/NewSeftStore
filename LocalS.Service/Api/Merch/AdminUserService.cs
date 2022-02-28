using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.BLL.UI;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using Lumos.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class AdminUserService : BaseService
    {
        public FieldModel GetStatus(bool isDisable)
        {
            var status = new FieldModel();

            if (isDisable)
            {
                status.Value = 2;
                status.Text = "禁用";
            }
            else
            {
                status.Value = 1;
                status.Text = "正常";
            }

            return status;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupAdminUserGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SysMerchUser
                         where (rup.UserName == null || u.UserName.Contains(rup.UserName)) &&
                         (rup.FullName == null || u.FullName.Contains(rup.FullName)) &&
                         u.IsDelete == false &&
                         u.MerchId == merchId &&
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
                    Status = GetStatus(item.IsDisable),
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

        public List<TreeNode> GetOrgTree(string merchId)
        {
            var sysOrgs = CurrentDb.SysOrg.Where(m => m.ReferenceId == merchId && m.BelongSite == Enumeration.BelongSite.Merch).OrderBy(m => m.Priority).ToList();
            return GetOrgTree(IdWorker.Build(IdType.EmptyGuid), sysOrgs);
        }

        public List<TreeNode> GetRoleTree()
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var sysRoles = CurrentDb.SysRole.Where(m => m.BelongSite == Enumeration.BelongSite.Merch && m.IsSuper == false).OrderBy(m => m.Priority).ToList();

            foreach (var sysRole in sysRoles)
            {
                treeNodes.Add(new TreeNode { Id = sysRole.Id, PId = "", Label = sysRole.Name });
            }

            return treeNodes;
        }

        public CustomJsonResult InitAdd(string operater, string merchId)
        {
            var result = new CustomJsonResult();
            var ret = new RetAdminUserInitAdd();

            // ret.Orgs = GetOrgTree();
            ret.Roles = GetRoleTree();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopAdminUserAdd rop)
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
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该用户名（{0}）已被使用或被其他商户使用", rop.UserName));
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var merchUser = new SysMerchUser();
                merchUser.Id = IdWorker.Build(IdType.NewGuid);
                merchUser.UserName = rop.UserName;
                merchUser.FullName = rop.FullName;
                merchUser.NickName = rop.NickName;
                merchUser.Avatar = rop.Avatar[0].Url;
                merchUser.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                merchUser.Email = rop.Email;
                merchUser.PhoneNumber = rop.PhoneNumber;
                merchUser.BelongType = Enumeration.BelongType.Agent;
                merchUser.IsDelete = false;
                merchUser.IsDisable = false;
                merchUser.IsMaster = false;
                merchUser.MerchId = merchId;
                merchUser.RegisterTime = DateTime.Now;
                merchUser.SecurityStamp = Guid.NewGuid().ToString().Replace("-", "");
                merchUser.ImIsUse = rop.ImIsUse;
                merchUser.WorkBench = rop.WorkBench;

                if (rop.ImIsUse)
                {
                    var imCountLimit = CurrentDb.Merch.Where(m => m.Id == merchId).First().ImAccountLimit;
                    var imCount = CurrentDb.SysMerchUser.Where(m => m.MerchId == merchId && m.IsDelete == false && m.ImIsUse == true).Count();
                    if (imCount >= imCountLimit)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "抱歉，您最大开通的音频服务账号数量为" + imCountLimit + "个,如需帮助，请联系客服");
                    }

                    merchUser.ImPartner = "Em";
                    merchUser.ImUserName = merchUser.UserName;
                    merchUser.ImPassword = "1a2b3c4d";
                    var rt_RegisterUser = SdkFactory.Easemob.RegisterUser(merchUser.ImUserName, merchUser.ImPassword, merchUser.NickName);
                    if (rt_RegisterUser.Result != ResultType.Success)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，音频服务存在问题");
                    }
                }


                merchUser.Creator = operater;
                merchUser.CreateTime = DateTime.Now;

                CurrentDb.SysMerchUser.Add(merchUser);


                if (rop.RoleIds != null)
                {
                    foreach (var roleId in rop.RoleIds)
                    {
                        if (!string.IsNullOrEmpty(roleId))
                        {
                            CurrentDb.SysUserRole.Add(new SysUserRole { Id = IdWorker.Build(IdType.NewGuid), RoleId = roleId, UserId = merchUser.Id, Creator = operater, CreateTime = DateTime.Now });
                        }
                    }
                }





                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.adminuser_add, string.Format("新建管理账号（{0}）成功", merchUser.UserName), rop);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            return result;
        }

        public CustomJsonResult InitEdit(string operater, string merchId, string userId)
        {
            var result = new CustomJsonResult();

            var ret = new RetAdminUserInitEdit();

            var merchUser = CurrentDb.SysMerchUser.Where(m => m.Id == userId).FirstOrDefault();

            ret.Id = merchUser.Id;
            ret.UserName = merchUser.UserName;
            ret.PhoneNumber = merchUser.PhoneNumber;
            var avatar = new List<ImgSet>();
            avatar.Add(new ImgSet { Url = merchUser.Avatar });
            ret.Avatar = avatar;
            ret.Email = merchUser.Email;
            ret.FullName = merchUser.FullName;
            ret.NickName = merchUser.NickName;
            ret.IsDisable = merchUser.IsDisable;
            ret.ImIsUse = merchUser.ImIsUse;
            ret.WorkBench = merchUser.WorkBench;
            ret.Roles = GetRoleTree();
            ret.RoleIds = (from m in CurrentDb.SysUserRole where m.UserId == merchUser.Id select m.RoleId).ToList();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopAdminUserEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var merchUser = CurrentDb.SysMerchUser.Where(m => m.MerchId == merchId && m.Id == rop.Id).FirstOrDefault();

                if (!string.IsNullOrEmpty(rop.Password))
                {
                    merchUser.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }

                merchUser.FullName = rop.FullName;
                merchUser.NickName = rop.NickName;
                merchUser.Avatar = rop.Avatar[0].Url;
                merchUser.Email = rop.Email;
                merchUser.PhoneNumber = rop.PhoneNumber;
                merchUser.IsDisable = rop.IsDisable;
                merchUser.MendTime = DateTime.Now;
                merchUser.Mender = operater;
                merchUser.ImIsUse = rop.ImIsUse;
                merchUser.WorkBench = rop.WorkBench;
                if (rop.ImIsUse)
                {
                    var imCountLimit = CurrentDb.Merch.Where(m => m.Id == merchId).First().ImAccountLimit;
                    var imCount = CurrentDb.SysMerchUser.Where(m => m.MerchId == merchId && m.Id != merchUser.Id && m.IsDelete == false && m.ImIsUse == true).Count();
                    if (imCount >= imCountLimit)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "抱歉，您最大开通的音频服务账号数量为" + imCountLimit + "个,如需帮助，请联系客服");
                    }

                    if (string.IsNullOrEmpty(merchUser.ImUserName))
                    {
                        merchUser.ImPartner = "Em";
                        merchUser.ImUserName = merchUser.UserName;
                        merchUser.ImPassword = "1a2b3c4d";
                        var rt_RegisterUser = SdkFactory.Easemob.RegisterUser(merchUser.ImUserName, merchUser.ImPassword, merchUser.NickName);
                        if (rt_RegisterUser.Result != ResultType.Success)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，音频服务存在问题");
                        }
                    }
                }



                var sysUserRoles = CurrentDb.SysUserRole.Where(r => r.UserId == rop.Id).ToList();

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
                            CurrentDb.SysUserRole.Add(new SysUserRole { Id = IdWorker.Build(IdType.NewGuid), RoleId = roleId, UserId = rop.Id, Creator = operater, CreateTime = DateTime.Now });
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.adminuser_edit, string.Format("保存管理账号（{0}）信息成功", merchUser.UserName), rop);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }


            return result;


        }


        public SysMerchUser GetInfo(string userId)
        {
            SysMerchUser sysMerchUser = CurrentDb.SysMerchUser.Where(m => m.Id == userId).FirstOrDefault();

            return sysMerchUser;
        }
    }
}
