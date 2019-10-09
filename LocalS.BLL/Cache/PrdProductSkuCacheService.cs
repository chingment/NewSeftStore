using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class PrdProductSkuCacheService : BaseDbContext
    {
        private static readonly string redis_key_productSku_info = "info:ProductSku";
        private static readonly string redis_key_productSku_stock = "stock:ProductSku:{0}";

        public void RemoveProductSkuInfo(string productSkuId)
        {
            RedisHashUtil.Remove(redis_key_productSku_info, productSkuId);
        }

        public ProductSkuInfoAndStockModel GetInfoAndStock(string productSkuId)
        {
            var productSkuInfoAndStockModel = new ProductSkuInfoAndStockModel();

            var productSkuInfo = GetInfo(productSkuId);
            var productSkuStock = GetStock(productSkuId);

            productSkuInfoAndStockModel.Id = productSkuInfo.Id;
            productSkuInfoAndStockModel.PrdProductId = productSkuInfo.PrdProductId;
            productSkuInfoAndStockModel.Name = productSkuInfo.Name;
            productSkuInfoAndStockModel.MainImgUrl = productSkuInfo.MainImgUrl;
            productSkuInfoAndStockModel.DisplayImgUrls = productSkuInfo.DisplayImgUrls;
            productSkuInfoAndStockModel.DetailsDes = productSkuInfo.DetailsDes;
            productSkuInfoAndStockModel.BriefDes = productSkuInfo.BriefDes;
            productSkuInfoAndStockModel.SpecDes = productSkuInfo.BriefDes;

            productSkuInfoAndStockModel.Stocks = productSkuStock;

            if (productSkuStock.Count > 0)
            {
                productSkuInfoAndStockModel.SalePrice = productSkuStock[0].SalePrice;
                productSkuInfoAndStockModel.SalePriceByVip = productSkuStock[0].SalePriceByVip;
                productSkuInfoAndStockModel.IsShowPrice = false;
                productSkuInfoAndStockModel.IsOffSell = productSkuStock[0].IsOffSell;
            }

            return productSkuInfoAndStockModel;
        }

        public ProductSkuInfoModel GetInfo(string productSkuId)
        {
            var prdProductSkuModel = new ProductSkuInfoModel();

            var prdProductSku2ByCache = RedisHashUtil.Get<ProductSkuInfoModel>(redis_key_productSku_info, productSkuId);

            //如商品信息从缓存取不到，读取数据库信息加载
            if (prdProductSku2ByCache == null)
            {
                var prdProductSkuByDb = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                if (prdProductSkuByDb == null)
                    return null;

                var prdProduct = CacheServiceFactory.Product.GetInfo(prdProductSkuByDb.PrdProductId);
                if (prdProduct == null)
                    return null;

                prdProductSku2ByCache = new ProductSkuInfoModel();
                prdProductSku2ByCache.Id = prdProductSkuByDb.Id;
                prdProductSku2ByCache.PrdProductId = prdProductSkuByDb.PrdProductId;
                prdProductSku2ByCache.Name = prdProductSkuByDb.Name.NullToEmpty();
                prdProductSku2ByCache.DisplayImgUrls = prdProduct.DisplayImgUrls;
                prdProductSku2ByCache.MainImgUrl = prdProduct.MainImgUrl.NullToEmpty();
                prdProductSku2ByCache.DetailsDes = prdProduct.DetailsDes.NullToEmpty();
                prdProductSku2ByCache.BriefDes = prdProduct.BriefDes.NullToEmpty();
                prdProductSku2ByCache.SpecDes = prdProductSkuByDb.SpecDes.NullToEmpty();

                RedisManager.Db.HashSetAsync(redis_key_productSku_info, productSkuId, Newtonsoft.Json.JsonConvert.SerializeObject(prdProductSku2ByCache), StackExchange.Redis.When.Always);
            }

            prdProductSkuModel.Id = prdProductSku2ByCache.Id;
            prdProductSkuModel.PrdProductId = prdProductSku2ByCache.PrdProductId;
            prdProductSkuModel.Name = prdProductSku2ByCache.Name;
            prdProductSkuModel.DisplayImgUrls = prdProductSku2ByCache.DisplayImgUrls;
            prdProductSkuModel.MainImgUrl = prdProductSku2ByCache.MainImgUrl;
            prdProductSkuModel.DetailsDes = prdProductSku2ByCache.DetailsDes;
            prdProductSkuModel.BriefDes = prdProductSku2ByCache.BriefDes;
            prdProductSkuModel.SpecDes = prdProductSku2ByCache.SpecDes;


            return prdProductSkuModel;
        }

        public List<ProductSkuStockModel> GetStock(string productSkuId)
        {
            var redis = new RedisClient<List<ProductSkuStockModel>>();

            var productSkuStockModels = redis.KGet(string.Format(redis_key_productSku_stock, productSkuId));

            if (productSkuStockModels == null)
            {

                var merchSellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.PrdProductSkuId == productSkuId).ToList();

                foreach (var merchSellChannelStock in merchSellChannelStocks)
                {
                    var productSkuStockModel = new ProductSkuStockModel();
                    productSkuStockModel.RefType = merchSellChannelStock.RefType;
                    productSkuStockModel.RefId = merchSellChannelStock.RefId;
                    productSkuStockModel.SlotId = merchSellChannelStock.SlotId;
                    productSkuStockModel.SumQuantity = merchSellChannelStock.SumQuantity;
                    productSkuStockModel.LockQuantity = merchSellChannelStock.LockQuantity;
                    productSkuStockModel.SellQuantity = merchSellChannelStock.SellQuantity;
                    productSkuStockModel.IsOffSell = merchSellChannelStock.IsOffSell;
                    productSkuStockModel.SalePrice = merchSellChannelStock.SalePrice;
                    productSkuStockModel.SalePriceByVip = merchSellChannelStock.SalePriceByVip;
                    productSkuStockModels.Add(productSkuStockModel);
                }

                redis.KSet(string.Format(redis_key_productSku_stock, productSkuId), productSkuStockModels, new TimeSpan(100, 0, 0));
            }

            return productSkuStockModels;
        }
    }
}
