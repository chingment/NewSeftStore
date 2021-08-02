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
        public CustomJsonResult HandleReplenishPlanBuild(ReplenishPlanBuildModel rop)
        {
            LogUtil.Info("已进入 HandleReplenishPlanBuild");

            var result = new CustomJsonResult();

            string replenishPlanId = rop.ReplenishPlanId;

            var d_ErpReplenishPlan = CurrentDb.ErpReplenishPlan.Where(m => m.Id == replenishPlanId).FirstOrDefault();
            d_ErpReplenishPlan.Status = E_ErpReplenishPlan_Status.Building;
            CurrentDb.SaveChanges();

            var merchId = d_ErpReplenishPlan.MerchId;

            var d_MerchDevices = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.IsStopUse == false && m.CurUseStoreId != null && m.CurUseShopId != null).ToList();

            var deviceIds = d_MerchDevices.Select(m => m.DeviceId).ToArray();

            var d_Stocks = (from m in CurrentDb.SellChannelStock
                            where m.MerchId == merchId && m.ShopMode == E_ShopMode.Device && deviceIds.Contains(m.DeviceId)
                            select new { m.StoreId, m.MerchId, m.SkuId, m.DeviceId, m.CabinetId, m.WarnQuantity, m.ShopId, m.ShopMode, m.SlotId, m.SellQuantity, m.WaitPayLockQuantity, m.WaitPickupLockQuantity, m.SumQuantity, m.MaxQuantity, m.IsOffSell }).OrderBy(m => m.DeviceId).ToList();
            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();
            var d_Shops = CurrentDb.Shop.Where(m => m.MerchId == merchId).ToList();
          

            DateTime buildTime = DateTime.Now;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    var count = CurrentDb.ErpReplenishPlanDeviceDetail.Where(m => m.PlanId == replenishPlanId).Count();

                    if (count > 0)
                    {
                        ts.Complete();
                        result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已生成");
                    }
                    else
                    {

                        bool is_Plan_Has = false;

                        var d_Device_Plans = (from m in d_Stocks
                                              select new { m.MerchId, m.StoreId, m.ShopId, m.DeviceId }).Distinct().ToList();

                        foreach (var d_Device_Plan in d_Device_Plans)
                        {
                            var l_Store = d_Stores.Where(m => m.Id == d_Device_Plan.StoreId).FirstOrDefault();
                            var l_Shop = d_Shops.Where(m => m.Id == d_Device_Plan.ShopId).FirstOrDefault();
                            var l_Device = d_MerchDevices.Where(m => m.DeviceId == d_Device_Plan.DeviceId).FirstOrDefault();

                            var d_PlanDevice = new ErpReplenishPlanDevice();
                            d_PlanDevice.Id = IdWorker.Build(IdType.NewGuid);
                            d_PlanDevice.PlanId = d_ErpReplenishPlan.Id;
                            d_PlanDevice.PlanCumCode = d_ErpReplenishPlan.CumCode;
                            d_PlanDevice.MerchId = d_Device_Plan.MerchId;
                            d_PlanDevice.StoreId = l_Store.Id;
                            d_PlanDevice.StoreName = l_Store.Name;
                            d_PlanDevice.ShopId = l_Shop.Id;
                            d_PlanDevice.ShopName = l_Shop.Name;
                            d_PlanDevice.DeviceId = l_Device.DeviceId;
                            d_PlanDevice.DeviceCumCode = l_Device.CumCode;
                            d_PlanDevice.MakeDate = d_ErpReplenishPlan.MakeDate;
                            d_PlanDevice.MakerId = d_ErpReplenishPlan.MakerId;
                            d_PlanDevice.MakerName = d_ErpReplenishPlan.MakerName;
                            d_PlanDevice.MakeTime = d_ErpReplenishPlan.MakeTime;
                            d_PlanDevice.Creator = d_ErpReplenishPlan.Creator;
                            d_PlanDevice.CreateTime = DateTime.Now;

                            var d_Device_Stocks = (from m in d_Stocks
                                                   where m.MerchId == d_Device_Plan.MerchId &&
                                                   m.StoreId == d_Device_Plan.StoreId &&
                                                   m.ShopId == d_Device_Plan.ShopId &&
                                                   m.DeviceId == d_Device_Plan.DeviceId
                                                   select new { m.StoreId, m.MerchId, m.SkuId, m.DeviceId, m.CabinetId, m.WarnQuantity, m.ShopId, m.ShopMode, m.SlotId, m.SellQuantity, m.WaitPayLockQuantity, m.WaitPickupLockQuantity, m.SumQuantity, m.MaxQuantity, m.IsOffSell });
                            bool is_Device_Has = false;
                            foreach (var d_Stock in d_Stocks)
                            {
                                var r_Sku = CacheServiceFactory.Product.GetSkuInfo(d_Stock.MerchId, d_Stock.SkuId);

                                int warnQuantity = d_Stock.WarnQuantity;
                                int sumQuantity = d_Stock.SumQuantity;

                                int maxQuantity = d_Stock.MaxQuantity;
                                int rshQuantity = maxQuantity - sumQuantity;
                                if ((sumQuantity < warnQuantity) && rshQuantity > 0)
                                {
                                    is_Device_Has = true;
                                    is_Plan_Has = true;

                                    var d_PlanDeviceDetail = new ErpReplenishPlanDeviceDetail();
                                    d_PlanDeviceDetail.Id = IdWorker.Build(IdType.NewGuid);
                                    d_PlanDeviceDetail.PlanId = d_ErpReplenishPlan.Id;
                                    d_PlanDeviceDetail.PlanDeviceId = d_PlanDevice.Id;
                                    d_PlanDeviceDetail.PlanCumCode = d_ErpReplenishPlan.CumCode;
                                    d_PlanDeviceDetail.MerchId = d_Stock.MerchId;
                                    d_PlanDeviceDetail.StoreId = l_Store.Id;
                                    d_PlanDeviceDetail.StoreName = l_Store.Name;
                                    d_PlanDeviceDetail.ShopId = l_Shop.Id;
                                    d_PlanDeviceDetail.ShopName = l_Shop.Name;
                                    d_PlanDeviceDetail.DeviceId = l_Device.DeviceId;
                                    d_PlanDeviceDetail.DeviceCumCode = l_Device.CumCode;
                                    d_PlanDeviceDetail.CabinetId = d_Stock.CabinetId;
                                    d_PlanDeviceDetail.CabinetName = d_Stock.CabinetId;
                                    d_PlanDeviceDetail.SlotId = d_Stock.SlotId;
                                    d_PlanDeviceDetail.SlotName = d_Stock.SlotId;
                                    d_PlanDeviceDetail.SpuId = r_Sku.SpuId;
                                    d_PlanDeviceDetail.SkuId = r_Sku.Id;
                                    d_PlanDeviceDetail.SkuName = r_Sku.Name;
                                    d_PlanDeviceDetail.SkuCumCode = r_Sku.CumCode;
                                    d_PlanDeviceDetail.SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes);
                                    d_PlanDeviceDetail.PlanRshQuantity = rshQuantity;
                                    d_PlanDeviceDetail.RealRshQuantity = 0;
                                    d_PlanDeviceDetail.BuildTime = buildTime;
                                    d_PlanDeviceDetail.MakerId = d_ErpReplenishPlan.MakerId;
                                    d_PlanDeviceDetail.MakerName = d_ErpReplenishPlan.MakerName;
                                    d_PlanDeviceDetail.Creator = d_ErpReplenishPlan.Creator;
                                    d_PlanDeviceDetail.CreateTime = DateTime.Now;
                                    CurrentDb.ErpReplenishPlanDeviceDetail.Add(d_PlanDeviceDetail);
                                    CurrentDb.SaveChanges();
                                }
                            }

                            if (is_Device_Has)
                            {
                                CurrentDb.ErpReplenishPlanDevice.Add(d_PlanDevice);
                                CurrentDb.SaveChanges();
                            }
                        }

                        if (is_Plan_Has)
                        {
                            d_ErpReplenishPlan.BuildTime = DateTime.Now;
                        }

                        CurrentDb.SaveChanges();
                        ts.Complete();

                        if (is_Plan_Has)
                        {
                            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "生成成功");
                        }
                        else
                        {
                            result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "生成失败，没有需补货的商品");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("", ex);
                result = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "生成异常，处理过程失败");
            }


            var dd_ErpReplenishPlan = CurrentDb.ErpReplenishPlan.Where(m => m.Id == replenishPlanId).FirstOrDefault();
            dd_ErpReplenishPlan.BuildTime = buildTime;
            if (result.Result == ResultType.Success)
            {
                dd_ErpReplenishPlan.Status = E_ErpReplenishPlan_Status.BuildSuccess;
            }
            else if (result.Result == ResultType.Failure || result.Result == ResultType.Exception)
            {
                dd_ErpReplenishPlan.Status = E_ErpReplenishPlan_Status.BuildFailure;
                dd_ErpReplenishPlan.FailReason = result.Message;
            }

            CurrentDb.SaveChanges();

            return result;
        }
    }
}
