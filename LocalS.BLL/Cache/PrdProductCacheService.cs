using Lumos;
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
        private static readonly string redis_key_product_info = "entity:LocalS.Entity.ProductInfo";
        private static readonly string redis_key_product_stock = "entity:LocalS.Entity.ProductStock:{0}_{1}";

        public PrdProductModel GetModelById(string storeId, string productId)
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

            var storeSellChannelStocks = CurrentDb.StoreSellChannelStock.Where(m => m.StoreId == storeId && m.PrdProductId == productId && m.PrdProductSkuId == prdProductByCache.RefSku.Id).ToList();

            var prdProductSkuStocksByCache = new List<PrdProductSkuStockModel>();

            foreach (var storeSellChannelStock in storeSellChannelStocks)
            {
                var prdProductSkuStockByCache = new PrdProductSkuStockModel();
                prdProductSkuStockByCache.PrdProductId = storeSellChannelStock.PrdProductId;
                prdProductSkuStockByCache.PrdProductSkuId = storeSellChannelStock.PrdProductSkuId;
                prdProductSkuStockByCache.RefId = storeSellChannelStock.RefId;
                prdProductSkuStockByCache.RefType = storeSellChannelStock.RefType;
                prdProductSkuStockByCache.SalePrice = storeSellChannelStock.SalePrice;
                prdProductSkuStockByCache.SalePriceByVip = storeSellChannelStock.SalePriceByVip;
                prdProductSkuStockByCache.SellQuantity = storeSellChannelStock.SellQuantity;
                prdProductSkuStockByCache.LockQuantity = storeSellChannelStock.LockQuantity;
                prdProductSkuStockByCache.SumQuantity = storeSellChannelStock.SumQuantity;
                prdProductSkuStockByCache.IsOffSell = storeSellChannelStock.IsOffSell;
                prdProductSkuStocksByCache.Add(prdProductSkuStockByCache);
            }

            var refSkus = prdProductSkuStocksByCache.Where(m => m.PrdProductSkuId == prdProductByCache.RefSku.Id).ToList();

            if (refSkus.Count == 0)
                return null;


            prdProductByCache.RefSku.SumQuantity = refSkus.Sum(m => m.SumQuantity);
            prdProductByCache.RefSku.LockQuantity = refSkus.Sum(m => m.LockQuantity);
            prdProductByCache.RefSku.SellQuantity = refSkus.Sum(m => m.SellQuantity);
            prdProductByCache.RefSku.SalePrice = refSkus[0].SalePrice;
            prdProductByCache.RefSku.SalePriceByVip = refSkus[0].SalePriceByVip;
            prdProductByCache.RefSku.IsOffSell = refSkus[0].IsOffSell;


            // prdProductByCache.RefSku.

            //for (var i = 0; i < prdProductByCache.AllSkus.Count; i++)
            //{
            //    string skuId = prdProductByCache.AllSkus[i].Id;
            //    var stockSku = prdProductSkuStocksByCache.Where(m => m.PrdProductSkuId == skuId).FirstOrDefault();
            //    if (stockSku != null)
            //    {
            //        prdProductByCache.AllSkus[i].SalePrice = stockSku.SalePrice;

            //        if (prdProductByCache.RefSku.Id == skuId)
            //        {
            //            prdProductByCache.RefSku.ShowPrice = stockSku.SalePrice;
            //        }
            //    }
            //}


            //if (prdProductByCache != null)
            //{
            //    if (prdProductByCache.RefSku.SalePrice == prdProductByCache.RefSku.ShowPrice)
            //    {
            //        prdProductByCache.RefSku.IsShowPrice = false;
            //    }

            //    if (prdProductByCache.RefSku.ShowPrice <= 0)
            //    {
            //        prdProductByCache.RefSku.IsShowPrice = false;
            //    }
            //}

            return prdProductByCache;
        }

        public PrdProductSkuModel GetSkuModelById(string storeId, string productSkuId)
        {
            return null;
        }
    }
}
