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
    public class StockSettingService : BaseService
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
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定店铺");
            }

            if (string.IsNullOrEmpty(machine.ShopId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定门店");
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

            ret.RowColLayout = cabinet.RowColLayout;
            var machineStocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Machine && m.MerchId == machine.MerchId && m.StoreId == machine.StoreId && m.ShopId == machine.ShopId && m.CabinetId == rup.CabinetId && m.MachineId == rup.MachineId).ToList();

            foreach (var item in machineStocks)
            {
                var r_ProductSku = CacheServiceFactory.Product.GetSkuInfo(item.MerchId, item.PrdProductSkuId);

                if (r_ProductSku != null)
                {
                    var slot = new SlotModel();
                    slot.SlotId = item.SlotId;
                    slot.StockId = item.Id;
                    slot.CabinetId = item.CabinetId;
                    slot.ProductSkuId = r_ProductSku.Id;
                    slot.CumCode = r_ProductSku.CumCode;
                    slot.Name = r_ProductSku.Name;
                    slot.MainImgUrl = ImgSet.Convert_S(r_ProductSku.MainImgUrl);
                    slot.SpecDes = SpecDes.GetDescribe(r_ProductSku.SpecDes);
                    slot.SumQuantity = item.SumQuantity;
                    slot.LockQuantity = item.WaitPayLockQuantity + item.WaitPickupLockQuantity;
                    slot.SellQuantity = item.SellQuantity;
                    slot.MaxQuantity = item.MaxQuantity;
                    slot.WarnQuantity = item.WarnQuantity;
                    slot.HoldQuantity = item.HoldQuantity;
                    slot.IsCanAlterMaxQuantity = true;
                    slot.Version = item.Version;
                    ret.Slots.Add(item.SlotId, slot);
                }
            }


            MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, rup.MachineId, EventCode.MachineCabinetGetSlots, string.Format("店铺[{0}]门店[{1}]机器[{2}]机柜[{3}]，查看库存", machine.StoreName, machine.ShopName, machine.MachineId, rup.CabinetId), rup);


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult SaveCabinetSlot(string operater, RopStockSettingSaveCabinetSlot rop)
        {
            var result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(rop.MachineId);

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
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定店铺");
            }

            if (string.IsNullOrEmpty(machine.ShopId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定门店");
            }

            if (string.IsNullOrEmpty(rop.ProductSkuId))
            {
                result = BizFactory.ProductSku.OperateSlot(operater, EventCode.MachineCabinetSlotRemove, machine.MerchId, machine.StoreId, machine.ShopId, rop.MachineId, rop.CabinetId, rop.SlotId, rop.ProductSkuId);
            }
            else
            {
                result = BizFactory.ProductSku.OperateSlot(operater, EventCode.MachineCabinetSlotSave, machine.MerchId, machine.StoreId, machine.ShopId, rop.MachineId, rop.CabinetId, rop.SlotId, rop.ProductSkuId);

                if (result.Result == ResultType.Success)
                {
                    result = BizFactory.ProductSku.AdjustStockQuantity(operater, E_ShopMode.Machine, machine.MerchId, machine.StoreId, machine.ShopId, rop.MachineId, rop.CabinetId, rop.SlotId, rop.ProductSkuId, rop.Version, rop.SumQuantity, rop.MaxQuantity);
                }

            }

            MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, rop.MachineId, EventCode.MachineCabinetSaveRowColLayout, string.Format("店铺[{0}]门店[{1}]机器[{2}]机柜[{3}]货道[{4}]，{5}", machine.StoreName, machine.ShopName, machine.MachineId, rop.CabinetId, rop.SlotId, result.Message), rop);

            return result;


        }

        public CustomJsonResult SaveCabinetRowColLayout(string operater, RopStockSettingSaveCabinetRowColLayout rop)
        {
            var result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(rop.MachineId);

            switch (rop.CabinetId)
            {
                case "dsx01n01":
                    result = SaveCabinetRowColLayoutByDS(operater, rop);
                    break;
                default:
                    result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描结果上传失败，未知道机柜类型");
                    break;
            }


            MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, rop.MachineId, EventCode.MachineCabinetSaveRowColLayout, string.Format("店铺[{0}]门店[{1}]机器[{2}]机柜[{3}]，{4}", machine.StoreName, machine.ShopName, machine.MachineId, rop.CabinetId, result.Message), rop);

            return result;
        }

        private CustomJsonResult SaveCabinetRowColLayoutByDS(string operater, RopStockSettingSaveCabinetRowColLayout rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (string.IsNullOrEmpty(rop.RowColLayout))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描结果上传失败，扫描结果为空");
                }

                CabinetRowColLayoutByDSModel newRowColLayout = rop.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                if (newRowColLayout == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描结果上传失败，解释新布局格式错误");
                }

                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();
                var cabinet = CurrentDb.MachineCabinet.Where(m => m.MachineId == rop.MachineId && m.CabinetId == rop.CabinetId).FirstOrDefault();
                if (cabinet == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描结果上传失败，机器未配置机柜");
                }

                CabinetRowColLayoutByDSModel oldRowColLayout = null;
                if (!string.IsNullOrEmpty(cabinet.RowColLayout))
                {
                    oldRowColLayout = cabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                    if (oldRowColLayout == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描结果上传失败，解释旧布局格式错误");
                    }

                    newRowColLayout.PendantRows = oldRowColLayout.PendantRows;

                }


                //旧布局代表有数据需要检测
                if (oldRowColLayout.Rows != null)
                {
                    List<string> slotIds = new List<string>();
                    for (int i = 0; i < newRowColLayout.Rows.Count; i++)
                    {
                        int colLength = newRowColLayout.Rows[i];
                        for (var j = 0; j < colLength; j++)
                        {
                            string slotId = string.Format("r{0}c{1}", i, j);
                            slotIds.Add(slotId);
                        }
                    }

                    var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Machine && m.MerchId == machine.CurUseMerchId && m.StoreId == machine.CurUseStoreId & m.ShopId == machine.CurUseShopId && m.MachineId == rop.MachineId && m.CabinetId == rop.CabinetId).ToList();

                    for (int i = 0; i < oldRowColLayout.Rows.Count; i++)
                    {
                        int colLength = oldRowColLayout.Rows[i];

                        for (var j = 0; j < colLength; j++)
                        {
                            string slotId = string.Format("r{0}c{1}", i, j);

                            var sellChannelStock = sellChannelStocks.Where(m => m.SlotId == slotId).FirstOrDefault();
                            if (sellChannelStock != null)
                            {
                                int lockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;

                                if (lockQuantity > 0)
                                {
                                    if (!slotIds.Contains(slotId))
                                    {
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描结果上传失败，货道存在锁定数量");
                                    }
                                }
                            }
                        }
                    }
                    var removeSellChannelStocks = sellChannelStocks.Where(m => !slotIds.Contains(m.SlotId)).ToList();
                    foreach (var removeSellChannelStock in removeSellChannelStocks)
                    {
                        BizFactory.ProductSku.OperateSlot(IdWorker.Build(IdType.NewGuid), EventCode.MachineCabinetSlotRemove, removeSellChannelStock.MerchId, removeSellChannelStock.StoreId, removeSellChannelStock.ShopId, rop.MachineId, removeSellChannelStock.CabinetId, removeSellChannelStock.SlotId, removeSellChannelStock.PrdProductSkuId);
                    }
                }

                cabinet.RowColLayout = newRowColLayout.ToJsonString();
                cabinet.MendTime = DateTime.Now;
                cabinet.Mender = operater;
                CurrentDb.SaveChanges();
                ts.Complete();


                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "扫描结果上传成功", new { RowColLayout = cabinet.RowColLayout });
            }

            return result;
        }

    }
}
