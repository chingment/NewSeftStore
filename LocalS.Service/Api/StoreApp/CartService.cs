using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.StoreApp
{
    public class CartService : BaseService
    {

        private CartDataModel GetCartData(string clientUserId, string storeId, string shopId, E_ShopMode shopMode)
        {
            var m_cartData = new CartDataModel();

            var m_store = BizFactory.Store.GetOne(storeId);

            var query = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == storeId & m.Status == E_ClientCartStatus.WaitSettle);


            if (!string.IsNullOrEmpty(shopId))
            {
                query.Where(m => m.ShopId == shopId);

            }
            var d_clientCarts = query.ToList();

            //构建购物车商品信息
            var m_cartProductSkus = new List<CartDataModel.ProductSkuModel>();

            foreach (var d_clientCart in d_clientCarts)
            {

                var r_productSku = CacheServiceFactory.Product.GetSkuStock(d_clientCart.ShopMode, d_clientCart.MerchId, d_clientCart.StoreId, d_clientCart.ShopId, null, d_clientCart.PrdProductSkuId);


                if (r_productSku != null)
                {
                    if (r_productSku.Stocks.Count > 0)
                    {
                        var m_cartProductSku = new CartDataModel.ProductSkuModel();
                        m_cartProductSku.CartId = d_clientCart.Id;
                        m_cartProductSku.Id = d_clientCart.PrdProductSkuId;
                        m_cartProductSku.ProductId = d_clientCart.PrdProductId;
                        m_cartProductSku.Name = r_productSku.Name;
                        m_cartProductSku.MainImgUrl = r_productSku.MainImgUrl;
                        m_cartProductSku.SalePrice = r_productSku.Stocks[0].SalePrice;
                        m_cartProductSku.IsOffSell = r_productSku.Stocks[0].IsOffSell;
                        m_cartProductSku.Quantity = d_clientCart.Quantity;
                        m_cartProductSku.SumPrice = d_clientCart.Quantity * m_cartProductSku.SalePrice;
                        m_cartProductSku.Selected = d_clientCart.Selected;
                        m_cartProductSku.ShopMode = d_clientCart.ShopMode;
                        m_cartProductSku.ShopId = d_clientCart.ShopId;
                        m_cartProductSkus.Add(m_cartProductSku);
                    }
                }
            }

            //分类块，自取或快递 各构建
            var m_shops = (from c in d_clientCarts select new { c.ShopMode, c.ShopId }).Distinct().ToList();

            foreach (var m_shop in m_shops)
            {
                var m_carBlock = new CartDataModel.BlockModel();
                m_carBlock.ShopMode = m_shop.ShopMode;
                m_carBlock.ProductSkus = m_cartProductSkus.Where(m => m.ShopMode == m_shop.ShopMode && m.ShopId == m_shop.ShopId).ToList();
                switch (m_shop.ShopMode)
                {
                    case E_ShopMode.Mall:
                        m_carBlock.TagName = "线上商城";
                        break;
                    case E_ShopMode.Machine:
                        var shop = CurrentDb.Shop.Where(m => m.Id == m_shop.ShopId).FirstOrDefault();
                        m_carBlock.TagName = string.Format("门店[{0}]/线下机器", shop.Name);
                        break;
                }

                if (m_carBlock.ProductSkus.Count > 0)
                {
                    m_cartData.Blocks.Add(m_carBlock);
                }
            }

            m_cartData.Count = m_cartProductSkus.Sum(m => m.Quantity);
            m_cartData.SumPrice = m_cartProductSkus.Sum(m => m.SumPrice);
            m_cartData.SumPriceBySelected = m_cartProductSkus.Where(m => m.Selected == true).Sum(m => m.SumPrice);
            m_cartData.CountBySelected = m_cartProductSkus.Where(m => m.Selected == true).Count();


            return m_cartData;
        }


        public CustomJsonResult GetCartData(string operater, string clientUserId, RupCartGetCartData rup)
        {
            var result = new CustomJsonResult();

            var ret = GetCartData(clientUserId, rup.StoreId, rup.ShopId, rup.ShopMode);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult<RetCartPageData> PageData(string operater, string clientUserId, RupCartPageData rup)
        {
            var result = new CustomJsonResult<RetCartPageData>();

            var ret = new RetCartPageData();

            ret.CartData = GetCartData(clientUserId, rup.StoreId, "0", rup.ShopMode);

            result = new CustomJsonResult<RetCartPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        private static readonly object lock_Operate = new object();
        public CustomJsonResult Operate(string operater, string clientUserId, RopCartOperate rop)
        {
            var result = new CustomJsonResult();

            if (rop.ProductSkus == null || rop.ProductSkus.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "选择商品为空");
            }


            lock (lock_Operate)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var store = BizFactory.Store.GetOne(rop.StoreId);

                    foreach (var item in rop.ProductSkus)
                    {

                        if (item.ShopMode == E_ShopMode.Unknow)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2NoSelectShopMode, "未选择购物方式");
                        }

                        var clientCart = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == rop.StoreId && m.PrdProductSkuId == item.Id && m.ShopMode == item.ShopMode && m.Status == E_ClientCartStatus.WaitSettle).FirstOrDefault();

                        switch (rop.Operate)
                        {
                            case E_CartOperateType.Selected:
                                clientCart.Selected = item.Selected;
                                break;
                            case E_CartOperateType.Decrease:
                                if (clientCart.Quantity >= 2)
                                {
                                    clientCart.Quantity -= 1;
                                    clientCart.MendTime = DateTime.Now;
                                    clientCart.Mender = operater;
                                }
                                break;
                            case E_CartOperateType.Increase:


                                var r_productSku = CacheServiceFactory.Product.GetSkuStock(item.ShopMode, store.MerchId, store.StoreId, item.ShopId, null, item.Id);


                                if (r_productSku == null || r_productSku.Stocks == null || r_productSku.Stocks.Count == 0)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品已经售完");
                                }

                                if (r_productSku.Stocks[0].IsOffSell)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品已下架");
                                }

                                if (clientCart == null)
                                {
                                    clientCart = new ClientCart();
                                    clientCart.Id = IdWorker.Build(IdType.NewGuid);
                                    clientCart.ClientUserId = clientUserId;
                                    clientCart.MerchId = store.MerchId;
                                    clientCart.StoreId = rop.StoreId;
                                    clientCart.ShopId = item.ShopId;
                                    clientCart.PrdProductId = r_productSku.ProductId;
                                    clientCart.PrdProductSkuId = item.Id;
                                    clientCart.Selected = true;
                                    clientCart.CreateTime = DateTime.Now;
                                    clientCart.Creator = operater;
                                    clientCart.Quantity = item.Quantity;
                                    clientCart.ShopMode = item.ShopMode;
                                    clientCart.Status = E_ClientCartStatus.WaitSettle;
                                    CurrentDb.ClientCart.Add(clientCart);
                                }
                                else
                                {
                                    clientCart.Quantity += item.Quantity;
                                    clientCart.MendTime = DateTime.Now;
                                    clientCart.Mender = operater;
                                }

                                break;
                            case E_CartOperateType.Delete:
                                clientCart.Status = E_ClientCartStatus.Deleted;
                                clientCart.MendTime = DateTime.Now;
                                clientCart.Mender = operater;
                                break;
                        }
                    }

                    CurrentDb.SaveChanges();

                    ts.Complete();

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                }
            }

            if (result.Result == ResultType.Success)
            {
                result.Data = GetCartData(clientUserId, rop.StoreId, rop.ShopId, rop.ShopMode);
            }

            return result;
        }
    }
}
