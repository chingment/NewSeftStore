using LocalS.BLL;
using LocalS.BLL.Biz;
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

            if (machine.CabinetRowColLayout_1 == null || machine.CabinetRowColLayout_1.Length == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未识别到行列布局，请点击扫描按钮");
            }

            ret.RowColLayout = machine.CabinetRowColLayout_1;

            var machineStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == machine.MerchId && m.StoreId == machine.StoreId && m.RefType == E_SellChannelRefType.Machine && m.RefId == rup.MachineId).ToList();

            foreach (var item in machineStocks)
            {
                var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(item.MerchId, item.StoreId, new string[] { rup.MachineId }, item.PrdProductSkuId);

                if (bizProductSku != null)
                {
                    var slot = new SlotModel();

                    slot.Id = item.SlotId;
                    slot.ProductSkuId = bizProductSku.Id;
                    slot.ProductSkuName = bizProductSku.Name;
                    slot.ProductSkuMainImgUrl = bizProductSku.MainImgUrl;
                    slot.SumQuantity = item.SumQuantity;
                    slot.LockQuantity = item.LockQuantity;
                    slot.SellQuantity = item.SellQuantity;
                    slot.MaxQuantity = 10;
                    ret.Slots.Add(item.SlotId, slot);
                }
            }


            StoreTermServiceFactory.Machine.LogAction(operater, machine.Id, "GetCabinetSlots", "查看库存");

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult SaveCabinetSlot(string operater, RopStockSettingSaveCabinetSlot rop)
        {
            var machine = BizFactory.Machine.GetOne(rop.MachineId);

            if (string.IsNullOrEmpty(rop.ProductSkuId))
            {

                var result = BizFactory.ProductSku.OperateSlot(GuidUtil.New(), OperateSlotType.MachineSlotRemove, machine.MerchId, machine.StoreId, rop.MachineId, rop.Id, rop.ProductSkuId);

                if (result.Result == ResultType.Success)
                {
                    StoreTermServiceFactory.Machine.LogAction(operater, rop.MachineId, "SaveCabinetSlot", "移除货道商品成功");
                }
                else
                {
                    StoreTermServiceFactory.Machine.LogAction(operater, rop.MachineId, "SaveCabinetSlot", "移除货道商品失败");
                }

                return result;
            }
            else
            {
                var result = BizFactory.ProductSku.OperateSlot(GuidUtil.New(), OperateSlotType.MachineSlotSave, machine.MerchId, machine.StoreId, rop.MachineId, rop.Id, rop.ProductSkuId);

                if (result.Result == ResultType.Success)
                {
                    StoreTermServiceFactory.Machine.LogAction(operater, rop.MachineId, "SaveCabinetSlot", "保存货道商品成功");
                }
                else
                {
                    StoreTermServiceFactory.Machine.LogAction(operater, rop.MachineId, "SaveCabinetSlot", "保存货道商品失败");
                }

                return result;
            }


        }

        public CustomJsonResult SaveCabinetRowColLayout(string operater, RopStockSettingSaveCabinetRowColLayout rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

                machine.CabinetRowColLayout_1 = string.Join(",", rop.CabinetRowColLayout);

                int rowLength = rop.CabinetRowColLayout.Length;

                List<string> slotIds = new List<string>();
                for (int i = 0; i < rowLength; i++)
                {
                    int colLength = rop.CabinetRowColLayout[i];

                    for (var j = 0; j < colLength; j++)
                    {
                        slotIds.Add(string.Format("n{0}r{1}c{2}", rop.CabinetId, i, j));
                    }
                }

                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == machine.CurUseMerchId && m.StoreId == machine.CurUseStoreId && m.RefType == E_SellChannelRefType.Machine && m.RefId == rop.MachineId && !slotIds.Contains(m.SlotId)).ToList();
                foreach (var sellChannelStock in sellChannelStocks)
                {
                    BizFactory.ProductSku.OperateSlot(GuidUtil.New(), OperateSlotType.MachineSlotRemove, sellChannelStock.MerchId, sellChannelStock.StoreId, rop.MachineId, sellChannelStock.SlotId, sellChannelStock.PrdProductSkuId);
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "扫描结果上传成功");
            }

            if (result.Result == ResultType.Success)
            {
                StoreTermServiceFactory.Machine.LogAction(operater, rop.MachineId, "SaveCabinetRowColLayout", "保存柜子货道扫描结果成功");
            }
            else
            {
                StoreTermServiceFactory.Machine.LogAction(operater, rop.MachineId, "SaveCabinetRowColLayout", "保存柜子货道扫描结果失败");
            }

            return result;
        }

    }
}
