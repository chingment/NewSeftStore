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

namespace LocalS.Service.Api.Merch
{
    public class MerchService : BaseService
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

        public CustomJsonResult GetList(string operater, string merchId, RupMerchGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from s in CurrentDb.Merch
                         join m in CurrentDb.SysMerchUser on s.Id equals m.MerchId into temp
                         from u in temp.DefaultIfEmpty()
                         where
                        s.PId == merchId
                         select new { s.Id, u.UserName, u.FullName, u.Email, u.PhoneNumber, u.CreateTime, u.IsDelete, u.IsDisable });


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
        public CustomJsonResult InitAdd(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new { };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopMerchAdd rop)
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

            string l_MerchId = new Random().Next(10000001, 99999999).ToString();

            if (l_MerchId.Length != 8)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商户号必须为8位数字");
            }

            var isExistMerchId = CurrentDb.Merch.Where(m => m.Id == l_MerchId).FirstOrDefault();
            if (isExistMerchId != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该商户号（{0}）已被使用", l_MerchId));
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var user = new SysMerchUser();
                user.Id = IdWorker.Build(IdType.NewGuid);
                user.MerchId = l_MerchId;
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
                merch.Id = l_MerchId;
                merch.MerchUserId = user.Id;
                merch.PId = merchId;
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

        public CustomJsonResult InitEdit(string operater, string merchId, string id)
        {
            var result = new CustomJsonResult();

            var ret = new object();

            var d_User = CurrentDb.SysMerchUser.Where(m => m.MerchId == id && m.IsMaster == true).FirstOrDefault();

            string userName = d_User.UserName;
            string phoneNumber = d_User.PhoneNumber;
            string email = d_User.Email;
            string fullName = d_User.FullName;
            bool isDisable = d_User.IsDisable;

            ret = new
            {
                merchId = id,
                userName = userName,
                phoneNumber = phoneNumber,
                email = email,
                fullName = fullName,
                isDisable = isDisable,
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopMerchEdit rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_User = CurrentDb.SysMerchUser.Where(m => m.MerchId == rop.MerchId && m.IsMaster == true).FirstOrDefault();

                if (!string.IsNullOrEmpty(rop.Password))
                {
                    d_User.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }

                d_User.FullName = rop.FullName;
                d_User.Email = rop.Email;
                d_User.PhoneNumber = rop.PhoneNumber;
                d_User.IsDisable = rop.IsDisable;
                d_User.MendTime = DateTime.Now;
                d_User.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            return result;

        }
    }
}
