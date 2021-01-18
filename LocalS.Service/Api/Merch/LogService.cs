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
    public class LogService : BaseService
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
                         &&
                         u.EventLevel == "A"
                         select new { u.Id, u.OperateUserName, u.EventName, u.Remark, u.CreateTime, u.AppId });

            if (!string.IsNullOrEmpty(rup.EventName))
            {
                query = query.Where(m => m.EventName == rup.EventName);
            }

            if (!string.IsNullOrEmpty(rup.OperateUserName))
            {
                query = query.Where(m => m.OperateUserName.Contains(rup.OperateUserName));
            }

            if (!string.IsNullOrEmpty(rup.Remark))
            {
                query = query.Where(m => m.Remark.Contains(rup.Remark));
            }

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
                    AppName = AppId.GetName(item.AppId),
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
                         select new { u.Id, u.PrdProductSkuId, u.PrdProductSkuName, u.StoreId, u.StoreName, u.ShopId, u.MachineId, u.CabinetId, u.SlotId, u.EventCode, u.EventName, u.ChangeQuantity, u.Remark, u.SellQuantity, u.WaitPayLockQuantity, u.WaitPickupLockQuantity, u.SumQuantity, u.CreateTime });

            if (!string.IsNullOrEmpty(rup.ProductSkuName))
            {
                query = query.Where(m => m.PrdProductSkuName.Contains(rup.ProductSkuName));
            }

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
                    StoreId = item.StoreId,
                    ProductSkuId = item.PrdProductSkuId,
                    ProductSkuName = item.PrdProductSkuName,
                    ShopId = item.ShopId,
                    MachineId = item.MachineId,
                    CabinetId = item.CabinetId,
                    SlotId = item.SlotId,
                    EventCode = item.EventCode,
                    EventName = item.EventName,
                    ChangeQuantity = item.ChangeQuantity,
                    SellQuantity = item.SellQuantity,
                    WaitPayLockQuantity = item.WaitPayLockQuantity,
                    WaitPickupLockQuantity = item.WaitPickupLockQuantity,
                    SumQuantity = item.SumQuantity,
                    Remark = item.Remark,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetListByRelStock(string operater, string merchId, RupLogGetListByRelStock rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SellChannelStockLog
                         where
                         u.MerchId == merchId && u.PrdProductSkuId == rup.ProductSkuId &&
                         u.StoreId == rup.StoreId
                         select new { u.Id, u.PrdProductSkuName, u.StoreName, u.EventCode, u.EventName, u.ChangeQuantity, u.Remark, u.SellQuantity, u.WaitPayLockQuantity, u.WaitPickupLockQuantity, u.SumQuantity, u.CreateTime });


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
                    EventCode = item.EventCode,
                    EventName = item.EventName,
                    ChangeQuantity = item.ChangeQuantity,
                    SellQuantity = item.SellQuantity,
                    WaitPayLockQuantity = item.WaitPayLockQuantity,
                    WaitPickupLockQuantity = item.WaitPickupLockQuantity,
                    SumQuantity = item.SumQuantity,
                    Remark = item.Remark,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }
    }
}
