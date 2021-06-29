﻿using LocalS.BLL;
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
            var m_CartData = new CartDataModel();

            var m_Store = BizFactory.Store.GetOne(storeId);

            var query = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == storeId & m.Status == E_ClientCartStatus.WaitSettle);


            if (!string.IsNullOrEmpty(shopId))
            {
                query.Where(m => m.ShopId == shopId);

            }
            var d_clientCarts = query.ToList();

            //构建购物车商品信息
            var m_CartSkus = new List<CartDataModel.SkuModel>();

            foreach (var d_clientCart in d_clientCarts)
            {

                var r_Sku = CacheServiceFactory.Product.GetSkuStock(d_clientCart.ShopMode, d_clientCart.MerchId, d_clientCart.StoreId, d_clientCart.ShopId, null, d_clientCart.SkuId);


                if (r_Sku != null)
                {
                    if (r_Sku.Stocks.Count > 0)
                    {
                        var m_CartSku = new CartDataModel.SkuModel();
                        m_CartSku.CartId = d_clientCart.Id;
                        m_CartSku.Id = d_clientCart.SkuId;
                        m_CartSku.SpuId = d_clientCart.SpuId;
                        m_CartSku.Name = r_Sku.Name;
                        m_CartSku.MainImgUrl = r_Sku.MainImgUrl;
                        m_CartSku.SalePrice = r_Sku.Stocks[0].SalePrice;
                        m_CartSku.IsOffSell = r_Sku.Stocks[0].IsOffSell;
                        m_CartSku.Quantity = d_clientCart.Quantity;
                        m_CartSku.SumPrice = d_clientCart.Quantity * m_CartSku.SalePrice;
                        m_CartSku.Selected = d_clientCart.Selected;
                        m_CartSku.ShopMode = d_clientCart.ShopMode;
                        m_CartSku.ShopId = d_clientCart.ShopId;
                        m_CartSkus.Add(m_CartSku);
                    }
                }
            }

            //分类块，自取或快递 各构建
            var m_shops = (from c in d_clientCarts select new { c.ShopMode, c.ShopId }).Distinct().ToList();

            foreach (var m_shop in m_shops)
            {
                var m_carBlock = new CartDataModel.BlockModel();
                m_carBlock.ShopMode = m_shop.ShopMode;
                m_carBlock.Skus = m_CartSkus.Where(m => m.ShopMode == m_shop.ShopMode && m.ShopId == m_shop.ShopId).ToList();
                switch (m_shop.ShopMode)
                {
                    case E_ShopMode.Mall:
                        m_carBlock.TagName = "线上商城";
                        break;
                    case E_ShopMode.Device:
                        var shop = CurrentDb.Shop.Where(m => m.Id == m_shop.ShopId).FirstOrDefault();
                        m_carBlock.TagName = string.Format("门店[{0}]/线下设备", shop.Name);
                        break;
                }

                if (m_carBlock.Skus.Count > 0)
                {
                    m_CartData.Blocks.Add(m_carBlock);
                }
            }

            m_CartData.Count = m_CartSkus.Sum(m => m.Quantity);
            m_CartData.SumPrice = m_CartSkus.Sum(m => m.SumPrice);
            m_CartData.SumPriceBySelected = m_CartSkus.Where(m => m.Selected == true).Sum(m => m.SumPrice);
            m_CartData.CountBySelected = m_CartSkus.Where(m => m.Selected == true).Count();


            return m_CartData;
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

            if (rop.Skus == null || rop.Skus.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "选择商品为空");
            }


            lock (lock_Operate)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var store = BizFactory.Store.GetOne(rop.StoreId);

                    foreach (var item in rop.Skus)
                    {

                        if (item.ShopMode == E_ShopMode.Unknow)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure2NoSelectShopMode, "未选择购物方式");
                        }

                        var clientCart = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == rop.StoreId && m.SkuId == item.Id && m.ShopMode == item.ShopMode && m.Status == E_ClientCartStatus.WaitSettle).FirstOrDefault();

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


                                var r_Sku = CacheServiceFactory.Product.GetSkuStock(item.ShopMode, store.MerchId, store.StoreId, item.ShopId, null, item.Id);


                                if (r_Sku == null || r_Sku.Stocks == null || r_Sku.Stocks.Count == 0)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品已经售完");
                                }

                                if (r_Sku.Stocks[0].IsOffSell)
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
                                    clientCart.SpuId = r_Sku.SpuId;
                                    clientCart.SkuId = item.Id;
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
