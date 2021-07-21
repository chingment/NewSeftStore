using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
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
    public class ReportTable
    {
        public ReportTable()
        {

        }

        public ReportTable(string html)
        {
            this.Html = html;
        }

        public string Html
        {
            get;
            set;
        }
    }

    public class ReplenishPlanDetailModel
    {
        public string PlanId { get; set; }
        public string PlanCumCode { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public string SpuId { get; set; }
        public string SkuId { get; set; }
        public string SkuName { get; set; }
        public string SkuCumCode { get; set; }
        public string SkuSpecDes { get; set; }
        public string DeviceId { get; set; }
        public string DeviceCumCode { get; set; }
        public int PlanQuantity { get; set; }
        public int RshQuantity { get; set; }
        public string MakerName { get; set; }
        public DateTime? BuildTime { get; set; }
        public DateTime? RshTime { get; set; }
        public string RsherName { get; set; }
    }

    public class ReportService : BaseService
    {

        private List<OptionNode> GetOptionsByDevice(string merchId)
        {
            var options = new List<OptionNode>();


            var d_MerchDevices = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId).OrderByDescending(r => r.CurUseStoreId).OrderBy(m => m.IsStopUse).ToList();

            foreach (var d_MerchDevice in d_MerchDevices)
            {
                string label = "";
                if (d_MerchDevice.IsStopUse)
                {
                    label = string.Format("{0} [{1}]", d_MerchDevice.DeviceId, "已停止使用");
                }
                else
                {
                    if (string.IsNullOrEmpty(d_MerchDevice.CurUseStoreId))
                    {
                        label = string.Format("{0} [未绑定店铺]", d_MerchDevice.DeviceId);
                    }
                    else if (string.IsNullOrEmpty(d_MerchDevice.CurUseShopId))
                    {
                        label = string.Format("{0} [未绑定门店]", d_MerchDevice.DeviceId);
                    }
                    else
                    {
                        var d_Store = CurrentDb.Store.Where(m => m.Id == d_MerchDevice.CurUseStoreId).FirstOrDefault();

                        var d_Shop = CurrentDb.Shop.Where(m => m.Id == d_MerchDevice.CurUseShopId).FirstOrDefault();

                        if (d_Store != null && d_Shop != null)
                        {
                            label = string.Format("{0} [{1}/{2}]", MerchServiceFactory.Device.GetCode(d_MerchDevice.DeviceId, d_MerchDevice.CumCode), d_Store.Name, d_Shop.Name);
                        }
                        else
                        {
                            label = "未知";
                        }
                    }

                }

                var option = new OptionNode();
                option.Value = d_MerchDevice.DeviceId; ;
                option.Label = label;

                options.Add(option);
            }

            return options;
        }

        public CustomJsonResult DeviceStockRealDataInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportDeviceStockRealDataInit();
            ret.OptionsByDevice = GetOptionsByDevice(merchId);
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult DeviceStockRealDataGet(string operater, string merchId, RopReportStoreStockRealDataGet rop)
        {
            var result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.DeviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择设备");
            }

            List<object> olist = new List<object>();

            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();
            var d_Shops = CurrentDb.Shop.Where(m => m.MerchId == merchId).ToList();
            var d_MerchDevices = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId).ToList();

            var query = (from m in CurrentDb.SellChannelStock
                         where
                         m.MerchId == merchId &&
                         m.DeviceId == rop.DeviceId &&
                         m.ShopMode == E_ShopMode.Device
                         select new { m.StoreId, m.MerchId, m.SkuId, m.DeviceId, m.ShopId, m.ShopMode, m.SlotId, m.SellQuantity, m.WaitPayLockQuantity, m.WaitPickupLockQuantity, m.SumQuantity, m.MaxQuantity, m.IsOffSell });

            var d_Stocks = query.OrderBy(m => m.DeviceId).ToList();

            var dt_Stocks = (from m in d_Stocks select new { m.StoreId, m.MerchId, m.SkuId, m.DeviceId, m.ShopId, m.ShopMode, m.IsOffSell }).Distinct();

            foreach (var dt_Stock in dt_Stocks)
            {
                var r_Sku = CacheServiceFactory.Product.GetSkuInfo(dt_Stock.MerchId, dt_Stock.SkuId);

                var l_Store = d_Stores.Where(m => m.Id == dt_Stock.StoreId).FirstOrDefault();
                var l_Shop = d_Shops.Where(m => m.Id == dt_Stock.ShopId).FirstOrDefault();
                var l_Device = d_MerchDevices.Where(m => m.DeviceId == dt_Stock.DeviceId).FirstOrDefault();

                var l_Stock = d_Stocks.Where(m => m.SkuId == dt_Stock.SkuId);

                int sellQuantity = l_Stock.Sum(m => m.SellQuantity);
                int waitPayLockQuantity = l_Stock.Sum(m => m.WaitPayLockQuantity);
                int waitPickupLockQuantity = l_Stock.Sum(m => m.WaitPickupLockQuantity);
                int sumQuantity = l_Stock.Sum(m => m.SumQuantity);
                int maxQuantity = l_Stock.Sum(m => m.MaxQuantity);
                string slotIds = String.Join(",", l_Stock.Select(m => m.SlotId).ToArray());
                olist.Add(new
                {
                    StoreName = l_Store.Name,
                    ShopName = l_Shop.Name,
                    DeviceName = MerchServiceFactory.Device.GetCode(l_Device.DeviceId, l_Device.CumCode),
                    SkuId = r_Sku.Id,
                    SkuName = r_Sku.Name,
                    SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes),
                    SkuCumCode = r_Sku.CumCode,
                    SellQuantity = sellQuantity,
                    WaitPayLockQuantity = waitPayLockQuantity,
                    WaitPickupLockQuantity = waitPickupLockQuantity,
                    LockQuantity = waitPayLockQuantity + waitPickupLockQuantity,
                    SumQuantity = sumQuantity,
                    MaxQuantity = maxQuantity,
                    RshQuantity = maxQuantity - sumQuantity,
                    IsOffSell = dt_Stock.IsOffSell,
                    SlotIds = slotIds
                });

            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }

        public CustomJsonResult DeviceStockDateHisInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportDeviceStockRealDataInit();
            ret.OptionsByDevice = GetOptionsByDevice(merchId);
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult DeviceStockDateHisGet(string operater, string merchId, RopReporStoreStockDateHisGet rop)
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


            var query = (from m in CurrentDb.SellChannelStockDateHis
                         join s in CurrentDb.Store on m.StoreId equals s.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where
                         m.MerchId == merchId
                         && rop.StoreIds.Contains(m.StoreId) &&
                         m.StockDate == rop.StockDate
                         select new { m.StoreId, StoreName = tt.Name, m.MerchId, m.SkuId, m.DeviceId, m.ShopMode, m.SlotId, m.SellQuantity, m.WaitPayLockQuantity, m.WaitPickupLockQuantity, m.SumQuantity, m.MaxQuantity, m.IsOffSell });

            if (rop.ShopMode != Entity.E_ShopMode.Unknow)
            {
                query = query.Where(m => m.ShopMode == rop.ShopMode);
            }

            var sellChannelStocks = query.OrderBy(m => m.SlotId).ToList();


            foreach (var sellChannelStock in sellChannelStocks)
            {
                var r_Sku = CacheServiceFactory.Product.GetSkuInfo(sellChannelStock.MerchId, sellChannelStock.SkuId);

                string sellChannelRefName = "";
                string sellChannelRemark = "";

                if (sellChannelStock.ShopMode == Entity.E_ShopMode.Mall)
                {
                    sellChannelRefName = "线上商城";
                }
                else if (sellChannelStock.ShopMode == Entity.E_ShopMode.Device)
                {
                    sellChannelRefName = "线下设备";
                    sellChannelRemark = string.Format("设备：{0}，货道：{1}", sellChannelStock.DeviceId, sellChannelStock.SlotId);
                }

                olist.Add(new
                {
                    StoreName = sellChannelStock.StoreName,
                    SellChannelRefName = sellChannelRefName,
                    SellChannelRemark = sellChannelRemark,
                    SkuId = r_Sku.Id,
                    SkuName = r_Sku.Name,
                    SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes),
                    SkuCumCode = r_Sku.CumCode,
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

        public CustomJsonResult SkuSalesDateHisInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportSkuSalesDateHisInit();

            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.IsDelete == false).OrderByDescending(r => r.CreateTime).ToList();


            foreach (var d_Store in d_Stores)
            {
                var optionsStore = new OptionNode();

                optionsStore.Value = d_Store.Id;
                optionsStore.Label = d_Store.Name;


                ret.OptionsStores.Add(optionsStore);
            }



            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult SkuSalesDateHisGet(string operater, string merchId, RopReportSkuSalesDateHisGet rop)
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


            var query = (from u in CurrentDb.OrderSub
                         where u.MerchId == merchId && (u.PayStatus == Entity.E_PayStatus.PaySuccess)
                        && (u.PayedTime >= tradeStartTime && u.PayedTime <= tradeEndTime) &&
                        u.IsTestMode == false
                         select new { u.StoreName, u.StoreId, u.ShopName, u.ReceiveModeName, u.ReceiveMode, u.DeviceId, u.DeviceCumCode, u.PayedTime, u.OrderId, u.SkuBarCode, u.SkuCumCode, u.SkuName, u.SkuSpecDes, u.SkuProducer, u.Quantity, u.SalePrice, u.ChargeAmount, u.PayWay, u.PickupStatus });

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

            if (rop.ReceiveMode != Entity.E_ReceiveMode.Unknow)
            {
                query = query.Where(u => u.ReceiveMode == rop.ReceiveMode);
            }


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

                var receiveRemark = "";

                if (item.ReceiveMode == Entity.E_ReceiveMode.SelfTakeByDevice)
                {
                    receiveRemark = string.Format("{0},{1}", item.ShopName, MerchServiceFactory.Device.GetCode(item.DeviceId, item.DeviceCumCode));
                }


                olist.Add(new
                {
                    StoreName = item.StoreName,
                    ReceiveModeName = item.ReceiveModeName,
                    ReceiveRemark = receiveRemark,
                    OrderId = item.OrderId,
                    TradeTime = item.PayedTime.ToUnifiedFormatDateTime(),
                    SkuName = item.SkuName,
                    SkuBarCode = item.SkuBarCode,
                    SkuCumCode = item.SkuCumCode,
                    SkuSpecDes = SpecDes.GetDescribe(item.SkuSpecDes),
                    SkuProducer = item.SkuProducer,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                    TradeAmount = item.ChargeAmount,
                    PayWay = BizFactory.Order.GetPayWay(item.PayWay).Text,
                    PickupStatus = pickupStatus
                });

            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }

        public CustomJsonResult OrderSalesDateHisInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportSkuSalesDateHisInit();

            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.IsDelete == false).OrderByDescending(r => r.CreateTime).ToList();


            foreach (var d_Store in d_Stores)
            {
                var optionsStore = new OptionNode();

                optionsStore.Value = d_Store.Id;
                optionsStore.Label = d_Store.Name;


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
                         where u.MerchId == merchId && u.PayStatus == Entity.E_PayStatus.PaySuccess &&
                         u.IsTestMode == false
                         select new { u.Id, u.StoreName, u.ShopName, u.StoreId, u.DeviceId, u.DeviceCumCode, u.ReceiveMode, u.ReceiveModeName, u.PayedTime, u.Quantity, u.ChargeAmount, u.PayWay, u.PayStatus });

            query = query.Where(m => m.PayedTime >= tradeStartTime && m.PayedTime <= tradeEndTime);

            if (rop.StoreIds != null && rop.StoreIds.Count > 0)
            {
                query = query.Where(u => rop.StoreIds.Contains(u.StoreId));
            }

            if (rop.ReceiveMode != Entity.E_ReceiveMode.Unknow)
            {
                query = query.Where(u => u.ReceiveMode == rop.ReceiveMode);
            }

            List<object> olist = new List<object>();

            var list = query.OrderByDescending(m => m.PayedTime).ToList();

            foreach (var item in list)
            {
                var receiveRemark = "";

                if (item.ReceiveMode == Entity.E_ReceiveMode.SelfTakeByDevice)
                {
                    receiveRemark = string.Format("{0},{1}", item.ShopName, MerchServiceFactory.Device.GetCode(item.DeviceId, item.DeviceCumCode));
                }

                olist.Add(new
                {
                    StoreName = item.StoreName,
                    ReceiveModeName = item.ReceiveModeName,
                    ReceiveRemark = receiveRemark,
                    OrderId = item.Id,
                    TradeTime = item.PayedTime.ToUnifiedFormatDateTime(),
                    Quantity = item.Quantity,
                    TradeAmount = item.ChargeAmount,
                    PayWay = BizFactory.Order.GetPayWay(item.PayWay).Text
                });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }

        public CustomJsonResult StoreSalesDateHisInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportSkuSalesDateHisInit();

            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.IsDelete == false).OrderByDescending(r => r.CreateTime).ToList();


            foreach (var d_Store in d_Stores)
            {
                var optionsSellChannel = new OptionNode();

                optionsSellChannel.Value = d_Store.Id;
                optionsSellChannel.Label = d_Store.Name;

                var storeDevices = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.CurUseStoreId == d_Store.Id).ToList();
                if (storeDevices.Count > 0)
                {
                    optionsSellChannel.Children = new List<OptionNode>();

                    foreach (var storeDevice in storeDevices)
                    {
                        optionsSellChannel.Children.Add(new OptionNode { Value = storeDevice.DeviceId, Label = storeDevice.Name });
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

            sql.Append(" IsNull(SumReceiveMode1,0) as SumReceiveMode1, ");
            sql.Append(" IsNull(SumReceiveMode2,0) as SumReceiveMode2, ");
            sql.Append(" IsNull(SumReceiveMode3,0) as SumReceiveMode3, ");

            sql.Append(" IsNull(SumQuantity,0) as SumQuantity, ");
            sql.Append(" IsNull(SumChargeAmount,0) as SumChargeAmount, ");
            sql.Append(" IsNull(SumRefundedAmount,0) as SumRefundedAmount, ");
            sql.Append(" IsNull((SumChargeAmount-SumRefundedAmount),0) as SumAmount, ");
            sql.Append(" IsNull(PayWayByWx,0) as PayWayByWx, ");
            sql.Append(" IsNull(PayWayByZfb,0) as PayWayByZfb ");
            sql.Append(" from Store tb1 left join (  ");
            sql.Append(" select StoreId,   ");
            sql.Append(" COUNT(Id) as SumCount,  ");
            sql.Append(" SUM( CASE ExIsHappen WHEN 1 THEN 1 ELSE 0 END) as SumEx,  ");
            sql.Append(" SUM( CASE ExIsHandle WHEN 1 THEN 1 ELSE 0 END) as SumExHandle,  ");
            sql.Append(" SUM( CASE ReceiveMode WHEN 1 THEN 1 ELSE 0 END) as SumReceiveMode1,  ");
            sql.Append(" SUM( CASE ReceiveMode WHEN 2 THEN 1 ELSE 0 END) as SumReceiveMode2,  ");
            sql.Append(" SUM( CASE ReceiveMode WHEN 3 THEN 1 ELSE 0 END) as SumReceiveMode3,  ");

            sql.Append(" SUM(Quantity) as SumQuantity,  ");
            sql.Append(" SUM(ChargeAmount)as SumChargeAmount,  ");
            sql.Append(" SUM(RefundedAmount) as SumRefundedAmount ,  ");
            sql.Append(" SUM( CASE PayWay WHEN 1 THEN 1 ELSE 0 END) as PayWayByWx,  ");
            sql.Append(" SUM( CASE PayWay WHEN 2 THEN 1 ELSE 0 END) as PayWayByZfb,  ");
            sql.Append(" SUM( CASE [Status] WHEN '4000' THEN 1 ELSE 0 END) as SumComplete   ");
            sql.Append(" from [Order]  a where PayStatus=3 and IsTestMode=0 and MerchId='" + merchId + "' and PayedTime>='" + tradeStartTime + "' and PayedTime<='" + tradeEndTime + "'  group by StoreId )   ");
            sql.Append(" tb on tb1.Id=tb.StoreId  where MerchId='" + merchId + "' and tb1.IsDelete=0  order by SumChargeAmount desc  ");


            var dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToJsonObject<List<object>>();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", dtData);

            return result;

        }

        public CustomJsonResult CheckRightExport(string operater, string merchId, RopReportCheckRightExport rop)
        {

            var result = new CustomJsonResult();

            MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.export_excel, string.Format("导出报表：{0}", rop.FileName), rop);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

            return result;

        }


        public CustomJsonResult DeviceReplenishPlanInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var optionsByPlan = new List<OptionNode>();

            var d_ErpReplenishPlans = CurrentDb.ErpReplenishPlan.Where(m => m.MerchId == merchId).OrderByDescending(m => m.CreateTime).ToList();

            foreach (var d_ErpReplenishPlan in d_ErpReplenishPlans)
            {
                optionsByPlan.Add(new OptionNode { Value = d_ErpReplenishPlan.Id, Label = d_ErpReplenishPlan.CumCode });
            }


            var ret = new { OptionsByPlan = optionsByPlan };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult DeviceReplenishPlanGet(string operater, string merchId, RopReportDeviceReplenishPlanGet rop)
        {
            var result = new CustomJsonResult();

            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table id=\"abc\" class='list-tb2' cellspacing='0' cellpadding='0'>");
            sbTable.Append("{content}");
            sbTable.Append("</table>");


            #region POST
            StringBuilder sql = new StringBuilder(" select StoreId, PlanCumCode,StoreName,ShopName,DeviceId,DeviceCumCode,SkuId,SkuName,SkuCumCode,SkuSpecDes,PlanQuantity,RshQuantity,BuildTime,RshTime,RsherName,MakerName  from  ");
            sql.Append(" ErpReplenishPlanDetail  ");

            sql.Append(" where merchId='" + merchId + "' and SkuCumCode is not null ");

            if (!string.IsNullOrEmpty(rop.ShopName))
            {
                sql.Append(" and ShopName like '%" + rop.ShopName + "%' ");
            }

            if (!string.IsNullOrEmpty(rop.MakerName))
            {
                sql.Append(" and MakerName like '%" + rop.MakerName + "%' ");
            }

            if (!string.IsNullOrEmpty(rop.SkuCumCode))
            {
                sql.Append(" and SkuCumCode like '%" + rop.SkuCumCode + "%' ");
            }


            if (!string.IsNullOrEmpty(rop.PlanId))
            {
                sql.Append(" and PlanId = '" + rop.PlanId + "' ");
            }

            sql.Append(" order by PlanCumCode desc ");


            List<ReplenishPlanDetailModel> dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToList<ReplenishPlanDetailModel>(true);

            StringBuilder sbTableContent = new StringBuilder();

            var dt1 = (from m in dtData
                       select new { m.PlanCumCode, m.BuildTime, m.MakerName }).Distinct().ToList();


            for (int r = 0; r < dt1.Count; r++)
            {
                sbTableContent.Append("<tr class=\"row1\">");
                sbTableContent.Append("<td colspan=\"4\">单号：" + dt1[r].PlanCumCode + " </td>");
                sbTableContent.Append("<td colspan=\"4\">制单时间：" + dt1[r].BuildTime.ToUnifiedFormatDateTime() + " </td>");
                sbTableContent.Append("<td colspan=\"4\">制单人：" + dt1[r].MakerName + " </td>");
                sbTableContent.Append("</tr>");
                sbTableContent.Append("<tr class=\"row2\">");
                sbTableContent.Append("<td>店铺</td>");
                sbTableContent.Append("<td>商品编码</td>");
                sbTableContent.Append("<td>商品名称</td>");
                sbTableContent.Append("<td>商品规格</td>");
                sbTableContent.Append("<td>补货数</td>");
                sbTableContent.Append("<td>门店</td>");
                sbTableContent.Append("<td>总量</td>");
                sbTableContent.Append("<td>设备</td>");
                sbTableContent.Append("<td>数量</td>");
                sbTableContent.Append("<td>补货人</td>");
                sbTableContent.Append("<td>补数时间</td>");
                sbTableContent.Append("<td>补货量</td>");
                sbTableContent.Append("</tr>");

                string planCumCode = dt1[r].PlanCumCode;
                var dt2 = (from m in dtData
                           where m.PlanCumCode == planCumCode
                           group m by new
                           {
                               m.StoreName
                           }
                           into g
                           select new { g.Key, MyCount = g.Count() }).Distinct().ToList();


                for (int j = 0; j < dt2.Count; j++)
                {
                    string storeName = dt2[j].Key.StoreName;
                    string storeCount = dt2[j].MyCount.ToString();

                    var dt3 = (from m in dtData
                               where m.PlanCumCode == planCumCode
                               && m.StoreName == storeName
                               group m by new
                               {
                                   m.SkuName,
                                   m.SkuCumCode,
                                   m.SkuSpecDes
                               } into g
                               select new { g.Key, MyCount = g.Count() }).Distinct().ToList();

                    LogUtil.Info("Store.SkuCumCode.Count:" + dt3.Count);

                    for (int x = 0; x < dt3.Count; x++)
                    {
                        string sku_Name = dt3[x].Key.SkuName;
                        string sku_CumCode = dt3[x].Key.SkuCumCode;
                        string sku_SpecDes = dt3[x].Key.SkuSpecDes;
                        int sku_Count = dt3[x].MyCount;
                        int sku_Quantity = (from m in dtData
                                            where m.PlanCumCode == planCumCode
                                            && m.StoreName == storeName
                                            && m.SkuCumCode == sku_CumCode
                                            select new { m.PlanQuantity }).Sum(m => m.PlanQuantity);

                        var dt4 = (from m in dtData
                                   where m.PlanCumCode == planCumCode
                                   && m.StoreName == storeName
                                   && m.SkuCumCode == sku_CumCode
                                   group m by new
                                   {
                                       m.ShopName
                                   } into g
                                   select new { g.Key, MyCount = g.Count() }).Distinct().ToList();


                        for (int y = 0; y < dt4.Count; y++)
                        {
                            string shop_Name = dt4[y].Key.ShopName;
                            int shop_Count = dt4[y].MyCount;
                            int shop_Quantity = (from m in dtData
                                                 where m.PlanCumCode == planCumCode
                                                 && m.StoreName == storeName
                                                 && m.SkuCumCode == sku_CumCode
                                                 && m.ShopName == shop_Name
                                                 select new { m.PlanQuantity }).Sum(m => m.PlanQuantity);

                            var dt5 = (from m in dtData
                                       where m.PlanCumCode == planCumCode
                                       && m.StoreName == storeName
                                       && m.SkuCumCode == sku_CumCode
                                       && m.ShopName == shop_Name
                                       group m by new
                                       {
                                           m.PlanQuantity,
                                           m.DeviceCumCode
                                       } into g
                                       select new { g.Key }).Distinct().ToList();
                            for (int o = 0; o < dt5.Count; o++)
                            {
                                string device_CumCode = dt5[o].Key.DeviceCumCode;

                                StringBuilder sbTableContent2 = new StringBuilder();
                                sbTableContent2.Append("<tr class=\"row3\">");

                                if (x == 0 && o == 0)
                                {
                                    sbTableContent2.Append("<td rowspan=\"" + storeCount + "\">" + storeName + "</td>");
                                }

                                if (sku_Count <= 1)
                                {
                                    sbTableContent2.Append("<td >" + sku_CumCode + "</td>");
                                    sbTableContent2.Append("<td >" + sku_Name + "</td>");
                                    sbTableContent2.Append("<td >" + sku_SpecDes + "</td>");
                                    sbTableContent2.Append("<td >" + sku_Quantity + "</td>");
                                }
                                else
                                {
                                    if (y == 0 && o == 0)
                                    {
                                        sbTableContent2.Append("<td rowspan=\"" + sku_Count + "\">" + sku_CumCode + "</td>");
                                        sbTableContent2.Append("<td rowspan=\"" + sku_Count + "\">" + sku_Name + "</td>");
                                        sbTableContent2.Append("<td rowspan=\"" + sku_Count + "\">" + sku_SpecDes + "</td>");
                                        sbTableContent2.Append("<td rowspan=\"" + sku_Count + "\">" + sku_Quantity + "</td>");
                                    }

                                }

                                if (shop_Count <= 1)
                                {
                                    sbTableContent2.Append("<td >" + shop_Name + "</td>");
                                    sbTableContent2.Append("<td >" + shop_Quantity + "</td>");
                                }
                                else
                                {
                                    if (o == 0)
                                    {
                                        sbTableContent2.Append("<td rowspan=\"" + shop_Count + "\">" + shop_Name + "</td>");
                                        sbTableContent2.Append("<td rowspan=\"" + shop_Count + "\">" + shop_Quantity + "</td>");
                                    }
                                }


                                sbTableContent2.Append("<td>" + dt5[o].Key.DeviceCumCode + "</td>");
                                sbTableContent2.Append("<td>" + dt5[o].Key.PlanQuantity + "</td>");

                                var dt6 = (from m in dtData
                                           where m.PlanCumCode == planCumCode
                                           && m.StoreName == storeName
                                           && m.SkuCumCode == sku_CumCode
                                           && m.ShopName == shop_Name
                                           && m.DeviceCumCode == device_CumCode
                                           select m).FirstOrDefault();

                                string rsherName = "";
                                string rshTime = "";
                                string rshQuantity = "";
                                if (dt6 != null)
                                {
                              
                                    rshTime = dt6.RshTime.ToUnifiedFormatDateTime();
                                    if (dt6.RshTime == null)
                                    {
                                        rsherName="";
                                        rshQuantity = "";
                                    }
                                    else
                                    {
                                        rsherName = dt6.RsherName.NullToEmpty();
                                        rshQuantity = dt6.RshQuantity.ToString();
                                    }
                                }

                                sbTableContent2.Append("<td>" + rsherName + "</td>");
                                sbTableContent2.Append("<td>" + rshTime + "</td>");
                                sbTableContent2.Append("<td>" + rshQuantity + "</td>");



                                sbTableContent2.Append("</tr>");
                                sbTableContent.Append(sbTableContent2);

                            }
                        }
                    }
                }

            }

            //for (int r = 0; r < dtData.Rows.Count; r++)
            //{
            //    sbTableContent.Append("<tr>");


            //    sbTableContent.Append("<td>" + dtData.Rows[r]["PlanCumCode"].ToString().Trim() + " </td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["BuildTime"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["MakerName"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["StoreName"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["SkuCumCode"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["SkuName"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["SkuSpecDes"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["PlanQuantity"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["ShopName"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["PlanQuantity"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["DeviceCumCode"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["PlanQuantity"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["RsherName"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["RshTime"].ToString().Trim() + "</td>");
            //    sbTableContent.Append("<td>" + dtData.Rows[r]["RshQuantity"].ToString().Trim() + "</td>");


            //    sbTableContent.Append("</tr>");
            //}

            sbTable.Replace("{content}", sbTableContent.ToString());

            ReportTable reportTable = new ReportTable(sbTable.ToString());

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", reportTable);

            //if (model.Operate == Enumeration.OperateType.Serach)
            //{
            //    return Json(ResultType.Success, reportTable, "");
            //}
            //else
            //{
            //    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "业务员对应POS机报表");

            //    return Json(ResultType.Success, "");
            //}
            #endregion





            //var query = (from m in CurrentDb.ErpReplenishPlanDetail
            //             where
            //             m.MerchId == merchId
            //             select new
            //             {
            //                 m.PlanCumCode,
            //                 m.StoreName,
            //                 m.ShopName,
            //                 m.DeviceId,
            //                 m.DeviceCumCode,
            //                 m.SkuId,
            //                 m.SkuName,
            //                 m.SkuCumCode,
            //                 m.SkuSpecDes,
            //                 m.PlanQuantity,
            //                 m.RshQuantity,
            //                 m.BuildTime,
            //                 m.RshTime,
            //                 m.RsherName,
            //                 m.MakerName
            //             });

            //var d_Details = query.OrderBy(m => m.DeviceId).ToList();

            //List<object> olist = new List<object>();

            //foreach (var d_Detail in d_Details)
            //{
            //    olist.Add(new
            //    {
            //        PlanCumCode = d_Detail.PlanCumCode,
            //        StoreName = d_Detail.StoreName,
            //        ShopName = d_Detail.ShopName,
            //        DeviceId = d_Detail.DeviceId,
            //        DeviceCumCode = d_Detail.DeviceCumCode,
            //        SkuId = d_Detail.SkuId,
            //        SkuName = d_Detail.SkuName,
            //        BuildTime=d_Detail.BuildTime,
            //        SkuCumCode = d_Detail.SkuCumCode,
            //        SkuSpecDes = d_Detail.SkuSpecDes,
            //        PlanQuantity = d_Detail.PlanQuantity,
            //        RshQuantity = d_Detail.RshQuantity,
            //        RshTime = d_Detail.RshTime,
            //        RsherName = d_Detail.RsherName,
            //        MakerName=d_Detail.MakerName
            //    });

            //}


            //result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }


    }
}
