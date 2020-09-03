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
    public class CartService : BaseDbContext
    {
        public CustomJsonResult<RetCartPageData> PageData(string operater, string clientUserId, RupCartPageData rup)
        {
            var result = new CustomJsonResult<RetCartPageData>();

            var ret = new RetCartPageData();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            var clientCarts = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == rup.StoreId && m.Status == E_ClientCartStatus.WaitSettle).ToList();

            //构建购物车商品信息
            var cartProductSkuModels = new List<CartProductSkuModel>();

            foreach (var clientCart in clientCarts)
            {
                var bizProductSku = CacheServiceFactory.Product.GetSkuStock(clientCart.MerchId, store.Id, store.GetSellChannelRefIds(clientCart.ShopMode), clientCart.PrdProductSkuId);
                if (bizProductSku != null)
                {
                    if (bizProductSku.Stocks.Count > 0)
                    {
                        var cartProcudtSkuModel = new CartProductSkuModel();
                        cartProcudtSkuModel.CartId = clientCart.Id;
                        cartProcudtSkuModel.Id = clientCart.PrdProductSkuId;
                        cartProcudtSkuModel.ProductId = clientCart.PrdProductId;
                        cartProcudtSkuModel.Name = bizProductSku.Name;
                        cartProcudtSkuModel.MainImgUrl = bizProductSku.MainImgUrl;
                        cartProcudtSkuModel.SalePrice = bizProductSku.Stocks[0].SalePrice;
                        cartProcudtSkuModel.IsOffSell = bizProductSku.Stocks[0].IsOffSell;
                        cartProcudtSkuModel.Quantity = clientCart.Quantity;
                        cartProcudtSkuModel.SumPrice = clientCart.Quantity * cartProcudtSkuModel.SalePrice;
                        cartProcudtSkuModel.Selected = clientCart.Selected;
                        cartProcudtSkuModel.ShopMode = clientCart.ShopMode;
                        cartProductSkuModels.Add(cartProcudtSkuModel);
                    }
                }
            }

            //分类块，自取或快递 各构建
            var shopModes = (from c in clientCarts select c.ShopMode).Distinct().ToList();

            foreach (var shopMode in shopModes)
            {
                var carBlock = new CartBlockModel();
                carBlock.ShopMode = shopMode;
                carBlock.ProductSkus = cartProductSkuModels.Where(m => m.ShopMode == shopMode).ToList();
                switch (shopMode)
                {
                    case E_SellChannelRefType.Mall:
                        carBlock.TagName = "线上商城";
                        break;
                    case E_SellChannelRefType.Machine:
                        carBlock.TagName = "线下机器";
                        break;
                }

                if (carBlock.ProductSkus.Count > 0)
                {
                    ret.Blocks.Add(carBlock);
                }
            }

            ret.Count = cartProductSkuModels.Sum(m => m.Quantity);
            ret.SumPrice = cartProductSkuModels.Sum(m => m.SumPrice);
            ret.SumPriceBySelected = cartProductSkuModels.Where(m => m.Selected == true).Sum(m => m.SumPrice);
            ret.CountBySelected = cartProductSkuModels.Where(m => m.Selected == true).Count();

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

                        if (item.ShopMode == E_SellChannelRefType.Unknow)
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

                                var bizProductSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.Id, store.GetSellChannelRefIds(item.ShopMode), item.Id);

                                if (bizProductSku == null || bizProductSku.Stocks == null || bizProductSku.Stocks.Count == 0)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品已经售完");
                                }

                                if (bizProductSku.Stocks[0].IsOffSell)
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
                                    clientCart.PrdProductId = bizProductSku.ProductId;
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

            return result;
        }
    }
}
