﻿using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class PrdProductCacheService : BaseDbContext
    {
        private static readonly string redis_key_product_info = "info:Product";
        private static readonly string redis_key_productSku_info = "info:ProductSku";


        public void RemoveProduct(string productId)
        {
            RedisHashUtil.Remove(redis_key_product_info, productId);
        }

        public void RemoveProductSku(string productSkuId)
        {
            RedisHashUtil.Remove(redis_key_productSku_info, productSkuId);
        }

        public PrdProductModel GetProduct(string storeId, string productId)
        {
            //先从缓存信息读取商品信息
            PrdProductModel prdProductByCache = RedisHashUtil.Get<PrdProductModel>(redis_key_product_info, productId);

            //如商品信息从缓存取不到，读取数据库信息加载
            if (prdProductByCache == null)
            {
                var prdProductByDb = CurrentDb.PrdProduct.Where(m => m.Id == productId).FirstOrDefault();
                if (prdProductByDb == null)
                    return null;

                var prdProductRefSkuByDb = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == prdProductByDb.Id && m.IsRef == true).FirstOrDefault();
                if (prdProductRefSkuByDb == null)
                    return null;

                prdProductByCache = new PrdProductModel();

                prdProductByCache.Id = prdProductByDb.Id;
                prdProductByCache.Name = prdProductByDb.Name;
                prdProductByCache.DispalyImgUrls = prdProductByDb.DispalyImgUrls.ToJsonObject<List<ImgSet>>();
                prdProductByCache.MainImgUrl = ImgSet.GetMain(prdProductByDb.DispalyImgUrls);
                prdProductByCache.DetailsDes = prdProductByDb.DetailsDes;
                prdProductByCache.BriefDes = prdProductByDb.BriefDes;


                prdProductByCache.RefSku = new PrdProductModel.RefSkuModel { Id = prdProductRefSkuByDb.Id, SalePrice = prdProductRefSkuByDb.SalePrice, SpecDes = prdProductRefSkuByDb.SpecDes };


                RedisManager.Db.HashSetAsync(redis_key_product_info, prdProductByCache.Id, Newtonsoft.Json.JsonConvert.SerializeObject(prdProductByCache), StackExchange.Redis.When.Always);
            }

            if (prdProductByCache.RefSku == null)
                return null;

            //从缓存中取店铺的商品库存信息

            var refSkuStock = CacheServiceFactory.StoreSellChannelStock.GetStock(storeId, prdProductByCache.RefSku.Id);

            if (refSkuStock == null)
            {
                LogUtil.Info(string.Format("库存,Product,StoreId：{0},SkuId:{1},数据为NULL", storeId, prdProductByCache.RefSku.Id));
            }
            else
            {
                if (refSkuStock.Stocks == null || refSkuStock.Stocks.Count == 0)
                {
                    LogUtil.Info(string.Format("库存,Product,StoreId：{0},SkuId:{1},Stocks为NULL", storeId, prdProductByCache.RefSku.Id));
                }
                else
                {
                    prdProductByCache.RefSku.ReceptionMode = Entity.E_ReceptionMode.Machine;
                    prdProductByCache.RefSku.SumQuantity = refSkuStock.Stocks.Where(m => m.RefType == Entity.E_StoreSellChannelRefType.Machine).Sum(m => m.SumQuantity);
                    prdProductByCache.RefSku.LockQuantity = refSkuStock.Stocks.Where(m => m.RefType == Entity.E_StoreSellChannelRefType.Machine).Sum(m => m.LockQuantity);
                    prdProductByCache.RefSku.SellQuantity = refSkuStock.Stocks.Where(m => m.RefType == Entity.E_StoreSellChannelRefType.Machine).Sum(m => m.SellQuantity);
                    prdProductByCache.RefSku.SalePrice = refSkuStock.Stocks[0].SalePrice;
                    prdProductByCache.RefSku.SalePriceByVip = refSkuStock.Stocks[0].SalePriceByVip;
                    prdProductByCache.RefSku.IsOffSell = refSkuStock.Stocks[0].IsOffSell;
                }
            }

            prdProductByCache.BriefDes = prdProductByCache.BriefDes.NullToEmpty();

            return prdProductByCache;
        }

        public PrdProductSkuModel GetProductSku(string storeId, string productSkuId)
        {

            var prdProductSkuByCache = RedisHashUtil.Get<PrdProductSkuModel>(redis_key_productSku_info, productSkuId);

            //如商品信息从缓存取不到，读取数据库信息加载
            if (prdProductSkuByCache == null)
            {
                var prdProductRefSkuByDb = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                if (prdProductRefSkuByDb == null)
                    return null;

                var prdProduct = GetProduct(storeId, prdProductRefSkuByDb.PrdProductId);
                if (prdProduct == null)
                    return null;

                prdProductSkuByCache = new PrdProductSkuModel();
                prdProductSkuByCache.Id = prdProductRefSkuByDb.Id;
                prdProductSkuByCache.Name = prdProductRefSkuByDb.Name;
                prdProductSkuByCache.DispalyImgUrls = prdProduct.DispalyImgUrls;
                prdProductSkuByCache.MainImgUrl = prdProduct.MainImgUrl;
                prdProductSkuByCache.DetailsDes = prdProduct.DetailsDes;
                prdProductSkuByCache.BriefDes = prdProduct.BriefDes;
                prdProductSkuByCache.SpecDes = prdProductRefSkuByDb.SpecDes;

                RedisManager.Db.HashSetAsync(redis_key_productSku_info, productSkuId, Newtonsoft.Json.JsonConvert.SerializeObject(prdProductSkuByCache), StackExchange.Redis.When.Always);
            }

            var skuStock = CacheServiceFactory.StoreSellChannelStock.GetStock(storeId, productSkuId);
            if (skuStock == null)
            {
                LogUtil.Info(string.Format("库存,ProductSku,StoreId：{0},SkuId:{1},数据为NULL", storeId, productSkuId));
            }
            else
            {
                if (skuStock.Stocks == null || skuStock.Stocks.Count == 0)
                {
                    LogUtil.Info(string.Format("库存,ProductSku,StoreId：{0},SkuId:{1},Stocks为NULL", storeId, productSkuId));
                }
                else
                {

                    prdProductSkuByCache.SalePrice = skuStock.Stocks[0].SalePrice;
                    prdProductSkuByCache.SalePriceByVip = skuStock.Stocks[0].SalePriceByVip;
                    prdProductSkuByCache.IsOffSell = skuStock.Stocks[0].IsOffSell;

                    foreach (var stock in skuStock.Stocks)
                    {
                        var l_stock = new PrdProductSkuModel.Stock();
                        l_stock.RefId = stock.RefId;
                        l_stock.RefType = stock.RefType;
                        l_stock.SlotId = stock.SlotId;
                        l_stock.SumQuantity = stock.SumQuantity;
                        l_stock.SellQuantity = stock.SellQuantity;
                        l_stock.LockQuantity = stock.LockQuantity;

                        if (prdProductSkuByCache.Stocks == null)
                        {
                            prdProductSkuByCache.Stocks = new List<PrdProductSkuModel.Stock>();
                        }

                        prdProductSkuByCache.Stocks.Add(l_stock);
                    }
                }
            }


            prdProductSkuByCache.BriefDes = prdProductSkuByCache.BriefDes.NullToEmpty();



            return prdProductSkuByCache;
        }



    }
}
