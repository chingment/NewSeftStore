using LocalS.Entity;
using Lumos;
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
        OrderReserve = 1,
        OrderCancle = 2,
        OrderPaySuccess = 3,
        OrderPickupOneSysMadeSignTake = 5,
        OrderPickupOneManMadeSignTakeByNotComplete = 6,
        OrderPickupOneManMadeSignNotTakeByComplete = 7,
        OrderPickupOneManMadeSignNotTakeByNotComplete = 8
    }

    public class ProductSkuService : BaseDbContext
    {

        private void SendUpdateProductSkuStock(string merchId, string storeId, string[] machineIds, string productSkuId)
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
                        BizFactory.Machine.SendUpdateProductSkuStock(machineId, updateProdcutSkuStock);
                    }
                }
            }
        }

        public CustomJsonResult OperateSlot(string operater, OperateSlotType type, string merchId, string storeId, string machineId, string slotId, string productSkuId)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                if (type == OperateSlotType.MachineSlotRemove)
                {
                    #region MachineSlotRemove
                    SellChannelStock sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId).FirstOrDefault();
                    if (sellChannelStock != null)
                    {
                        int lockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;
                        if (lockQuantity > 0)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "删除失败，存在有预定数量不能删除");
                        }

                        CurrentDb.SellChannelStock.Remove(sellChannelStock);


                        var sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                        sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.SlotRemove;
                        sellChannelStockLog.ChangeQuantity = 0;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = "移除库存";
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);

                        CurrentDb.SaveChanges();
                        ts.Complete();
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
                else if (type == OperateSlotType.MachineSlotSave)
                {
                    #region MachineSlotSave
                    SellChannelStock sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId).FirstOrDefault();
                    var bizProductSku = CacheServiceFactory.ProductSku.GetInfo(merchId, productSkuId);
                    var productSku = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                    if (sellChannelStock == null)
                    {


                        sellChannelStock = new SellChannelStock();
                        sellChannelStock.Id = GuidUtil.New();
                        sellChannelStock.MerchId = merchId;
                        sellChannelStock.StoreId = storeId;
                        sellChannelStock.RefType = E_SellChannelRefType.Machine;
                        sellChannelStock.RefId = machineId;
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
                        sellChannelStock.MaxLimitSumQuantity = 10;
                        sellChannelStock.CreateTime = DateTime.Now;
                        sellChannelStock.Creator = GuidUtil.Empty();
                        CurrentDb.SellChannelStock.Add(sellChannelStock);



                        var sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                        sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.SlotInit;
                        sellChannelStockLog.ChangeQuantity = 0;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = "初次加载";
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);


                    }
                    else
                    {

                        if (sellChannelStock.PrdProductSkuId != productSkuId)
                        {

                            var sellChannelStockLog = new SellChannelStockLog();
                            sellChannelStockLog.Id = GuidUtil.New();
                            sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                            sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                            sellChannelStockLog.RefId = sellChannelStock.RefId;
                            sellChannelStockLog.RefType = sellChannelStock.RefType;
                            sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                            sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                            sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                            sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                            sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                            sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                            sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.SlotRemove;
                            sellChannelStockLog.ChangeQuantity = 0;
                            sellChannelStockLog.Creator = operater;
                            sellChannelStockLog.CreateTime = DateTime.Now;
                            sellChannelStockLog.RemarkByDev = "货道移除";
                            CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);

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
                            sellChannelStock.MaxLimitSumQuantity = 10;
                            sellChannelStock.MendTime = DateTime.Now;
                            sellChannelStock.Mender = GuidUtil.Empty();



                            var sellChannelStockLog2 = new SellChannelStockLog();
                            sellChannelStockLog2.Id = GuidUtil.New();
                            sellChannelStockLog2.MerchId = sellChannelStock.MerchId;
                            sellChannelStockLog2.StoreId = sellChannelStock.StoreId;
                            sellChannelStockLog2.RefId = sellChannelStock.RefId;
                            sellChannelStockLog2.RefType = sellChannelStock.RefType;
                            sellChannelStockLog2.SlotId = sellChannelStock.SlotId;
                            sellChannelStockLog2.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                            sellChannelStockLog2.SumQuantity = sellChannelStock.SumQuantity;
                            sellChannelStockLog2.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                            sellChannelStockLog2.WaitPayLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                            sellChannelStockLog2.SellQuantity = sellChannelStock.SellQuantity;
                            sellChannelStockLog2.ChangeType = E_SellChannelStockLogChangeTpye.SlotInit;
                            sellChannelStockLog2.ChangeQuantity = 0;
                            sellChannelStockLog2.Creator = operater;
                            sellChannelStockLog2.CreateTime = DateTime.Now;
                            sellChannelStockLog2.RemarkByDev = "变换货道，初次加载";
                            CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);

                        }

                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();

                    var slot = new
                    {
                        Id = slotId,
                        ProductSkuId = bizProductSku.Id,
                        ProductSkuName = bizProductSku.Name,
                        ProductSkuMainImgUrl = bizProductSku.MainImgUrl,
                        SumQuantity = sellChannelStock.SumQuantity,
                        LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity,
                        SellQuantity = sellChannelStock.SellQuantity,
                        MaxQuantity = 10,
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

        public CustomJsonResult OperateStockQuantity(string operater, OperateStockType type, string merchId, string storeId, string machineId, string slotId, string productSkuId, int quantity)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                SellChannelStock sellChannelStock = null;
                SellChannelStockLog sellChannelStockLog = null;
                switch (type)
                {
                    case OperateStockType.OrderReserve:
                        #region OrderReserve

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPayLockQuantity += quantity;
                        sellChannelStock.SellQuantity -= quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                        sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.ReserveSuccess;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("预定成功，未支付，减少可销库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                        #endregion
                        break;
                    case OperateStockType.OrderCancle:
                        #region OrderCancle

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPayLockQuantity -= quantity;
                        sellChannelStock.SellQuantity += quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                        sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderCancle;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("未支付，取消订单，恢复可销售库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                        #endregion
                        break;
                    case OperateStockType.OrderPaySuccess:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPayLockQuantity -= quantity;
                        sellChannelStock.WaitPickupLockQuantity += quantity;

                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                        sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderPaySuccess;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("成功支付，增加待取货库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                        #endregion
                        break;
                    case OperateStockType.OrderPickupOneSysMadeSignTake:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.SumQuantity -= quantity;

                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                        sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderPickupOneSysMadeSignTake;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("成功取货，减少实际库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                        #endregion
                        break;
                    case OperateStockType.OrderPickupOneManMadeSignTakeByNotComplete:
                        #region OrderPickupOneManMadeSignTakeByNotComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.SumQuantity -= quantity;

                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;


                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                        sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderPickupOneManMadeSignTakeByNotComplete;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("人为标记为取货成功，减去待取货库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case OperateStockType.OrderPickupOneManMadeSignNotTakeByComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }

                        sellChannelStock.SellQuantity += quantity;
                        sellChannelStock.SumQuantity += quantity;

                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;


                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                        sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderPickupOneManMadeSignNotTakeByComplete;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("系统已经标识出货成功，但实际未取货成功，人为标记未取货成功，恢复库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                    case OperateStockType.OrderPickupOneManMadeSignNotTakeByNotComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId));
                        }


                        sellChannelStock.SellQuantity += 1;
                        sellChannelStock.WaitPayLockQuantity -= quantity;


                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;


                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                        sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderPickupOneManMadeSignNotTakeByNotComplete;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("人为标记未取货成功，恢复库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

                        #endregion
                        break;
                }

                CurrentDb.SaveChanges();
                ts.Complete();
            }

            if (result.Result == ResultType.Success)
            {
                SendUpdateProductSkuStock(merchId, storeId, new string[] { machineId }, productSkuId);
            }

            return result;
        }

        public CustomJsonResult AdjustStockQuantity(string operater, string merchId, string storeId, string machineId, string slotId, string productSkuId, int version, int sumQuantity)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.PrdProductSkuId == productSkuId && m.SlotId == slotId).FirstOrDefault();
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

                var bizProductSku = CacheServiceFactory.ProductSku.GetInfo(merchId, productSkuId);

                sellChannelStock.SumQuantity = sumQuantity;
                sellChannelStock.SellQuantity = sumQuantity - sellChannelStock.WaitPayLockQuantity - sellChannelStock.WaitPickupLockQuantity;
                sellChannelStock.Version += 1;
                sellChannelStock.MaxLimitSumQuantity = sumQuantity;//取最近一次为置满库存

                var sellChannelStockLog = new SellChannelStockLog();
                sellChannelStockLog.Id = GuidUtil.New();
                sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                sellChannelStockLog.StoreId = sellChannelStock.StoreId;
                sellChannelStockLog.RefId = sellChannelStock.RefId;
                sellChannelStockLog.RefType = sellChannelStock.RefType;
                sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                sellChannelStockLog.WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity;
                sellChannelStockLog.WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity;
                sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.SlotEdit;
                sellChannelStockLog.ChangeQuantity = 0;
                sellChannelStockLog.Creator = operater;
                sellChannelStockLog.CreateTime = DateTime.Now;
                sellChannelStockLog.RemarkByDev = "库存调整";
                CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                CurrentDb.SaveChanges();
                ts.Complete();

                var slot = new
                {
                    Id = slotId,
                    ProductSkuId = bizProductSku.Id,
                    ProductSkuName = bizProductSku.Name,
                    ProductSkuMainImgUrl = bizProductSku.MainImgUrl,
                    SumQuantity = sellChannelStock.SumQuantity,
                    LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity,
                    SellQuantity = sellChannelStock.SellQuantity,
                    MaxQuantity = 10,
                    Version = sellChannelStock.Version
                };

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", slot);
            }

            if (result.Result == ResultType.Success)
            {
                SendUpdateProductSkuStock(merchId, storeId, new string[] { machineId }, productSkuId);
            }

            return result;

        }

        public CustomJsonResult AdjustStockSalePrice(string operater, string merchId, string storeId, string productSkuId, decimal salePrice, bool isOffSell)
        {
            var result = new CustomJsonResult();

            string[] machineIds = null;

            using (TransactionScope ts = new TransactionScope())
            {
                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.PrdProductSkuId == productSkuId).ToList();
                machineIds = sellChannelStocks.Where(m => m.RefType == E_SellChannelRefType.Machine).Select(m => m.RefId).Distinct().ToArray();
                foreach (var sellChannelStock in sellChannelStocks)
                {

                    sellChannelStock.SalePrice = salePrice;
                    sellChannelStock.IsOffSell = isOffSell;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            if (result.Result == ResultType.Success)
            {
                SendUpdateProductSkuStock(merchId, storeId, machineIds, productSkuId);
            }

            return result;
        }
    }
}
