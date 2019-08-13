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
    public class AgentMasterService : BaseDbContext
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

        public CustomJsonResult GetList(string operater, RupAgentMasterGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SysAgentUser
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

        public CustomJsonResult InitAdd(string operater)
        {
            var result = new CustomJsonResult();
            var ret = new RetAgentMasterInitAdd();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, RopAgentMasterAdd rop)
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

                string agentId = GuidUtil.New();

                var user = new SysAgentUser();
                user.Id = GuidUtil.New();
                user.PId = GuidUtil.Empty();
                user.UserName = rop.UserName;
                user.FullName = rop.FullName;
                user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                user.Email = rop.Email;
                user.PhoneNumber = rop.PhoneNumber;
                user.BelongSite = Enumeration.BelongSite.Agent;
                user.IsDelete = false;
                user.IsDisable = false;
                user.IsMaster = true;
                user.Depth = 0;
                user.AgentId = agentId;
                user.Creator = operater;
                user.CreateTime = DateTime.Now;
                user.RegisterTime = DateTime.Now;
                user.SecurityStamp = Guid.NewGuid().ToString().Replace("-", "");
                CurrentDb.SysAgentUser.Add(user);

                var agent = new LocalS.Entity.Agent();
                agent.Id = agentId;
                agent.UserId = user.Id;
                agent.Name = rop.FullName;
                agent.CreateTime = DateTime.Now;
                agent.Creator = operater;
                CurrentDb.Agent.Add(agent);


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

            }

            return result;
        }

        public CustomJsonResult InitEdit(string operater, string userId)
        {
            var result = new CustomJsonResult();

            var ret = new RetAgentMasterInitEdit();

            var user = CurrentDb.SysAgentUser.Where(m => m.Id == userId).FirstOrDefault();

            ret.UserId = user.Id;
            ret.UserName = user.UserName;
            ret.PhoneNumber = user.PhoneNumber;
            ret.Email = user.Email;
            ret.FullName = user.FullName;
            ret.IsDisable = user.IsDisable;


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, RopAgentMasterEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var user = CurrentDb.SysAgentUser.Where(m => m.Id == rop.UserId).FirstOrDefault();

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
