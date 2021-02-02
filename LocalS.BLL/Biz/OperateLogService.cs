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

            var sysUserOperateLog = new SysUserOperateLog();
            sysUserOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            sysUserOperateLog.UserId = model.Operater;
            sysUserOperateLog.EventCode = model.EventCode;
            sysUserOperateLog.EventName = EventCode.GetEventName(model.EventCode);
            sysUserOperateLog.EventData = model.EventData.ToJsonString();
            sysUserOperateLog.AppId = model.AppId;
            sysUserOperateLog.Remark = model.EventRemark;
            sysUserOperateLog.CreateTime = DateTime.Now;
            sysUserOperateLog.Creator = model.Operater;
            CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
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
                var machine = BizFactory.Machine.GetOne(model.TrgerId);
                if (machine != null)
                {
                    merchName = machine.MerchName;
                    merchId = machine.MerchId;
                    trgerName = machine.MachineId;
                }
            }

            var merchOperateLog = new MerchOperateLog();
            merchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            merchOperateLog.AppId = model.AppId;
            merchOperateLog.TrgerId = model.TrgerId;
            merchOperateLog.TrgerName = trgerName;
            merchOperateLog.MerchId = merchId;
            merchOperateLog.MerchName = merchName;
            merchOperateLog.OperateUserId = model.Operater;
            merchOperateLog.OperateUserName = BizFactory.Merch.GetOperaterUserName(merchId, model.Operater);
            merchOperateLog.EventCode = model.EventCode;
            merchOperateLog.EventName = EventCode.GetEventName(model.EventCode);
            merchOperateLog.EventLevel = EventCode.GetEventLevel(model.EventCode);
            merchOperateLog.Remark = model.EventRemark;
            merchOperateLog.EventData = model.EventData.ToJsonString();
            merchOperateLog.Creator = model.Operater;
            merchOperateLog.CreateTime = DateTime.Now;
            CurrentDb.MerchOperateLog.Add(merchOperateLog);
            CurrentDb.SaveChanges();

            if (!string.IsNullOrEmpty(merchOperateLog.EventData))
            {
                if (merchOperateLog.EventData.Contains("stockChangeRecords"))
                {
                    var jToken = JToken.Parse(merchOperateLog.EventData);
                    if (jToken["stockChangeRecords"] != null)
                    {
                        var m_ChangeRecords = jToken["stockChangeRecords"].ToObject<List<RetOperateStock.ChangeRecordModel>>();

                        foreach (var m_ChangeRecord in m_ChangeRecords)
                        {
                            var r_ProductSku = CacheServiceFactory.Product.GetSkuInfo(m_ChangeRecord.MerchId, m_ChangeRecord.SkuId);
                            string storeName = BizFactory.Merch.GetStoreName(m_ChangeRecord.MerchId, m_ChangeRecord.StoreId);
                            var d_SellChannelStockLog = new SellChannelStockLog();
                            d_SellChannelStockLog.Id = IdWorker.Build(IdType.NewGuid);
                            d_SellChannelStockLog.MerchId = m_ChangeRecord.MerchId;
                            d_SellChannelStockLog.MerchName = merchName;
                            d_SellChannelStockLog.StoreId = m_ChangeRecord.StoreId;
                            d_SellChannelStockLog.StoreName = storeName;
                            d_SellChannelStockLog.ShopId = m_ChangeRecord.ShopId;
                            d_SellChannelStockLog.MachineId = m_ChangeRecord.MachineId;
                            d_SellChannelStockLog.ShopMode = m_ChangeRecord.ShopMode;
                            d_SellChannelStockLog.CabinetId = m_ChangeRecord.CabinetId;
                            d_SellChannelStockLog.SlotId = m_ChangeRecord.SlotId;
                            d_SellChannelStockLog.PrdProductSkuId = m_ChangeRecord.SkuId;
                            if (r_ProductSku != null)
                            {
                                d_SellChannelStockLog.PrdProductId = r_ProductSku.ProductId;
                                d_SellChannelStockLog.PrdProductSkuName = r_ProductSku.Name;
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
