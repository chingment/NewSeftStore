using LocalS.BLL.Mq;
using LocalS.BLL.Push;
using LocalS.BLL.Task;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public class ErpService : BaseService
    {
        public string GetSlotName(string cabinetId, string slotId)
        {
            if (cabinetId.IndexOf("ds") > -1)
                return slotId.Split('-')[2];
            else if (cabinetId.IndexOf("zs") > -1)
                return slotId.Split('-')[2];

            return "";
        }

        public CustomJsonResult HandleReplenishPlanBuild(ReplenishPlanBuildModel rop)
        {
            LogUtil.Info("已进入 HandleReplenishPlanBuild");

            var result = new CustomJsonResult();

            string planId = rop.ReplenishPlanId;

            var d_Plan = CurrentDb.ErpReplenishPlan.Where(m => m.Id == planId).FirstOrDefault();

            if (d_Plan == null)
                new CustomJsonResult(ResultType.Success, ResultCode.Success, "已生成");

            d_Plan.Status = E_ErpReplenishPlan_Status.Building;
            d_Plan.MendTime = DateTime.Now;
            d_Plan.Mender = IdWorker.Build(IdType.EmptyGuid);
            CurrentDb.SaveChanges();

            var merchId = d_Plan.MerchId;

            var d_Devices = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.IsStopUse == false && m.CurUseStoreId != null && m.CurUseShopId != null).ToList();

            var deviceIds = d_Devices.Select(m => m.DeviceId).ToArray();

            var d_Stocks = (from m in CurrentDb.SellChannelStock
                            where m.MerchId == merchId && m.ShopMode == E_ShopMode.Device && deviceIds.Contains(m.DeviceId)
                            select new { m.StoreId, m.MerchId, m.SkuId, m.DeviceId, m.CabinetId, m.WarnQuantity, m.ShopId, m.ShopMode, m.SlotId, m.SellQuantity, m.WaitPayLockQuantity, m.WaitPickupLockQuantity, m.SumQuantity, m.MaxQuantity, m.IsOffSell }).OrderBy(m => m.DeviceId).ToList();
            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();
            var d_Shops = CurrentDb.Shop.Where(m => m.MerchId == merchId).ToList();

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var d_Plan1 = CurrentDb.ErpReplenishPlan.Where(m => m.Id == planId).FirstOrDefault();

                    if (d_Plan1.BuildTime != null)
                    {
                        ts.Complete();
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已生成");
                    }

                    bool is_Has_Plan = false;

                    var d_Device_Plans = (from m in d_Stocks select new { m.MerchId, m.StoreId, m.ShopId, m.DeviceId }).Distinct().ToList();

                    foreach (var d_Device_Plan in d_Device_Plans)
                    {
                        var l_Store = d_Stores.Where(m => m.Id == d_Device_Plan.StoreId).FirstOrDefault();
                        var l_Shop = d_Shops.Where(m => m.Id == d_Device_Plan.ShopId).FirstOrDefault();
                        var l_Device = d_Devices.Where(m => m.DeviceId == d_Device_Plan.DeviceId).FirstOrDefault();
                        var l_CabinetIds = d_Stocks.Where(m => m.DeviceId == d_Device_Plan.DeviceId).Select(m => m.CabinetId).ToArray();

                        var d_Cabinets = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == d_Device_Plan.DeviceId && l_CabinetIds.Contains(m.CabinetId)).ToList();
                        List<CabinetModel> l_Cabinets = new List<CabinetModel>();
                        foreach (var d_Cabinet in d_Cabinets)
                        {
                            l_Cabinets.Add(new CabinetModel
                            {
                                CabinetId = d_Cabinet.CabinetId,
                                RowColLayout = d_Cabinet.RowColLayout,
                                Priority = d_Cabinet.Priority,
                                Name = d_Cabinet.CabinetName
                            });
                        }

                        var d_PlanDevice = new ErpReplenishPlanDevice();
                        d_PlanDevice.Id = IdWorker.Build(IdType.NewGuid);
                        d_PlanDevice.PlanId = d_Plan.Id;
                        d_PlanDevice.PlanCumCode = d_Plan.CumCode;
                        d_PlanDevice.MerchId = d_Device_Plan.MerchId;
                        d_PlanDevice.StoreId = l_Store.Id;
                        d_PlanDevice.StoreName = l_Store.Name;
                        d_PlanDevice.ShopId = l_Shop.Id;
                        d_PlanDevice.ShopName = l_Shop.Name;
                        d_PlanDevice.DeviceId = l_Device.DeviceId;
                        d_PlanDevice.DeviceCumCode = l_Device.CumCode;
                        d_PlanDevice.Cabinets = l_Cabinets.ToJsonString();
                        d_PlanDevice.MakeDate = d_Plan.MakeDate;
                        d_PlanDevice.MakerId = d_Plan.MakerId;
                        d_PlanDevice.MakerName = d_Plan.MakerName;
                        d_PlanDevice.MakeTime = d_Plan.MakeTime;
                        d_PlanDevice.Creator = d_Plan.Creator;
                        d_PlanDevice.CreateTime = DateTime.Now;

                        var l_Stocks = (from m in d_Stocks
                                        where
                            m.MerchId == d_Device_Plan.MerchId &&
                            m.StoreId == d_Device_Plan.StoreId &&
                            m.ShopId == d_Device_Plan.ShopId &&
                            m.DeviceId == d_Device_Plan.DeviceId
                                        select new { m.StoreId, m.MerchId, m.SkuId, m.DeviceId, m.CabinetId, m.WarnQuantity, m.ShopId, m.ShopMode, m.SlotId, m.SellQuantity, m.WaitPayLockQuantity, m.WaitPickupLockQuantity, m.SumQuantity, m.MaxQuantity, m.IsOffSell });
                        bool is_Has_Device = false;
                        foreach (var l_Stock in l_Stocks)
                        {
                            var r_Sku = CacheServiceFactory.Product.GetSkuInfo(l_Stock.MerchId, l_Stock.SkuId);
                            int warnQuantity = l_Stock.WarnQuantity;
                            int sumQuantity = l_Stock.SumQuantity;
                            int maxQuantity = l_Stock.MaxQuantity;
                            int rshQuantity = maxQuantity - sumQuantity;
                            if ((sumQuantity < warnQuantity) && rshQuantity > 0)
                            {
                                is_Has_Device = true;
                                is_Has_Plan = true;

                                var d_PlanDeviceDetail = new ErpReplenishPlanDeviceDetail();
                                d_PlanDeviceDetail.Id = IdWorker.Build(IdType.NewGuid);
                                d_PlanDeviceDetail.PlanId = d_Plan.Id;
                                d_PlanDeviceDetail.PlanDeviceId = d_PlanDevice.Id;
                                d_PlanDeviceDetail.PlanCumCode = d_Plan.CumCode;
                                d_PlanDeviceDetail.MerchId = l_Stock.MerchId;
                                d_PlanDeviceDetail.StoreId = l_Store.Id;
                                d_PlanDeviceDetail.StoreName = l_Store.Name;
                                d_PlanDeviceDetail.ShopId = l_Shop.Id;
                                d_PlanDeviceDetail.ShopName = l_Shop.Name;
                                d_PlanDeviceDetail.DeviceId = l_Device.DeviceId;
                                d_PlanDeviceDetail.DeviceCumCode = l_Device.CumCode;
                                d_PlanDeviceDetail.CabinetId = l_Stock.CabinetId;
                                d_PlanDeviceDetail.CabinetName = l_Stock.CabinetId;
                                d_PlanDeviceDetail.SlotId = l_Stock.SlotId;
                                d_PlanDeviceDetail.SlotName = GetSlotName(l_Stock.CabinetId, l_Stock.SlotId);
                                d_PlanDeviceDetail.SpuId = r_Sku.SpuId;
                                d_PlanDeviceDetail.SkuId = r_Sku.Id;
                                d_PlanDeviceDetail.SkuName = r_Sku.Name;
                                d_PlanDeviceDetail.SkuCumCode = r_Sku.CumCode;
                                d_PlanDeviceDetail.SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes);
                                d_PlanDeviceDetail.PlanRshQuantity = rshQuantity;
                                d_PlanDeviceDetail.RealRshQuantity = 0;
                                d_PlanDeviceDetail.BuildTime = DateTime.Now;
                                d_PlanDeviceDetail.MakerId = d_Plan.MakerId;
                                d_PlanDeviceDetail.MakerName = d_Plan.MakerName;
                                d_PlanDeviceDetail.MakeTime = d_Plan.MakeTime;
                                d_PlanDeviceDetail.Creator = d_Plan.Creator;
                                d_PlanDeviceDetail.CreateTime = DateTime.Now;
                                CurrentDb.ErpReplenishPlanDeviceDetail.Add(d_PlanDeviceDetail);
                                CurrentDb.SaveChanges();
                            }
                        }

                        if (is_Has_Device)
                        {
                            CurrentDb.ErpReplenishPlanDevice.Add(d_PlanDevice);
                            CurrentDb.SaveChanges();
                        }
                    }

                    d_Plan.BuildTime = DateTime.Now;

                    if (is_Has_Plan)
                    {
                        d_Plan.Status = E_ErpReplenishPlan_Status.BuildSuccess;
                    }
                    else
                    {
                        d_Plan.FailReason = "没有需补货的商品";
                        d_Plan.Status = E_ErpReplenishPlan_Status.BuildFailure;
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();

                    if (is_Has_Plan)
                    {
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "生成成功");
                    }
                    else
                    {
                        result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "生成失败，没有需补货的商品");
                    }

                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("", ex);

                var dd_Plan = CurrentDb.ErpReplenishPlan.Where(m => m.Id == planId).FirstOrDefault();
                dd_Plan.BuildTime = DateTime.Now;
                dd_Plan.Status = E_ErpReplenishPlan_Status.BuildFailure;
                dd_Plan.FailReason = "生成异常，处理过程失败";
                CurrentDb.SaveChanges();

                result = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "生成异常，处理过程失败");
            }
            return result;
        }
    }
}
