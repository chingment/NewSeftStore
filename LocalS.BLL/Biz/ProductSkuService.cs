using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{

    public enum OperateStockType
    {
        Unknow = 0,
        MachineSlotRemove = 1,
        MachineSlotSave = 2,
        OrderReserve = 3,
        OrderCancle = 4,
        OrderPaySuccess = 5
    }

    public class ProductSkuService : BaseDbContext
    {
        public CustomJsonResult OperateStock(string operater, OperateStockType type, string merchId, string productSkuId, string machineId, string slotId, int quantity = 0)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (productSkuId == "2b239e36688e4910adffe36848921015")
                {
                    //int.Parse("dadsdd");
                }
                SellChannelStock sellChannelStock = null;
                SellChannelStockLog sellChannelStockLog = null;
                switch (type)
                {
                    case OperateStockType.MachineSlotRemove:
                        #region MachineSlotRemove
                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId).FirstOrDefault();
                        if (sellChannelStock != null)
                        {
                            CurrentDb.SellChannelStock.Remove(sellChannelStock);
                            CurrentDb.SaveChanges();
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
                        break;
                    case OperateStockType.MachineSlotSave:
                        #region MachineSlotSave
                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            var productSku = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();

                            var bizProdcutSku = CacheServiceFactory.ProductSku.GetInfo(merchId, productSkuId);
                            sellChannelStock = new SellChannelStock();
                            sellChannelStock.Id = GuidUtil.New();
                            sellChannelStock.MerchId = merchId;
                            sellChannelStock.RefType = E_SellChannelRefType.Machine;
                            sellChannelStock.RefId = machineId;
                            sellChannelStock.PrdProductId = bizProdcutSku.ProductId;
                            sellChannelStock.PrdProductSkuId = bizProdcutSku.Id;
                            sellChannelStock.SlotId = slotId;
                            sellChannelStock.SumQuantity = quantity;
                            sellChannelStock.SellQuantity = 0;
                            sellChannelStock.LockQuantity = 0;
                            sellChannelStock.IsOffSell = false;
                            sellChannelStock.SalePrice = productSku.SalePrice;
                            sellChannelStock.SalePriceByVip = productSku.SalePrice;
                            sellChannelStock.CreateTime = DateTime.Now;
                            sellChannelStock.Creator = GuidUtil.Empty();
                            CurrentDb.SellChannelStock.Add(sellChannelStock);
                            CurrentDb.SaveChanges();
                        }
                        else
                        {
                            sellChannelStock.SumQuantity = quantity;
                            sellChannelStock.SellQuantity = quantity - sellChannelStock.LockQuantity;
                            CurrentDb.SaveChanges();
                        }

                        var bizProductSku = CacheServiceFactory.ProductSku.GetInfo(merchId, productSkuId);

                        var slot2 = new
                        {
                            Id = slotId,
                            ProductSkuId = bizProductSku.Id,
                            ProductSkuName = bizProductSku.Name,
                            ProductSkuMainImgUrl = bizProductSku.MainImgUrl,
                            SumQuantity = sellChannelStock.SumQuantity,
                            LockQuantity = sellChannelStock.LockQuantity,
                            SellQuantity = sellChannelStock.SellQuantity,
                            MaxQuantity = 10
                        };

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", slot2);
                        #endregion
                        break;
                    case OperateStockType.OrderReserve:
                        #region OrderReserve

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.PrdProductSkuId == productSkuId && m.SlotId == slotId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "查找不到库存信息");
                        }

                        sellChannelStock.LockQuantity += quantity;
                        sellChannelStock.SellQuantity -= quantity;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.ReserveSuccess;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("预定成功，减少可销库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                        #endregion
                        break;
                    case OperateStockType.OrderCancle:
                        #region OrderCancle

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.PrdProductSkuId == productSkuId && m.SlotId == slotId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "取消失败");
                        }

                        sellChannelStock.LockQuantity -= quantity;
                        sellChannelStock.SellQuantity += quantity;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderCancle;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("取消订单，恢复可销库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                        #endregion
                        break;
                    case OperateStockType.OrderPaySuccess:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.PrdProductSkuId == productSkuId && m.SlotId == slotId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{}", productSkuId));
                        }

                        sellChannelStock.LockQuantity -= quantity;
                        sellChannelStock.SumQuantity -= quantity;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = sellChannelStock.MerchId;
                        sellChannelStockLog.RefId = sellChannelStock.RefId;
                        sellChannelStockLog.RefType = sellChannelStock.RefType;
                        sellChannelStockLog.SlotId = sellChannelStock.SlotId;
                        sellChannelStockLog.PrdProductSkuId = sellChannelStock.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderPaySuccess;
                        sellChannelStockLog.ChangeQuantity = quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("成功支付，减少实际库存：{0}", quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                        #endregion
                        break;
                }

                CurrentDb.SaveChanges();
                ts.Complete();


            }

            return result;
        }

        public CustomJsonResult OperateStock(string operater, string merchId, string productSkuId, string machineId, string slotId, int sellQuantity, int lockQuantity, bool isOffSell, decimal salePrice)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.PrdProductSkuId == productSkuId).ToList();

                foreach (var sellChannelStock in sellChannelStocks)
                {
                    if (sellChannelStock.SlotId == slotId)
                    {
                        sellChannelStock.LockQuantity = lockQuantity;
                        sellChannelStock.SellQuantity = sellQuantity;
                        sellChannelStock.SumQuantity = sellQuantity + lockQuantity;
                    }

                    sellChannelStock.IsOffSell = isOffSell;
                    sellChannelStock.SalePrice = salePrice;
                }

                CacheServiceFactory.ProductSku.Remove(merchId, productSkuId);

                CurrentDb.SaveChanges();

                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
            }

            return result;

        }
    }
}
