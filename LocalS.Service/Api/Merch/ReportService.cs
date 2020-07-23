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
                var optionsStores = new OptionNode();

                optionsStores.Value = store.Id;
                optionsStores.Label = store.Name;

                //var storeMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.CurUseStoreId == store.Id).ToList();
                //if (storeMachines.Count > 0)
                //{
                //    optionsSellChannel.Children = new List<OptionNode>();

                //    foreach (var storeMachine in storeMachines)
                //    {
                //        optionsSellChannel.Children.Add(new OptionNode { Value = storeMachine.MachineId, Label = storeMachine.Name });
                //    }

                //    ret.OptionsSellChannels.Add(optionsSellChannel);
                //}

                ret.OptionsStores.Add(optionsStores);
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult MachineStockRealDataGet(string operater, string merchId, RopReportMachineStockRealDataGet rop)
        {

            var result = new CustomJsonResult();

            if (rop.StoreIds == null || rop.StoreIds.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择店铺");
            }

            List<object> olist = new List<object>();


            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && rop.StoreIds.Contains(m.StoreId) && m.SellChannelRefType == Entity.E_SellChannelRefType.Machine).OrderBy(m => m.SlotId).ToList();

            foreach (var sellChannelStock in sellChannelStocks)
            {
                var machineInfo = BizFactory.Machine.GetOne(sellChannelStock.SellChannelRefId);

                var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(sellChannelStock.MerchId, sellChannelStock.PrdProductSkuId);

                olist.Add(new
                {
                    StoreName = machineInfo.StoreName,
                    MachineName = machineInfo.Name,
                    ProductSkuId = bizProductSku.Id,
                    ProductSkuName = bizProductSku.Name,
                    ProductSkuSpecDes = SpecDes.GetDescribe(bizProductSku.SpecDes),
                    ProductSkuCumCode = bizProductSku.CumCode,
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
                var optionsStore = new OptionNode();

                optionsStore.Value = store.Id;
                optionsStore.Label = store.Name;

                //var storeMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.CurUseStoreId == store.Id).ToList();
                //if (storeMachines.Count > 0)
                //{
                //    optionsSellChannel.Children = new List<OptionNode>();

                //    foreach (var storeMachine in storeMachines)
                //    {
                //        optionsSellChannel.Children.Add(new OptionNode { Value = storeMachine.MachineId, Label = storeMachine.Name });
                //    }

                //    ret.OptionsSellChannels.Add(optionsSellChannel);
                //}

                ret.OptionsStores.Add(optionsStore);
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult MachineStockDateHisGet(string operater, string merchId, RopReportMachineStockDateHisGet rop)
        {

            var result = new CustomJsonResult();

            if (rop.StoreIds == null || rop.StoreIds.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择店铺");
            }

            if (string.IsNullOrEmpty(rop.StockDate))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择日期");
            }

            List<object> olist = new List<object>();

            var sellChannelStocks = CurrentDb.SellChannelStockDateHis.Where(m => m.MerchId == merchId && rop.StoreIds.Contains(m.StoreId) && m.StockDate == rop.StockDate && m.SellChannelRefType == Entity.E_SellChannelRefType.Machine).OrderBy(m => m.SlotId).ToList();

            foreach (var sellChannelStock in sellChannelStocks)
            {
                var machineInfo = BizFactory.Machine.GetOne(sellChannelStock.SellChannelRefId);
                var bizProductSku = CacheServiceFactory.Product.GetSkuInfo(sellChannelStock.MerchId, sellChannelStock.PrdProductSkuId);
                olist.Add(new
                {
                    StoreName = machineInfo.StoreName,
                    MachineName = machineInfo.Name,
                    ProductSkuId = bizProductSku.Id,
                    ProductSkuName = bizProductSku.Name,
                    ProductSkuSpecDes = SpecDes.GetDescribe(bizProductSku.SpecDes),
                    ProductSkuCumCode = bizProductSku.CumCode,
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
                var optionsStore = new OptionNode();

                optionsStore.Value = store.Id;
                optionsStore.Label = store.Name;

                //var storeMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.CurUseStoreId == store.Id).ToList();
                //if (storeMachines.Count > 0)
                //{
                //    optionsSellChannel.Children = new List<OptionNode>();

                //    foreach (var storeMachine in storeMachines)
                //    {
                //        optionsSellChannel.Children.Add(new OptionNode { Value = storeMachine.MachineId, Label = storeMachine.Name });
                //    }

                //    ret.OptionsSellChannels.Add(optionsSellChannel);
                //}

                ret.OptionsStores.Add(optionsStore);
            }



            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult ProductSkuSalesDateHisGet(string operater, string merchId, RopReportProductSkuSalesDateHisGet rop)
        {

            var result = new CustomJsonResult();

            if (rop.TradeDateTimeArea == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择日期");
            }

            if (rop.TradeDateTimeArea.Length != 2)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择日期");
            }


            LogUtil.Info("rup.TradeDateTimeArea[0]" + rop.TradeDateTimeArea[0]);
            LogUtil.Info("rup.TradeDateTimeArea[1]" + rop.TradeDateTimeArea[1]);

            DateTime? tradeStartTime = CommonUtil.ConverToStartTime(rop.TradeDateTimeArea[0]);

            DateTime? tradeEndTime = CommonUtil.ConverToEndTime(rop.TradeDateTimeArea[1]);


            //if ((tradeEndTime.Value - tradeStartTime.Value).Days > 60)
            //{
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "日期范围不能超过60天");
            //}


            var query = (from u in CurrentDb.OrderSubChild
                         where u.MerchId == merchId && (u.PayStatus == Entity.E_OrderPayStatus.PaySuccess)
                        && (u.PayedTime >= tradeStartTime && u.PayedTime <= tradeEndTime)
                         select new { u.StoreName, u.StoreId, u.SellChannelRefId, u.PayedTime, u.OrderId, u.PrdProductSkuBarCode, u.PrdProductSkuCumCode, u.PrdProductSkuName, u.PrdProductSkuSpecDes, u.PrdProductSkuProducer, u.Quantity, u.SalePrice, u.ChargeAmount, u.PayWay, u.PickupStatus });

            if (rop.StoreIds != null && rop.StoreIds.Count > 0)
            {
                query = query.Where(u => rop.StoreIds.Contains(u.StoreId));
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
                    SellChannelRefName = item.SellChannelRefId,
                    OrderId = item.OrderId,
                    TradeTime = item.PayedTime.ToUnifiedFormatDateTime(),
                    ProductSkuName = item.PrdProductSkuName,
                    ProductSkuBarCode = item.PrdProductSkuBarCode,
                    ProductSkuCumCode = item.PrdProductSkuCumCode,
                    ProductSkuSpecDes = SpecDes.GetDescribe(item.PrdProductSkuSpecDes),
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
                var optionsStore = new OptionNode();

                optionsStore.Value = store.Id;
                optionsStore.Label = store.Name;

                //var storeMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.CurUseStoreId == store.Id).ToList();
                //if (storeMachines.Count > 0)
                //{
                //    optionsSellChannel.Children = new List<OptionNode>();

                //    foreach (var storeMachine in storeMachines)
                //    {
                //        optionsSellChannel.Children.Add(new OptionNode { Value = storeMachine.MachineId, Label = storeMachine.Name });
                //    }

                //    ret.OptionsStores.Add(optionsSellChannel);
                //}

                ret.OptionsStores.Add(optionsStore);
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



            var query = (from u in CurrentDb.Order
                         where u.MerchId == merchId && u.PayStatus == Entity.E_OrderPayStatus.PaySuccess
                         select new { u.Id, u.StoreName, u.StoreId, u.ReceiveModeNames, u.SellChannelRefIds, u.PayedTime, u.Quantity, u.ChargeAmount, u.PayWay, u.Status });

            query = query.Where(m => m.PayedTime >= tradeStartTime && m.PayedTime <= tradeEndTime);

            if (rop.StoreIds != null && rop.StoreIds.Count > 0)
            {
                query = query.Where(u => rop.StoreIds.Contains(u.StoreId));
            }

            List<object> olist = new List<object>();

            var list = query.OrderByDescending(m => m.PayedTime).ToList();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    StoreName = item.StoreName,
                    ReceiveModeNames = item.ReceiveModeNames,
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

                    ret.OptionsStores.Add(optionsSellChannel);
                }
            }



            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult StoreSalesDateHisGet(string operater, string merchId, RopReporOrderSalesDateHisGet rop)
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

            string tradeStartTime = DateTime.Parse(CommonUtil.ConverToStartTime(rop.TradeDateTimeArea[0]).ToString()).ToString("yyyy-MM-dd HH:mm:ss");

            string tradeEndTime = DateTime.Parse(CommonUtil.ConverToEndTime(rop.TradeDateTimeArea[1]).ToString()).ToString("yyyy-MM-dd HH:mm:ss");


            StringBuilder sql = new StringBuilder(" select tb1.id as StoreId, tb1.Name as StoreName,  ");
            sql.Append(" IsNull(SumCount,0) as SumCount,");
            sql.Append(" IsNull(SumComplete,0) as SumComplete, ");
            sql.Append(" IsNull((SumCount-SumComplete),0) as SumNoComplete, ");
            sql.Append(" IsNull(SumEx,0) as SumEx,  ");
            sql.Append(" IsNull(SumExHandle,0) as SumExHandle,");
            sql.Append(" IsNull((SumEx-SumExHandle),0) as SumExNoHandle, ");
            sql.Append(" IsNull(SumQuantity,0) as SumQuantity, ");
            sql.Append(" IsNull(SumChargeAmount,0) as SumChargeAmount, ");
            sql.Append(" IsNull(SumRefundAmount,0) as SumRefundAmount, ");
            sql.Append(" IsNull((SumChargeAmount-SumRefundAmount),0) as SumAmount, ");
            sql.Append(" IsNull(PayWayByWx,0) as PayWayByWx, ");
            sql.Append(" IsNull(PayWayByZfb,0) as PayWayByZfb ");
            sql.Append(" from Store tb1 left join (  ");
            sql.Append(" select StoreId,   ");
            sql.Append(" COUNT(Id) as SumCount,  ");
            sql.Append(" SUM( CASE ExIsHappen WHEN 1 THEN 1 ELSE 0 END) as SumEx,  ");
            sql.Append(" SUM( CASE ExIsHandle WHEN 1 THEN 1 ELSE 0 END) as SumExHandle,  ");
            sql.Append(" SUM(Quantity) as SumQuantity,  ");
            sql.Append(" SUM(ChargeAmount)as SumChargeAmount,  ");
            sql.Append(" SUM(RefundAmount) as SumRefundAmount ,  ");
            sql.Append(" SUM( CASE PayWay WHEN 1 THEN 1 ELSE 0 END) as PayWayByWx,  ");
            sql.Append(" SUM( CASE PayWay WHEN 2 THEN 1 ELSE 0 END) as PayWayByZfb,  ");
            sql.Append(" SUM( CASE [Status] WHEN '4000' THEN 1 ELSE 0 END) as SumComplete   ");
            sql.Append(" from [Order]  a where PayStatus=3 and MerchId='" + merchId + "' and PayedTime>='" + tradeStartTime + "' and PayedTime<='" + tradeEndTime + "'  group by StoreId )   ");
            sql.Append(" tb on tb1.Id=tb.StoreId  where MerchId='" + merchId + "'  order by SumChargeAmount desc  ");


            var dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToJsonObject<List<object>>();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", dtData);

            return result;

        }

    }
}
