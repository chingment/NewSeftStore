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

    public enum OperateSlotType
    {
        Unknow = 0,
        MachineSlotRemove = 1,
        MachineSlotSave = 2
    }

    public enum OperateStockType
    {
        Unknow = 0,
        OrderReserveSuccess = 11,
        OrderCancle = 12,
        OrderPaySuccess = 13,
        OrderPickupOneSysMadeSignTake = 15,
        OrderPickupOneManMadeSignTakeByNotComplete = 16,
        OrderPickupOneManMadeSignNotTakeByComplete = 17,
        OrderPickupOneManMadeSignNotTakeByNotComplete = 18
    }

    public class ProductSkuService : BaseDbContext
    {

        private void SendUpdateProductSkuStock(string operater, string appId, string merchId, string storeId, string[] machineIds, string productSkuId)
        {
            if (machineIds != null)
            {
                foreach (var machineId in machineIds)
                {

                    var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(merchId, storeId, new string[] { machineId }, productSkuId);

                    if (bizProductSku != null)
                    {

                        var updateProdcutSkuStock = new UpdateMachineProdcutSkuStockModel();
                        updateProdcutSkuStock.Id = bizProductSku.Id;
                        updateProdcutSkuStock.IsOffSell = bizProductSku.Stocks[0].IsOffSell;
                        updateProdcutSkuStock.SalePrice = bizProductSku.Stocks[0].SalePrice;
                        updateProdcutSkuStock.SalePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
                        updateProdcutSkuStock.LockQuantity = bizProductSku.Stocks.Sum(m => m.LockQuantity);
                        updateProdcutSkuStock.SellQuantity = bizProductSku.Stocks.Sum(m => m.SellQuantity);
                        updateProdcutSkuStock.SumQuantity = bizProductSku.Stocks.Sum(m => m.SumQuantity);
                        BizFactory.Machine.SendUpdateProductSkuStock(operater, appId, merchId, machineId, updateProdcutSkuStock);
                    }
                }
            }
        }

        public CustomJsonResult OperateSlot(string operater, OperateSlotType operateType, string appId, string merchId, string storeId, string machineId, string cabinetId, string slotId, string productSkuId)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (operateType == OperateSlotType.MachineSlotRemove)
                {
                    #region MachineSlotRemove
                    SellChannelStock sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                    if (sellChannelStock != null)
                    {
                        var produtSkuName = CacheServiceFactory.ProductSku.GetName(merchId, sellChannelStock.PrdProductSkuId);
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
                            SellChannelRefId = sellChannelStock.SellChannelRefId,
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

                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotSave, string.Format("机柜：{0}，货道：{1}，商品：{2}，删除成功，移除实际库存：{3}", cabinetId, slotId, produtSkuName, sellChannelStock.SumQuantity), eventContent);
                    }

                    var slot = new
                    {
                        Id = slotId,
                        ProductSkuId = "",
                        ProductSkuName = "暂无商品",
                        ProductSkuMainImgUrl = "",
                        SumQuantity = 0,
                        LockQuantity = 0,
                        SellQuantity = 0,
                        MaxQuantity = 10
                    };

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", slot);
                    #endregion MachineSlotRemove
                }
                else if (operateType == OperateSlotType.MachineSlotSave)
                {
                    #region MachineSlotSave
                    SellChannelStock sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                    var bizProductSku = CacheServiceFactory.ProductSku.GetInfo(merchId, productSkuId);
                    var productSku = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                    if (sellChannelStock == null)
                    {
                        sellChannelStock = new SellChannelStock();
                        sellChannelStock.Id = IdWorker.Build(IdType.NewGuid);
                        sellChannelStock.MerchId = merchId;
                        sellChannelStock.StoreId = storeId;
                        sellChannelStock.SellChannelRefType = E_SellChannelRefType.Machine;
                        sellChannelStock.SellChannelRefId = machineId;
                        sellChannelStock.CabinetId = cabinetId;
                        sellChannelStock.SlotId = slotId;
                        sellChannelStock.PrdProductId = bizProductSku.ProductId;
                        sellChannelStock.PrdProductSkuId = productSkuId;
                        sellChannelStock.WaitPayLockQuantity = 0;
                        sellChannelStock.WaitPickupLockQuantity = 0;
                        sellChannelStock.SumQuantity = 0;
                        sellChannelStock.SellQuantity = 0;
                        sellChannelStock.IsOffSell = false;
                        sellChannelStock.SalePrice = productSku.SalePrice;
                        sellChannelStock.SalePriceByVip = productSku.SalePrice;
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
                            SellChannelRefId = sellChannelStock.SellChannelRefId,
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

                            var oldBizProductSku = CacheServiceFactory.ProductSku.GetInfo(merchId, sellChannelStock.PrdProductSkuId);

                            int oldSumQuantity = sellChannelStock.SumQuantity;
                            var oldEventContent = new SellChannelStockChangeModel
                            {
                                MerchId = sellChannelStock.MerchId,
                                StoreId = sellChannelStock.StoreId,
                                SellChannelRefId = sellChannelStock.SellChannelRefId,
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
                            sellChannelStock.SalePriceByVip = productSku.SalePrice;
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
                                SellChannelRefId = sellChannelStock.SellChannelRefId,
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
                        Id = slotId,
                        ProductSkuId = bizProductSku.Id,
                        ProductSkuName = bizProductSku.Name,
                        ProductSkuMainImgUrl = bizProductSku.MainImgUrl,
                        SumQuantity = sellChannelStock.SumQuantity,
                        LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity,
                        SellQuantity = sellChannelStock.SellQuantity,
                        MaxQuantity = sellChannelStock.MaxQuantity,
                        Version = sellChannelStock.Version
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

        public CustomJsonResult OperateStockQuantity(string operater, OperateStockType operateType, string appId, string merchId, string storeId, string machineId, string cabinetId, string slotId, string productSkuId, int quantity)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                var productSkuName = CacheServiceFactory.ProductSku.GetName(merchId, productSkuId);

                SellChannelStockChangeModel eventContent = new SellChannelStockChangeModel();
                SellChannelStock sellChannelStock = null;
                switch (operateType)
                {
                    case OperateStockType.OrderReserveSuccess:
                        #region OrderReserve

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            SellChannelRefId = sellChannelStock.SellChannelRefId,
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
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


                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.StockOrderReserveSuccess, string.Format("机柜：{0}，货道：{1}，商品：{2}，预定成功，未支付，减少可售库存：{3}，增加待支付库存：{3}，实际库存不变", cabinetId, slotId, productSkuName, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case OperateStockType.OrderCancle:
                        #region OrderCancle

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPayLockQuantity -= quantity;
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
                            SellChannelRefId = sellChannelStock.SellChannelRefId,
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

                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.StockOrderCancle, string.Format("机柜：{0}，货道：{1}，商品：{2}，未支付，取消订单，增加可售库存：{3}，减少未支付库存：{3}，实际库存不变", cabinetId, slotId, productSkuName, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                        #endregion
                        break;
                    case OperateStockType.OrderPaySuccess:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPayLockQuantity -= quantity;
                        sellChannelStock.WaitPickupLockQuantity += quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        eventContent = new SellChannelStockChangeModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            SellChannelRefId = sellChannelStock.SellChannelRefId,
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


                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.StockOrderPaySuccess, string.Format("机柜：{0}，货道：{1}，商品：{2}，成功支付，待取货，减少待支付库存：{3}，增加待取货库存：{3}，可售库存不变，实际库存不变", cabinetId, slotId, productSkuName, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case OperateStockType.OrderPickupOneSysMadeSignTake:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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
                            SellChannelRefId = sellChannelStock.SellChannelRefId,
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


                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.StockOrderPickupOneSysMadeSignTake, string.Format("机柜：{0}，货道：{1}，商品：{2}，成功取货，减少实际库存：{3}，减少待取货库存：{3}，可售库存不变", cabinetId, slotId, productSkuName, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case OperateStockType.OrderPickupOneManMadeSignTakeByNotComplete:
                        #region OrderPickupOneManMadeSignTakeByNotComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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
                            SellChannelRefId = sellChannelStock.SellChannelRefId,
                            SellChannelRefType = sellChannelStock.SellChannelRefType,
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


                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.StockOrderPickupOneManMadeSignTakeByNotComplete, string.Format("机柜：{0}，货道：{1}，商品：{2}，人为标记为取货成功，减去实际库存：{3}，减去待取货库存：{3}，可售库存不变", cabinetId, slotId, productSkuName, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case OperateStockType.OrderPickupOneManMadeSignNotTakeByComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.SellQuantity += quantity;
                        sellChannelStock.SumQuantity += quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        eventContent = new SellChannelStockChangeModel
                        {
                            MerchId = sellChannelStock.MerchId,
                            StoreId = sellChannelStock.StoreId,
                            SellChannelRefId = sellChannelStock.SellChannelRefId,
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

                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.StockOrderPickupOneManMadeSignNotTakeByComplete, string.Format("机柜：{0}，货道：{1}，商品：{2}，系统已经标识出货成功，但实际未取货成功，人为标记未取货成功，增加可售库存：{3}，增加实际库存：{3}", cabinetId, slotId, productSkuName, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case OperateStockType.OrderPickupOneManMadeSignNotTakeByNotComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
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
                            SellChannelRefId = sellChannelStock.SellChannelRefId,
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

                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete, string.Format("机柜：{0}，货道：{1}，商品：{2}，人为标记未取货成功，恢复可售库存：{3}，减去待取货库存：{3}，实际库存不变", cabinetId, slotId, productSkuName, quantity), eventContent);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                }
            }

            if (result.Result == ResultType.Success)
            {
                SendUpdateProductSkuStock(operater, appId, merchId, storeId, new string[] { machineId }, productSkuId);
            }

            return result;
        }

        public CustomJsonResult AdjustStockQuantity(string operater, string appId, string merchId, string storeId, string machineId, string cabinetId, string slotId, string productSkuId, int version, int sumQuantity, int? maxQuantity = null)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                SellChannelStockChangeModel eventContent = new SellChannelStockChangeModel();

                var productSkuName = CacheServiceFactory.ProductSku.GetName(merchId, productSkuId);

                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId && m.PrdProductSkuId == productSkuId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                if (sellChannelStock == null)
                {
                    MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotAdjustStockQuantity, string.Format("机柜：{0}，货道：{1}，商品：{2}，保存失败，找不到该数据", cabinetId, slotId, productSkuName));
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，找不到该数据");
                }

                if (sellChannelStock.Version != -1)
                {
                    if (sellChannelStock.Version != version)
                    {
                        MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotAdjustStockQuantity, string.Format("机柜：{0}，货道：{1}，商品：{2}，保存失败，数据已经被更改", cabinetId, slotId, productSkuName));
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，数据已经被更改，请刷新页面再尝试");
                    }
                }

                int oldSumQuantity = sellChannelStock.SumQuantity;

                var bizProductSku = CacheServiceFactory.ProductSku.GetInfo(merchId, productSkuId);

                sellChannelStock.SumQuantity = sumQuantity;
                sellChannelStock.SellQuantity = sumQuantity - sellChannelStock.WaitPayLockQuantity - sellChannelStock.WaitPickupLockQuantity;
                sellChannelStock.Version += 1;
                sellChannelStock.MendTime = DateTime.Now;
                sellChannelStock.Mender = operater;
                if (maxQuantity == null)
                {
                    sellChannelStock.MaxQuantity = sumQuantity;//取最近一次为置满库存
                }
                else
                {
                    sellChannelStock.MaxQuantity = maxQuantity.Value;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                eventContent = new SellChannelStockChangeModel
                {
                    MerchId = sellChannelStock.MerchId,
                    StoreId = sellChannelStock.StoreId,
                    SellChannelRefId = sellChannelStock.SellChannelRefId,
                    SellChannelRefType = sellChannelStock.SellChannelRefType,
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


                MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineCabinetSlotAdjustStockQuantity, string.Format("机柜：{0}，货道：{1}，商品：{2}，库存数量将{3}调整成{4}", cabinetId, slotId, productSkuName, oldSumQuantity, sumQuantity), eventContent);

                var slot = new
                {
                    Id = slotId,
                    CabinetId = cabinetId,
                    ProductSkuId = bizProductSku.Id,
                    ProductSkuName = bizProductSku.Name,
                    ProductSkuMainImgUrl = bizProductSku.MainImgUrl,
                    SumQuantity = sellChannelStock.SumQuantity,
                    LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity,
                    SellQuantity = sellChannelStock.SellQuantity,
                    MaxQuantity = sellChannelStock.MaxQuantity,
                    Version = sellChannelStock.Version
                };

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", slot);
            }

            if (result.Result == ResultType.Success)
            {
                SendUpdateProductSkuStock(operater, appId, merchId, storeId, new string[] { machineId }, productSkuId);
            }

            return result;

        }

        public CustomJsonResult AdjustStockSalePrice(string operater, string appId, string merchId, string storeId, string productSkuId, decimal salePrice, bool isOffSell)
        {
            var result = new CustomJsonResult();

            string[] machineIds = null;

            using (TransactionScope ts = new TransactionScope())
            {
                var productSkuName = CacheServiceFactory.ProductSku.GetName(merchId, productSkuId);

                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.PrdProductSkuId == productSkuId).ToList();
                machineIds = sellChannelStocks.Where(m => m.SellChannelRefType == E_SellChannelRefType.Machine).Select(m => m.SellChannelRefId).Distinct().ToArray();
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
                    MqFactory.Global.PushEventNotify(operater, appId, merchId, storeId, machineId, EventCode.MachineAdjustStockSalePrice, string.Format("商品：{0}，调整价格为：{1}", productSkuId, salePrice));
                }

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            if (result.Result == ResultType.Success)
            {
                SendUpdateProductSkuStock(operater, appId, merchId, storeId, machineIds, productSkuId);
            }

            return result;
        }
    }
}
