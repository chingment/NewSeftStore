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

namespace LocalS.Service.Api.Agent
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

        private IEnumerable<SysAgentUser> GetSons(string pId)
        {
            var query = from c in CurrentDb.SysAgentUser
                        where c.PId == pId
                        select c;

            return query.ToList().Concat(query.ToList().SelectMany(t => GetSons(t.Id)));
        }

        public List<string> GetSonIds(string pId)
        {
            var list = new List<string>();
            var son = GetSons(pId);
            if (son != null)
            {
                list = GetSons(pId).Select(m => m.Id).ToList();
            }
            return list;
        }


        public CustomJsonResult GetList(string operater, string agentId, RupUserGetList rup)
        {
            var result = new CustomJsonResult();

            var sonIds = GetSonIds(operater);

            var query = (from u in CurrentDb.SysAgentUser
                         where (rup.UserName == null || u.UserName.Contains(rup.UserName)) &&
                         (rup.FullName == null || u.FullName.Contains(rup.FullName)) &&
                         u.IsDelete == false &&
                         u.IsMaster == false &&
                         sonIds.Contains(u.Id)
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

        public CustomJsonResult InitAdd(string operater, string agentId)
        {
            var result = new CustomJsonResult();
            var ret = new RetUserInitAdd();

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
                var pAgentUser = CurrentDb.SysAgentUser.Where(m => m.Id == operater).FirstOrDefault();

                var agentUser = new SysAgentUser();
                agentUser.Id = GuidUtil.New();
                agentUser.PId = pAgentUser.Id;
                agentUser.UserName = rop.UserName;
                agentUser.FullName = rop.FullName;
                agentUser.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                agentUser.Email = rop.Email;
                agentUser.PhoneNumber = rop.PhoneNumber;
                agentUser.BelongSite = Enumeration.BelongSite.Agent;
                agentUser.IsDelete = false;
                agentUser.IsDisable = false;
                agentUser.IsMaster = false;
                agentUser.AgentId = pAgentUser.AgentId;
                agentUser.Depth = pAgentUser.Depth + 1;
                agentUser.Creator = operater;
                agentUser.CreateTime = DateTime.Now;
                agentUser.RegisterTime = DateTime.Now;
                agentUser.SecurityStamp = Guid.NewGuid().ToString().Replace("-", "");
                CurrentDb.SysAgentUser.Add(agentUser);
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


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string agentId, RopUserEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var agentUser = CurrentDb.SysAgentUser.Where(m => m.Id == rop.UserId).FirstOrDefault();

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

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }


            return result;


        }
    }
}
