﻿using LocalS.BLL;
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
            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var m_Status = new FieldModel();
                if (item.RshTime == null)
                {
                    m_Status = new FieldModel(1, "未完成");
                }
                else
                {
                    m_Status = new FieldModel(2, "已完成");
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

            var d_Stocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Device && m.MerchId == m_Device.MerchId && m.StoreId == m_Device.StoreId && m.ShopId == m_Device.ShopId && m.DeviceId == rop.DeviceId).ToList();
            var d_PlanDevice = CurrentDb.ErpReplenishPlanDevice.Where(m => m.Id == rop.PlanDeviceId).FirstOrDefault();
            var d_PlanDeviceDetails = (from u in CurrentDb.ErpReplenishPlanDeviceDetail where u.PlanDeviceId == rop.PlanDeviceId select new { u.Id, u.PlanId, u.PlanCumCode, u.RsherName, u.CabinetId, u.CabinetName, u.SlotId, u.SlotName, u.SkuId, u.SkuCumCode, u.SkuSpecDes, u.PlanRshQuantity, u.RealRshQuantity, u.RshTime }).ToList();

            Dictionary<string, object> cabinets = new Dictionary<string, object>();

            var l_Cabinets = d_PlanDevice.Cabinets.ToJsonObject<List<CabinetModel>>();

            foreach (var l_Cabinet in l_Cabinets)
            {
                Dictionary<string, object> slots = new Dictionary<string, object>();

                var l_Stocks = d_Stocks.Where(m => m.CabinetId == l_Cabinet.CabinetId).ToList();

                foreach (var l_Stock in l_Stocks)
                {
                    var r_Sku = CacheServiceFactory.Product.GetSkuInfo(l_Stock.MerchId, l_Stock.SkuId);
                    var l_Rsh = d_PlanDeviceDetails.Where(m => m.CabinetId == l_Stock.CabinetId && m.SlotId == l_Stock.SlotId).FirstOrDefault();

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
                            SlotId = l_Stock.SlotId,
                            StockId = l_Stock.Id,
                            CabinetId = l_Stock.CabinetId,
                            SkuId = r_Sku.Id,
                            SkuCumCode = r_Sku.CumCode,
                            SkuName = r_Sku.Name,
                            SkuMainImgUrl = ImgSet.Convert_S(r_Sku.MainImgUrl),
                            SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes),
                            SumQuantity = l_Stock.SumQuantity + planRshQuantity,
                            LockQuantity = l_Stock.WaitPayLockQuantity + l_Stock.WaitPickupLockQuantity,
                            SellQuantity = l_Stock.SellQuantity + planRshQuantity,
                            PlanRshQuantity = planRshQuantity,
                            RealRshQuantity = realRshQuantity,
                            IsPlanRsh = isPlanRsh,
                            Version = l_Stock.Version
                        };

                        slots.Add(l_Stock.SlotId, m_Slot);
                    }
                }

                cabinets.Add(l_Cabinet.CabinetId, new { CabinetId = l_Cabinet.CabinetId, Name = l_Cabinet.Name, Priority = l_Cabinet.Priority, RowColLayout = l_Cabinet.RowColLayout, RshSlots = slots });
            }

            var ret = new { Cabinets = cabinets };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public IResult ConfirmReplenish(string operater, RopReplenishConfirmReplenish rop)
        {
            var result = new CustomJsonResult();

            List<StockChangeRecordModel> s_StockChangeRecords = new List<StockChangeRecordModel>();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_SysUser = CurrentDb.SysMerchUser.Where(m => m.Id == operater).FirstOrDefault();

                var d_PlanDevice = CurrentDb.ErpReplenishPlanDevice.Where(m => m.Id == rop.PlanDeviceId).FirstOrDefault();

                if (d_PlanDevice.RshTime != null)
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "补货计划单已处理");

                d_PlanDevice.RsherId = operater;
                d_PlanDevice.RsherName = d_SysUser.FullName;
                d_PlanDevice.RshTime = DateTime.Now;
                d_PlanDevice.Mender = operater;
                d_PlanDevice.MendTime = DateTime.Now;

                var d_PlanDeviceDetails = CurrentDb.ErpReplenishPlanDeviceDetail.Where(m => m.PlanDeviceId == rop.PlanDeviceId).ToList();

                foreach (var d_PlanDeviceDetail in d_PlanDeviceDetails)
                {
                    var slot = rop.RshSlots.Where(m => m.CabinetId == d_PlanDeviceDetail.CabinetId && m.SlotId == d_PlanDeviceDetail.SlotId).FirstOrDefault();
                    if (slot != null)
                    {
                        d_PlanDeviceDetail.RsherId = d_PlanDevice.RsherId;
                        d_PlanDeviceDetail.RsherName = d_PlanDevice.RsherName;
                        d_PlanDeviceDetail.RshTime = d_PlanDevice.RshTime;
                        d_PlanDeviceDetail.RealRshQuantity = slot.RealRshQuantity;
                        d_PlanDeviceDetail.Mender = d_PlanDevice.Mender;
                        d_PlanDeviceDetail.MendTime = d_PlanDevice.MendTime;

                        if (slot.RealRshQuantity > 0)
                        {
                            var ret_OperateStock = BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.device_slot_rsh, E_ShopMode.Device, d_PlanDeviceDetail.MerchId, d_PlanDeviceDetail.StoreId, d_PlanDeviceDetail.ShopId, d_PlanDeviceDetail.DeviceId, d_PlanDeviceDetail.CabinetId, d_PlanDeviceDetail.SlotId, d_PlanDeviceDetail.SkuId, slot.RealRshQuantity);
                            if (ret_OperateStock.Result != ResultType.Success)
                            {
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, ret_OperateStock.Message);
                            }
                            s_StockChangeRecords.AddRange(ret_OperateStock.Data.ChangeRecords);
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "处理成功");


                MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, rop.DeviceId, EventCode.device_slot_rsh, "补货处理成功", new
                {
                    Rop = rop,
                    StockChangeRecords = s_StockChangeRecords
                });
            }
            return result;
        }
    }
}
