using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using Lumos.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class ErpReplenishPlanService : BaseService
    {
        public StatusModel GetStatus(E_ErpReplenishPlan_Status status)
        {
            var m_Status = new StatusModel();

            if (status == E_ErpReplenishPlan_Status.Submit)
            {
                m_Status = new StatusModel(1, "已提交");
            }
            else if (status == E_ErpReplenishPlan_Status.Building)
            {
                m_Status = new StatusModel(2, "生成中");
            }
            else if (status == E_ErpReplenishPlan_Status.BuildSuccess)
            {
                m_Status = new StatusModel(3, "生成成功");
            }
            else if (status == E_ErpReplenishPlan_Status.BuildFailure)
            {
                m_Status = new StatusModel(4, "生成失败");
            }

            return m_Status;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupErpReplenishPlanGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.ErpReplenishPlan
                         where
                          (rup.CumCode == null || u.CumCode.Contains(rup.CumCode)) &&
                         u.MerchId == merchId
                         select new { u.Id, u.CumCode, u.MakerName, u.MakeDate, u.Status, u.BuildTime, u.Remark, u.CreateTime });

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
                    CumCode = item.CumCode,
                    MakerName = item.MakerName,
                    MakeDate = item.MakeDate,
                    Status = GetStatus(item.Status),
                    BuildTime = item.BuildTime.ToUnifiedFormatDateTime(),
                    Remark = item.Remark,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult InitNew(string operater, string merchId)
        {
            var result = new CustomJsonResult();


            var maker = CurrentDb.SysUser.Where(m => m.Id == operater).FirstOrDefault();


            var ret = new { MakerId = maker.Id, MakerName = maker.FullName, MakeDate = DateTime.Now.ToUnifiedFormatDate() };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
        public CustomJsonResult New(string operater, string merchId, RopErpReplenishPlanNew rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (string.IsNullOrEmpty(rop.CumCode))
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "单号不能为空");

                if (string.IsNullOrEmpty(rop.MakeDate))
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "制单日期不能为空");

                var d_ErpReplenishPlan = CurrentDb.ErpReplenishPlan.Where(m => m.MerchId == merchId && m.CumCode == rop.CumCode).FirstOrDefault();
                if (d_ErpReplenishPlan != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该单号已经存在");
                }

                var maker = CurrentDb.SysUser.Where(m => m.Id == operater).FirstOrDefault();

                d_ErpReplenishPlan = new ErpReplenishPlan();
                d_ErpReplenishPlan.Id = IdWorker.Build(IdType.ErpReplenishPlanId);
                d_ErpReplenishPlan.MerchId = merchId;
                d_ErpReplenishPlan.CumCode = rop.CumCode;
                d_ErpReplenishPlan.MakerId = maker.Id;
                d_ErpReplenishPlan.MakerName = maker.FullName;
                d_ErpReplenishPlan.MakeTime = DateTime.Now;
                d_ErpReplenishPlan.MakeDate = rop.MakeDate;
                d_ErpReplenishPlan.Status = E_ErpReplenishPlan_Status.Submit;
                d_ErpReplenishPlan.Remark = rop.Remark;
                d_ErpReplenishPlan.Creator = operater;
                d_ErpReplenishPlan.CreateTime = DateTime.Now;
                CurrentDb.ErpReplenishPlan.Add(d_ErpReplenishPlan);
                CurrentDb.SaveChanges();

                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

                Task.Factory.StartNew(() =>
                {
                    BizFactory.Erp.HandleReplenishPlanBuild(new ReplenishPlanBuildModel { ReplenishPlanId = d_ErpReplenishPlan.Id });
                });


            }

            return result;
        }

    }
}
