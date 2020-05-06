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
        public CustomJsonResult MachineStockRealDataInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportMachineStockRealDataInit();

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

        public CustomJsonResult MachineStockRealDataGet(string operater, string merchId, RopReportMachineStockRealDataGet rop)
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

        public CustomJsonResult MachineStockDateHisInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportMachineStockRealDataInit();

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

        public CustomJsonResult MachineStockDateHisGet(string operater, string merchId, RopReportMachineStockDateHisGet rop)
        {

            var result = new CustomJsonResult();

            if (rop.SellChannels == null || rop.SellChannels.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择机器");
            }

            if (string.IsNullOrEmpty(rop.StockDate))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择日期");
            }

            List<object> olist = new List<object>();

            foreach (string[] sellChannel in rop.SellChannels)
            {
                string sellChannelRefId = sellChannel[1];
                var sellChannelStocks = CurrentDb.SellChannelStockDateHis.Where(m => m.MerchId == merchId && m.SellChannelRefType == Entity.E_SellChannelRefType.Machine && m.SellChannelRefId == sellChannelRefId && m.StockDate == rop.StockDate).OrderBy(m => m.SlotId).ToList();

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

        public CustomJsonResult ProductSkuSalesDateHisInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportProductSkuSalesDateHisInit();

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

        public CustomJsonResult ProductSkuSalesDateHisGet(string operater, string merchId, RopReportProductSkuSalesDateHisGet rop)
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


            var query = (from u in CurrentDb.OrderSubChildUnique
                         where u.MerchId == merchId && (u.PayStatus == Entity.E_OrderPayStatus.PaySuccess)
                         select new { u.StoreName, u.StoreId, u.SellChannelRefName, u.SellChannelRefId, u.PayedTime, u.OrderId, u.PrdProductSkuBarCode, u.PrdProductSkuCumCode, u.PrdProductSkuName, u.PrdProductSkuSpecDes, u.PrdProductSkuProducer, u.Quantity, u.SalePrice, u.ChargeAmount, u.PayWay, u.PickupStatus });




            query = query.Where(m => m.PayedTime >= tradeStartTime && m.PayedTime <= tradeEndTime);

            if (sellChannelRefIds.Count > 0)
            {
                query = query.Where(m => sellChannelRefIds.Contains(m.SellChannelRefId));
            }

            if (rop.PickupStatus == "1")
            {
                query = query.Where(m => m.PickupStatus == Entity.E_OrderPickupStatus.Payed
                || m.PickupStatus == Entity.E_OrderPickupStatus.WaitPickup
                || m.PickupStatus == Entity.E_OrderPickupStatus.SendPickupCmd
                || m.PickupStatus == Entity.E_OrderPickupStatus.Pickuping
                || m.PickupStatus == Entity.E_OrderPickupStatus.Exception);
            }
            else if (rop.PickupStatus == "2")
            {
                query = query.Where(m => m.PickupStatus == Entity.E_OrderPickupStatus.ExPickupSignUnTaked);
            }
            else if (rop.PickupStatus == "3")
            {
                query = query.Where(m => m.PickupStatus == Entity.E_OrderPickupStatus.Taked || m.PickupStatus == Entity.E_OrderPickupStatus.ExPickupSignTaked);
            }

            //var machine = BizFactory.Machine.GetOne(rup.MachineId);

            List<object> olist = new List<object>();

            var list = query.OrderByDescending(m => m.PayedTime).ToList();

            foreach (var item in list)
            {
                string pickupStatus = "";
                if (item.PickupStatus == Entity.E_OrderPickupStatus.Taked || item.PickupStatus == Entity.E_OrderPickupStatus.ExPickupSignTaked)
                {
                    pickupStatus = "已取货";
                }
                else if (item.PickupStatus == Entity.E_OrderPickupStatus.ExPickupSignUnTaked)
                {
                    pickupStatus = "未取货";
                }
                else if (item.PickupStatus == Entity.E_OrderPickupStatus.Exception)
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
                    OrderId = item.OrderId,
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

        public CustomJsonResult OrderSalesDateHisInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportProductSkuSalesDateHisInit();

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

        public CustomJsonResult OrderSalesDateHisGet(string operater, string merchId, RopReporOrderSalesDateHisGet rop)
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
                         where u.MerchId == merchId && u.PayStatus == Entity.E_OrderPayStatus.PaySuccess
                         select new { u.Id, u.StoreName, u.StoreId, u.SellChannelRefIds, u.SellChannelRefNames, u.PayedTime, u.Quantity, u.ChargeAmount, u.PayWay, u.Status });

            query = query.Where(m => m.PayedTime >= tradeStartTime && m.PayedTime <= tradeEndTime);

            if (sellChannelRefIds.Count > 0)
            {
                // query = query.Where(m => m.SellChannelRefIds.Contains(sellChannelRefIds));
            }

            List<object> olist = new List<object>();

            var list = query.OrderByDescending(m => m.PayedTime).ToList();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    StoreName = item.StoreName,
                    SellChannelRefNames = item.SellChannelRefNames,
                    OrderId = item.Id,
                    TradeTime = item.PayedTime.ToUnifiedFormatDateTime(),
                    Quantity = item.Quantity,
                    TradeAmount = item.ChargeAmount,
                    PayWay = BizFactory.Order.GetPayWayName(item.PayWay)
                });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }

        public CustomJsonResult StoreSalesDateHisInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportProductSkuSalesDateHisInit();

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

        public CustomJsonResult StoreSalesDateHisGet(string operater, string merchId, RopReporOrderSalesDateHisGet rop)
        {

            var result = new CustomJsonResult();

            //if (rop.TradeDateTimeArea == null)
            //{
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择时间");
            //}

            //if (rop.TradeDateTimeArea.Length != 2)
            //{
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择时间范围");
            //}


            StringBuilder sql = new StringBuilder(" select StoreName,SumCount as SumCount,  ");

            sql.Append(" SumComplete,(SumCount-SumComplete) as SumNoComplete, ");
            sql.Append(" SumEx,SumExHandle,(SumEx-SumExHandle) as SumExNoHandle,  ");
            sql.Append(" SumQuantity,SumChargeAmount,SumRefundAmount,(SumChargeAmount-SumRefundAmount) as SumAmount,PayWayByWx,PayWayByZfb ");
            sql.Append("  from (  ");
            sql.Append(" select StoreId,StoreName,COUNT(Id) as SumCount, ");
            sql.Append(" SUM( CASE ExIsHappen WHEN 1 THEN 1 ELSE 0 END) as SumEx,");
            sql.Append(" SUM( CASE ExIsHandle WHEN 1 THEN 1 ELSE 0 END) as SumExHandle, ");
            sql.Append(" SUM(Quantity) as SumQuantity, ");
            sql.Append(" SUM(ChargeAmount)as SumChargeAmount, ");
            sql.Append(" SUM(RefundAmount) as SumRefundAmount , ");
            sql.Append(" SUM( CASE PayWay WHEN 1 THEN 1 ELSE 0 END) as PayWayByWx, ");
            sql.Append(" SUM( CASE PayWay WHEN 2 THEN 1 ELSE 0 END) as PayWayByZfb, ");
            sql.Append(" SUM( CASE [Status] WHEN '4000' THEN 1 ELSE 0 END) as SumComplete  ");
            sql.Append("  from [Order]  a where PayStatus=3 and MerchId='" + merchId + "'   ");
            sql.Append("  group by StoreId,StoreName ) tb  order by SumChargeAmount desc  ");


            var dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToJsonObject<List<object>>();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", dtData);

            return result;

        }

    }
}
