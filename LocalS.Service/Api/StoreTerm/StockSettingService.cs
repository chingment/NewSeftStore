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
        public IResult GetCabinetSlots(string operater, RopStockSettingGetCabinetSlots rop)
        {
            var ret = new RetStockSettingGetSlots();

            var m_Device = BizFactory.Device.GetOne(rop.DeviceId);

            if (m_Device == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未登记");
            }

            if (string.IsNullOrEmpty(m_Device.MerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定商户");
            }

            if (string.IsNullOrEmpty(m_Device.StoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定店铺");
            }

            if (string.IsNullOrEmpty(m_Device.ShopId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定门店");
            }

            var d_DeviceCabinet = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == rop.DeviceId && m.CabinetId == rop.CabinetId && m.IsUse == true).FirstOrDefault();

            if (d_DeviceCabinet == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置对应的机柜，请联系管理员");
            }

            if (string.IsNullOrEmpty(d_DeviceCabinet.RowColLayout))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未识别到行列布局，请点击扫描按钮");
            }

            ret.RowColLayout = d_DeviceCabinet.RowColLayout;

            var d_DeviceStocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Device && m.MerchId == m_Device.MerchId && m.StoreId == m_Device.StoreId && m.ShopId == m_Device.ShopId && m.CabinetId == rop.CabinetId && m.DeviceId == rop.DeviceId).ToList();

            foreach (var d_DeviceStock in d_DeviceStocks)
            {
                var r_Sku = CacheServiceFactory.Product.GetSkuInfo(d_DeviceStock.MerchId, d_DeviceStock.SkuId);

                if (r_Sku != null)
                {
                    var m_Slot = new SlotModel();
                    m_Slot.SlotId = d_DeviceStock.SlotId;
                    m_Slot.StockId = d_DeviceStock.Id;
                    m_Slot.CabinetId = d_DeviceStock.CabinetId;
                    m_Slot.SkuId = r_Sku.Id;
                    m_Slot.SkuCumCode = r_Sku.CumCode;
                    m_Slot.SkuName = r_Sku.Name;
                    m_Slot.SkuMainImgUrl = ImgSet.Convert_S(r_Sku.MainImgUrl);
                    m_Slot.SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes);
                    m_Slot.SumQuantity = d_DeviceStock.SumQuantity;
                    m_Slot.LockQuantity = d_DeviceStock.WaitPayLockQuantity + d_DeviceStock.WaitPickupLockQuantity;
                    m_Slot.SellQuantity = d_DeviceStock.SellQuantity;
                    m_Slot.MaxQuantity = d_DeviceStock.MaxQuantity;
                    m_Slot.WarnQuantity = d_DeviceStock.WarnQuantity;
                    m_Slot.HoldQuantity = d_DeviceStock.HoldQuantity;
                    m_Slot.IsCanAlterMaxQuantity = true;
                    m_Slot.Version = d_DeviceStock.Version;
                    ret.Slots.Add(d_DeviceStock.SlotId, m_Slot);
                }
            }


            MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, rop.DeviceId, EventCode.device_saw_stock, string.Format("店铺：{0}，门店：{1}，设备：{2}，机柜：{3}，查看库存", m_Device.StoreName, m_Device.ShopName, m_Device.DeviceId, rop.CabinetId), rop);


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public IResult<RetOperateSlot> SaveCabinetSlot(string operater, RopStockSettingSaveCabinetSlot rop)
        {
            var result = new CustomJsonResult<RetOperateSlot>();

            var m_Device = BizFactory.Device.GetOne(rop.DeviceId);

            if (m_Device == null)
            {
                return new CustomJsonResult<RetOperateSlot>(ResultType.Failure, ResultCode.Failure, "设备未登记", null);
            }

            if (string.IsNullOrEmpty(m_Device.MerchId))
            {
                return new CustomJsonResult<RetOperateSlot>(ResultType.Failure, ResultCode.Failure, "设备未绑定商户", null);
            }

            if (string.IsNullOrEmpty(m_Device.StoreId))
            {
                return new CustomJsonResult<RetOperateSlot>(ResultType.Failure, ResultCode.Failure, "设备未绑定店铺", null);
            }

            if (string.IsNullOrEmpty(m_Device.ShopId))
            {
                return new CustomJsonResult<RetOperateSlot>(ResultType.Failure, ResultCode.Failure, "设备未绑定门店", null);
            }

            if (string.IsNullOrEmpty(rop.SkuId))
            {
                result = BizFactory.ProductSku.OperateSlot(operater, EventCode.device_remove_slot, m_Device.MerchId, m_Device.StoreId, m_Device.ShopId, rop.DeviceId, rop.CabinetId, rop.SlotId, rop.SkuId);
            }
            else
            {
                result = BizFactory.ProductSku.OperateSlot(operater, EventCode.device_save_slot, m_Device.MerchId, m_Device.StoreId, m_Device.ShopId, rop.DeviceId, rop.CabinetId, rop.SlotId, rop.SkuId, rop.Version, rop.SumQuantity, rop.MaxQuantity, rop.WarnQuantity, rop.HoldQuantity);
            }

            if (result.Result == ResultType.Success)
            {
                MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, rop.DeviceId, EventCode.device_save_slot, string.Format("店铺：{0}，门店：{1}，设备：{2}，机柜：{3}，货道：{4}，{5}", m_Device.StoreName, m_Device.ShopName, m_Device.DeviceId, rop.CabinetId, rop.SlotId, result.Message), new { Rop = rop, StockChangeRecords = result.Data.ChangeRecords });
            }

            return result;


        }

        public CustomJsonResult SaveCabinetRowColLayout(string operater, RopStockSettingSaveCabinetRowColLayout rop)
        {
            var result = new CustomJsonResult();

            var m_Device = BizFactory.Device.GetOne(rop.DeviceId);

            switch (rop.CabinetId)
            {
                case "dsx01n01":
                    result = SaveCabinetRowColLayoutByDS(operater, rop);
                    break;
                default:
                    result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描结果上传失败，未知道机柜类型");
                    break;
            }

            return result;
        }

        private CustomJsonResult SaveCabinetRowColLayoutByDS(string operater, RopStockSettingSaveCabinetRowColLayout rop)
        {
            var result = new CustomJsonResult();

            List<StockChangeRecordModel> m_StockChangeRecords = new List<StockChangeRecordModel>();

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

                var d_Device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();
                var d_DeviceCabinet = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == rop.DeviceId && m.CabinetId == rop.CabinetId).FirstOrDefault();
                if (d_Device == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描结果上传失败，设备未配置机柜");
                }

                CabinetRowColLayoutByDSModel oldRowColLayout = null;
                if (!string.IsNullOrEmpty(d_DeviceCabinet.RowColLayout))
                {
                    oldRowColLayout = d_DeviceCabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                    if (oldRowColLayout == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "扫描结果上传失败，解释旧布局格式错误");
                    }

                    newRowColLayout.PendantRows = oldRowColLayout.PendantRows;
                }

                if (oldRowColLayout != null)
                {
                    //旧布局代表有数据需要检测
                    if (oldRowColLayout.Rows != null)
                    {
                        int num = 1;
                        List<string> slotIds = new List<string>();
                        for (int i = 0; i < newRowColLayout.Rows.Count; i++)
                        {
                            int colLength = newRowColLayout.Rows[i];
                            for (var j = colLength - 1; j > 0; j--)
                            {
                                string slotId = string.Format("{0}-{1}-{2}", i, j, num);
                                slotIds.Add(slotId);
                                num++;
                            }
                        }

                        var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Device && m.MerchId == d_Device.CurUseMerchId && m.StoreId == d_Device.CurUseStoreId & m.ShopId == d_Device.CurUseShopId && m.DeviceId == rop.DeviceId && m.CabinetId == rop.CabinetId).ToList();

                        num = 1;
                        for (int i = 0; i < oldRowColLayout.Rows.Count; i++)
                        {
                            int colLength = oldRowColLayout.Rows[i];

                            for (var j = colLength - 1; j > 0; j--)
                            {
                                string slotId = string.Format("{0}-{1}-{2}", i, j, num);

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

                                num++;
                            }
                        }
                        var removeSellChannelStocks = sellChannelStocks.Where(m => !slotIds.Contains(m.SlotId)).ToList();
                        foreach (var removeSellChannelStock in removeSellChannelStocks)
                        {
                            var resultOperateSlot = BizFactory.ProductSku.OperateSlot(IdWorker.Build(IdType.NewGuid), EventCode.device_remove_slot, removeSellChannelStock.MerchId, removeSellChannelStock.StoreId, removeSellChannelStock.ShopId, rop.DeviceId, removeSellChannelStock.CabinetId, removeSellChannelStock.SlotId, removeSellChannelStock.SkuId);

                            if (resultOperateSlot.Result != ResultType.Success)
                            {
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "货道库存扣减失败");
                            }

                            m_StockChangeRecords.AddRange(resultOperateSlot.Data.ChangeRecords);
                        }
                    }
                }

                d_DeviceCabinet.RowColLayout = newRowColLayout.ToJsonString();
                d_DeviceCabinet.MendTime = DateTime.Now;
                d_DeviceCabinet.Mender = operater;
                CurrentDb.SaveChanges();
                ts.Complete();


                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "扫描结果上传成功", new { RowColLayout = d_DeviceCabinet.RowColLayout });

                if (result.Result == ResultType.Success)
                {
                    MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, rop.DeviceId, EventCode.vending_scan_slots, "设备扫描货道成功", new { Rop = rop, StockChangeRecords = m_StockChangeRecords });
                }
            }

            return result;
        }
    }
}
