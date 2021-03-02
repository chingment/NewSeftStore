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
                         select new { u.Id, u.OperateUserName, u.TrgerId, u.TrgerName, u.EventName, u.Remark, u.CreateTime, u.AppId });

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
                string appName = AppId.GetName(item.AppId);
                if (item.AppId == AppId.STORETERM)
                {
                    appName = string.Format("终端设备[{0}]", item.TrgerName);
                }
                else if (item.AppId == AppId.WXMINPRAGROM)
                {
                    appName = string.Format("小程序[{0}]", item.TrgerName);
                }

                olist.Add(new
                {
                    Id = item.Id,
                    OperateObjectName = "",
                    OperateUserName = item.OperateUserName,
                    EventName = item.EventName,
                    Remark = item.Remark,
                    AppName = appName,
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
                         select new { u.Id, u.SkuId, u.MerchId, u.StoreName, u.ShopMode, u.ShopName, u.SkuName, u.StoreId, u.ShopId, u.MachineId, u.CabinetId, u.SlotId, u.EventCode, u.EventName, u.ChangeQuantity, u.Remark, u.SellQuantity, u.WaitPayLockQuantity, u.WaitPickupLockQuantity, u.SumQuantity, u.CreateTime });

            if (!string.IsNullOrEmpty(rup.ProductSkuName))
            {
                query = query.Where(m => m.SkuName.Contains(rup.ProductSkuName));
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string sellChannelName = "";
                if (item.ShopMode == Entity.E_ShopMode.Mall)
                {
                    sellChannelName = string.Format("{0}/线上商城", item.StoreName);
                }
                else if (item.ShopMode == Entity.E_ShopMode.Shop)
                {
                    sellChannelName = string.Format("{0}/{1}", item.StoreName, item.ShopName);
                }
                else if (item.ShopMode == Entity.E_ShopMode.Machine)
                {
                    sellChannelName = string.Format("{0}/{1}/{2}", item.StoreName, item.ShopName, item.MachineId);
                }

                olist.Add(new
                {
                    Id = item.Id,
                    StoreId = item.StoreId,
                    SkuId = item.SkuId,
                    SkuName = item.SkuName,
                    SellChannelName = sellChannelName,
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
                         u.MerchId == merchId && u.SkuId == rup.SkuId &&
                         u.StoreId == rup.StoreId
                         select new { u.Id, u.SkuName, u.ShopId, u.StoreName, u.CabinetId, u.SlotId, u.EventCode, u.ShopMode, u.ShopName, u.MachineId, u.EventName, u.ChangeQuantity, u.Remark, u.SellQuantity, u.WaitPayLockQuantity, u.WaitPickupLockQuantity, u.SumQuantity, u.CreateTime });


            if (!string.IsNullOrEmpty(rup.ShopId))
            {
                query = query.Where(m => m.ShopId == rup.ShopId);
            }

            if (!string.IsNullOrEmpty(rup.MachineId))
            {
                query = query.Where(m => m.MachineId == rup.MachineId);
            }

            if (!string.IsNullOrEmpty(rup.CabinetId))
            {
                query = query.Where(m => m.CabinetId == rup.CabinetId);
            }

            if (!string.IsNullOrEmpty(rup.SlotId))
            {
                query = query.Where(m => m.SlotId == rup.SlotId);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string sellChannelName = "";
                if (item.ShopMode == Entity.E_ShopMode.Mall)
                {
                    sellChannelName = string.Format("{0}/线上商城", item.StoreName);
                }
                else if (item.ShopMode == Entity.E_ShopMode.Shop)
                {
                    sellChannelName = string.Format("{0}/{1}", item.StoreName, item.ShopName);
                }
                else if (item.ShopMode == Entity.E_ShopMode.Machine)
                {
                    sellChannelName = string.Format("{0}/{1}/{2}", item.StoreName, item.ShopName, item.MachineId);
                }

                olist.Add(new
                {
                    Id = item.Id,
                    SellChannelName = sellChannelName,
                    SkuName = item.SkuName,
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
