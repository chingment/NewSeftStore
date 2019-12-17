using Lumos;
using Lumos.Redis;
using StackExchange.Redis;
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
        private static readonly string redis_key_all_sku_info_by_merchId = "info_Sku_all:{0}";
        private static readonly string redis_key_search_SkuByBarCode = "search_SkuByBarCode:{0}";
        private static readonly string redis_key_search_SkuByPinYinIndex = "search_SkuByPinYinIndex:{0}";
        private static readonly string redis_key_search_SkuByName = "search_SkuByName:{0}";

        public void Update(string merchId, string productSkuId)
        {
            var prdProductSkuModel = RedisHashUtil.Get<ProductSkuInfoModel>(string.Format(redis_key_all_sku_info_by_merchId, merchId), productSkuId);
            if (prdProductSkuModel != null)
            {
                if (!string.IsNullOrEmpty(prdProductSkuModel.BarCode))
                {
                    RedisManager.Db.HashDelete(string.Format(redis_key_search_SkuByBarCode, merchId), prdProductSkuModel.BarCode);
                }

                if (!string.IsNullOrEmpty(prdProductSkuModel.PinYinIndex))
                {
                    RedisManager.Db.HashDelete(string.Format(redis_key_search_SkuByPinYinIndex, merchId), prdProductSkuModel.PinYinIndex);
                }

                if (!string.IsNullOrEmpty(prdProductSkuModel.Name))
                {
                    RedisManager.Db.HashDelete(string.Format(redis_key_search_SkuByName, merchId), prdProductSkuModel.Name);
                }
            }

            RedisHashUtil.Remove(string.Format(redis_key_all_sku_info_by_merchId, merchId), productSkuId);
            GetInfo(merchId, productSkuId);
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
            productSkuInfoAndStockModel.ProductId = productSkuInfo.ProductId;
            productSkuInfoAndStockModel.Name = productSkuInfo.Name;
            productSkuInfoAndStockModel.MainImgUrl = productSkuInfo.MainImgUrl;
            productSkuInfoAndStockModel.DisplayImgUrls = productSkuInfo.DisplayImgUrls;
            productSkuInfoAndStockModel.DetailsDes = productSkuInfo.DetailsDes;
            productSkuInfoAndStockModel.BriefDes = productSkuInfo.BriefDes;
            productSkuInfoAndStockModel.SpecDes = productSkuInfo.SpecDes;
            productSkuInfoAndStockModel.Stocks = GetStock(merchId, productSkuId);


            return productSkuInfoAndStockModel;
        }

        public ProductSkuInfoAndStockModel GetInfoAndStock(string merchId, string storeId, string[] machineIds, string productSkuId)
        {
            var productSku = GetInfoAndStock(merchId, productSkuId);
            if (productSku != null)
            {
                productSku.Stocks = productSku.Stocks.Where(m => m.StoreId == storeId && m.RefType == Entity.E_SellChannelRefType.Machine && machineIds.Contains(m.RefId)).ToList();
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
                prdProductSkuModel.PinYinIndex = prdProductSkuByDb.PinYinIndex;
                prdProductSkuModel.ProductId = prdProductSkuByDb.PrdProductId;
                prdProductSkuModel.Name = prdProductSkuByDb.Name.NullToEmpty();
                prdProductSkuModel.DisplayImgUrls = prdProductDb.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                prdProductSkuModel.MainImgUrl = ImgSet.GetMain_O(prdProductDb.DisplayImgUrls);
                prdProductSkuModel.DetailsDes = prdProductDb.DetailsDes.NullToEmpty();
                prdProductSkuModel.BriefDes = prdProductDb.BriefDes.NullToEmpty();
                prdProductSkuModel.SpecDes = prdProductSkuByDb.SpecDes.NullToEmpty();

                if (!string.IsNullOrEmpty(prdProductSkuModel.BarCode))
                {
                    RedisManager.Db.HashSetAsync(string.Format(redis_key_search_SkuByBarCode, prdProductSkuByDb.MerchId), prdProductSkuModel.BarCode, productSkuId, StackExchange.Redis.When.Always);
                }

                if (!string.IsNullOrEmpty(prdProductSkuModel.PinYinIndex))
                {
                    RedisManager.Db.HashSetAsync(string.Format(redis_key_search_SkuByPinYinIndex, prdProductSkuByDb.MerchId), prdProductSkuModel.PinYinIndex, productSkuId, StackExchange.Redis.When.Always);
                }

                if (!string.IsNullOrEmpty(prdProductSkuModel.Name))
                {
                    RedisManager.Db.HashSetAsync(string.Format(redis_key_search_SkuByName, prdProductSkuByDb.MerchId), prdProductSkuModel.Name, productSkuId, StackExchange.Redis.When.Always);
                }

                RedisManager.Db.HashSetAsync(string.Format(redis_key_all_sku_info_by_merchId, prdProductSkuByDb.MerchId), productSkuId, Newtonsoft.Json.JsonConvert.SerializeObject(prdProductSkuModel), StackExchange.Redis.When.Always);
            }

            return prdProductSkuModel;
        }

        public List<ProductSkuStockModel> GetStock(string merchId, string productSkuId)
        {
            var productSkuStockModels = new List<ProductSkuStockModel>();

            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.PrdProductSkuId == productSkuId).ToList();

            foreach (var sellChannelStock in sellChannelStocks)
            {
                var productSkuStockModel = new ProductSkuStockModel();
                productSkuStockModel.StoreId = sellChannelStock.StoreId;
                productSkuStockModel.RefType = sellChannelStock.SellChannelRefType;
                productSkuStockModel.RefId = sellChannelStock.SellChannelRefId;
                productSkuStockModel.SlotId = sellChannelStock.SlotId;
                productSkuStockModel.SumQuantity = sellChannelStock.SumQuantity;
                productSkuStockModel.LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;
                productSkuStockModel.SellQuantity = sellChannelStock.SellQuantity;
                productSkuStockModel.IsOffSell = sellChannelStock.IsOffSell;
                productSkuStockModel.SalePrice = sellChannelStock.SalePrice;
                productSkuStockModel.SalePriceByVip = sellChannelStock.SalePriceByVip;
                productSkuStockModels.Add(productSkuStockModel);
            }

            return productSkuStockModels;
        }

        public List<ProductSkuInfoBySearchModel> Search(string merchId, string type, string key)
        {

            List<ProductSkuInfoBySearchModel> searchModels = new List<ProductSkuInfoBySearchModel>();

            List<RedisValue> productSkuIds = new List<RedisValue>();

            if (type == "BarCode" || type == "All")
            {
                var search_Scan_BarCode = RedisManager.Db.HashScan(string.Format(redis_key_search_SkuByBarCode, merchId), string.Format("{0}*", key));
                foreach (var item in search_Scan_BarCode)
                {
                    productSkuIds.Add(item.Value);
                }
            }

            if (type == "PinYinIndex" || type == "All")
            {
                var search_Scan_PinYinIndex = RedisManager.Db.HashScan(string.Format(redis_key_search_SkuByPinYinIndex, merchId), string.Format("{0}*", key));
                foreach (var item in search_Scan_PinYinIndex)
                {
                    productSkuIds.Add(item.Value);
                }
            }

            productSkuIds = productSkuIds.Distinct().ToList();


            if (productSkuIds.Count > 0)
            {
                var productSkus = RedisManager.Db.HashGet(string.Format(redis_key_all_sku_info_by_merchId, merchId), productSkuIds.ToArray());

                foreach (var productSku in productSkus)
                {
                    var productSkuModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductSkuInfoModel>(productSku);
                    var searchModel = new ProductSkuInfoBySearchModel();
                    searchModel.Id = productSkuModel.Id;
                    searchModel.Name = productSkuModel.Name;
                    searchModel.BarCode = productSkuModel.BarCode;
                    searchModel.SpecDes = productSkuModel.SpecDes;
                    searchModel.MainImgUrl = ImgSet.Convert_S(productSkuModel.MainImgUrl);
                    searchModels.Add(searchModel);
                }
            }

            return searchModels;
        }

        public void ReLoad()
        {
            var merchs = CurrentDb.Merch.ToList();
            foreach (var merch in merchs)
            {
                RedisManager.Db.KeyDelete(string.Format(redis_key_search_SkuByBarCode, merch.Id));
                RedisManager.Db.KeyDelete(string.Format(redis_key_search_SkuByPinYinIndex, merch.Id));
                RedisManager.Db.KeyDelete(string.Format(redis_key_search_SkuByName, merch.Id));
                RedisManager.Db.KeyDelete(string.Format(redis_key_all_sku_info_by_merchId, merch.Id));
            }

            foreach (var merch in merchs)
            {
                var productSkus = CurrentDb.PrdProductSku.Where(m => m.MerchId == merch.Id).ToList();
                foreach (var productSku in productSkus)
                {
                    GetInfo(merch.Id, productSku.Id);
                }
            }
        }
    }
}
