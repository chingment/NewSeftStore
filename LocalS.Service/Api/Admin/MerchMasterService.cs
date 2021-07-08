using LocalS.BLL;
using LocalS.Entity;
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
    public class MerchMasterService : BaseService
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

        public CustomJsonResult GetList(string operater, RupMerchMasterGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SysMerchUser
                         where (rup.UserName == null || u.UserName.Contains(rup.UserName)) &&
                         (rup.FullName == null || u.FullName.Contains(rup.FullName)) &&
                         u.IsDelete == false &&
                         u.IsMaster == true
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


        public List<TreeNode> GetRoleTree()
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var sysRoles = CurrentDb.SysRole.Where(m => m.BelongSite == Enumeration.BelongSite.Merch).OrderBy(m => m.Priority).ToList();

            foreach (var sysRole in sysRoles)
            {
                treeNodes.Add(new TreeNode { Id = sysRole.Id, PId = "", Label = sysRole.Name });
            }

            return treeNodes;
        }

        public CustomJsonResult InitAdd(string operater)
        {
            var result = new CustomJsonResult();
            var ret = new RetMerchMasterInitAdd();

            ret.Roles = GetRoleTree();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, RopMerchMasterAdd rop)
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

            if(!CommonUtil.IsInt(rop.MerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商户号必须为8位数字");
            }

            if (rop.MerchId.Length != 8)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商户号必须为8位数字");
            }

            var isExistMerchId = CurrentDb.Merch.Where(m => m.Id == rop.MerchId).FirstOrDefault();
            if (isExistMerchId != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该商户号（{0}）已被使用", rop.MerchId));
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var user = new SysMerchUser();
                user.Id = IdWorker.Build(IdType.NewGuid);
                user.MerchId = rop.MerchId;
                user.PId = IdWorker.Build(IdType.EmptyGuid);
                user.UserName = rop.UserName;
                user.FullName = rop.FullName;
                user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                user.Email = rop.Email;
                user.PhoneNumber = rop.PhoneNumber;
                user.BelongType = Enumeration.BelongType.Merch;
                user.IsDelete = false;
                user.IsDisable = false;
                user.IsMaster = true;
                user.RegisterTime = DateTime.Now;
                user.SecurityStamp = Guid.NewGuid().ToString().Replace("-", "");
                user.Creator = operater;
                user.CreateTime = DateTime.Now;

                CurrentDb.SysMerchUser.Add(user);

                var merch = new LocalS.Entity.Merch();
                merch.Id = rop.MerchId;
                merch.MerchUserId = user.Id;
                merch.Name = rop.FullName;
                merch.CreateTime = DateTime.Now;
                merch.Creator = operater;
                CurrentDb.Merch.Add(merch);

                var sysRole = CurrentDb.SysRole.Where(m => m.BelongSite == Enumeration.BelongSite.Merch && m.IsSuper == true).FirstOrDefault();
                if (sysRole == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置系统管理角色");
                }


                CurrentDb.SysUserRole.Add(new SysUserRole { Id = IdWorker.Build(IdType.NewGuid), RoleId = sysRole.Id, UserId = user.Id, Creator = operater, CreateTime = DateTime.Now });


                var sysOrg = new SysOrg();
                sysOrg.Id = IdWorker.Build(IdType.NewGuid);
                sysOrg.Name = "根组织";
                sysOrg.PId = IdWorker.Build(IdType.EmptyGuid);
                sysOrg.BelongSite = Enumeration.BelongSite.Merch;
                sysOrg.ReferenceId = merch.Id;
                sysOrg.IsDelete = false;
                sysOrg.Priority = 0;
                sysOrg.Depth = 0;
                sysOrg.CreateTime = DateTime.Now;
                sysOrg.Creator = operater;
                CurrentDb.SysOrg.Add(sysOrg);

                var sysUserOrg = new SysUserOrg();
                sysUserOrg.Id = IdWorker.Build(IdType.NewGuid);
                sysUserOrg.OrgId = sysOrg.Id;
                sysUserOrg.UserId = user.Id;
                sysUserOrg.CreateTime = DateTime.Now;
                sysUserOrg.Creator = operater;
                CurrentDb.SysUserOrg.Add(sysUserOrg);

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            return result;
        }

        public CustomJsonResult InitEdit(string operater, string userId)
        {
            var result = new CustomJsonResult();

            var ret = new RetMerchMasterInitEdit();

            var user = CurrentDb.SysMerchUser.Where(m => m.Id == userId).FirstOrDefault();

            ret.Id = user.Id;
            ret.UserName = user.UserName;
            ret.PhoneNumber = user.PhoneNumber;
            ret.Email = user.Email;
            ret.FullName = user.FullName;
            ret.IsDisable = user.IsDisable;

            ret.Roles = GetRoleTree();
            ret.RoleIds = (from m in CurrentDb.SysUserRole where m.UserId == user.Id select m.RoleId).ToList();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, RopMerchMasterEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var user = CurrentDb.SysMerchUser.Where(m => m.Id == rop.Id).FirstOrDefault();

                if (!string.IsNullOrEmpty(rop.Password))
                {
                    user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }

                user.FullName = rop.FullName;
                user.Email = rop.Email;
                user.PhoneNumber = rop.PhoneNumber;
                user.IsDisable = rop.IsDisable;
                user.MendTime = DateTime.Now;
                user.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }


            return result;


        }
    }
}
