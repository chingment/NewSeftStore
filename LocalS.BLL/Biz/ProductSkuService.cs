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

        private void SendUpdateProductSkuStock(string operater, string appId, E_SellChannelRefType refType, string merchId, string storeId, string shopId, string[] machineIds, string productSkuId)
        {

            var r_ProductSku = CacheServiceFactory.Product.GetSkuStock(refType, merchId, storeId, shopId, machineIds, productSkuId);

            if (r_ProductSku != null)
            {

                var updateProdcutSkuStock = new UpdateMachineProdcutSkuStockModel();
                updateProdcutSkuStock.ProductSkuId = r_ProductSku.Id;
                updateProdcutSkuStock.IsOffSell = r_ProductSku.Stocks[0].IsOffSell;
                updateProdcutSkuStock.SalePrice = r_ProductSku.Stocks[0].SalePrice;
                updateProdcutSkuStock.LockQuantity = r_ProductSku.Stocks.Sum(m => m.LockQuantity);
                updateProdcutSkuStock.SellQuantity = r_ProductSku.Stocks.Sum(m => m.SellQuantity);
                updateProdcutSkuStock.SumQuantity = r_ProductSku.Stocks.Sum(m => m.SumQuantity);
                updateProdcutSkuStock.IsTrgVideoService = r_ProductSku.IsTrgVideoService;
                // BizFactory.Machine.SendUpdateProductSkuStock(operater, appId, merchId, null, updateProdcutSkuStock);
            }


        }

        public CustomJsonResult OperateSlot(string operater, string operateEvent, string appId, string merchId, string storeId, string shopId, string machineId, string cabinetId, string slotId, string productSkuId)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (operateEvent == EventCode.MachineCabinetSlotRemove)
                {
                    #region MachineSlotRemove
                    SellChannelStock sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == E_SellChannelRefType.Machine && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                    if (sellChannelStock != null)
                    {
                        var bizProdutSku = CacheServiceFactory.Product.GetSkuInfo(merchId, sellChannelStock.PrdProductSkuId);
                        int lockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;
                        if (lockQuantity > 0)
                        {
                            MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotSave, string.Format("机柜：{0}，货道：{1}，删除失败，存在有预定数量不能删除", cabinetId, slotId));
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "删除失败，存在有预定数量不能删除");
                        }

                        CurrentDb.SellChannelStock.Remove(sellChannelStock);
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        var eventContent = new SellChannelStockChangeModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.MachineCabinetSlotRemove,
                            ChangeQuantity = -sellChannelStock.SumQuantity
                        };

                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotSave, string.Format("机柜：{0}，货道：{1}，商品：{2}，删除成功，移除实际库存：{3}", cabinetId, slotId, bizProdutSku.Name, sellChannelStock.SumQuantity), eventContent);
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

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", slot);
                    #endregion MachineSlotRemove
                }
                else if (operateEvent == EventCode.MachineCabinetSlotSave)
                {
                    #region MachineSlotSave
                    SellChannelStock sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == E_SellChannelRefType.Machine && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                    var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);
                    var productSku = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                    if (sellChannelStock == null)
                    {
                        sellChannelStock = new SellChannelStock();
                        sellChannelStock.Id = IdWorker.Build(IdType.NewGuid);
                        sellChannelStock.SellChannelRefType = E_SellChannelRefType.Machine;
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

                        var eventContent = new SellChannelStockChangeModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = 0,
                            WaitPayLockQuantity = 0,
                            WaitPickupLockQuantity = 0,
                            SumQuantity = 0,
                            EventCode = EventCode.MachineCabinetSlotSave,
                            ChangeQuantity = 0
                        };


                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotSave, string.Format("机柜：{0}，货道：{1}，商品：{2}，初次录入", cabinetId, slotId, bizProductSku.Name));
                    }
                    else
                    {
                        if (sellChannelStock.PrdProductSkuId != productSkuId)
                        {
                            int lockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;
                            if (lockQuantity > 0)
                            {

                                MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotRemove, string.Format("机柜：{0}，货道：{1}，商品：{2}，货道删除失败，存在有预定数量不能删除", cabinetId, slotId, bizProductSku.Name));

                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "删除失败，存在有预定数量不能删除");
                            }

                            var oldBizProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, sellChannelStock.PrdProductSkuId);

                            int oldSumQuantity = sellChannelStock.SumQuantity;
                            var oldEventContent = new SellChannelStockChangeModel
                            {
                                MerchId = sellChannelStock.MerchId,
                                StoreId = sellChannelStock.StoreId,
                                ShopId = sellChannelStock.ShopId,
                                MachineId = sellChannelStock.MachineId,
                                SellChannelRefType = sellChannelStock.SellChannelRefType,
                                CabinetId = sellChannelStock.CabinetId,
                                SlotId = sellChannelStock.SlotId,
                                PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                                SellQuantity = sellChannelStock.SellQuantity,
                                WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                                WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                                SumQuantity = sellChannelStock.SumQuantity,
                                EventCode = EventCode.MachineCabinetSlotRemove,
                                ChangeQuantity = sellChannelStock.SumQuantity
                            };

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


                            MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotSave, string.Format("机柜：{0}，货道：{1}，商品变换，删除商品：{2}，删除数量：{3}，替换成商品：{4}", cabinetId, slotId, oldBizProductSku.Name, oldSumQuantity, bizProductSku.Name), oldEventContent);

                            var newEeventContent = new SellChannelStockChangeModel
                            {
                                MerchId = sellChannelStock.MerchId,
                                StoreId = sellChannelStock.StoreId,
                                ShopId = sellChannelStock.ShopId,
                                MachineId = sellChannelStock.MachineId,
                                SellChannelRefType = sellChannelStock.SellChannelRefType,
                                CabinetId = sellChannelStock.CabinetId,
                                SlotId = sellChannelStock.SlotId,
                                PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                                SellQuantity = 0,
                                WaitPayLockQuantity = 0,
                                WaitPickupLockQuantity = 0,
                                SumQuantity = 0,
                                EventCode = EventCode.MachineCabinetSlotInit,
                                ChangeQuantity = 0
                            };


                            MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotSave, string.Format("机柜：{0}，货道：{1}，商品变换，原来商品：{2}，替换成商品：{3}", cabinetId, slotId, oldBizProductSku.Name, bizProductSku.Name), newEeventContent);
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
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", slot);
                    #endregion
                }
                else
                {
                    result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未知操作类型");
                }

            }

            return result;
        }

        public CustomJsonResult OperateStockQuantity(string operater, string operateEvent, string appId, E_SellChannelRefType refType, string merchId, string storeId, string shopId, string machineId, string cabinetId, string slotId, string productSkuId, int quantity)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);

                SellChannelStockChangeModel eventContent = new SellChannelStockChangeModel();
                SellChannelStock sellChannelStock = null;
                switch (operateEvent)
                {
                    case EventCode.StockOrderReserveSuccess:
                        #region OrderReserve

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == refType && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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

                        eventContent = new SellChannelStockChangeModel
                        {
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.StockOrderReserveSuccess,
                            ChangeQuantity = quantity
                        };


                        //MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, sellChannelRefId, EventCode.StockOrderReserveSuccess, string.Format("机柜：{0}，货道：{1}，商品：{2}，预定成功，未支付，减少可售库存：{3}，增加待支付库存：{3}，实际库存不变", cabinetId, slotId, bizProductSku.Name, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case EventCode.StockOrderCancle:
                        #region OrderCancle

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == refType && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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

                        eventContent = new SellChannelStockChangeModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.StockOrderCancle,
                            ChangeQuantity = quantity
                        };

                        //MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, sellChannelRefId, EventCode.StockOrderCancle, string.Format("机柜：{0}，货道：{1}，商品：{2}，未支付，取消订单，增加可售库存：{3}，减少未支付库存：{3}，实际库存不变", cabinetId, slotId, bizProductSku.Name, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                        #endregion
                        break;
                    case EventCode.StockOrderPaySuccess:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == refType && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPayLockQuantity -= quantity;

                        if (sellChannelStock.SellChannelRefType == E_SellChannelRefType.Machine)
                        {
                            sellChannelStock.WaitPickupLockQuantity += quantity;
                        }
                        else if (sellChannelStock.SellChannelRefType == E_SellChannelRefType.Mall)
                        {
                            sellChannelStock.SumQuantity -= quantity;
                        }

                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        eventContent = new SellChannelStockChangeModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.StockOrderPaySuccess,
                            ChangeQuantity = quantity
                        };


                        //MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, sellChannelRefId, EventCode.StockOrderPaySuccess, string.Format("机柜：{0}，货道：{1}，商品：{2}，成功支付，待取货，减少待支付库存：{3}，增加待取货库存：{3}，可售库存不变，实际库存不变", cabinetId, slotId, bizProductSku.Name, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case EventCode.StockOrderPickupOneSysMadeSignTake:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == refType && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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

                        eventContent = new SellChannelStockChangeModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.StockOrderPickupOneSysMadeSignTake,
                            ChangeQuantity = quantity
                        };


                        //MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, sellChannelRefId, EventCode.StockOrderPickupOneSysMadeSignTake, string.Format("机柜：{0}，货道：{1}，商品：{2}，成功取货，减少实际库存：{3}，减少待取货库存：{3}，可售库存不变", cabinetId, slotId, bizProductSku.Name, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case EventCode.StockOrderPickupOneManMadeSignTakeByNotComplete:
                        #region OrderPickupOneManMadeSignTakeByNotComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == refType && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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

                        eventContent = new SellChannelStockChangeModel
                        {
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.StockOrderPickupOneManMadeSignTakeByNotComplete,
                            ChangeQuantity = quantity
                        };


                        //MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, sellChannelRefId, EventCode.StockOrderPickupOneManMadeSignTakeByNotComplete, string.Format("机柜：{0}，货道：{1}，商品：{2}，人为标记为取货成功，减去实际库存：{3}，减去待取货库存：{3}，可售库存不变", cabinetId, slotId, bizProductSku.Name, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case EventCode.StockOrderPickupOneManMadeSignNotTakeByComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == refType && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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

                        eventContent = new SellChannelStockChangeModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.StockOrderPickupOneManMadeSignNotTakeByComplete,
                            ChangeQuantity = quantity
                        };

                        //MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, sellChannelRefId, EventCode.StockOrderPickupOneManMadeSignNotTakeByComplete, string.Format("机柜：{0}，货道：{1}，商品：{2}，系统已经标识出货成功，但实际未取货成功，人为标记未取货成功，增加可售库存：{3}，增加实际库存：{3}", cabinetId, slotId, bizProductSku.Name, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == refType && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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

                        eventContent = new SellChannelStockChangeModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            ShopId = sellChannelStock.ShopId,
                            MachineId = sellChannelStock.MachineId,
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
                            CabinetId = sellChannelStock.CabinetId,
                            SlotId = sellChannelStock.SlotId,
                            PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            EventCode = EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete,
                            ChangeQuantity = quantity
                        };

                        //MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, sellChannelRefId, EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete, string.Format("机柜：{0}，货道：{1}，商品：{2}，人为标记未取货成功，恢复可售库存：{3}，减去待取货库存：{3}，实际库存不变", cabinetId, slotId, bizProductSku.Name, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                }
            }

            if (result.Result == ResultType.Success)
            {
                //try
                //{
                //    SendUpdateProductSkuStock(operater, appId, merchId, storeId, new string[] { sellChannelRefId }, productSkuId);
                //}
                //catch (Exception ex)
                //{

                //}
            }

            return result;
        }

        public CustomJsonResult AdjustStockQuantity(string operater, string appId, string merchId, string storeId, string shopId, string machineId, string cabinetId, string slotId, string productSkuId, int version, int sumQuantity, int? maxQuantity = null, int? warnQuantity = null, int? holdQuantity = null)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                SellChannelStockChangeModel eventContent = new SellChannelStockChangeModel();

                var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);
                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.SellChannelRefType == E_SellChannelRefType.Machine && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.PrdProductSkuId == productSkuId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                if (sellChannelStock == null)
                {
                    MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotAdjustStockQuantity, string.Format("机柜：{0}，货道：{1}，商品：{2}，保存失败，找不到该数据", cabinetId, slotId, bizProductSku.Name));
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，找不到该数据");
                }

                if (sellChannelStock.Version != -1)
                {
                    if (sellChannelStock.Version != version)
                    {
                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotAdjustStockQuantity, string.Format("机柜：{0}，货道：{1}，商品：{2}，保存失败，数据已经被更改", cabinetId, slotId, bizProductSku.Name));
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

                eventContent = new SellChannelStockChangeModel
                {
                    SellChannelRefType = sellChannelStock.SellChannelRefType,
                    MerchId = sellChannelStock.MerchId,
                    StoreId = sellChannelStock.StoreId,
                    ShopId = sellChannelStock.ShopId,
                    MachineId = sellChannelStock.MachineId,
                    CabinetId = sellChannelStock.CabinetId,
                    SlotId = sellChannelStock.SlotId,
                    PrdProductSkuId = sellChannelStock.PrdProductSkuId,
                    SellQuantity = sellChannelStock.SellQuantity,
                    WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                    WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                    SumQuantity = sellChannelStock.SumQuantity,
                    EventCode = EventCode.MachineCabinetSlotAdjustStockQuantity,
                    ChangeQuantity = sellChannelStock.SumQuantity - oldSumQuantity
                };


                MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotAdjustStockQuantity, string.Format("机柜：{0}，货道：{1}，商品：{2}，库存数量将{3}调整成{4}", cabinetId, slotId, bizProductSku.Name, oldSumQuantity, sumQuantity), eventContent);

                var slot = new
                {
                    MachineId = sellChannelStock.MachineId,
                    ShopId = sellChannelStock.ShopId,
                    StockId = sellChannelStock.Id,
                    CabinetId = cabinetId,
                    SlotId = slotId,
                    ProductSkuId = bizProductSku.Id,
                    CumCode = bizProductSku.CumCode,
                    Name = bizProductSku.Name,
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

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", slot);
            }

            if (result.Result == ResultType.Success)
            {
                //SendUpdateProductSkuStock(operater, appId, merchId, storeId, new string[] { machineId }, productSkuId);
            }

            return result;

        }

        public CustomJsonResult AdjustStockSalePrice(string operater, string appId, string merchId, string storeId, string productSkuId, decimal salePrice, bool isOffSell)
        {
            var result = new CustomJsonResult();

            string[] machineIds = null;

            using (TransactionScope ts = new TransactionScope())
            {


                var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);

                CacheServiceFactory.Product.RemoveSpuInfo(merchId, bizProductSku.ProductId);

                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.PrdProductSkuId == productSkuId).ToList();
                machineIds = sellChannelStocks.Where(m => m.SellChannelRefType == E_SellChannelRefType.Machine).Select(m => m.MachineId).Distinct().ToArray();
                foreach (var sellChannelStock in sellChannelStocks)
                {

                    sellChannelStock.SalePrice = salePrice;
                    sellChannelStock.IsOffSell = isOffSell;
                    sellChannelStock.MendTime = DateTime.Now;
                    sellChannelStock.Mender = operater;
                }

                CurrentDb.SaveChanges();
                ts.Complete();


                foreach (string machineId in machineIds)
                {
                    MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineAdjustStockSalePrice, string.Format("商品：{0}，调整价格为：{1}", bizProductSku.Name, salePrice));
                }

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            if (result.Result == ResultType.Success)
            {
                //SendUpdateProductSkuStock(operater, appId, merchId, storeId, machineIds, productSkuId);
            }

            return result;
        }
    }
}
