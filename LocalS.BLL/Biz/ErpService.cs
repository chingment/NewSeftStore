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

            var query = (from m in CurrentDb.SellChannelStock
                         where
                         m.MerchId == merchId &&
                         m.ShopMode == E_ShopMode.Device

                         select new { m.StoreId, m.MerchId, m.SkuId, m.DeviceId, m.CabinetId, m.WarnQuantity, m.ShopId, m.ShopMode, m.SlotId, m.SellQuantity, m.WaitPayLockQuantity, m.WaitPickupLockQuantity, m.SumQuantity, m.MaxQuantity, m.IsOffSell });

            var d_Stocks = query.OrderBy(m => m.DeviceId).ToList();

            //  var dt_Stocks = (from m in d_Stocks select new { m.StoreId, m.MerchId, m.SkuId, m.DeviceId, m.ShopId, m.ShopMode, m.IsOffSell }).Distinct();

            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();
            var d_Shops = CurrentDb.Shop.Where(m => m.MerchId == merchId).ToList();
            var d_MerchDevices = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId).ToList();

            DateTime buildTime = DateTime.Now;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    var count = CurrentDb.ErpReplenishPlanDetail.Where(m => m.PlanId == replenishPlanId).Count();

                    if (count > 0)
                    {
                        ts.Complete();
                        result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已生成");
                    }
                    else
                    {

                        foreach (var d_Stock in d_Stocks)
                        {
                            var r_Sku = CacheServiceFactory.Product.GetSkuInfo(d_Stock.MerchId, d_Stock.SkuId);

                            var l_Store = d_Stores.Where(m => m.Id == d_Stock.StoreId).FirstOrDefault();
                            var l_Shop = d_Shops.Where(m => m.Id == d_Stock.ShopId).FirstOrDefault();
                            var l_Device = d_MerchDevices.Where(m => m.DeviceId == d_Stock.DeviceId).FirstOrDefault();

                            //var l_Stock = d_Stocks.Where(m => m.SkuId == dt_Stock.SkuId);

                            //int warnQuantity = l_Stock.Sum(m => m.WarnQuantity);
                            //int sellQuantity = l_Stock.Sum(m => m.SellQuantity);
                            //int waitPayLockQuantity = l_Stock.Sum(m => m.WaitPayLockQuantity);
                            //int waitPickupLockQuantity = l_Stock.Sum(m => m.WaitPickupLockQuantity);

                            int sumQuantity = d_Stock.SumQuantity;
                            int maxQuantity = d_Stock.MaxQuantity;
                            ////sumQuantity <= warnQuantity
                            if (true)
                            {
                                var d_ErpReplenishPlanDetail = new ErpReplenishPlanDetail();
                                d_ErpReplenishPlanDetail.Id = IdWorker.Build(IdType.NewGuid);
                                d_ErpReplenishPlanDetail.PlanId = d_ErpReplenishPlan.Id;
                                d_ErpReplenishPlanDetail.PlanCumCode = d_ErpReplenishPlan.CumCode;
                                d_ErpReplenishPlanDetail.MerchId = d_Stock.MerchId;
                                d_ErpReplenishPlanDetail.StoreId = l_Store.Id;
                                d_ErpReplenishPlanDetail.StoreName = l_Store.Name;
                                d_ErpReplenishPlanDetail.ShopId = l_Shop.Id;
                                d_ErpReplenishPlanDetail.ShopName = l_Shop.Name;
                                d_ErpReplenishPlanDetail.DeviceId = l_Device.DeviceId;
                                d_ErpReplenishPlanDetail.DeviceCumCode = l_Device.CumCode;
                                d_ErpReplenishPlanDetail.CabinetId = d_Stock.CabinetId;
                                d_ErpReplenishPlanDetail.CabinetName = d_Stock.CabinetId;
                                d_ErpReplenishPlanDetail.SlotId = d_Stock.SlotId;
                                d_ErpReplenishPlanDetail.SlotName = d_Stock.SlotId;
                                d_ErpReplenishPlanDetail.SpuId = r_Sku.SpuId;
                                d_ErpReplenishPlanDetail.SkuId = r_Sku.Id;
                                d_ErpReplenishPlanDetail.SkuName = r_Sku.Name;
                                d_ErpReplenishPlanDetail.SkuCumCode = r_Sku.CumCode;
                                d_ErpReplenishPlanDetail.SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes);
                                d_ErpReplenishPlanDetail.PlanQuantity = maxQuantity - sumQuantity;
                                d_ErpReplenishPlanDetail.RshQuantity = 0;
                                d_ErpReplenishPlanDetail.BuildTime = buildTime;
                                d_ErpReplenishPlanDetail.MakerId = d_ErpReplenishPlan.MakerId;
                                d_ErpReplenishPlanDetail.MakerName = d_ErpReplenishPlan.MakerName;
                                d_ErpReplenishPlanDetail.Creator = d_ErpReplenishPlan.Creator;
                                d_ErpReplenishPlanDetail.CreateTime = DateTime.Now;

                                CurrentDb.ErpReplenishPlanDetail.Add(d_ErpReplenishPlanDetail);
                            }
                        }

                        CurrentDb.SaveChanges();
                        ts.Complete();
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "生成成功");
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("", ex);
                result = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "生成异常");
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
