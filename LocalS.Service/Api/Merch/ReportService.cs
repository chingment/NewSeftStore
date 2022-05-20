using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.BLL.UI;
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
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public int PlanRshQuantity { get; set; }
        public int RealRshQuantity { get; set; }
        public string MakerName { get; set; }
        public DateTime? BuildTime { get; set; }
        public DateTime? RshTime { get; set; }
        public string RsherName { get; set; }
    }
    public class DeviceSalesModel
    {
        public string StoreName { get; set; }
        public string ShopName { get; set; }
        public string DeviceCumCode { get; set; }
        public string SumChargeAmount { get; set; }
        public string SumCount { get; set; }
        public string PerCumPrice { get; set; }

        public string SalesDate { get; set; }
    }
    public class SkuSaleModel
    {
        public string StoreName { get; set; }
        public string ShopName { get; set; }
        public string DeviceCode { get; set; }
        public string ReceiveMode { get; set; }
        public string OrderId { get; set; }
        public string PayedTime { get; set; }
        public string SkuName { get; set; }
        public string SkuBarCode { get; set; }
        public string SkuCumCode { get; set; }
        public string SkuSpecDes { get; set; }
        public string SkuProducer { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal RefundedAmount { get; set; }
        public int RefundedQuantity { get; set; }
        public int TradeQuantity { get; set; }
        public decimal TradeAmount { get; set; }
        public string PayWay { get; set; }
        public string PayStatus { get; set; }
        public string PickupStatus { get; set; }
    }
    public class OrderSaleModel
    {
        public string StoreName { get; set; }
        public string ShopName { get; set; }
        public string ReceiveMode { get; set; }
        public string DeviceCode { get; set; }
        public string OrderId { get; set; }
        public string PayedTime { get; set; }
        public int Quantity { get; set; }
        public decimal ChargeAmount { get; set; }
        public string PayWay { get; set; }
        public string PayStatus { get; set; }
        public decimal RefundedAmount { get; set; }
        public int RefundedQuantity { get; set; }
        public int TradeQuantity { get; set; }
        public decimal TradeAmount { get; set; }
    }

    public class DeviceSkuSummaryModel
    {
        public string StoreName { get; set; }
        public string SkuName { get; set; }
        public string SkuCumCode { get; set; }
        public string SkuSpecDes { get; set; }
        public int SellQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SumQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public int RshQuantity { get; set; }

        public int SaleQuantity { get; set; }
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


        public CustomJsonResult DeviceStockSummaryInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.IsDelete == false).OrderByDescending(r => r.CreateTime).ToList();

            List<object> optionsStores = new List<object>();
            foreach (var d_Store in d_Stores)
            {
                optionsStores.Add(new { Value = d_Store.Id, Label = d_Store.Name });
            }

            var ret = new { optionsStores = optionsStores };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult<PageEntity<DeviceSkuSummaryModel>> DeviceStockSummaryGet(string operater, string merchId, RopReportDeviceSkuSummaryGet rop)
        {
            var result = new CustomJsonResult<PageEntity<DeviceSkuSummaryModel>>();

            if (rop.TradeDateArea == null)
            {
                return new CustomJsonResult<PageEntity<DeviceSkuSummaryModel>>(ResultType.Failure, ResultCode.Failure, "请选择日期", null);
            }

            if (rop.TradeDateArea.Length != 2)
            {
                return new CustomJsonResult<PageEntity<DeviceSkuSummaryModel>>(ResultType.Failure, ResultCode.Failure, "请选择日期", null);
            }

            DateTime? tradeStartTime = CommonUtil.ConverToStartTime(rop.TradeDateArea[0]);

            DateTime? tradeEndTime = CommonUtil.ConverToEndTime(rop.TradeDateArea[1]);

            LogUtil.Info("tradeStartTime:" + tradeStartTime + ",tradeEndTime:" + tradeEndTime);

            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();


            var query = from p in CurrentDb.SellChannelStock

                        where
                         p.MerchId == merchId &&
                         p.ShopMode == E_ShopMode.Device
                        group p by new
                        {
                            p.MerchId,
                            p.StoreId,
                            p.SkuId
                        }
into g
                        select new
                        {
                            g.Key,
                            SellQuantity = g.Sum(p => p.SellQuantity),
                            WaitPayLockQuantity = g.Sum(p => p.WaitPayLockQuantity),
                            WaitPickupLockQuantity = g.Sum(p => p.WaitPickupLockQuantity),
                            SumQuantity = g.Sum(p => p.SumQuantity),
                            MaxQuantity = g.Sum(p => p.MaxQuantity),
                            SaleQuantity = (from a in CurrentDb.OrderSub
                                            where
               a.ShopMode == E_ShopMode.Device
               && a.SkuId == g.Key.SkuId
               && a.MerchId == merchId
               && a.PayStatus == E_PayStatus.PaySuccess
               && a.PayedTime >= tradeStartTime
               && a.PayedTime <= tradeEndTime
                                            select new { Quantity = a.Quantity - a.RefundedQuantity }).Select(m => m.Quantity).DefaultIfEmpty(0).Sum()
                        };

            int total = query.Count();

            int pageIndex = rop.Page - 1;
            int pageSize = rop.Limit;

            query = query.OrderByDescending(r => r.Key).Skip(pageSize * (pageIndex)).Take(pageSize);


            var list = query.ToList();

            List<DeviceSkuSummaryModel> olist = new List<DeviceSkuSummaryModel>();


            foreach (var item in list)
            {
                var r_Sku = CacheServiceFactory.Product.GetSkuInfo(item.Key.MerchId, item.Key.SkuId);

                var l_Store = d_Stores.Where(m => m.Id == item.Key.StoreId).FirstOrDefault();

                olist.Add(new DeviceSkuSummaryModel
                {
                    StoreName = l_Store.Name,
                    SkuName = r_Sku.Name,
                    SkuCumCode = r_Sku.CumCode,
                    SkuSpecDes = SpecDes.GetDescribe(r_Sku.SpecDes),
                    SellQuantity = item.SellQuantity,
                    LockQuantity = item.WaitPayLockQuantity + item.WaitPickupLockQuantity,
                    SumQuantity = item.SumQuantity,
                    MaxQuantity = item.MaxQuantity,
                    RshQuantity = item.MaxQuantity - item.SumQuantity,
                    SaleQuantity = item.SaleQuantity
                });
            }

            PageEntity<DeviceSkuSummaryModel> pageEntity = new PageEntity<DeviceSkuSummaryModel> { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult<PageEntity<DeviceSkuSummaryModel>>(ResultType.Success, ResultCode.Success, "", pageEntity);

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

        public CustomJsonResult SkuSalesHisInit(string operater, string merchId)
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

        public CustomJsonResult<PageEntity<SkuSaleModel>> SkuSalesHisGet(string operater, string merchId, RopReportSkuSalesHisGet rop)
        {

            var result = new CustomJsonResult<PageEntity<SkuSaleModel>>();

            if (rop.TradeDateTimeArea == null)
            {
                return new CustomJsonResult<PageEntity<SkuSaleModel>>(ResultType.Failure, ResultCode.Failure, "请选择日期", null);
            }

            if (rop.TradeDateTimeArea.Length != 2)
            {
                return new CustomJsonResult<PageEntity<SkuSaleModel>>(ResultType.Failure, ResultCode.Failure, "请选择日期", null);
            }

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
                         select new { u.StoreName, u.StoreId, u.DeviceId, u.RefundedQuantity, u.RefundedAmount, u.PayStatus, u.ShopName, u.ReceiveModeName, u.ReceiveMode, u.DeviceCumCode, u.PayedTime, u.OrderId, u.SkuBarCode, u.SkuCumCode, u.SkuName, u.SkuSpecDes, u.SkuProducer, u.Quantity, u.SalePrice, u.ChargeAmount, u.PayWay, u.PickupStatus });

            var query_all = (from u in CurrentDb.OrderSub
                             where u.MerchId == merchId && (u.PayStatus == Entity.E_PayStatus.PaySuccess)
                            && (u.PayedTime >= tradeStartTime && u.PayedTime <= tradeEndTime) &&
                            u.IsTestMode == false
                             select new { u.StoreId, u.SkuCumCode, u.SkuName, u.PickupStatus, u.ReceiveMode, u.RefundedQuantity, u.RefundedAmount, u.Quantity, u.ChargeAmount });

            if (rop.StoreIds != null && rop.StoreIds.Count > 0)
            {
                query = query.Where(u => rop.StoreIds.Contains(u.StoreId));
                query_all = query_all.Where(u => rop.StoreIds.Contains(u.StoreId));
            }

            if (rop.PickupStatus == "1")
            {
                query = query.Where(m => m.PickupStatus == Entity.E_OrderPickupStatus.Payed
                || m.PickupStatus == Entity.E_OrderPickupStatus.WaitPickup
                || m.PickupStatus == Entity.E_OrderPickupStatus.SendPickupCmd
                || m.PickupStatus == Entity.E_OrderPickupStatus.Pickuping
                || m.PickupStatus == Entity.E_OrderPickupStatus.Exception);

                query_all = query_all.Where(m => m.PickupStatus == Entity.E_OrderPickupStatus.Payed
                || m.PickupStatus == Entity.E_OrderPickupStatus.WaitPickup
                || m.PickupStatus == Entity.E_OrderPickupStatus.SendPickupCmd
                || m.PickupStatus == Entity.E_OrderPickupStatus.Pickuping
                || m.PickupStatus == Entity.E_OrderPickupStatus.Exception);
            }
            else if (rop.PickupStatus == "2")
            {
                query = query.Where(m => m.PickupStatus == Entity.E_OrderPickupStatus.UnTaked);
                query_all = query_all.Where(m => m.PickupStatus == Entity.E_OrderPickupStatus.UnTaked);
            }
            else if (rop.PickupStatus == "3")
            {
                query = query.Where(m => m.PickupStatus == Entity.E_OrderPickupStatus.Taked);
                query_all = query_all.Where(m => m.PickupStatus == Entity.E_OrderPickupStatus.Taked);
            }

            if (rop.ReceiveMode != Entity.E_ReceiveMode.Unknow)
            {
                query = query.Where(u => u.ReceiveMode == rop.ReceiveMode);
                query_all = query_all.Where(u => u.ReceiveMode == rop.ReceiveMode);
            }

            if (!string.IsNullOrEmpty(rop.Product))
            {
                query = query.Where(u => u.SkuCumCode == rop.Product || u.SkuName.Contains(rop.Product));
                query_all = query_all.Where(u => u.SkuCumCode == rop.Product || u.SkuName.Contains(rop.Product));
            }

            int total = query.Count();

            int pageIndex = rop.Page - 1;
            int pageSize = rop.Limit;

            query = query.OrderByDescending(r => r.PayedTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<SkuSaleModel> olist = new List<SkuSaleModel>();

            foreach (var item in list)
            {
                string pickupStatus = "";
                if (item.PickupStatus == Entity.E_OrderPickupStatus.Taked)
                {
                    pickupStatus = "已取货";
                }
                else if (item.PickupStatus == Entity.E_OrderPickupStatus.UnTaked)
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

                olist.Add(new SkuSaleModel
                {
                    StoreName = item.StoreName,
                    ShopName = item.ShopName,
                    DeviceCode = MerchServiceFactory.Device.GetCode(item.DeviceId, item.DeviceCumCode),
                    ReceiveMode = item.ReceiveModeName,
                    OrderId = item.OrderId,
                    PayedTime = item.PayedTime.ToUnifiedFormatDateTime(),
                    SkuName = item.SkuName,
                    SkuBarCode = item.SkuBarCode,
                    SkuCumCode = item.SkuCumCode,
                    SkuSpecDes = SpecDes.GetDescribe(item.SkuSpecDes),
                    SkuProducer = item.SkuProducer,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                    ChargeAmount = item.ChargeAmount,
                    RefundedAmount = item.RefundedAmount,
                    RefundedQuantity = item.RefundedQuantity,
                    TradeQuantity = item.Quantity - item.RefundedQuantity,
                    TradeAmount = item.ChargeAmount - item.RefundedAmount,
                    PayWay = BizFactory.Order.GetPayWay(item.PayWay).Text,
                    PayStatus = BizFactory.Order.GetPayStatus(item.PayStatus).Text,
                    PickupStatus = pickupStatus
                });
            }

            object extend = new { };

            var items = query_all.ToList();

            int quantity = items.Sum(m => m.Quantity);
            decimal chargeAmount = items.Sum(m => m.ChargeAmount);
            int refundedQuantity = items.Sum(m => m.RefundedQuantity);
            decimal refundedAmount = items.Sum(m => m.RefundedAmount);
            int tradeQuantity = quantity - refundedQuantity;
            decimal tradeAmount = chargeAmount - refundedAmount;

            extend = new
            {
                Quantity = quantity,
                ChargeAmount = chargeAmount,
                RefundedQuantity = refundedQuantity,
                RefundedAmount = refundedAmount,
                TradeQuantity = tradeQuantity,
                TradeAmount = tradeAmount
            };


            var pageEntity = new PageEntity<SkuSaleModel> { PageSize = pageSize, Total = total, Items = olist, Extend = extend };

            result = new CustomJsonResult<PageEntity<SkuSaleModel>>(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult OrderSalesHisInit(string operater, string merchId)
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

        public CustomJsonResult<PageEntity<OrderSaleModel>> OrderSalesHisGet(string operater, string merchId, RopReporOrderSalesHisGet rop)
        {

            var result = new CustomJsonResult<PageEntity<OrderSaleModel>>();

            if (rop.TradeDateTimeArea == null)
            {
                return new CustomJsonResult<PageEntity<OrderSaleModel>>(ResultType.Failure, ResultCode.Failure, "请选择时间", null);
            }

            if (rop.TradeDateTimeArea.Length != 2)
            {
                return new CustomJsonResult<PageEntity<OrderSaleModel>>(ResultType.Failure, ResultCode.Failure, "请选择时间范围", null);
            }

            LogUtil.Info("rup.TradeDateTimeArea[0]" + rop.TradeDateTimeArea[0]);
            LogUtil.Info("rup.TradeDateTimeArea[1]" + rop.TradeDateTimeArea[1]);

            DateTime? tradeStartTime = CommonUtil.ConverToStartTime(rop.TradeDateTimeArea[0]);

            DateTime? tradeEndTime = CommonUtil.ConverToEndTime(rop.TradeDateTimeArea[1]);



            var query = (from u in CurrentDb.Order
                         where u.MerchId == merchId && u.PayStatus == Entity.E_PayStatus.PaySuccess &&
                         u.IsTestMode == false
                         select new { u.Id, u.StoreName, u.ShopName, u.RefundedAmount, u.RefundedQuantity, u.StoreId, u.DeviceId, u.DeviceCumCode, u.ReceiveMode, u.ReceiveModeName, u.PayedTime, u.Quantity, u.ChargeAmount, u.PayWay, u.PayStatus });

            query = query.Where(m => m.PayedTime >= tradeStartTime && m.PayedTime <= tradeEndTime);

            if (rop.StoreIds != null && rop.StoreIds.Count > 0)
            {
                query = query.Where(u => rop.StoreIds.Contains(u.StoreId));
            }

            if (rop.ReceiveMode != Entity.E_ReceiveMode.Unknow)
            {
                query = query.Where(u => u.ReceiveMode == rop.ReceiveMode);
            }

            int total = query.Count();

            int pageIndex = rop.Page - 1;
            int pageSize = rop.Limit;

            query = query.OrderByDescending(r => r.PayedTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<OrderSaleModel> olist = new List<OrderSaleModel>();


            foreach (var item in list)
            {
                olist.Add(new OrderSaleModel
                {
                    StoreName = item.StoreName,
                    ShopName = item.ShopName,
                    ReceiveMode = item.ReceiveModeName,
                    DeviceCode = MerchServiceFactory.Device.GetCode(item.DeviceId, item.DeviceCumCode),
                    OrderId = item.Id,
                    PayedTime = item.PayedTime.ToUnifiedFormatDateTime(),
                    Quantity = item.Quantity,
                    ChargeAmount = item.ChargeAmount,
                    PayWay = BizFactory.Order.GetPayWay(item.PayWay).Text,
                    PayStatus = BizFactory.Order.GetPayStatus(item.PayStatus).Text,
                    RefundedAmount = item.RefundedAmount,
                    RefundedQuantity = item.RefundedQuantity,
                    TradeQuantity = item.Quantity - item.RefundedQuantity,
                    TradeAmount = item.ChargeAmount - item.RefundedAmount,

                });
            }

            PageEntity<OrderSaleModel> pageEntity = new PageEntity<OrderSaleModel> { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult<PageEntity<OrderSaleModel>>(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;

        }

        public CustomJsonResult StoreSummaryInit(string operater, string merchId)
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
                        optionsSellChannel.Children.Add(new OptionNode { Value = storeDevice.DeviceId, Label = "" });
                    }

                    ret.OptionsStores.Add(optionsSellChannel);
                }
            }



            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult StoreSummaryGet(string operater, string merchId, RopReporStoreSummaryGet rop)
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
            sql.Append(" IsNull(SumRefundedQuantity,0) as SumRefundedQuantity, ");
            sql.Append(" IsNull((SumQuantity-SumRefundedQuantity),0) as sumTradeQuantity, ");
            sql.Append(" IsNull((SumChargeAmount-SumRefundedAmount),0) as sumTradeAmount, ");
            sql.Append(" IsNull(PayWayByWx,0) as PayWayByWx, ");
            sql.Append(" IsNull(PayWayByZfb,0) as PayWayByZfb ");
            sql.Append(" from Store tb1 left join (  ");
            sql.Append(" select StoreId,   ");
            sql.Append(" COUNT(Id) as SumCount,  ");
            sql.Append(" SUM( CASE ExIsHappen WHEN 1 THEN 1 ELSE 0 END) as SumEx,  ");
            sql.Append(" SUM( CASE ExIsHandle WHEN 1 THEN 1 ELSE 0 END) as SumExHandle,  ");
            sql.Append(" SUM( CASE ReceiveMode WHEN 1 THEN 1 ELSE 0 END) as SumReceiveMode1,  ");
            sql.Append(" SUM( CASE ReceiveMode WHEN 2 THEN 1 ELSE 0 END) as SumReceiveMode2,  ");
            sql.Append(" SUM( CASE ReceiveMode WHEN 4 THEN 1 ELSE 0 END) as SumReceiveMode3,  ");

            sql.Append(" SUM(Quantity) as SumQuantity,  ");
            sql.Append(" SUM(ChargeAmount)as SumChargeAmount,  ");
            sql.Append(" SUM(RefundedQuantity) as SumRefundedQuantity ,  ");
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

        public CustomJsonResult DeviceSummaryInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { });
        }

        public CustomJsonResult DeviceSummaryGet(string operater, string merchId, RopReportDeviceSummaryGet rop)
        {

            var result = new CustomJsonResult();

            if (rop.TradeDateTimeArea == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择交易时间");
            }

            if (rop.TradeDateTimeArea.Length != 2)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择交易时间");
            }

            string salesDate = "";

            if (rop.TradeDateTimeArea[0] == rop.TradeDateTimeArea[1])
            {
                salesDate = DateTime.Parse(rop.TradeDateTimeArea[0]).ToUnifiedFormatDate();
            }
            else
            {
                salesDate = DateTime.Parse(rop.TradeDateTimeArea[0]).ToUnifiedFormatDate() + "~" + DateTime.Parse(rop.TradeDateTimeArea[1]).ToUnifiedFormatDate();
            }

            string tradeStartTime = DateTime.Parse(CommonUtil.ConverToStartTime(rop.TradeDateTimeArea[0]).ToString()).ToString("yyyy-MM-dd HH:mm:ss");

            string tradeEndTime = DateTime.Parse(CommonUtil.ConverToEndTime(rop.TradeDateTimeArea[1]).ToString()).ToString("yyyy-MM-dd HH:mm:ss");

            StringBuilder sql = new StringBuilder(" select StoreName,ShopName, DeviceCumCode,SumChargeAmount,SumCount, Convert(decimal(18,2),SumChargeAmount/SumCount) as PerCumPrice  from ( ");
            sql.Append(" select StoreName,ShopName, DeviceCumCode,SUM(ChargeAmount)-Sum(RefundedAmount) as SumChargeAmount,COUNT(*) as SumCount from dbo.[Order] where PayStatus=3 ");
            sql.Append(" and IsTestMode=0 and MerchId='" + merchId + "' and PayedTime>='" + tradeStartTime + "' and PayedTime<='" + tradeEndTime + "' ");
            sql.Append(" group by  StoreName,ShopName,DeviceCumCode) tb ");


            var dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0];

            var d_MerchDevices = (from u in CurrentDb.MerchDevice where u.MerchId == merchId && u.CurUseStoreId != null && u.CurUseShopId != null select new { u.DeviceId, u.CumCode, u.CurUseStoreId, u.CurUseShopId }).ToList();


            List<DeviceSalesModel> sales1 = new List<DeviceSalesModel>();

            foreach (var d_MerchDevice in d_MerchDevices)
            {
                var d_Store = CurrentDb.Store.Where(m => m.Id == d_MerchDevice.CurUseStoreId).FirstOrDefault();
                var d_Shop = CurrentDb.Shop.Where(m => m.Id == d_MerchDevice.CurUseShopId).FirstOrDefault();
                sales1.Add(new DeviceSalesModel
                {
                    StoreName = d_Store.Name,
                    ShopName = d_Shop.Name,
                    DeviceCumCode = MerchServiceFactory.Device.GetCode(d_MerchDevice.DeviceId, d_MerchDevice.CumCode),
                    SumChargeAmount = "0",
                    SumCount = "0",
                    PerCumPrice = "0",
                    SalesDate = salesDate
                });
            }


            List<DeviceSalesModel> sales2 = new List<DeviceSalesModel>();

            for (var i = 0; i < dtData.Rows.Count; i++)
            {
                string StoreName = dtData.Rows[i]["StoreName"].ToString();
                string ShopName = dtData.Rows[i]["ShopName"].ToString();
                string DeviceCumCode = dtData.Rows[i]["DeviceCumCode"].ToString();
                string SumChargeAmount = dtData.Rows[i]["SumChargeAmount"].ToString();
                string SumCount = dtData.Rows[i]["SumCount"].ToString();
                string PerCumPrice = dtData.Rows[i]["PerCumPrice"].ToString();

                sales2.Add(new DeviceSalesModel
                {
                    StoreName = StoreName,
                    ShopName = ShopName,
                    DeviceCumCode = DeviceCumCode,
                    SumChargeAmount = SumChargeAmount,
                    SumCount = SumCount,
                    PerCumPrice = PerCumPrice,
                    SalesDate = salesDate
                });

            }


            List<DeviceSalesModel> sales3 = new List<DeviceSalesModel>();
            sales3.AddRange(sales1);
            sales3.AddRange(sales2);


            var sales4 = (from u in sales3 select new { u.ShopName, u.StoreName, u.DeviceCumCode }).Distinct().ToList();

            var sale5 = new List<object>();

            for (int i = 0; i < sales4.Count; i++)
            {
                string storeName = sales4[i].StoreName;
                string shopName = sales4[i].ShopName;
                string deviceCumCode = sales4[i].DeviceCumCode;
                string sumChargeAmount = "0";
                string sumCount = "0";
                string perCumPrice = "0";

                var sale = sales2.Where(m => m.StoreName == storeName && m.ShopName == shopName && m.DeviceCumCode == deviceCumCode).FirstOrDefault();
                if (sale != null)
                {
                    sumChargeAmount = sale.SumChargeAmount;
                    sumCount = sale.SumCount;
                    perCumPrice = sale.PerCumPrice;
                }

                sale5.Add(new
                {
                    storeName = storeName,
                    shopName = shopName,
                    deviceCumCode = deviceCumCode,
                    sumChargeAmount = sumChargeAmount,
                    sumCount = sumCount,
                    perCumPrice = perCumPrice,
                    salesDate = salesDate
                });
            }




            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", sale5);

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


        public string GetCabinetName(string cabinetId)
        {
            return cabinetId;
        }


        public CustomJsonResult DeviceReplenishPlanGet(string operater, string merchId, RopReportDeviceReplenishPlanGet rop)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.ErpReplenishPlanDeviceDetail
                         where u.MerchId == merchId
                         select new { u.PlanCumCode, u.PlanId, u.MakeTime, u.MakerName, u.StoreName, u.ShopName, u.DeviceId, u.DeviceCumCode, u.CabinetId, u.SlotId, u.SlotName, u.SkuCumCode, u.SkuName, u.SkuSpecDes, u.PlanRshQuantity, u.RealRshQuantity, u.RsherName, u.RshTime });

            if (!string.IsNullOrEmpty(rop.PlanId))
            {
                query = query.Where(m => m.PlanId == rop.PlanId);
            }

            if (!string.IsNullOrEmpty(rop.PlanCumCode))
            {
                query = query.Where(m => m.PlanCumCode == rop.PlanCumCode);
            }

            if (!string.IsNullOrEmpty(rop.SkuCumCode))
            {
                query = query.Where(m => m.SkuCumCode.Contains(rop.SkuCumCode));
            }

            if (!string.IsNullOrEmpty(rop.ShopName))
            {
                query = query.Where(m => m.ShopName.Contains(rop.ShopName));
            }

            if (!string.IsNullOrEmpty(rop.MakerName))
            {
                query = query.Where(m => m.MakerName.Contains(rop.MakerName));
            }

            if (rop.MakeDateArea != null)
            {
                if (rop.MakeDateArea.Length == 2)
                {
                    DateTime? makeStartTime = CommonUtil.ConverToStartTime(rop.MakeDateArea[0]);
                    DateTime? makeEndTime = CommonUtil.ConverToEndTime(rop.MakeDateArea[1]);

                    if (makeStartTime != null && makeEndTime != null)
                    {
                        query = query.Where(m => m.MakeTime >= makeStartTime && m.MakeTime <= makeEndTime);

                    }
                }
            }



            List<object> olist = new List<object>();

            var list = query.OrderBy(m => m.DeviceId).ThenBy(m => m.CabinetId).ThenBy(x => x.SlotName.Length).ThenBy(m => m.SlotName).ToList();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    PlanCumCode = item.PlanCumCode,
                    MakeTime = item.MakeTime.ToUnifiedFormatDateTime(),
                    MakerName = item.MakerName,
                    StoreName = item.StoreName,
                    ShopName = item.ShopName,
                    DeviceCode = MerchServiceFactory.Device.GetCode(item.DeviceId, item.DeviceCumCode),
                    CabinetName = GetCabinetName(item.CabinetId),
                    SlotName = item.SlotName,
                    SkuCumCode = item.SkuCumCode,
                    SkuName = item.SkuName,
                    SkuSpecDes = item.SkuSpecDes,
                    PlanRshQuantity = item.PlanRshQuantity,
                    RealRshQuantity = item.RealRshQuantity,
                    RsherName = item.RsherName,
                    RshTime = item.RshTime.ToUnifiedFormatDateTime()
                });

            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }


    }
}
