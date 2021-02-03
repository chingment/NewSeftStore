﻿using LocalS.BLL.Mq;
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
        public CustomJsonResult<RetOperateSlot> OperateSlot(string operater, string operateEvent, string merchId, string storeId, string shopId, string machineId, string cabinetId, string slotId, string productSkuId, int version = 0, int? sumQuantity = null, int? maxQuantity = null, int? warnQuantity = null, int? holdQuantity = null)
        {
            var result = new CustomJsonResult<RetOperateSlot>();
            var ret = new RetOperateSlot();
            using (TransactionScope ts = new TransactionScope())
            {
                if (operateEvent == EventCode.MachineCabinetSlotRemove)
                {
                    #region MachineCabinetSlotRemove
                    SellChannelStock d_SellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Machine && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                    if (d_SellChannelStock != null)
                    {
                        var r_ProdutSku = CacheServiceFactory.Product.GetSkuInfo(merchId, d_SellChannelStock.PrdProductSkuId);
                        int l_LockQuantity = d_SellChannelStock.WaitPayLockQuantity + d_SellChannelStock.WaitPickupLockQuantity;
                        if (l_LockQuantity > 0)
                        {
                            return new CustomJsonResult<RetOperateSlot>(ResultType.Failure, ResultCode.Failure, string.Format("货道删除失败,商品[{0}]存在有预定数量不能删除", r_ProdutSku.Name), ret);
                        }

                        CurrentDb.SellChannelStock.Remove(d_SellChannelStock);
                        CurrentDb.SaveChanges();
                        ts.Complete();
                    }

                    ret.StockId = "";
                    ret.CabinetId = cabinetId;
                    ret.SlotId = slotId;
                    ret.ProductSkuId = "";
                    ret.Name = "暂无商品";
                    ret.CumCode = "";
                    ret.SpecDes = "";
                    ret.MainImgUrl = "";
                    ret.SumQuantity = 0;
                    ret.LockQuantity = 0;
                    ret.SellQuantity = 0;
                    ret.MaxQuantity = 0;
                    ret.WarnQuantity = 0;
                    ret.HoldQuantity = 0;
                    ret.Version = 0;
                    ret.IsCanAlterMaxQuantity = true;

                    var record = new StockChangeRecordModel
                    {
                        MerchId = d_SellChannelStock.MerchId,
                        StoreId = d_SellChannelStock.StoreId,
                        ShopId = d_SellChannelStock.ShopId,
                        MachineId = d_SellChannelStock.MachineId,
                        CabinetId = d_SellChannelStock.CabinetId,
                        ShopMode = d_SellChannelStock.ShopMode,
                        SlotId = d_SellChannelStock.SlotId,
                        SkuId = d_SellChannelStock.PrdProductSkuId,
                        SellQuantity = d_SellChannelStock.SellQuantity,
                        WaitPayLockQuantity = d_SellChannelStock.WaitPayLockQuantity,
                        WaitPickupLockQuantity = d_SellChannelStock.WaitPickupLockQuantity,
                        SumQuantity = d_SellChannelStock.SumQuantity,
                        EventCode = operateEvent,
                        ChangeQuantity = d_SellChannelStock.SumQuantity
                    };

                    ret.ChangeRecords.Add(record);
                    result = new CustomJsonResult<RetOperateSlot>(ResultType.Success, ResultCode.Success, "货道删除成功", ret);
                    #endregion MachineCabinetSlotRemove
                }
                else if (operateEvent == EventCode.MachineCabinetSlotSave)
                {
                    #region MachineCabinetSlotSave
                    SellChannelStock d_SellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Machine && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId).FirstOrDefault();
                    var r_ProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);
                    if (d_SellChannelStock == null)
                    {
                        d_SellChannelStock = new SellChannelStock();
                        d_SellChannelStock.Id = IdWorker.Build(IdType.NewGuid);
                        d_SellChannelStock.ShopMode = E_ShopMode.Machine;
                        d_SellChannelStock.MerchId = merchId;
                        d_SellChannelStock.StoreId = storeId;
                        d_SellChannelStock.ShopId = shopId;
                        d_SellChannelStock.MachineId = machineId;
                        d_SellChannelStock.CabinetId = cabinetId;
                        d_SellChannelStock.SlotId = slotId;
                        d_SellChannelStock.PrdProductId = r_ProductSku.ProductId;
                        d_SellChannelStock.PrdProductSkuId = productSkuId;
                        d_SellChannelStock.WaitPayLockQuantity = 0;
                        d_SellChannelStock.WaitPickupLockQuantity = 0;
                        d_SellChannelStock.SumQuantity = sumQuantity.Value;
                        d_SellChannelStock.SellQuantity = sumQuantity.Value;
                        d_SellChannelStock.WarnQuantity = 0;
                        d_SellChannelStock.HoldQuantity = 0;
                        d_SellChannelStock.IsOffSell = false;
                        d_SellChannelStock.SalePrice = r_ProductSku.SalePrice;
                        d_SellChannelStock.Version = 0;
                        d_SellChannelStock.MaxQuantity = maxQuantity.Value;
                        d_SellChannelStock.CreateTime = DateTime.Now;
                        d_SellChannelStock.Creator = operater;
                        CurrentDb.SellChannelStock.Add(d_SellChannelStock);
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        var record = new StockChangeRecordModel
                        {
                            MerchId = d_SellChannelStock.MerchId,
                            StoreId = d_SellChannelStock.StoreId,
                            ShopId = d_SellChannelStock.ShopId,
                            MachineId = d_SellChannelStock.MachineId,
                            CabinetId = d_SellChannelStock.CabinetId,
                            ShopMode = d_SellChannelStock.ShopMode,
                            SlotId = d_SellChannelStock.SlotId,
                            SkuId = d_SellChannelStock.PrdProductSkuId,
                            SellQuantity = d_SellChannelStock.SellQuantity,
                            WaitPayLockQuantity = d_SellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = d_SellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = d_SellChannelStock.SumQuantity,
                            EventCode = operateEvent,
                            ChangeQuantity = sumQuantity.Value
                        };

                        ret.ChangeRecords.Add(record);
                    }
                    else
                    {
                        if (d_SellChannelStock.PrdProductSkuId != productSkuId)
                        {
                            var o_ProdutSku = CacheServiceFactory.Product.GetSkuInfo(merchId, d_SellChannelStock.PrdProductSkuId);
                            int l_LockQuantity = d_SellChannelStock.WaitPayLockQuantity + d_SellChannelStock.WaitPickupLockQuantity;
                            if (l_LockQuantity > 0)
                            {
                                return new CustomJsonResult<RetOperateSlot>(ResultType.Failure, ResultCode.Failure, string.Format("库存保存失败，商品[{0}]存在有预定数量不能更换商品", o_ProdutSku.Name), null);
                            }

                            var record_1 = new StockChangeRecordModel
                            {
                                MerchId = d_SellChannelStock.MerchId,
                                StoreId = d_SellChannelStock.StoreId,
                                ShopId = d_SellChannelStock.ShopId,
                                MachineId = d_SellChannelStock.MachineId,
                                CabinetId = d_SellChannelStock.CabinetId,
                                ShopMode = d_SellChannelStock.ShopMode,
                                SlotId = d_SellChannelStock.SlotId,
                                SkuId = d_SellChannelStock.PrdProductSkuId,
                                SellQuantity = d_SellChannelStock.SellQuantity,
                                WaitPayLockQuantity = d_SellChannelStock.WaitPayLockQuantity,
                                WaitPickupLockQuantity = d_SellChannelStock.WaitPickupLockQuantity,
                                SumQuantity = d_SellChannelStock.SumQuantity,
                                EventCode = EventCode.MachineCabinetSlotRemove,
                                ChangeQuantity = sumQuantity.Value
                            };

                            ret.ChangeRecords.Add(record_1);

                            d_SellChannelStock.PrdProductId = r_ProductSku.ProductId;
                            d_SellChannelStock.PrdProductSkuId = productSkuId;
                            d_SellChannelStock.SalePrice = r_ProductSku.SalePrice;
                        }

                        d_SellChannelStock.IsOffSell = false;
                        d_SellChannelStock.SumQuantity = sumQuantity.Value;
                        d_SellChannelStock.SellQuantity = sumQuantity.Value - d_SellChannelStock.WaitPayLockQuantity - d_SellChannelStock.WaitPickupLockQuantity;
                        d_SellChannelStock.Version += 1;
                        d_SellChannelStock.MaxQuantity = maxQuantity.Value;
                        d_SellChannelStock.MendTime = DateTime.Now;
                        d_SellChannelStock.Mender = operater;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        var record_2 = new StockChangeRecordModel
                        {
                            MerchId = d_SellChannelStock.MerchId,
                            StoreId = d_SellChannelStock.StoreId,
                            ShopId = d_SellChannelStock.ShopId,
                            MachineId = d_SellChannelStock.MachineId,
                            CabinetId = d_SellChannelStock.CabinetId,
                            ShopMode = d_SellChannelStock.ShopMode,
                            SlotId = d_SellChannelStock.SlotId,
                            SkuId = d_SellChannelStock.PrdProductSkuId,
                            SellQuantity = d_SellChannelStock.SellQuantity,
                            WaitPayLockQuantity = d_SellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = d_SellChannelStock.WaitPickupLockQuantity,
                            SumQuantity = d_SellChannelStock.SumQuantity,
                            EventCode = EventCode.MachineCabinetSlotSave,
                            ChangeQuantity = sumQuantity.Value
                        };

                        ret.ChangeRecords.Add(record_2);

                    }

                    ret.StockId = d_SellChannelStock.Id;
                    ret.CabinetId = cabinetId;
                    ret.SlotId = slotId;
                    ret.ProductSkuId = r_ProductSku.Id;
                    ret.Name = r_ProductSku.Name;
                    ret.CumCode = r_ProductSku.CumCode;
                    ret.MainImgUrl = ImgSet.Convert_S(r_ProductSku.MainImgUrl);
                    ret.SpecDes = SpecDes.GetDescribe(r_ProductSku.SpecDes);
                    ret.SumQuantity = d_SellChannelStock.SumQuantity;
                    ret.LockQuantity = d_SellChannelStock.WaitPayLockQuantity + d_SellChannelStock.WaitPickupLockQuantity;
                    ret.SellQuantity = d_SellChannelStock.SellQuantity;
                    ret.MaxQuantity = d_SellChannelStock.MaxQuantity;
                    ret.WarnQuantity = d_SellChannelStock.WarnQuantity;
                    ret.HoldQuantity = d_SellChannelStock.HoldQuantity;
                    ret.Version = d_SellChannelStock.Version;
                    ret.IsCanAlterMaxQuantity = true;

                    result = new CustomJsonResult<RetOperateSlot>(ResultType.Success, ResultCode.Success, "库存保存成功", ret);
                    #endregion
                }
                else
                {
                    result = new CustomJsonResult<RetOperateSlot>(ResultType.Failure, ResultCode.Failure, "库存保存失败，未知操作类型", ret);
                }

            }

            return result;
        }

        public CustomJsonResult<RetOperateStock> OperateStockQuantity(string operater, string operateEvent, E_ShopMode shopMode, string merchId, string storeId, string shopId, string machineId, string cabinetId, string slotId, string productSkuId, int quantity)
        {
            var result = new CustomJsonResult<RetOperateStock>();

            var ret = new RetOperateStock();

            SellChannelStock sellChannelStock = null;
            using (TransactionScope ts = new TransactionScope())
            {
                var r_ProductSku = CacheServiceFactory.Product.GetSkuInfo(merchId, productSkuId);
                switch (operateEvent)
                {
                    case EventCode.OrderReserveSuccess:
                        #region OrderReserve

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult<RetOperateStock>(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId), null);
                        }

                        sellChannelStock.WaitPayLockQuantity += quantity;
                        sellChannelStock.SellQuantity -= quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        result = new CustomJsonResult<RetOperateStock>(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.OrderCancle:
                        #region OrderCancle

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult<RetOperateStock>(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId), null);
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

                        result = new CustomJsonResult<RetOperateStock>(ResultType.Success, ResultCode.Success, "操作成功", ret);
                        #endregion
                        break;
                    case EventCode.OrderPaySuccess:
                        #region OrderPaySuccess

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult<RetOperateStock>(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId), null);
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


                        result = new CustomJsonResult<RetOperateStock>(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.OrderPickupOneSysMadeSignTake:
                        #region OrderPickupOneSysMadeSignTake

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult<RetOperateStock>(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId), null);
                        }

                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.SumQuantity -= quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();


                        result = new CustomJsonResult<RetOperateStock>(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.OrderPickupOneManMadeSignTakeByNotComplete:
                        #region OrderPickupOneManMadeSignTakeByNotComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult<RetOperateStock>(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId), null);
                        }

                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.SumQuantity -= quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        result = new CustomJsonResult<RetOperateStock>(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.OrderPickupOneManMadeSignNotTakeByComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete


                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult<RetOperateStock>(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId), null);
                        }

                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.SellQuantity += quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();

                        result = new CustomJsonResult<RetOperateStock>(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                    case EventCode.OrderPickupOneManMadeSignNotTakeByNotComplete:
                        #region OrderPickupOneManMadeSignNotTakeByComplete

                        sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.ShopMode == shopMode && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId && m.CabinetId == cabinetId && m.SlotId == slotId && m.PrdProductSkuId == productSkuId).FirstOrDefault();
                        if (sellChannelStock == null)
                        {
                            ts.Complete();
                            return new CustomJsonResult<RetOperateStock>(ResultType.Failure, ResultCode.Failure, string.Format("库存信息找不到:{0}", productSkuId), null);
                        }

                        sellChannelStock.SellQuantity += 1;
                        sellChannelStock.WaitPickupLockQuantity -= quantity;
                        sellChannelStock.Version += 1;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;
                        CurrentDb.SaveChanges();
                        ts.Complete();



                        result = new CustomJsonResult<RetOperateStock>(ResultType.Success, ResultCode.Success, "操作成功", ret);

                        #endregion
                        break;
                }
            }

            if (result.Result == ResultType.Success)
            {
                var record = new StockChangeRecordModel
                {
                    MerchId = sellChannelStock.MerchId,
                    StoreId = sellChannelStock.StoreId,
                    ShopId = sellChannelStock.ShopId,
                    MachineId = sellChannelStock.MachineId,
                    CabinetId = sellChannelStock.CabinetId,
                    ShopMode = sellChannelStock.ShopMode,
                    SlotId = sellChannelStock.SlotId,
                    SkuId = sellChannelStock.PrdProductSkuId,
                    SellQuantity = sellChannelStock.SellQuantity,
                    WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                    WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                    SumQuantity = sellChannelStock.SumQuantity,
                    EventCode = operateEvent,
                    ChangeQuantity = quantity
                };
                result.Data = new RetOperateStock();
                result.Data.ChangeRecords = new List<StockChangeRecordModel>();
                result.Data.ChangeRecords.Add(record);
            }

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
            return result;
        }
    }
}
