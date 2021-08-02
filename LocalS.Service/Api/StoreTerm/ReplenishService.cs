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
            var d_DeviceCabinets = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == rop.DeviceId && m.IsUse == true).ToList();
            var d_DeviceStocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Device && m.MerchId == m_Device.MerchId && m.StoreId == m_Device.StoreId && m.ShopId == m_Device.ShopId && m.DeviceId == rop.DeviceId).ToList();
            var d_DeviceReplenishs = (from u in CurrentDb.ErpReplenishPlanDeviceDetail where u.PlanDeviceId == rop.PlanDeviceId select new { u.Id, u.PlanId, u.PlanCumCode, u.RsherName, u.CabinetId, u.CabinetName, u.SlotId, u.SlotName, u.SkuId, u.SkuCumCode, u.SkuSpecDes, u.PlanRshQuantity, u.RealRshQuantity, u.RshTime }).ToList();

            Dictionary<string, object> cabinets = new Dictionary<string, object>();

            foreach (var d_DeviceCabinet in d_DeviceCabinets)
            {
                Dictionary<string, object> slots = new Dictionary<string, object>();

                var l_DeviceStocks = d_DeviceStocks.Where(m => m.CabinetId == d_DeviceCabinet.CabinetId).ToList();

                foreach (var l_DeviceStock in l_DeviceStocks)
                {
                    var r_Sku = CacheServiceFactory.Product.GetSkuInfo(l_DeviceStock.MerchId, l_DeviceStock.SkuId);
                    var l_Rsh = d_DeviceReplenishs.Where(m => m.CabinetId == l_DeviceStock.CabinetId && m.SlotId == l_DeviceStock.SlotId).FirstOrDefault();

                    int planRshQuantity = 0;
                    int realRshQuantity = 0;
                    bool isPlanRsh = false;

                    if (l_Rsh != null)
                    {
                        planRshQuantity = l_Rsh.PlanRshQuantity;
                        if (l_Rsh.RshTime == null)
                        {
                            realRshQuantity = l_Rsh.PlanRshQuantity;
                        }
                        else
                        {
                            realRshQuantity = l_Rsh.RealRshQuantity;
                        }
                        isPlanRsh = true;
                    }

                    if (r_Sku != null)
                    {
                        var m_Slot = new
                        {
                            SlotId = l_DeviceStock.SlotId,
                            StockId = l_DeviceStock.Id,
                            CabinetId = l_DeviceStock.CabinetId,
                            SkuId = r_Sku.Id,
                            SkuCumCode = r_Sku.CumCode,
                            SkuName = r_Sku.Name,
                            SkuMainImgUrl = ImgSet.Convert_S(r_Sku.MainImgUrl),
                            SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes),
                            SumQuantity = l_DeviceStock.SumQuantity + planRshQuantity,
                            LockQuantity = l_DeviceStock.WaitPayLockQuantity + l_DeviceStock.WaitPickupLockQuantity,
                            SellQuantity = l_DeviceStock.SellQuantity + planRshQuantity,
                            PlanRshQuantity = planRshQuantity,
                            RealRshQuantity = realRshQuantity,
                            IsPlanRsh = isPlanRsh,
                            Version = l_DeviceStock.Version
                        };

                        slots.Add(l_DeviceStock.SlotId, m_Slot);
                    }
                }

                cabinets.Add(d_DeviceCabinet.CabinetId, new { RowColLayout = d_DeviceCabinet.RowColLayout, Slots = slots });
            }

            var ret = new { Cabinets = cabinets };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public IResult ConfirmReplenish(string operater, RopReplenishConfirmReplenish rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_SysUser = CurrentDb.SysMerchUser.Where(m => m.Id == operater).FirstOrDefault();

                var d_PlanDevice = CurrentDb.ErpReplenishPlanDevice.Where(m => m.Id == rop.PlanDeviceId).FirstOrDefault();

                d_PlanDevice.RsherId = operater;
                d_PlanDevice.RsherName = d_SysUser.FullName;
                d_PlanDevice.RshTime = DateTime.Now;
                d_PlanDevice.Mender = operater;
                d_PlanDevice.MendTime = DateTime.Now;

                var d_PlanDeviceDetails = CurrentDb.ErpReplenishPlanDeviceDetail.Where(m => m.PlanDeviceId == rop.PlanDeviceId).ToList();

                foreach (var d_PlanDeviceDetail in d_PlanDeviceDetails)
                {
                    var slot = rop.Slots.Where(m => m.CabinetId == d_PlanDeviceDetail.CabinetId && m.SlotId == d_PlanDeviceDetail.SlotId).FirstOrDefault();
                    if (slot != null)
                    {
                        d_PlanDeviceDetail.RsherId = d_PlanDevice.RsherId;
                        d_PlanDeviceDetail.RsherName = d_PlanDevice.RsherName;
                        d_PlanDeviceDetail.RshTime = d_PlanDevice.RshTime;
                        d_PlanDeviceDetail.RealRshQuantity = slot.RealRshQuantity;
                        d_PlanDeviceDetail.Mender = d_PlanDevice.Mender;
                        d_PlanDeviceDetail.MendTime = d_PlanDevice.MendTime;
                        CurrentDb.SaveChanges();

                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "异常处理成功");
            }
            return result;
        }
    }
}
