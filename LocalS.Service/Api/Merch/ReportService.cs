using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Service.UI;
using Lumos;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class ReportService : BaseDbContext
    {
        public CustomJsonResult MachineStockInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportMachineStockInit();

            var stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();


            foreach (var store in stores)
            {
                var optionsSellChannel = new OptionNode();

                optionsSellChannel.Value = store.Id;
                optionsSellChannel.Label = store.Name;

                var storeMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.CurUseStoreId == store.Id).ToList();
                if (storeMachines.Count > 0)
                {
                    optionsSellChannel.Children = new List<OptionNode>();

                    foreach (var storeMachine in storeMachines)
                    {
                        optionsSellChannel.Children.Add(new OptionNode { Value = storeMachine.MachineId, Label = storeMachine.Name });
                    }

                    ret.OptionsSellChannels.Add(optionsSellChannel);
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult MachineStockGet(string operater, string merchId, RopReportMachineStockGet rop)
        {

            var result = new CustomJsonResult();

            if (rop.SellChannels == null || rop.SellChannels.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择机器");
            }

            List<object> olist = new List<object>();

            foreach (string[] sellChannel in rop.SellChannels)
            {
                string sellChannelRefId = sellChannel[1];
                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.SellChannelRefType == Entity.E_SellChannelRefType.Machine && m.SellChannelRefId == sellChannelRefId).ToList();

                var machineInfo = BizFactory.Machine.GetOne(sellChannelRefId);

                foreach (var sellChannelStock in sellChannelStocks)
                {
                    var productSku = CacheServiceFactory.ProductSku.GetInfo(sellChannelStock.MerchId, sellChannelStock.PrdProductSkuId);
                    if (productSku != null)
                    {
                        olist.Add(new
                        {
                            StoreName = machineInfo.StoreName,
                            MachineName = machineInfo.Name,
                            ProductSkuId = productSku.Id,
                            ProductSkuName = productSku.Name,
                            ProductSkuSpecDes = productSku.SpecDes,
                            ProductSkuCumCode = productSku.CumCode,
                            SlotId = sellChannelStock.SlotId,
                            SellQuantity = sellChannelStock.SellQuantity,
                            WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                            WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                            LockQuantity = sellChannelStock.WaitPickupLockQuantity + sellChannelStock.WaitPayLockQuantity,
                            SumQuantity = sellChannelStock.SumQuantity,
                            MaxQuantity = sellChannelStock.MaxQuantity,
                            RshQuantity = sellChannelStock.MaxQuantity - sellChannelStock.SumQuantity,
                            IsOffSell = sellChannelStock.IsOffSell
                        });
                    }
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }

        public CustomJsonResult ProductSkuDaySalesInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportProductSkuDaySalesInit();

            var stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();

            foreach (var store in stores)
            {
                ret.Stores.Add(new StoreModel { Id = store.Id, Name = store.Name });
            }

            var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).ToList();


            foreach (var merchMachine in merchMachines)
            {
                string storeName = "未绑定店铺";
                var machie = BizFactory.Machine.GetOne(merchMachine.MachineId);
                if (!string.IsNullOrEmpty(machie.StoreName))
                {
                    storeName = machie.StoreName;
                }

                ret.Machines.Add(new MachineModel { Id = merchMachine.MachineId, Name = merchMachine.Name, StoreName = storeName });
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult ProductSkuDaySalesGet(string operater, string merchId, RupReportProductSkuDaySalesGet rup)
        {

            var result = new CustomJsonResult();

            if (rup.TradeDateTimeArea == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择时间");
            }

            if (rup.TradeDateTimeArea.Length != 2)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择时间范围");
            }

            LogUtil.Info("rup.TradeDateTimeArea[0]" + rup.TradeDateTimeArea[0]);
            LogUtil.Info("rup.TradeDateTimeArea[1]" + rup.TradeDateTimeArea[1]);

            DateTime? tradeStartTime = CommonUtil.ConverToStartTime(rup.TradeDateTimeArea[0]);

            DateTime? tradeEndTime = CommonUtil.ConverToEndTime(rup.TradeDateTimeArea[1]);

            var query = (from u in CurrentDb.RptOrderDetailsChild
                         where u.MerchId == merchId
                         select new { u.StoreName, u.StoreId, u.SellChannelRefName, u.TradeTime, u.OrderSn, u.PrdProductSkuBarCode, u.PrdProductSkuCumCode, u.PrdProductSkuName, u.PrdProductSkuSpecDes, u.PrdProductSkuProducer, u.Quantity, u.SalePrice, u.TradeAmount, u.TradeType, u.PayWay });

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.StoreId == rup.StoreId);
            }


            query = query.Where(m => m.TradeTime >= tradeStartTime && m.TradeTime <= tradeEndTime);

            //var machine = BizFactory.Machine.GetOne(rup.MachineId);

            List<object> olist = new List<object>();

            var list = query.OrderByDescending(m => m.TradeTime).ToList();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    StoreName = item.StoreName,
                    SellChannelRefName = item.SellChannelRefName,
                    OrderSn = item.OrderSn,
                    TradeTime = item.TradeTime.ToUnifiedFormatDateTime(),
                    ProductSkuName = item.PrdProductSkuName,
                    ProductSkuBarCode = item.PrdProductSkuBarCode,
                    ProductSkuCumCode = item.PrdProductSkuCumCode,
                    ProductSkuSpecDes = item.PrdProductSkuSpecDes,
                    ProductSkuProducer = item.PrdProductSkuProducer,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                    TradeAmount = item.TradeAmount,
                    TradeType = BizFactory.Order.GetTradeTypeName(item.TradeType),
                    PayWay = BizFactory.Order.GetPayWayName(item.PayWay)
                });

            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }
    }
}
