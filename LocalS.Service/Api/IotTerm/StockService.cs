using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{
    public class StockService : BaseService
    {
        public IResult2 RelienishPlans(string merchId, RopStockRelienishPlans rop)
        {
            var result = new CustomJsonResult2();

            if (!CommonUtil.IsDateTime(rop.make_date))
                return new CustomJsonResult2(ResultCode.Failure, "日期格式不符合");


            DateTime? startTime = CommonUtil.ConverToStartTime(rop.make_date);
            DateTime? endTime = CommonUtil.ConverToEndTime(rop.make_date);

            var query = (from u in CurrentDb.ErpReplenishPlan
                         where u.MerchId == merchId &&
                         u.MakeTime >= startTime &&
                         u.MakeTime <= endTime &&
                         u.Status == Entity.E_ErpReplenishPlan_Status.BuildSuccess
                         select new { u.CumCode, u.MakeTime, u.MakerName });

            int total = query.Count();

            int pageIndex = rop.page;

            int pageSize = rop.limit;

            query = query.OrderByDescending(r => r.MakeTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> items = new List<object>();

            foreach (var r in list)
            {
                items.Add(new
                {
                    plan_cum_code = r.CumCode,
                    maker_name = r.MakerName,
                    make_time = r.MakeTime.ToUnifiedFormatDateTime()
                });

            }


            var ret = new { Total = total, Items = items };

            result = new CustomJsonResult2(ResultCode.Success, "", ret);

            return result;
        }

        public IResult2 RelienishPlanDetails(string merchId, RopStockRelienishPlanDetails rop)
        {
            var result = new CustomJsonResult2();

            if (string.IsNullOrEmpty(rop.plan_cum_code))
                return new CustomJsonResult2(ResultCode.Failure, "计划单不能为空");

            var query = (from u in CurrentDb.ErpReplenishPlanDeviceDetail
                         where
                         u.MerchId == merchId
                         &&
                         u.PlanCumCode == rop.plan_cum_code
                         select new { u.DeviceId, u.PlanCumCode, u.MakeTime, u.DeviceCumCode, u.CabinetId, u.SlotId, u.SkuId, u.SkuCumCode, u.PlanRshQuantity, u.RealRshQuantity, u.RshTime });

            int total = query.Count();

            int pageIndex = rop.page;

            int pageSize = rop.limit;

            query = query.OrderByDescending(r => r.DeviceId).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> items = new List<object>();

            foreach (var r in list)
            {
                items.Add(new
                {
                    plan_cum_code = r.PlanCumCode,
                    make_time = r.MakeTime,
                    device_id = r.DeviceId,
                    device_cum_code = r.DeviceCumCode,
                    cabinet_id = r.CabinetId,
                    slot_id = r.SlotId,
                    sku_id = r.SkuId,
                    sku_cum_code = r.SkuCumCode,
                    plan_rsh_quantity = r.PlanRshQuantity,
                    real_rsh_quantity = r.RealRshQuantity,
                    rsh_time = r.RshTime.ToUnifiedFormatDateTime()
                });

            }

            var ret = new { Total = total, Items = items };

            result = new CustomJsonResult2(ResultCode.Success, "", ret);

            return result;
        }
    }
}
