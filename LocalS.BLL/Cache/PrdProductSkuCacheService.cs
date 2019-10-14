using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public enum StockOperateType
    {
        Unknow = 0,
        OrderReserveSuccess = 1,
        //OrderReserveFailure = 2,
        OrderPaySuccess = 3,
        //OrderPayFailure = 4,
        OrderCancle = 5
        //OrderPayTimeout = 6
    }

    public class PrdProductSkuCacheService : BaseDbContext
    {
        private static readonly string redis_key_productSku_info = "info:ProductSku";
        private static readonly string redis_key_productSku_stock = "stock:ProductSku:{0}";
        private static readonly string redis_key_productSku_search = "search:ProductSku:{0}";
        public void RemoveInfo(string productSkuId)
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
            var prdProductSkuModel = RedisHashUtil.Get<ProductSkuInfoModel>(redis_key_productSku_info, productSkuId);

            //如商品信息从缓存取不到，读取数据库信息加载
            if (prdProductSkuModel == null)
            {
                var prdProductSkuByDb = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                if (prdProductSkuByDb == null)
                    return null;

                var prdProduct = CacheServiceFactory.Product.GetInfo(prdProductSkuByDb.PrdProductId);
                if (prdProduct == null)
                    return null;

                prdProductSkuModel = new ProductSkuInfoModel();
                prdProductSkuModel.Id = prdProductSkuByDb.Id;
                prdProductSkuModel.PrdProductId = prdProductSkuByDb.PrdProductId;
                prdProductSkuModel.Name = prdProductSkuByDb.Name.NullToEmpty();
                prdProductSkuModel.DisplayImgUrls = prdProduct.DisplayImgUrls;
                prdProductSkuModel.MainImgUrl = prdProduct.MainImgUrl.NullToEmpty();
                prdProductSkuModel.DetailsDes = prdProduct.DetailsDes.NullToEmpty();
                prdProductSkuModel.BriefDes = prdProduct.BriefDes.NullToEmpty();
                prdProductSkuModel.SpecDes = prdProductSkuByDb.SpecDes.NullToEmpty();

                //RedisManager.Db.HashSetAsync(string.Format(redis_key_productSku_search,prdProductSkuByDb.MerchId, "barcode:" + prdProductSkuModel.BarCode + ",name:" + sku.Name + ",simplecode:" + p, Newtonsoft.Json.JsonConvert.SerializeObject(prdProductSkuModel), StackExchange.Redis.When.Always);

                RedisManager.Db.HashSetAsync(redis_key_productSku_info, productSkuId, Newtonsoft.Json.JsonConvert.SerializeObject(prdProductSkuModel), StackExchange.Redis.When.Always);
            }

            return prdProductSkuModel;
        }

        public List<ProductSkuStockModel> GetStock(string productSkuId)
        {
            var redis = new RedisClient<List<ProductSkuStockModel>>();

            var productSkuStockModels = redis.KGet(string.Format(redis_key_productSku_stock, productSkuId));

            if (productSkuStockModels == null)
            {
                productSkuStockModels = new List<ProductSkuStockModel>();

                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.PrdProductSkuId == productSkuId).ToList();

                foreach (var sellChannelStock in sellChannelStocks)
                {
                    var productSkuStockModel = new ProductSkuStockModel();
                    productSkuStockModel.RefType = sellChannelStock.RefType;
                    productSkuStockModel.RefId = sellChannelStock.RefId;
                    productSkuStockModel.SlotId = sellChannelStock.SlotId;
                    productSkuStockModel.SumQuantity = sellChannelStock.SumQuantity;
                    productSkuStockModel.LockQuantity = sellChannelStock.LockQuantity;
                    productSkuStockModel.SellQuantity = sellChannelStock.SellQuantity;
                    productSkuStockModel.IsOffSell = sellChannelStock.IsOffSell;
                    productSkuStockModel.SalePrice = sellChannelStock.SalePrice;
                    productSkuStockModel.SalePriceByVip = sellChannelStock.SalePriceByVip;
                    productSkuStockModels.Add(productSkuStockModel);
                }

                redis.KSet(string.Format(redis_key_productSku_stock, productSkuId), productSkuStockModels, new TimeSpan(100, 0, 0));
            }

            return productSkuStockModels;
        }

        public void StockOperate(StockOperateType operateType, string productSkuId, Entity.E_SellChannelRefType refType, string refId, string slotId, int quantity)
        {
            var redis = new RedisClient<List<ProductSkuStockModel>>();

            var stock = GetStock(productSkuId);

            for (int i = 0; i < stock.Count; i++)
            {
                if (stock[i].RefType == refType && stock[i].RefId == refId && stock[i].SlotId == slotId)
                {
                    switch (operateType)
                    {
                        case StockOperateType.OrderReserveSuccess:
                            stock[i].LockQuantity += quantity;
                            stock[i].SellQuantity -= quantity;
                            break;
                        case StockOperateType.OrderPaySuccess:
                            stock[i].LockQuantity -= quantity;
                            stock[i].SumQuantity -= quantity;
                            break;
                        case StockOperateType.OrderCancle:
                            stock[i].LockQuantity -= quantity;
                            stock[i].SellQuantity += quantity;
                            break;
                    }
                }
            }

            redis.KSet(string.Format(redis_key_productSku_stock, productSkuId), stock, new TimeSpan(100, 0, 0));
        }
    }
}
