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
    public class StockSettingService : BaseDbContext
    {
        public CustomJsonResult GetCabinetSlots(string operater, RupStockSettingGetCabinetSlots rup)
        {
            var ret = new RetStockSettingGetSlots();

            var machine = BizFactory.Machine.GetOne(rup.MachineId);

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未登记");
            }

            if (string.IsNullOrEmpty(machine.MerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户");
            }

            if (string.IsNullOrEmpty(machine.StoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户店铺");
            }

            var cabinet = CurrentDb.MachineCabinet.Where(m => m.MachineId == rup.MachineId && m.CabinetId == rup.CabinetId && m.IsUse == true).FirstOrDefault();

            if (cabinet == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置对应的机柜，请联系管理员");
            }

            if (string.IsNullOrEmpty(cabinet.RowColLayout))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未识别到行列布局，请点击扫描按钮");
            }

            ret.RowColLayout = BLL.Biz.MachineService.GetLayout(cabinet.RowColLayout);
            ret.PendantRows = BLL.Biz.MachineService.GetPendantRows(cabinet.PendantRows);

            var machineStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == machine.MerchId && m.StoreId == machine.StoreId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.CabinetId == rup.CabinetId && m.SellChannelRefId == rup.MachineId).ToList();

            foreach (var item in machineStocks)
            {
                var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(item.MerchId, item.StoreId, new string[] { rup.MachineId }, item.PrdProductSkuId);

                if (bizProductSku != null)
                {
                    var slot = new SlotModel();

                    slot.Id = item.SlotId;
                    slot.ProductSkuId = bizProductSku.Id;
                    slot.ProductSkuCumCode = bizProductSku.CumCode;
                    slot.ProductSkuName = bizProductSku.Name;
                    slot.ProductSkuMainImgUrl = ImgSet.Convert_S(bizProductSku.MainImgUrl);
                    slot.ProductSkuSpecDes = bizProductSku.SpecDes;
                    slot.SumQuantity = item.SumQuantity;
                    slot.LockQuantity = item.WaitPayLockQuantity + item.WaitPickupLockQuantity;
                    slot.SellQuantity = item.SellQuantity;
                    slot.MaxQuantity = item.MaxQuantity;
                    slot.Version = item.Version;
                    ret.Slots.Add(item.SlotId, slot);
                }
            }


            MqFactory.Global.PushOperateLog(AppId.STORETERM, operater, machine.Id, "GetCabinetSlots", "查看库存");


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult SaveCabinetSlot(string operater, RopStockSettingSaveCabinetSlot rop)
        {
            var machine = BizFactory.Machine.GetOne(rop.MachineId);

            if (string.IsNullOrEmpty(rop.ProductSkuId))
            {

                var result = BizFactory.ProductSku.OperateSlot(GuidUtil.New(), OperateSlotType.MachineSlotRemove, machine.MerchId, machine.StoreId, rop.MachineId, rop.CabinetId, rop.Id, rop.ProductSkuId);

                if (result.Result == ResultType.Success)
                {
                    MqFactory.Global.PushOperateLog(AppId.STORETERM, operater, rop.MachineId, "SaveCabinetSlot", "移除货道商品成功");
                }
                else
                {
                    MqFactory.Global.PushOperateLog(AppId.STORETERM, operater, rop.MachineId, "SaveCabinetSlot", "移除货道商品失败");
                }

                return result;
            }
            else
            {
                var result = BizFactory.ProductSku.OperateSlot(GuidUtil.New(), OperateSlotType.MachineSlotSave, machine.MerchId, machine.StoreId, rop.MachineId, rop.CabinetId, rop.Id, rop.ProductSkuId);

                if (result.Result == ResultType.Success)
                {
                    result = BizFactory.ProductSku.AdjustStockQuantity(GuidUtil.New(), machine.MerchId, machine.StoreId, rop.MachineId, rop.Id, rop.ProductSkuId, rop.Version, rop.SumQuantity, rop.MaxQuantity);

                    if (result.Result == ResultType.Success)
                    {
                        MqFactory.Global.PushOperateLog(AppId.STORETERM, operater, rop.MachineId, "SaveCabinetSlot", "保存货道商品成功");
                    }
                    else
                    {
                        MqFactory.Global.PushOperateLog(AppId.STORETERM, operater, rop.MachineId, "SaveCabinetSlot", "保存货道商品失败");
                    }
                }
                else
                {
                    MqFactory.Global.PushOperateLog(AppId.STORETERM, operater, rop.MachineId, "SaveCabinetSlot", "保存货道商品失败");
                }

                return result;
            }


        }

        public CustomJsonResult SaveCabinetRowColLayout(string operater, RopStockSettingSaveCabinetRowColLayout rop)
        {
            var result = new CustomJsonResult();

            if (rop.CabinetRowColLayout == null || rop.CabinetRowColLayout.Length == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描货道结果为空，上传失败");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

                var cabinet = CurrentDb.MachineCabinet.Where(m => m.MachineId == rop.MachineId && m.CabinetId == rop.CabinetId).FirstOrDefault();

                if (cabinet == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未配置机柜，请联系管理员");
                }

                if (string.IsNullOrEmpty(cabinet.RowColLayout))
                {
                    cabinet.RowColLayout = string.Join(",", rop.CabinetRowColLayout);
                }
                else
                {
                    List<string> cabinetPendantRows = new List<string>();
                    if (!string.IsNullOrEmpty(cabinet.PendantRows))
                    {
                        cabinetPendantRows = cabinet.PendantRows.Split(',').ToList();
                    }

                    List<string> slotIds = new List<string>();
                    for (int i = 0; i < rop.CabinetRowColLayout.Length; i++)
                    {
                        int colLength = rop.CabinetRowColLayout[i];

                        for (var j = 0; j < colLength; j++)
                        {
                            string slotId = string.Format("n{0}r{1}c{2}", rop.CabinetId, i, j);
                            LogUtil.Info("All.slotId:" + slotId);
                            slotIds.Add(slotId);
                        }
                    }

                    var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == machine.CurUseMerchId && m.StoreId == machine.CurUseStoreId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == rop.MachineId).ToList();

                    var oldCabinetRowColLayout = cabinet.RowColLayout.Split(',');

                    for (int i = 0; i < oldCabinetRowColLayout.Length; i++)
                    {
                        int colLength = int.Parse(oldCabinetRowColLayout[i]);

                        for (var j = 0; j < colLength; j++)
                        {
                            string slotId = string.Format("n{0}r{1}c{2}", rop.CabinetId, i, j);

                            var sellChannelStock = sellChannelStocks.Where(m => m.SlotId == slotId).FirstOrDefault();
                            if (sellChannelStock != null)
                            {
                                int lockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;

                                if (lockQuantity > 0)
                                {
                                    if (!slotIds.Contains(slotId))
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描货道存在有上传失败");
                                    }
                                }
                            }
                        }
                    }

                    cabinet.RowColLayout = string.Join(",", rop.CabinetRowColLayout);

                    var removeSellChannelStocks = sellChannelStocks.Where(m => !slotIds.Contains(m.SlotId)).ToList();
                    foreach (var removeSellChannelStock in removeSellChannelStocks)
                    {
                        LogUtil.Info("Remove.SlotId:" + removeSellChannelStock.SlotId);

                        BizFactory.ProductSku.OperateSlot(GuidUtil.New(), OperateSlotType.MachineSlotRemove, removeSellChannelStock.MerchId, removeSellChannelStock.StoreId, rop.MachineId, removeSellChannelStock.CabinetId, removeSellChannelStock.SlotId, removeSellChannelStock.PrdProductSkuId);
                    }

                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "扫描结果上传成功");
            }

            if (result.Result == ResultType.Success)
            {
                MqFactory.Global.PushOperateLog(AppId.STORETERM, operater, rop.MachineId, "SaveCabinetRowColLayout", "保存柜子货道扫描结果成功");
            }
            else
            {
                MqFactory.Global.PushOperateLog(AppId.STORETERM, operater, rop.MachineId, "SaveCabinetRowColLayout", "保存柜子货道扫描结果失败");
            }

            return result;
        }

        //public CustomJsonResult TestPickupEventNotify(string operater, RopStockSettingTestPickupEventNotify rop)
        //{
        //    var machine = BizFactory.Machine.GetOne(rop.MachineId);

        //    string productSkuId = "";
        //    string productSkuName = "";
        //    if (!string.IsNullOrEmpty(rop.ProductSkuId))
        //    {
        //        var bizProduct = CacheServiceFactory.ProductSku.GetInfo(machine.MerchId, rop.ProductSkuId);
        //        if (bizProduct != null)
        //        {
        //            productSkuId = bizProduct.Id;
        //            productSkuName = bizProduct.Name;
        //        }
        //    }

        //    string message = "";
        //    if (rop.IsPickupComplete)
        //    {
        //        message = string.Format("商品({0}):{1},货槽:{2},当前动作({3}):{4},取货完成，用时：{5}", productSkuId, productSkuName, rop.SlotId, rop.ActionId, rop.ActionName, rop.PickupUseTime);
        //    }
        //    else
        //    {
        //        message = string.Format("商品({0}):{1},货槽:{2},当前动作({3}):{4}，状态({5})：{6}", productSkuId, productSkuName, rop.SlotId, rop.ActionId, rop.ActionName, rop.ActionStatusCode, rop.ActionStatusName);
        //    }
        //    LogUtil.Info(message);

        //    MqFactory.Global.PushOperateLog(AppId.STORETERM, operater, rop.MachineId, "TestPickup", message);
        //    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        //}

    }
}
