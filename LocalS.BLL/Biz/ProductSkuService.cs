using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using MyPushSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public class ProductSkuService : BaseService
    {
        public CustomJsonResult OperateSlot(string operater, string operateEvent, string merchId, string storeId, string shopId, string machineId, string cabinetId, string slotId, string productSkuId)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (operateEvent == EventCode.MachineCabinetSlotRemove)
                {
                    #region MachineSlotRemove
                    SellChannelStock sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Machine && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                    if (sellChannelStock != null)
                    {
                        var bizProdutSku = CacheServiceFactory.Product.GetSkuInfo(merchId, sellChannelStock.PrdProductSkuId);
                        int lockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;
                        if (lockQuantity > 0)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "货道删除失败，存在有预定数量不能删除");
                        }

                        CurrentDb.SellChannelStock.Remove(sellChannelStock);
                        CurrentDb.SaveChanges();
                        ts.Complete();
                    }


                    var slot = new
                    {
                        StockId = "",
                        CabinetId = cabinetId,
                        SlotId = slotId,
                        ProductSkuId = "",
                        Name = "暂无商品",
                        CumCode = "",
                        SpecDes = "",
                        MainImgUrl = "",
                        SumQuantity = 0,
                        LockQuantity = 0,
                        SellQuantity = 0,
                        MaxQuantity = 0,
                        WarnQuantity = 0,
                        HoldQuantity = 0,
                        Version = 0,
                        IsCanAlterMaxQuantity = true
                    };

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "货道删除成功", slot);
                    #endregion MachineSlotRemove
                }
                else if (operateEvent == EventCode.MachineCabinetSlotSave)
                {
                    #region MachineSlotSave
                    SellChannelStock sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Machine && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                    var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);
                    var productSku = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                    if (sellChannelStock == null)
                    {
                        sellChannelStock = new SellChannelStock();
                        sellChannelStock.Id = IdWorker.Build(IdType.NewGuid);
                        sellChannelStock.ShopMode = E_ShopMode.Machine;
                        sellChannelStock.MerchId = merchId;
                        sellChannelStock.StoreId = storeId;
                        sellChannelStock.ShopId = shopId;
                        sellChannelStock.MachineId = machineId;
                        sellChannelStock.CabinetId = cabinetId;
                        sellChannelStock.SlotId = slotId;
                        sellChannelStock.PrdProductId = bizProductSku.ProductId;
                        sellChannelStock.PrdProductSkuId = productSkuId;
                        sellChannelStock.WaitPayLockQuantity = 0;
                        sellChannelStock.WaitPickupLockQuantity = 0;
                        sellChannelStock.SumQuantity = 0;
                        sellChannelStock.SellQuantity = 0;
                        sellChannelStock.WarnQuantity = 0;
                        sellChannelStock.HoldQuantity = 0;
                        sellChannelStock.IsOffSell = false;
                        sellChannelStock.SalePrice = productSku.SalePrice;
                        sellChannelStock.Version = 0;
                        sellChannelStock.MaxQuantity = 10;
                        sellChannelStock.CreateTime = DateTime.Now;
                        sellChannelStock.Creator = operater;
                        CurrentDb.SellChannelStock.Add(sellChannelStock);
                        CurrentDb.SaveChanges();
                        ts.Complete();
                    }
                    else
                    {
                        if (sellChannelStock.PrdProductSkuId != productSkuId)
                        {
                            int lockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;
                            if (lockQuantity > 0)
                            {
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "库存保存失败，存在有预定数量不能更换商品");
                            }

                            sellChannelStock.PrdProductId = bizProductSku.ProductId;
                            sellChannelStock.PrdProductSkuId = productSkuId;
                            sellChannelStock.IsOffSell = false;
                            sellChannelStock.SalePrice = productSku.SalePrice;
                            sellChannelStock.SumQuantity = 0;
                            sellChannelStock.WaitPayLockQuantity = 0;
                            sellChannelStock.WaitPickupLockQuantity = 0;
                            sellChannelStock.SellQuantity = 0;
                            sellChannelStock.Version = -1;
                            sellChannelStock.MaxQuantity = 10;
                            sellChannelStock.MendTime = DateTime.Now;
                            sellChannelStock.Mender = operater;
                            CurrentDb.SaveChanges();
                            ts.Complete();
                        }
                    }



                    var slot = new
                    {
                        StockId = sellChannelStock.Id,
                        CabinetId = cabinetId,
                        SlotId = slotId,
                        ProductSkuId = bizProductSku.Id,
                        Name = bizProductSku.Name,
                        CumCode = bizProductSku.CumCode,
                        MainImgUrl = ImgSet.Convert_S(bizProductSku.MainImgUrl),
                        SpecDes = SpecDes.GetDescribe(bizProductSku.SpecDes),
                        SumQuantity = sellChannelStock.SumQuantity,
                        LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity,
                        SellQuantity = sellChannelStock.SellQuantity,
                        MaxQuantity = sellChannelStock.MaxQuantity,
                        WarnQuantity = sellChannelStock.WarnQuantity,
                        HoldQuantity = sellChannelStock.HoldQuantity,
                        Version = sellChannelStock.Version,
                        IsCanAlterMaxQuantity = true
                    };
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "库存保存成功", slot);
                    #endregion
                }
                else
                {
                    result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "库存保存失败，未知操作类型");
                }

            }

            return result;
        }

        public CustomJsonResult OperateStockQuantity(string operater, string operateEvent, E_ShopMode shopMode, string merchId, string storeId, string shopId, string machineId, string cabinetId, string slotId, string productSkuId, int quantity)
        {
            var result = new CustomJsonResult();


            var ret = new RetOperateStock();

            using (TransactionScope ts = new TransactionScope())
            {
                var r_ProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);
                SellChannelStock sellChannelStock = null;
                switch (operateEvent)
                {
                    case EventCode.StockOrderReserveSuccess:
                        #region OrderReserve

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPayLockQuantity += quantity;
                        sellChannelStock.SellQuantity -= quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        var record_1 = new RetOperateStock.ChangeRecordModel
                        {
                            ShopMode = sellChannelStock.ShopMode,
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            SkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.StockOrderReserveSuccess,
                            ChangeQuantity = quantity
                        };


                        ret.ChangeRecords.Add(record_1);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.StockOrderCancle:
                        #region OrderCancle

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPayLockQuantity -= quantity;

                        if (sellChannelStock.WaitPayLockQuantity < 0)
                        {
                            LogUtil.Error("sellChannelStock.为负数，PrdProductSkuId：" + productSkuId);
                        }

                        sellChannelStock.SellQuantity += quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        var record2 = new RetOperateStock.ChangeRecordModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            SkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.StockOrderCancle,
                            ChangeQuantity = quantity
                        };

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
                        #endregion
                        break;
                    case EventCode.StockOrderPaySuccess:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPayLockQuantity -= quantity;

                        if (sellChannelStock.ShopMode == E_ShopMode.Machine)
                        {
                            sellChannelStock.WaitPickupLockQuantity += quantity;
                        }
                        else if (sellChannelStock.ShopMode == E_ShopMode.Mall)
                        {
                            sellChannelStock.SumQuantity -= quantity;
                        }

                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.StockOrderPickupOneSysMadeSignTake:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.SumQuantity -= quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.StockOrderPickupOneManMadeSignTakeByNotComplete:
                        #region OrderPickupOneManMadeSignTakeByNotComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.SumQuantity -= quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.StockOrderPickupOneManMadeSignNotTakeByComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.SellQuantity += quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.SellQuantity += 1;
                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                }
            }

            //var eventContent = new SellChannelStockChangeModel
            //{
            //    MerchId = sellChannelStock.MerchId,
            //    StoreId = sellChannelStock.StoreId,
            //    ShopId = sellChannelStock.ShopId,
            //    MachineId = sellChannelStock.MachineId,
            //    SellChannelRefType = sellChannelStock.SellChannelRefType,
            //    CabinetId = sellChannelStock.CabinetId,
            //    SlotId = sellChannelStock.SlotId,
            //    PrdProductSkuId = sellChannelStock.PrdProductSkuId,
            //    SellQuantity = sellChannelStock.SellQuantity,
            //    WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
            //    WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
            //    SumQuantity = sellChannelStock.SumQuantity,
            //    EventCode = EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete,
            //    ChangeQuantity = quantity
            //};

            //MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, shopId, machineId, EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete, string.Format("机柜：{0}，货道：{1}，商品：{2}，人为标记未取货成功，恢复可售库存：{3}，减去待取货库存：{3}，实际库存不变", cabinetId, slotId, r_ProductSku.Name, quantity), eventContent);

            //if (result.Result == ResultType.Success)
            //{
            //    SendStock(operater, appId, merchId, productSkuId);
            //}

            return result;
        }

        public CustomJsonResult AdjustStockQuantity(string operater, E_ShopMode shopMode, string merchId, string storeId, string shopId, string machineId, string cabinetId, string slotId, string productSkuId, int version, int sumQuantity, int? maxQuantity = null, int? warnQuantity = null, int? holdQuantity = null)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var r_ProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);
                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.PrdProductSkuId == productSkuId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                if (sellChannelStock == null)
                {

                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，找不到该数据");
                }

                if (sellChannelStock.Version != -1)
                {
                    if (sellChannelStock.Version != version)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，数据已经被更改，请刷新页面再尝试");
                    }
                }

                int oldSumQuantity = sellChannelStock.SumQuantity;

                sellChannelStock.SumQuantity = sumQuantity;
                sellChannelStock.SellQuantity = sumQuantity - sellChannelStock.WaitPayLockQuantity - sellChannelStock.WaitPickupLockQuantity;
                sellChannelStock.Version += 1;
                sellChannelStock.MendTime = DateTime.Now;
                sellChannelStock.Mender = operater;

                if (maxQuantity != null)
                {
                    sellChannelStock.MaxQuantity = maxQuantity.Value;
                }


                if (holdQuantity != null)
                {
                    sellChannelStock.HoldQuantity = holdQuantity.Value;
                }

                if (warnQuantity != null)
                {
                    sellChannelStock.WarnQuantity = warnQuantity.Value;
                }

                CurrentDb.SaveChanges();
                ts.Complete();


                var slot = new
                {
                    MachineId = sellChannelStock.MachineId,
                    ShopId = sellChannelStock.ShopId,
                    StockId = sellChannelStock.Id,
                    CabinetId = cabinetId,
                    SlotId = slotId,
                    ProductSkuId = r_ProductSku.Id,
                    CumCode = r_ProductSku.CumCode,
                    Name = r_ProductSku.Name,
                    MainImgUrl = ImgSet.Convert_S(r_ProductSku.MainImgUrl),
                    SpecDes = SpecDes.GetDescribe(r_ProductSku.SpecDes),
                    SumQuantity = sellChannelStock.SumQuantity,
                    LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity,
                    SellQuantity = sellChannelStock.SellQuantity,
                    MaxQuantity = sellChannelStock.MaxQuantity,
                    WarnQuantity = sellChannelStock.WarnQuantity,
                    HoldQuantity = sellChannelStock.HoldQuantity,
                    Version = sellChannelStock.Version,
                    IsCanAlterMaxQuantity = true
                };

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功", slot);
            }

            //if (result.Result == ResultType.Success)
            //{
            //    SendStock(operater, appId, merchId, productSkuId);
            //}

            return result;

        }

        public CustomJsonResult AdjustStockSalePrice(string operater, string merchId, string storeId, string productSkuId, decimal salePrice, bool isOffSell)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var r_ProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);

                CacheServiceFactory.Product.RemoveSpuInfo(merchId, r_ProductSku.ProductId);

                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.PrdProductSkuId == productSkuId).ToList();

                foreach (var sellChannelStock in sellChannelStocks)
                {
                    sellChannelStock.SalePrice = salePrice;
                    sellChannelStock.IsOffSell = isOffSell;
                    sellChannelStock.MendTime = DateTime.Now;
                    sellChannelStock.Mender = operater;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            //if (result.Result == ResultType.Success)
            //{
            //    SendStock(operater, appId, merchId, productSkuId);
            //}

            return result;
        }
    }
}
