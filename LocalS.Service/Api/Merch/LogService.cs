using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Service.UI;
using Lumos;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LocalS.Service.Api.Merch
{
    public class LogService : BaseDbContext
    {
        public CustomJsonResult InitListByOperate(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            return result;
        }

        public CustomJsonResult GetListByOperate(string operater, string merchId, RupLogGetListByOperate rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.MerchOperateLog
                         where
                         u.MerchId == merchId
                         select new { u.Id, u.StoreName, u.MachineName, u.OperateUserName, u.EventName, u.Remark, u.CreateTime });


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
                    OperateObjectName = "",
                    OperateUserName = item.OperateUserName,
                    EventName = item.EventName,
                    Remark = item.Remark,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult InitListByStock(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            return result;
        }

        public CustomJsonResult GetListByStock(string operater, string merchId, RupLogGetListByStock rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SellChannelStockLog
                         where
                         u.MerchId == merchId
                         select new { u.Id, u.PrdProductSkuName, u.StoreName, u.SellChannelRefName, u.ChangeType, u.ChangeTypeName, u.ChangeQuantity, u.RemarkByDev, u.SellQuantity, u.WaitPayLockQuantity, u.WaitPickupLockQuantity, u.SumQuantity, u.CreateTime });


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
                    ProductSkuName = item.PrdProductSkuName,
                    SellChannelRefName = item.SellChannelRefName,
                    ChangeTypeName = item.ChangeTypeName,
                    ChangeQuantity = item.ChangeQuantity,
                    SellQuantity = item.SellQuantity,
                    WaitPayLockQuantity = item.WaitPayLockQuantity,
                    WaitPickupLockQuantity = item.WaitPickupLockQuantity,
                    SumQuantity = item.SumQuantity,
                    Remark = item.RemarkByDev,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }
    }
}
