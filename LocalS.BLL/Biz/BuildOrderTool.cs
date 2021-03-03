using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class BuildSku
    {
        private bool _isOffSell = true;

        public string Id { get; set; }
        public string SpuId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public string Producer { get; set; }
        public string BarCode { get; set; }
        public string CumCode { get; set; }
        public string SvcConsulterId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal CouponAmount { get; set; }
        public string BriefDes { get; set; }
        public string SpecDes { get; set; }
        public string CartId { get; set; }
        public int Quantity { get; set; }
        public E_ShopMode ShopMode { get; set; }
        public E_ShopMethod ShopMethod { get; set; }
        public int KindId1 { get; set; }
        public int KindId2 { get; set; }
        public int KindId3 { get; set; }
        public E_RentTermUnit RentTermUnit { get; set; }
        public int RentTermValue { get; set; }
        public string RentTermUnitText { get; set; }
        public decimal RentAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public E_SupReceiveMode SupReceiveMode { get; set; }
        public E_ReceiveMode ReceiveMode { get; set; }
        public List<SkuStockModel> Stocks { get; set; }
        public decimal CouponAmountByShop { get; set; }
        public decimal CouponAmountByDeposit { get; set; }
        public decimal CouponAmountByRent { get; set; }
        public string ShopId { get; set; }
        public string[] MachineIds { get; set; }
        public bool IsOffSell
        {
            get
            {
                return _isOffSell;
            }
            set
            {
                _isOffSell = value;
            }
        }
    }

    public class BuildOrder
    {
        public BuildOrder()
        {
            this.Childs = new List<Child>();
        }
        public E_ShopMode ShopMode { get; set; }
        public string ShopId { get; set; }
        public string MachineId { get; set; }
        public int Quantity { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal CouponAmountByShop { get; set; }
        public decimal CouponAmountByRent { get; set; }
        public decimal CouponAmountByDeposit { get; set; }
        public E_ReceiveMode ReceiveMode { get; set; }
        public List<Child> Childs { get; set; }
        public class Child
        {
            public E_ShopMode ShopMode { get; set; }
            public string SkuId { get; set; }
            public decimal SalePrice { get; set; }
            public decimal OriginalPrice { get; set; }
            public int Quantity { get; set; }
            public decimal OriginalAmount { get; set; }
            public decimal SaleAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal ChargeAmount { get; set; }

            public string ShopId { get; set; }
            public string MachineId { get; set; }
            public string CabinetId { get; set; }
            public string SlotId { get; set; }
            public E_RentTermUnit RentTermUnit { get; set; }
            public int RentTermValue { get; set; }
            public decimal RentAmount { get; set; }
            public decimal DepositAmount { get; set; }
            public decimal CouponAmountByShop { get; set; }
            public decimal CouponAmountByRent { get; set; }
            public decimal CouponAmountByDeposit { get; set; }
            public E_ReceiveMode ReceiveMode { get; set; }
        }
    }

    public class BuildOrderTool : BaseService
    {
        private int _memberLevel = 0;
        private string _merchId = "";
        private string _storeId = "";


        private List<string> _errorPoints;
        private List<BuildSku> _buildSkus;
        private List<string> _couponIdsByShop;

        private decimal _memberDiscountAmount = 0;


        public decimal MemberDiscountAmount
        {
            get
            {
                return _memberDiscountAmount;
            }
        }

        public bool IsSuccess
        {
            get
            {
                if (_errorPoints.Count == 0)
                    return true;

                return false;
            }
        }

        public string Message
        {
            get
            {
                if (_errorPoints.Count == 0)
                    return "构建成功";

                return string.Join("\n", _errorPoints.ToArray());
            }
        }

        public BuildOrderTool(string merchId, string storeId, int memberLevel, List<string> couponIdsByShop)
        {
            _merchId = merchId;
            _storeId = storeId;

            _memberLevel = memberLevel;
            _buildSkus = new List<BuildSku>();
            _errorPoints = new List<string>();
            _couponIdsByShop = new List<string>();

            if (couponIdsByShop != null)
            {
                foreach (var couponIdByShop in couponIdsByShop)
                {
                    if (!string.IsNullOrEmpty(couponIdByShop))
                        _couponIdsByShop.Add(couponIdByShop);
                }
            }
        }

        public void AddSku(string id, int quantity, string cartId, E_ShopMode shopMode, E_ShopMethod shopMethod, E_ReceiveMode receiveMode, string shopId, string[] machineIds)
        {
            _buildSkus.Add(new BuildSku { Id = id, Quantity = quantity, CartId = cartId, ShopMode = shopMode, ShopMethod = shopMethod, ReceiveMode = receiveMode, ShopId = shopId, MachineIds = machineIds });
        }

        public List<BuildSku> BuildSkus()
        {
            if (_buildSkus.Count == 0)
            {
                _errorPoints.Add("商品数据为空");
            }

            decimal memberDiscount = 0;
            if (_memberLevel > 0)
            {
                var d_MemberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.MerchId == _merchId && m.Level == _memberLevel).FirstOrDefault();
                if (d_MemberLevelSt != null)
                {
                    memberDiscount = d_MemberLevelSt.Discount;
                }
            }

            foreach (var buildSku in _buildSkus)
            {
                if (buildSku.ShopMethod == E_ShopMethod.Buy)
                {
                    #region Shop

                    var r_Sku = CacheServiceFactory.Product.GetSkuStock(buildSku.ShopMode, _merchId, _storeId, buildSku.ShopId, buildSku.MachineIds, buildSku.Id);

                    if (r_Sku == null)
                    {
                        _errorPoints.Add(string.Format("商品ID[{0}]信息不存在", buildSku.Id));
                    }
                    else
                    {

                        buildSku.SpuId = r_Sku.SpuId;
                        buildSku.Name = r_Sku.Name;
                        buildSku.MainImgUrl = r_Sku.MainImgUrl;
                        buildSku.BarCode = r_Sku.BarCode;
                        buildSku.CumCode = r_Sku.CumCode;
                        buildSku.SpecDes = r_Sku.SpecDes.ToJsonString();
                        buildSku.Producer = r_Sku.Producer;
                        buildSku.KindId1 = r_Sku.KindId1;
                        buildSku.KindId2 = r_Sku.KindId2;
                        buildSku.KindId3 = r_Sku.KindId3;
                        buildSku.SupReceiveMode = r_Sku.SupReceiveMode;

                        if (r_Sku.Stocks.Count == 0)
                        {
                            _errorPoints.Add(string.Format("商品[{0}]信息库存为空", buildSku.Name));
                        }
                        else
                        {
                            buildSku.IsOffSell = r_Sku.Stocks[0].IsOffSell;

                            if (buildSku.IsOffSell)
                            {
                                _errorPoints.Add(string.Format("商品[{0}]已下架", buildSku.Name));
                            }
                            else
                            {
                                var sellQuantity = r_Sku.Stocks.Sum(m => m.SellQuantity);

                                if (sellQuantity < buildSku.Quantity)
                                {
                                    _errorPoints.Add(string.Format("商品[{0}]的可销售数量为{1}个", buildSku.Name, sellQuantity));
                                }
                            }

                            buildSku.Stocks = r_Sku.Stocks;

                            decimal salePrice = r_Sku.Stocks[0].SalePrice;

                            decimal originalPrice = salePrice;

                            //切换特定商品会员价
                            if (_memberLevel > 0)
                            {
                                if (memberDiscount > 0)
                                {
                                    if (_couponIdsByShop == null || _couponIdsByShop.Count == 0)
                                    {
                                        salePrice = salePrice * memberDiscount;
                                    }
                                }

                                //var d_MemberSkuSt = CurrentDb.MemberSkuSt.Where(m => m.MerchId == _merchId && m.StoreId == _storeId && m.SkuId == Sku.Id && m.MemberLevel == _memberLevel && m.IsDisabled == false).FirstOrDefault();
                                //if (d_MemberSkuSt != null)
                                //{
                                //    salePrice = d_MemberSkuSt.MemberPrice;
                                //    LogUtil.Info("clientUser.MemberPrice:" + d_MemberSkuSt.MemberPrice);
                                //}
                            }

                            buildSku.SalePrice = salePrice;
                            buildSku.SaleAmount = salePrice * buildSku.Quantity;
                            buildSku.OriginalPrice = originalPrice;
                            buildSku.OriginalAmount = originalPrice * buildSku.Quantity;

                            if (memberDiscount > 0)
                            {
                                _memberDiscountAmount += (buildSku.OriginalAmount - buildSku.SaleAmount);
                            }

                        }

                    }
                    #endregion
                }
                else if (buildSku.ShopMethod == E_ShopMethod.Rent)
                {
                    #region Rent
                    var r_Sku = CacheServiceFactory.Product.GetSkuStock(E_ShopMode.Mall, _merchId, _storeId, "", null, buildSku.Id);

                    if (r_Sku == null)
                    {
                        _errorPoints.Add(string.Format("商品ID[{0}]信息不存在", buildSku.Id));
                    }
                    else
                    {

                        buildSku.SpuId = r_Sku.SpuId;
                        buildSku.Name = r_Sku.Name;
                        buildSku.MainImgUrl = r_Sku.MainImgUrl;
                        buildSku.BarCode = r_Sku.BarCode;
                        buildSku.CumCode = r_Sku.CumCode;
                        buildSku.SpecDes = r_Sku.SpecDes.ToJsonString();
                        buildSku.Producer = r_Sku.Producer;
                        buildSku.KindId1 = r_Sku.KindId1;
                        buildSku.KindId2 = r_Sku.KindId2;
                        buildSku.KindId3 = r_Sku.KindId3;
                        buildSku.SupReceiveMode = r_Sku.SupReceiveMode;
                        if (r_Sku.Stocks.Count == 0)
                        {
                            _errorPoints.Add(string.Format("商品[{0}]信息库存为空", buildSku.Name));
                        }
                        else
                        {
                            buildSku.IsOffSell = r_Sku.Stocks[0].IsOffSell;

                            if (buildSku.IsOffSell)
                            {
                                _errorPoints.Add(string.Format("商品[{0}]已下架", buildSku.Name));
                            }
                            else
                            {
                                var sellQuantity = r_Sku.Stocks.Sum(m => m.SellQuantity);

                                if (sellQuantity < buildSku.Quantity)
                                {
                                    _errorPoints.Add(string.Format("商品[{0}]的可销售数量为{1}个", buildSku.Name, sellQuantity));
                                }
                            }

                            buildSku.Stocks = r_Sku.Stocks;

                            decimal salePrice = r_Sku.Stocks[0].DepositPrice + r_Sku.Stocks[0].RentMhPrice;
                            decimal originalPrice = r_Sku.Stocks[0].DepositPrice + r_Sku.Stocks[0].RentMhPrice;

                            buildSku.SalePrice = salePrice;
                            buildSku.SaleAmount = salePrice * buildSku.Quantity;
                            buildSku.OriginalPrice = originalPrice;
                            buildSku.OriginalAmount = originalPrice * buildSku.Quantity;
                            buildSku.DepositAmount = r_Sku.Stocks[0].DepositPrice;
                            buildSku.RentAmount = r_Sku.Stocks[0].RentMhPrice;
                            buildSku.RentTermUnit = E_RentTermUnit.Month;
                            buildSku.RentTermValue = 1;
                            buildSku.RentTermUnitText = "月";
                        }
                    }

                    #endregion
                }
                else if (buildSku.ShopMethod == E_ShopMethod.RentFee)
                {
                    #region RentFee
                    var r_Sku = CacheServiceFactory.Product.GetSkuStock(E_ShopMode.Mall, _merchId, _storeId, "", null, buildSku.Id);

                    if (r_Sku == null)
                    {
                        _errorPoints.Add(string.Format("商品ID[{0}]信息不存在", buildSku.Id));
                    }
                    else
                    {

                        buildSku.SpuId = r_Sku.SpuId;
                        buildSku.Name = r_Sku.Name;
                        buildSku.MainImgUrl = r_Sku.MainImgUrl;
                        buildSku.BarCode = r_Sku.BarCode;
                        buildSku.CumCode = r_Sku.CumCode;
                        buildSku.SpecDes = r_Sku.SpecDes.ToJsonString();
                        buildSku.Producer = r_Sku.Producer;
                        buildSku.KindId1 = r_Sku.KindId1;
                        buildSku.KindId2 = r_Sku.KindId2;
                        buildSku.KindId3 = r_Sku.KindId3;
                        buildSku.SupReceiveMode = r_Sku.SupReceiveMode;
                        if (r_Sku.Stocks.Count == 0)
                        {
                            _errorPoints.Add(string.Format("商品[{0}]信息库存为空", buildSku.Name));
                        }
                        else
                        {
                            buildSku.IsOffSell = r_Sku.Stocks[0].IsOffSell;

                            if (buildSku.IsOffSell)
                            {
                                _errorPoints.Add(string.Format("商品[{0}]已下架", buildSku.Name));
                            }
                            else
                            {
                                var sellQuantity = r_Sku.Stocks.Sum(m => m.SellQuantity);

                                if (sellQuantity < buildSku.Quantity)
                                {
                                    _errorPoints.Add(string.Format("商品[{0}]的可销售数量为{1}个", buildSku.Name, sellQuantity));
                                }
                            }

                            buildSku.Stocks = r_Sku.Stocks;

                            decimal salePrice = r_Sku.Stocks[0].RentMhPrice;
                            decimal originalPrice = r_Sku.Stocks[0].RentMhPrice;

                            buildSku.SalePrice = salePrice;
                            buildSku.SaleAmount = salePrice * buildSku.Quantity;
                            buildSku.OriginalPrice = originalPrice;
                            buildSku.OriginalAmount = originalPrice * buildSku.Quantity;
                            buildSku.DepositAmount = r_Sku.Stocks[0].DepositPrice;
                            buildSku.RentAmount = r_Sku.Stocks[0].RentMhPrice;
                            buildSku.RentTermUnit = E_RentTermUnit.Month;
                            buildSku.RentTermValue = 1;
                            buildSku.RentTermUnitText = "月";
                        }
                    }

                    #endregion
                }
                else if (buildSku.ShopMethod == E_ShopMethod.MemberFee)
                {
                    #region MemberFee

                    var memberFeeSt = CurrentDb.MemberFeeSt.Where(m => m.MerchId == _merchId && m.Id == buildSku.Id).FirstOrDefault();
                    if (memberFeeSt == null)
                    {
                        _errorPoints.Add(string.Format("商品ID[{0}]信息不存在", buildSku.Id));
                    }
                    else
                    {
                        var stocks = new List<SkuStockModel>();
                        var stock = new SkuStockModel();
                        stock.ShopMode = E_ShopMode.Mall;
                        stock.ShopId = "0";
                        stock.MachineId = "0";
                        stock.CabinetId = "0";
                        stock.SlotId = "0";
                        stock.SumQuantity = 0;
                        stock.LockQuantity = 0;
                        stock.SellQuantity = 0;
                        stock.IsOffSell = false;
                        stock.SalePrice = memberFeeSt.FeeSaleValue;
                        stocks.Add(stock);

                        buildSku.Name = memberFeeSt.Name;
                        buildSku.SupReceiveMode = E_SupReceiveMode.FeeByMember;
                        buildSku.ReceiveMode = E_ReceiveMode.FeeByMember;
                        buildSku.MainImgUrl = memberFeeSt.MainImgUrl;
                        buildSku.SalePrice = memberFeeSt.FeeSaleValue;
                        buildSku.SaleAmount = buildSku.Quantity * memberFeeSt.FeeSaleValue;
                        buildSku.OriginalPrice = memberFeeSt.FeeOriginalValue;
                        buildSku.OriginalAmount = buildSku.Quantity * memberFeeSt.FeeOriginalValue;
                        buildSku.SpuId = IdWorker.Build(IdType.EmptyGuid);
                        buildSku.BarCode = "MEMBER_FEE";
                        buildSku.CumCode = "MEMBER_FEE";

                        List<SpecDes> specDes = new List<BLL.SpecDes>();
                        specDes.Add(new SpecDes { Name = "单规格", Value = "会员费" });
                        buildSku.SpecDes = specDes.ToJsonString();
                        buildSku.Producer = "商家";
                        buildSku.CartId = "";
                        buildSku.SvcConsulterId = "";
                        buildSku.KindId1 = 0;
                        buildSku.KindId2 = 0;
                        buildSku.KindId3 = 0;
                        buildSku.IsOffSell = false;
                        buildSku.Stocks = stocks;
                    }



                    #endregion
                }
            }

            return _buildSkus;
        }

        public void CalCouponAmount(decimal atLeastAmount, E_Coupon_UseAreaType useAreaType, string useAreaValue, E_Coupon_FaceType faceType, decimal faceValue)
        {
            decimal cal_sum_amount = 0;
            if (useAreaType == E_Coupon_UseAreaType.All)
            {
                cal_sum_amount = _buildSkus.Sum(m => m.SaleAmount);
            }
            else if (useAreaType == E_Coupon_UseAreaType.Store)
            {
                var list = useAreaValue.ToJsonObject<List<UseAreaModel>>();
                if (list != null)
                {
                    string[] ids = list.Select(m => m.Id).ToArray();
                    if (ids != null)
                    {
                        if (ids.Contains(_storeId))
                        {
                            cal_sum_amount = _buildSkus.Sum(m => m.SaleAmount);
                        }
                    }
                }
            }
            else if (useAreaType == E_Coupon_UseAreaType.ProductKind)
            {
                var list = useAreaValue.ToJsonObject<List<UseAreaModel>>();
                if (list != null)
                {
                    int[] ids = list.Select(s => Int32.Parse(s.Id)).ToArray();

                    if (ids != null)
                    {
                        cal_sum_amount = _buildSkus.Where(m => ids.Contains(m.KindId3)).Sum(m => m.SaleAmount);
                    }
                }
            }
            else if (useAreaType == E_Coupon_UseAreaType.ProductSpu)
            {
                var list = useAreaValue.ToJsonObject<List<UseAreaModel>>();
                if (list != null)
                {
                    string[] ids = list.Select(m => m.Id).ToArray();

                    if (ids != null)
                    {
                        cal_sum_amount = _buildSkus.Where(m => ids.Contains(m.SpuId)).Sum(m => m.SaleAmount);
                    }
                }

            }

            foreach (var buildOrderSku in _buildSkus)
            {
                decimal amount = Decimal.Round(BizFactory.Order.CalCouponAmount(cal_sum_amount, atLeastAmount, useAreaType, useAreaValue, faceType, faceValue, _storeId, buildOrderSku.SpuId, buildOrderSku.KindId3, buildOrderSku.SaleAmount), 2);

                if (faceType == E_Coupon_FaceType.ShopDiscount || faceType == E_Coupon_FaceType.ShopVoucher)
                {
                    buildOrderSku.CouponAmountByShop = amount;
                }
                else if (faceType == E_Coupon_FaceType.DepositVoucher)
                {
                    buildOrderSku.CouponAmountByDeposit = amount;
                }
                else if (faceType == E_Coupon_FaceType.RentVoucher)
                {
                    buildOrderSku.CouponAmountByRent = amount;
                }
            }

            //金额补差
            if (faceType == E_Coupon_FaceType.ShopVoucher)
            {
                var sumCouponAmount1 = faceValue;
                var sumCouponAmount2 = _buildSkus.Sum(m => m.CouponAmountByShop);
                if (sumCouponAmount1 != sumCouponAmount2)
                {
                    var diff = sumCouponAmount1 - sumCouponAmount2;
                    _buildSkus[_buildSkus.Count - 1].CouponAmountByShop = _buildSkus[_buildSkus.Count - 1].CouponAmountByShop + diff;
                }
            }
            else if (faceType == E_Coupon_FaceType.DepositVoucher)
            {
                var sumCouponAmount1 = faceValue;
                var sumCouponAmount2 = _buildSkus.Sum(m => m.CouponAmountByDeposit);
                if (sumCouponAmount1 != sumCouponAmount2)
                {
                    var diff = sumCouponAmount1 - sumCouponAmount2;
                    _buildSkus[_buildSkus.Count - 1].CouponAmountByDeposit = _buildSkus[_buildSkus.Count - 1].CouponAmountByDeposit + diff;
                }
            }
            else if (faceType == E_Coupon_FaceType.RentVoucher)
            {
                var sumCouponAmount1 = faceValue;
                var sumCouponAmount2 = _buildSkus.Sum(m => m.CouponAmountByRent);
                if (sumCouponAmount1 != sumCouponAmount2)
                {
                    var diff = sumCouponAmount1 - sumCouponAmount2;
                    _buildSkus[_buildSkus.Count - 1].CouponAmountByRent = _buildSkus[_buildSkus.Count - 1].CouponAmountByRent + diff;
                }
            }
        }

        public List<BuildOrder> BuildOrders()
        {
            List<BuildOrder> buildOrders = new List<BuildOrder>();

            if (_buildSkus == null || _buildSkus.Count == 0)
                return buildOrders;

            List<BuildOrder.Child> buildOrderChilds = new List<BuildOrder.Child>();

            var d_s_orders = (from d in _buildSkus select new { d.ShopMode, d.ReceiveMode, d.ShopId }).Distinct().ToArray();
            LogUtil.Info("myabc.d_s_orders:" + d_s_orders.ToJsonString());
            foreach (var d_s_order in d_s_orders)
            {
                var shopModeSkus = _buildSkus.Where(m => m.ShopMode == d_s_order.ShopMode && m.ReceiveMode == d_s_order.ReceiveMode && m.ShopId == d_s_order.ShopId).ToList();

                foreach (var buildSku in _buildSkus)
                {
                    var sku_Stocks = buildSku.Stocks;

                    if (d_s_order.ShopMode == E_ShopMode.Mall)
                    {
                        //SalePrice,OriginalPrice 以 shopModeSku 的 SalePrice和 OriginalPrice作为标准
                        var buildOrderChild = new BuildOrder.Child();
                        buildOrderChild.ShopMode = sku_Stocks[0].ShopMode;
                        buildOrderChild.SkuId = buildSku.Id;
                        buildOrderChild.ReceiveMode = d_s_order.ReceiveMode;
                        buildOrderChild.ShopId = sku_Stocks[0].ShopId;
                        buildOrderChild.MachineId = sku_Stocks[0].MachineId;
                        buildOrderChild.CabinetId = sku_Stocks[0].CabinetId;
                        buildOrderChild.SlotId = sku_Stocks[0].SlotId;
                        buildOrderChild.Quantity = buildSku.Quantity;
                        buildOrderChild.SalePrice = buildSku.SalePrice;
                        buildOrderChild.SaleAmount = buildSku.SaleAmount;
                        buildOrderChild.OriginalPrice = buildSku.OriginalPrice;
                        buildOrderChild.OriginalAmount = buildSku.OriginalAmount;
                        buildOrderChild.RentTermUnit = buildSku.RentTermUnit;
                        buildOrderChild.RentTermValue = buildSku.RentTermValue;
                        buildOrderChild.RentAmount = buildSku.RentAmount;
                        buildOrderChild.DepositAmount = buildSku.DepositAmount;
                        buildOrderChild.CouponAmountByDeposit = buildSku.CouponAmountByDeposit;
                        buildOrderChild.CouponAmountByShop = buildSku.CouponAmountByShop;
                        buildOrderChild.CouponAmountByRent = buildSku.CouponAmountByRent;
                        buildOrderChild.DiscountAmount = buildSku.OriginalAmount - buildSku.SaleAmount;
                        buildOrderChild.ChargeAmount = buildSku.SaleAmount - buildSku.CouponAmountByDeposit - buildSku.CouponAmountByShop - buildSku.CouponAmountByRent;
                        buildOrderChilds.Add(buildOrderChild);
                    }
                    else if (d_s_order.ShopMode == E_ShopMode.Machine)
                    {
                        foreach (var item in sku_Stocks)
                        {
                            bool isFlag = false;


                            for (var i = 0; i < item.SellQuantity; i++)
                            {
                                int reservedQuantity = buildOrderChilds.Where(m => m.ShopId == item.ShopId && m.SkuId == buildSku.Id && m.ShopMode == item.ShopMode).Sum(m => m.Quantity);//已订的数量
                                LogUtil.Info("myabc.reservedQuantity:" + reservedQuantity);
                                LogUtil.Info("myabc.needReserveQuantity:" + buildSku.Quantity);
                                int needReserveQuantity = buildSku.Quantity;//需要订的数量
                                if (reservedQuantity != needReserveQuantity)
                                {
                                    var buildOrderChild = new BuildOrder.Child();
                                    buildOrderChild.ShopMode = item.ShopMode;
                                    buildOrderChild.ShopId = item.ShopId;
                                    buildOrderChild.MachineId = item.MachineId;
                                    buildOrderChild.SkuId = buildSku.Id;
                                    buildOrderChild.ReceiveMode = d_s_order.ReceiveMode;
                                    buildOrderChild.CabinetId = item.CabinetId;
                                    buildOrderChild.SlotId = item.SlotId;
                                    buildOrderChild.Quantity = 1;
                                    buildOrderChild.SalePrice = buildSku.SalePrice;
                                    buildOrderChild.SaleAmount = buildOrderChild.Quantity * buildSku.SalePrice;
                                    buildOrderChild.OriginalPrice = buildSku.OriginalPrice;
                                    buildOrderChild.OriginalAmount = buildOrderChild.Quantity * buildSku.OriginalPrice;
                                    buildOrderChild.CouponAmountByDeposit = buildSku.CouponAmountByDeposit;
                                    buildOrderChild.CouponAmountByShop = buildSku.CouponAmountByShop;
                                    buildOrderChild.CouponAmountByRent = buildSku.CouponAmountByRent;
                                    buildOrderChild.DiscountAmount = buildOrderChild.OriginalAmount - buildOrderChild.SaleAmount;
                                    buildOrderChild.ChargeAmount = buildSku.SaleAmount - buildSku.CouponAmountByDeposit - buildSku.CouponAmountByShop - buildSku.CouponAmountByRent;
                                    buildOrderChilds.Add(buildOrderChild);
                                }
                                else
                                {
                                    isFlag = true;
                                    break;
                                }
                            }

                            if (isFlag)
                                break;
                        }

                    }

                }

            }


            LogUtil.Info("buildOrderChilds:" + buildOrderChilds.ToJsonString());

            var sumSaleAmount = buildOrderChilds.Sum(m => m.SaleAmount);
            var sumDiscountAmount = buildOrderChilds.Sum(m => m.DiscountAmount);
            for (int i = 0; i < buildOrderChilds.Count; i++)
            {
                decimal scale = (sumSaleAmount == 0 ? 0 : (buildOrderChilds[i].SaleAmount / sumSaleAmount));
                buildOrderChilds[i].DiscountAmount = Decimal.Round(scale * sumDiscountAmount, 2);
            }

            var sumDiscountAmount2 = buildOrderChilds.Sum(m => m.DiscountAmount);
            if (sumDiscountAmount != sumDiscountAmount2)
            {
                var diff = sumDiscountAmount - sumDiscountAmount2;

                buildOrderChilds[buildOrderChilds.Count - 1].DiscountAmount = buildOrderChilds[buildOrderChilds.Count - 1].DiscountAmount + diff;
            }

            var l_buildOrders = (from c in buildOrderChilds
                                 select new
                                 {
                                     c.ShopMode,
                                     c.ShopId,
                                     c.MachineId,
                                     c.ReceiveMode,
                                 }).Distinct().ToList();


            foreach (var l_buildOrder in l_buildOrders)
            {
                var buildOrder = new BuildOrder();
                buildOrder.ShopMode = l_buildOrder.ShopMode;
                buildOrder.ReceiveMode = l_buildOrder.ReceiveMode;
                buildOrder.ShopId = l_buildOrder.ShopId;
                buildOrder.MachineId = l_buildOrder.MachineId;
                buildOrder.Quantity = buildOrderChilds.Where(m => m.ShopMode == l_buildOrder.ShopMode && m.ShopId == l_buildOrder.ShopId && m.MachineId == l_buildOrder.MachineId && m.ReceiveMode == l_buildOrder.ReceiveMode).Sum(m => m.Quantity);
                buildOrder.SaleAmount = buildOrderChilds.Where(m => m.ShopMode == l_buildOrder.ShopMode && m.ShopId == l_buildOrder.ShopId && m.MachineId == l_buildOrder.MachineId && m.ReceiveMode == l_buildOrder.ReceiveMode).Sum(m => m.SaleAmount);
                buildOrder.OriginalAmount = buildOrderChilds.Where(m => m.ShopMode == l_buildOrder.ShopMode && m.ShopId == l_buildOrder.ShopId && m.MachineId == l_buildOrder.MachineId && m.ReceiveMode == l_buildOrder.ReceiveMode).Sum(m => m.OriginalAmount);
                buildOrder.DiscountAmount = buildOrderChilds.Where(m => m.ShopMode == l_buildOrder.ShopMode && m.ShopId == l_buildOrder.ShopId && m.MachineId == l_buildOrder.MachineId && m.ReceiveMode == l_buildOrder.ReceiveMode).Sum(m => m.DiscountAmount);
                buildOrder.ChargeAmount = buildOrderChilds.Where(m => m.ShopMode == l_buildOrder.ShopMode && m.ShopId == l_buildOrder.ShopId && m.MachineId == l_buildOrder.MachineId && m.ReceiveMode == l_buildOrder.ReceiveMode).Sum(m => m.ChargeAmount);
                buildOrder.CouponAmountByDeposit = buildOrderChilds.Where(m => m.ShopMode == l_buildOrder.ShopMode && m.ShopId == l_buildOrder.ShopId && m.MachineId == l_buildOrder.MachineId && m.ReceiveMode == l_buildOrder.ReceiveMode).Sum(m => m.CouponAmountByDeposit);
                buildOrder.CouponAmountByRent = buildOrderChilds.Where(m => m.ShopMode == l_buildOrder.ShopMode && m.ShopId == l_buildOrder.ShopId && m.MachineId == l_buildOrder.MachineId && m.ReceiveMode == l_buildOrder.ReceiveMode).Sum(m => m.CouponAmountByRent);
                buildOrder.CouponAmountByShop = buildOrderChilds.Where(m => m.ShopMode == l_buildOrder.ShopMode && m.ShopId == l_buildOrder.ShopId && m.MachineId == l_buildOrder.MachineId && m.ReceiveMode == l_buildOrder.ReceiveMode).Sum(m => m.CouponAmountByShop);
                buildOrder.Childs = buildOrderChilds.Where(m => m.ShopMode == l_buildOrder.ShopMode && m.ShopId == l_buildOrder.ShopId && m.MachineId == l_buildOrder.MachineId && m.ReceiveMode == l_buildOrder.ReceiveMode).ToList();
                buildOrders.Add(buildOrder);
            }


            return buildOrders;
        }
    }
}
