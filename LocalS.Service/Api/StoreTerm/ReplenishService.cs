using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.StoreTerm
{
    public class ReplenishService : BaseService
    {
        public IResult GetPlans(string operater, RopReplenishGetPlans rop)
        {
            var result = new CustomJsonResult();

            var m_Device = BizFactory.Device.GetOne(rop.DeviceId);

            var query = (from u in CurrentDb.ErpReplenishPlanDevice
                         where
                         u.MerchId == m_Device.MerchId &&
                         u.StoreId == m_Device.StoreId &&
                         u.ShopId == m_Device.ShopId &&
                         u.DeviceId == m_Device.DeviceId
                         select new { u.Id, u.PlanId, u.PlanCumCode, u.RsherName, u.MakerName, u.MakeTime, u.RshTime, u.CreateTime });


            int total = query.Count();

            int pageIndex = rop.Page;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var m_Status = new StatusModel();
                if (item.RshTime == null)
                {
                    m_Status = new StatusModel(1, "未完成");
                }
                else
                {
                    m_Status = new StatusModel(1, "已完成");
                }

                olist.Add(new
                {
                    Id = item.Id,
                    PlanId = item.PlanId,
                    PlanCumCode = item.PlanCumCode,
                    RsherName = item.RsherName,
                    RshTime = item.RshTime,
                    Status = m_Status,
                    MakerName = item.MakerName,
                    MakeTime = item.MakeTime.ToUnifiedFormatDateTime(),
                    CreateTime = item.CreateTime
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;

        }

        public IResult GetPlanDetail(string operater, RopReplenishGetPlanDetail rop)
        {

            var m_Device = BizFactory.Device.GetOne(rop.DeviceId);

            var d_DeviceCabinet = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == rop.DeviceId && m.CabinetId == rop.CabinetId && m.IsUse == true).FirstOrDefault();

            var d_DeviceStocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Device && m.MerchId == m_Device.MerchId && m.StoreId == m_Device.StoreId && m.ShopId == m_Device.ShopId && m.CabinetId == rop.CabinetId && m.DeviceId == rop.DeviceId).ToList();

            var d_DeviceReplenishs = (from u in CurrentDb.ErpReplenishPlanDeviceDetail
                                      where
                                      u.PlanDeviceId == rop.PlanDeviceId
                                      select new { u.Id, u.PlanId, u.PlanCumCode, u.RsherName, u.CabinetId, u.CabinetName, u.SlotId, u.SlotName, u.SkuId, u.SkuCumCode, u.SkuSpecDes, u.PlanQuantity, u.RshQuantity, u.RshTime, u.CreateTime }).ToList();

            Dictionary<string, object> slots = new Dictionary<string, object>();

            foreach (var d_DeviceStock in d_DeviceStocks)
            {
                var r_Sku = CacheServiceFactory.Product.GetSkuInfo(d_DeviceStock.MerchId, d_DeviceStock.SkuId);

                if (r_Sku != null)
                {
                    var m_Slot = new
                    {
                        SlotId = d_DeviceStock.SlotId,
                        StockId = d_DeviceStock.Id,
                        CabinetId = d_DeviceStock.CabinetId,
                        SkuId = r_Sku.Id,
                        SkuCumCode = r_Sku.CumCode,
                        SkuName = r_Sku.Name,
                        SkuMainImgUrl = ImgSet.Convert_S(r_Sku.MainImgUrl),
                        SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes),
                        SumQuantity = d_DeviceStock.SumQuantity,
                        LockQuantity = d_DeviceStock.WaitPayLockQuantity + d_DeviceStock.WaitPickupLockQuantity,
                        SellQuantity = d_DeviceStock.SellQuantity,
                        Version = d_DeviceStock.Version
                    };

                    slots.Add(d_DeviceStock.SlotId, m_Slot);
                }
            }

            var ret = new { RowColLayout = d_DeviceCabinet.RowColLayout, slots = slots };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public IResult ConfirmReplenish(string operater, RopReplenishConfirmReplenish rop)
        {
            return null;
        }
    }
}
