using LocalS.BLL.Mq;
using LocalS.BLL.Task;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public class OperateLogService : BaseService
    {
        public void Handle(OperateLogModel model)
        {

            var d_SysUserOperateLog = new SysUserOperateLog();
            d_SysUserOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            d_SysUserOperateLog.UserId = model.Operater;
            d_SysUserOperateLog.EventCode = model.EventCode;
            d_SysUserOperateLog.EventName = EventCode.GetEventName(model.EventCode);
            d_SysUserOperateLog.EventData = model.EventData.ToJsonString();
            d_SysUserOperateLog.AppId = model.AppId;
            d_SysUserOperateLog.Remark = model.EventRemark;
            d_SysUserOperateLog.CreateTime = DateTime.Now;
            d_SysUserOperateLog.Creator = model.Operater;
            CurrentDb.SysUserOperateLog.Add(d_SysUserOperateLog);
            CurrentDb.SaveChanges();

            string trgerName = "";
            string merchId = "";
            string merchName = "";

            if (model.AppId == AppId.MERCH)
            {
                merchId = model.TrgerId;
                merchName = BizFactory.Merch.GetMerchName(merchId);
                trgerName = merchName;
            }
            else if (model.AppId == AppId.WXMINPRAGROM)
            {
                var store = BizFactory.Store.GetOne(model.TrgerId);
                if (store != null)
                {
                    trgerName = store.Name;
                    merchId = store.MerchId;
                    merchName = store.MerchName;
                }
            }
            else if (model.AppId == AppId.STORETERM)
            {
                var l_Device = BizFactory.Device.GetOne(model.TrgerId);
                if (l_Device != null)
                {
                    merchName = l_Device.MerchName;
                    merchId = l_Device.MerchId;
                    trgerName = l_Device.DeviceId;
                }
            }

            var d_MerchOperateLog = new MerchOperateLog();
            d_MerchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            d_MerchOperateLog.AppId = model.AppId;
            d_MerchOperateLog.TrgerId = model.TrgerId;
            d_MerchOperateLog.TrgerName = trgerName;
            d_MerchOperateLog.MerchId = merchId;
            d_MerchOperateLog.MerchName = merchName;
            d_MerchOperateLog.OperateUserId = model.Operater;
            d_MerchOperateLog.OperateUserName = BizFactory.Merch.GetOperaterUserName(merchId, model.Operater);
            d_MerchOperateLog.EventCode = model.EventCode;
            d_MerchOperateLog.EventName = EventCode.GetEventName(model.EventCode);
            d_MerchOperateLog.EventLevel = EventCode.GetEventLevel(model.EventCode);
            d_MerchOperateLog.Remark = model.EventRemark;
            d_MerchOperateLog.EventData = model.EventData.ToJsonString();
            d_MerchOperateLog.Creator = model.Operater;
            d_MerchOperateLog.CreateTime = DateTime.Now;
            CurrentDb.MerchOperateLog.Add(d_MerchOperateLog);
            CurrentDb.SaveChanges();

            if (!string.IsNullOrEmpty(d_MerchOperateLog.EventData))
            {
                if (d_MerchOperateLog.EventData.Contains("stockChangeRecords"))
                {
                    var jToken = JToken.Parse(d_MerchOperateLog.EventData);
                    if (jToken["stockChangeRecords"] != null)
                    {
                        var m_ChangeRecords = jToken["stockChangeRecords"].ToObject<List<StockChangeRecordModel>>();

                        foreach (var m_ChangeRecord in m_ChangeRecords)
                        {
                            var r_Sku = CacheServiceFactory.Product.GetSkuInfo(m_ChangeRecord.MerchId, m_ChangeRecord.SkuId);
                            string storeName = BizFactory.Merch.GetStoreName(m_ChangeRecord.MerchId, m_ChangeRecord.StoreId);
                            string shopName = BizFactory.Merch.GetStoreName(m_ChangeRecord.MerchId, m_ChangeRecord.ShopId);
                            var d_SellChannelStockLog = new SellChannelStockLog();
                            d_SellChannelStockLog.Id = IdWorker.Build(IdType.NewGuid);
                            d_SellChannelStockLog.MerchId = m_ChangeRecord.MerchId;
                            d_SellChannelStockLog.MerchName = merchName;
                            d_SellChannelStockLog.StoreId = m_ChangeRecord.StoreId;
                            d_SellChannelStockLog.StoreName = storeName;
                            d_SellChannelStockLog.ShopId = m_ChangeRecord.ShopId;
                            d_SellChannelStockLog.ShopName = shopName;
                            d_SellChannelStockLog.DeviceId = m_ChangeRecord.DeviceId;
                            d_SellChannelStockLog.ShopMode = m_ChangeRecord.ShopMode;
                            d_SellChannelStockLog.CabinetId = m_ChangeRecord.CabinetId;
                            d_SellChannelStockLog.SlotId = m_ChangeRecord.SlotId;
                            d_SellChannelStockLog.SkuId = m_ChangeRecord.SkuId;
                            if (r_Sku != null)
                            {
                                d_SellChannelStockLog.SpuId = r_Sku.SpuId;
                                d_SellChannelStockLog.SkuName = r_Sku.Name;
                            }
                            d_SellChannelStockLog.SellQuantity = m_ChangeRecord.SellQuantity;
                            d_SellChannelStockLog.WaitPayLockQuantity = m_ChangeRecord.WaitPayLockQuantity;
                            d_SellChannelStockLog.WaitPickupLockQuantity = m_ChangeRecord.WaitPickupLockQuantity;
                            d_SellChannelStockLog.SumQuantity = m_ChangeRecord.SumQuantity;
                            d_SellChannelStockLog.EventCode = m_ChangeRecord.EventCode;
                            d_SellChannelStockLog.EventName = EventCode.GetEventName(m_ChangeRecord.EventCode);
                            d_SellChannelStockLog.ChangeQuantity = m_ChangeRecord.ChangeQuantity;
                            d_SellChannelStockLog.Creator = model.Operater;
                            d_SellChannelStockLog.CreateTime = DateTime.Now;
                            CurrentDb.SellChannelStockLog.Add(d_SellChannelStockLog);
                            CurrentDb.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
