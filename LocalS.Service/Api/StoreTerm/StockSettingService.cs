using LocalS.BLL;
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

            ret.RowColLayout = cabinet.RowColLayout;
            var machineStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == machine.MerchId && m.StoreId == machine.StoreId && m.CabinetId == rup.CabinetId && m.SellChannelRefId == rup.MachineId).ToList();

            foreach (var item in machineStocks)
            {
                var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(item.MerchId, item.PrdProductSkuId);

                if (bizProductSku != null)
                {
                    var slot = new SlotModel();
                    slot.SlotId = item.SlotId;
                    slot.StockId = item.Id;
                    slot.CabinetId = item.CabinetId;
                    slot.ProductSkuId = bizProductSku.Id;
                    slot.CumCode = bizProductSku.CumCode;
                    slot.Name = bizProductSku.Name;
                    slot.MainImgUrl = ImgSet.Convert_S(bizProductSku.MainImgUrl);
                    slot.SpecDes = SpecDes.GetDescribe(bizProductSku.SpecDes);
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


            MqFactory.Global.PushEventNotify(operater, AppId.STORETERM, machine.MerchId, machine.StoreId, rup.MachineId, EventCode.MachineCabinetGetSlots, string.Format("查看机柜({0})的库存", rup.CabinetId));


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult SaveCabinetSlot(string operater, RopStockSettingSaveCabinetSlot rop)
        {
            var machine = BizFactory.Machine.GetOne(rop.MachineId);

            if (string.IsNullOrEmpty(rop.ProductSkuId))
            {
                var result = BizFactory.ProductSku.OperateSlot(operater, EventCode.MachineCabinetSlotRemove, AppId.STORETERM, machine.MerchId, machine.StoreId, rop.MachineId, rop.CabinetId, rop.SlotId, rop.ProductSkuId);
                return result;
            }
            else
            {
                var result = BizFactory.ProductSku.OperateSlot(operater, EventCode.MachineCabinetSlotSave, AppId.STORETERM, machine.MerchId, machine.StoreId, rop.MachineId, rop.CabinetId, rop.SlotId, rop.ProductSkuId);

                if (result.Result == ResultType.Success)
                {
                    result = BizFactory.ProductSku.AdjustStockQuantity(operater, AppId.STORETERM, machine.MerchId, machine.StoreId, rop.MachineId, rop.CabinetId, rop.SlotId, rop.ProductSkuId, rop.Version, rop.SumQuantity, rop.MaxQuantity);

                }

                return result;
            }


        }

        public CustomJsonResult SaveCabinetRowColLayout(string operater, RopStockSettingSaveCabinetRowColLayout rop)
        {
            var result = new CustomJsonResult();
            var machine = BizFactory.Machine.GetOne(rop.MachineId);
            if (string.IsNullOrEmpty(rop.RowColLayout))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描货道结果为空，上传失败");
            }

            switch (rop.CabinetId)
            {
                case "dsx01n01":
                    result = SaveCabinetRowColLayoutByDS(operater, rop);
                    break;
                default:
                    result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "解释柜子布局失败");
                    break;
            }

            if (result.Result == ResultType.Success)
            {
                MqFactory.Global.PushEventNotify(operater, AppId.STORETERM, machine.MerchId, machine.StoreId, rop.MachineId, EventCode.MachineCabinetSaveRowColLayout, string.Format("机柜：{0}，保存扫描结果成功", rop.CabinetId));
            }
            else
            {
                MqFactory.Global.PushEventNotify(operater, AppId.STORETERM, machine.MerchId, machine.StoreId, rop.MachineId, EventCode.MachineCabinetSaveRowColLayout, string.Format("机柜：{0}，保存扫描结果失败", rop.CabinetId));
            }

            return result;
        }

        private CustomJsonResult SaveCabinetRowColLayoutByDS(string operater, RopStockSettingSaveCabinetRowColLayout rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                CabinetRowColLayoutByDSModel newRowColLayout = rop.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                if (newRowColLayout == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，解释新布局格式错误");
                }

                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();
                var cabinet = CurrentDb.MachineCabinet.Where(m => m.MachineId == rop.MachineId && m.CabinetId == rop.CabinetId).FirstOrDefault();
                if (cabinet == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，机器未配置机柜，请联系管理员");
                }

                CabinetRowColLayoutByDSModel oldRowColLayout = null;
                if (!string.IsNullOrEmpty(cabinet.RowColLayout))
                {
                    oldRowColLayout = cabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                    if (oldRowColLayout == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，解释旧布局格式错误");
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

                    var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == machine.CurUseMerchId && m.StoreId == machine.CurUseStoreId && m.SellChannelRefId == rop.MachineId && m.CabinetId == rop.CabinetId).ToList();

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
                                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描货道存在有上传失败");
                                    }
                                }
                            }
                        }
                    }
                    var removeSellChannelStocks = sellChannelStocks.Where(m => !slotIds.Contains(m.SlotId)).ToList();
                    foreach (var removeSellChannelStock in removeSellChannelStocks)
                    {
                        BizFactory.ProductSku.OperateSlot(IdWorker.Build(IdType.NewGuid), EventCode.MachineCabinetSlotRemove, AppId.STORETERM, removeSellChannelStock.MerchId, removeSellChannelStock.StoreId, rop.MachineId, removeSellChannelStock.CabinetId, removeSellChannelStock.SlotId, removeSellChannelStock.PrdProductSkuId);
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
