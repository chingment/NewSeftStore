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
                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.SellChannelRefType == Entity.E_SellChannelRefType.Machine && m.SellChannelRefId == sellChannelRefId).OrderBy(m => m.SlotId).ToList();

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

        public CustomJsonResult ProductSkuDaySalesGet(string operater, string merchId, RopReportProductSkuDaySalesGet rop)
        {

            var result = new CustomJsonResult();

            if (rop.TradeDateTimeArea == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择时间");
            }

            if (rop.TradeDateTimeArea.Length != 2)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择时间范围");
            }

            LogUtil.Info("rup.TradeDateTimeArea[0]" + rop.TradeDateTimeArea[0]);
            LogUtil.Info("rup.TradeDateTimeArea[1]" + rop.TradeDateTimeArea[1]);

            DateTime? tradeStartTime = CommonUtil.ConverToStartTime(rop.TradeDateTimeArea[0]);

            DateTime? tradeEndTime = CommonUtil.ConverToEndTime(rop.TradeDateTimeArea[1]);


            List<string> sellChannelRefIds = new List<string>();

            if (rop.SellChannels != null)
            {
                foreach (var sellChannel in rop.SellChannels)
                {
                    if (sellChannel.Length == 2)
                    {
                        if (!string.IsNullOrEmpty(sellChannel[1]))
                        {
                            sellChannelRefIds.Add(sellChannel[1]);
                        }
                    }
                }
            }


            var query = (from u in CurrentDb.OrderDetailsChildSon
                         where u.MerchId == merchId && (u.Status != Entity.E_OrderDetailsChildSonStatus.Submitted && u.Status != Entity.E_OrderDetailsChildSonStatus.Canceled)
                         select new { u.StoreName, u.StoreId, u.SellChannelRefName, u.SellChannelRefId, u.PayedTime, u.OrderSn, u.PrdProductSkuBarCode, u.PrdProductSkuCumCode, u.PrdProductSkuName, u.PrdProductSkuSpecDes, u.PrdProductSkuProducer, u.Quantity, u.SalePrice, u.ChargeAmount, u.PayWay, u.Status });




            query = query.Where(m => m.PayedTime >= tradeStartTime && m.PayedTime <= tradeEndTime);

            if (sellChannelRefIds.Count > 0)
            {
                query = query.Where(m => sellChannelRefIds.Contains(m.SellChannelRefId));
            }

            //if (rop.PickupStatus == "1")
            //{
            //    query = query.Where(m => m.Status == Entity.E_OrderDetailsChildSonStatus.Completed || m.Status == Entity.E_OrderDetailsChildSonStatus.ExPickupSignTaked);
            //}

            //var machine = BizFactory.Machine.GetOne(rup.MachineId);

            List<object> olist = new List<object>();

            var list = query.OrderByDescending(m => m.PayedTime).ToList();

            foreach (var item in list)
            {
                string pickupStatus = "";
                if (item.Status == Entity.E_OrderDetailsChildSonStatus.Completed || item.Status == Entity.E_OrderDetailsChildSonStatus.ExPickupSignTaked)
                {
                    pickupStatus = "已取货";
                }
                else if (item.Status == Entity.E_OrderDetailsChildSonStatus.ExPickupSignUnTaked)
                {
                    pickupStatus = "未取货";
                }
                else if (item.Status == Entity.E_OrderDetailsChildSonStatus.Exception)
                {
                    pickupStatus = "取货异常待处理";
                }
                else
                {
                    pickupStatus = "待取货";
                }

                olist.Add(new
                {
                    StoreName = item.StoreName,
                    SellChannelRefName = item.SellChannelRefName,
                    OrderSn = item.OrderSn,
                    TradeTime = item.PayedTime.ToUnifiedFormatDateTime(),
                    ProductSkuName = item.PrdProductSkuName,
                    ProductSkuBarCode = item.PrdProductSkuBarCode,
                    ProductSkuCumCode = item.PrdProductSkuCumCode,
                    ProductSkuSpecDes = item.PrdProductSkuSpecDes,
                    ProductSkuProducer = item.PrdProductSkuProducer,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                    TradeAmount = item.ChargeAmount,
                    PayWay = BizFactory.Order.GetPayWayName(item.PayWay),
                    PickupStatus = pickupStatus
                });

            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }


        public CustomJsonResult OrderInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportProductSkuDaySalesInit();

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

        public CustomJsonResult OrderGet(string operater, string merchId, RopReporOrderGet rop)
        {

            var result = new CustomJsonResult();

            if (rop.TradeDateTimeArea == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择时间");
            }

            if (rop.TradeDateTimeArea.Length != 2)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择时间范围");
            }

            LogUtil.Info("rup.TradeDateTimeArea[0]" + rop.TradeDateTimeArea[0]);
            LogUtil.Info("rup.TradeDateTimeArea[1]" + rop.TradeDateTimeArea[1]);

            DateTime? tradeStartTime = CommonUtil.ConverToStartTime(rop.TradeDateTimeArea[0]);

            DateTime? tradeEndTime = CommonUtil.ConverToEndTime(rop.TradeDateTimeArea[1]);


            List<string> sellChannelRefIds = new List<string>();

            if (rop.SellChannels != null)
            {
                foreach (var sellChannel in rop.SellChannels)
                {
                    if (sellChannel.Length == 2)
                    {
                        if (!string.IsNullOrEmpty(sellChannel[1]))
                        {
                            sellChannelRefIds.Add(sellChannel[1]);
                        }
                    }
                }
            }


            var query = (from u in CurrentDb.Order
                         where u.MerchId == merchId && (u.Status == Entity.E_OrderStatus.Payed || u.Status == Entity.E_OrderStatus.Completed)
                         select new { u.StoreName, u.StoreId, u.SellChannelRefIds, u.PayedTime, u.Sn, u.Quantity, u.ChargeAmount, u.PayWay, u.Status });

            query = query.Where(m => m.PayedTime >= tradeStartTime && m.PayedTime <= tradeEndTime);

            if (sellChannelRefIds.Count > 0)
            {
                //query = query.Where(m => m.SellChannelRefIds.a(sellChannelRefIds));
            }

            List<object> olist = new List<object>();

            var list = query.OrderByDescending(m => m.PayedTime).ToList();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    StoreName = item.StoreName,
                    SellChannelRefNames = "",
                    OrderSn = item.Sn,
                    TradeTime = item.PayedTime.ToUnifiedFormatDateTime(),
                    Quantity = item.Quantity,
                    TradeAmount = item.ChargeAmount,
                    PayWay = BizFactory.Order.GetPayWayName(item.PayWay)
                });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }

    }
}
