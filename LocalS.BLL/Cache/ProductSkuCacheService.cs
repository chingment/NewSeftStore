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

    public class ProductSkuCacheService : BaseDbContext
    {
        private static readonly string redis_key_all_sku_info_by_merchId = "info_sku_all:{0}";
        private static readonly string redis_key_one_sku_stock_by_productId = "stock_sku_one:{0}";
        private static readonly string redis_key_all_sku_search_by_merchId = "search_sku_all:{0}";
        public void Remove(string merchId, string productSkuId)
        {
            RedisHashUtil.Remove(string.Format(redis_key_all_sku_info_by_merchId, merchId, productSkuId));
            RedisHashUtil.Remove(string.Format(redis_key_all_sku_search_by_merchId, merchId, productSkuId));
            RedisHashUtil.Remove(string.Format(redis_key_one_sku_stock_by_productId, productSkuId));
        }

        private ProductSkuInfoAndStockModel GetInfoAndStock(string merchId, string productSkuId)
        {
            var productSkuInfoAndStockModel = new ProductSkuInfoAndStockModel();

            var productSkuInfo = GetInfo(merchId, productSkuId);

            if (productSkuInfo == null)
            {
                return null;
            }

            productSkuInfoAndStockModel.Id = productSkuInfo.Id;
            productSkuInfoAndStockModel.ProductId = productSkuInfo.PrdProductId;
            productSkuInfoAndStockModel.Name = productSkuInfo.Name;
            productSkuInfoAndStockModel.MainImgUrl = productSkuInfo.MainImgUrl;
            productSkuInfoAndStockModel.DisplayImgUrls = productSkuInfo.DisplayImgUrls;
            productSkuInfoAndStockModel.DetailsDes = productSkuInfo.DetailsDes;
            productSkuInfoAndStockModel.BriefDes = productSkuInfo.BriefDes;
            productSkuInfoAndStockModel.SpecDes = productSkuInfo.BriefDes;
            productSkuInfoAndStockModel.Stocks = GetStock(merchId, productSkuId);


            return productSkuInfoAndStockModel;
        }


        public ProductSkuInfoAndStockModel GetInfoAndStock(string merchId, string[] machineIds, string productSkuId)
        {
            var productSku = GetInfoAndStock(merchId, productSkuId);

            if (productSku.Stocks != null)
            {
                if (productSku.Stocks.Count > 0)
                {
                    var stocks = productSku.Stocks.Where(m => m.RefType == Entity.E_SellChannelRefType.Machine && machineIds.Contains(m.RefId)).ToList();
                    productSku.Stocks = stocks;

                }
            }

            return productSku;
        }

        public ProductSkuInfoModel GetInfo(string merchId, string productSkuId)
        {
            var prdProductSkuModel = RedisHashUtil.Get<ProductSkuInfoModel>(string.Format(redis_key_all_sku_info_by_merchId, merchId), productSkuId);

            //如商品信息从缓存取不到，读取数据库信息加载
            if (prdProductSkuModel == null)
            {
                var prdProductSkuByDb = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                if (prdProductSkuByDb == null)
                    return null;

                var prdProductDb = CurrentDb.PrdProduct.Where(m => m.Id == prdProductSkuByDb.PrdProductId).FirstOrDefault();
                if (prdProductDb == null)
                    return null;

                prdProductSkuModel = new ProductSkuInfoModel();
                prdProductSkuModel.Id = prdProductSkuByDb.Id;
                prdProductSkuModel.BarCode = prdProductSkuByDb.BarCode;
                prdProductSkuModel.PinYinIndx = prdProductSkuByDb.PinYinIndex;
                prdProductSkuModel.PrdProductId = prdProductSkuByDb.PrdProductId;
                prdProductSkuModel.Name = prdProductSkuByDb.Name.NullToEmpty();
                prdProductSkuModel.DisplayImgUrls = prdProductDb.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                prdProductSkuModel.MainImgUrl = ImgSet.GetMain(prdProductDb.DisplayImgUrls);
                prdProductSkuModel.DetailsDes = prdProductDb.DetailsDes.NullToEmpty();
                prdProductSkuModel.BriefDes = prdProductDb.BriefDes.NullToEmpty();
                prdProductSkuModel.SpecDes = prdProductSkuByDb.SpecDes.NullToEmpty();

                var productSkuInfoBySearchModel = new ProductSkuInfoBySearchModel { Id = prdProductSkuByDb.Id, BarCode = prdProductSkuByDb.BarCode, Name = prdProductSkuModel.Name, MainImgUrl = prdProductSkuModel.MainImgUrl };

                RedisManager.Db.HashSetAsync(string.Format(redis_key_all_sku_search_by_merchId, prdProductSkuByDb.MerchId), "barcode:" + prdProductSkuModel.BarCode + ",pyindex:" + prdProductSkuModel.PinYinIndx + ",name:" + prdProductSkuModel.Name, Newtonsoft.Json.JsonConvert.SerializeObject(productSkuInfoBySearchModel), StackExchange.Redis.When.Always);

                RedisManager.Db.HashSetAsync(string.Format(redis_key_all_sku_info_by_merchId, prdProductSkuByDb.MerchId), productSkuId, Newtonsoft.Json.JsonConvert.SerializeObject(prdProductSkuModel), StackExchange.Redis.When.Always);
            }

            return prdProductSkuModel;
        }

        public List<ProductSkuStockModel> GetStock(string merchId, string productSkuId)
        {
            var redis = new RedisClient<List<ProductSkuStockModel>>();

            var productSkuStockModels = redis.KGet(string.Format(redis_key_one_sku_stock_by_productId, productSkuId));

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

                redis.KSet(string.Format(redis_key_one_sku_stock_by_productId, productSkuId), productSkuStockModels, new TimeSpan(100, 0, 0));
            }

            return productSkuStockModels;
        }

        public void StockOperate(StockOperateType operateType, string merchId, string productSkuId, Entity.E_SellChannelRefType refType, string refId, string slotId, int quantity)
        {
            var redis = new RedisClient<List<ProductSkuStockModel>>();

            var stock = GetStock(merchId, productSkuId);

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

            redis.KSet(string.Format(redis_key_one_sku_stock_by_productId, productSkuId), stock, new TimeSpan(100, 0, 0));
        }

        public List<ProductSkuInfoBySearchModel> Search(string merchId, string key)
        {
            List<ProductSkuInfoBySearchModel> list = new List<ProductSkuInfoBySearchModel>();
            var hs = RedisManager.Db.HashGetAll(string.Format(redis_key_all_sku_search_by_merchId, merchId));

            key = key.ToUpper();

            var d = (from i in hs select i).Where(x => x.Name.ToString().Contains(key)).ToList();

            foreach (var item in d)
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductSkuInfoBySearchModel>(item.Value);
                list.Add(obj);
            }
            return list;
        }
    }
}
