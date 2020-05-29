using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ProductCacheService : BaseDbContext
    {
        public void Update(string merchId, string productSkuId)
        {
            var prdProductSkuModel = RedisHashUtil.Get<ProductSkuInfoModel>(string.Format(RedisKeyS.PRD_SKU_INF, merchId), productSkuId);
            if (prdProductSkuModel != null)
            {
                if (!string.IsNullOrEmpty(prdProductSkuModel.BarCode))
                {
                    RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SKU_SBR, merchId), prdProductSkuModel.BarCode);
                }

                if (!string.IsNullOrEmpty(prdProductSkuModel.PinYinIndex))
                {
                    RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SKU_SPY, merchId), prdProductSkuModel.PinYinIndex);
                }

                if (!string.IsNullOrEmpty(prdProductSkuModel.Name))
                {
                    RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SKU_SNA, merchId), prdProductSkuModel.Name);
                }

                if (!string.IsNullOrEmpty(prdProductSkuModel.CumCode))
                {
                    RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SKU_SCC, merchId), prdProductSkuModel.CumCode);
                }
            }

            RedisHashUtil.Remove(string.Format(RedisKeyS.PRD_SKU_INF, merchId), productSkuId);
            GetSkuInfo(merchId, productSkuId);
        }

        public ProductSkuInfoModel GetSkuInfo(string merchId, string storeId, string[] sellChannelRefIds, string productSkuId)
        {
            var productSkuInfo = GetSkuInfo(merchId, productSkuId);

            var productSkuStocks = new List<ProductSkuStockModel>();

            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && sellChannelRefIds.Contains(m.SellChannelRefId) && m.PrdProductSkuId == productSkuId).ToList();

            foreach (var sellChannelStock in sellChannelStocks)
            {
                var productSkuStock = new ProductSkuStockModel();
                productSkuStock.StoreId = sellChannelStock.StoreId;
                productSkuStock.RefType = sellChannelStock.SellChannelRefType;
                productSkuStock.RefId = sellChannelStock.SellChannelRefId;
                productSkuStock.CabinetId = sellChannelStock.CabinetId;
                productSkuStock.SlotId = sellChannelStock.SlotId;
                productSkuStock.SumQuantity = sellChannelStock.SumQuantity;
                productSkuStock.LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;
                productSkuStock.SellQuantity = sellChannelStock.SellQuantity;
                productSkuStock.IsOffSell = sellChannelStock.IsOffSell;
                productSkuStock.SalePrice = sellChannelStock.SalePrice;
                productSkuStock.SalePriceByVip = sellChannelStock.SalePriceByVip;
                productSkuStocks.Add(productSkuStock);
            }

            productSkuInfo.Stocks = productSkuStocks;

            return productSkuInfo;
        }

        public ProductSkuInfoModel GetSkuInfo(string merchId, string productSkuId)
        {
            var productSkuByCache = RedisHashUtil.Get<ProductSkuInfoModel>(string.Format(RedisKeyS.PRD_SKU_INF, merchId), productSkuId);
            //判断商品信息在缓存数据库是否存在，不存在则加载数据到缓存中
            if (productSkuByCache == null)
            {
                bool isFindInBbIsNull = false;
                PrdProduct prdProductDb = null;
                var productSkuByDb = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                if (productSkuByDb == null)
                {
                    isFindInBbIsNull = true;
                }
                else
                {
                    prdProductDb = CurrentDb.PrdProduct.Where(m => m.Id == productSkuByDb.PrdProductId).FirstOrDefault();
                }

                if (prdProductDb == null)
                {
                    isFindInBbIsNull = true;
                }

                if (isFindInBbIsNull)
                {
                    productSkuByCache = new ProductSkuInfoModel();
                    productSkuByCache.Id = productSkuId;
                }
                else
                {
                    productSkuByCache = new ProductSkuInfoModel();
                    productSkuByCache.Id = productSkuByDb.Id;
                    productSkuByCache.BarCode = productSkuByDb.BarCode;
                    productSkuByCache.CumCode = productSkuByDb.CumCode;
                    productSkuByCache.PinYinIndex = productSkuByDb.PinYinIndex;
                    productSkuByCache.ProductId = productSkuByDb.PrdProductId;
                    productSkuByCache.Name = productSkuByDb.Name.NullToEmpty();
                    productSkuByCache.DisplayImgUrls = prdProductDb.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                    productSkuByCache.MainImgUrl = prdProductDb.MainImgUrl;
                    productSkuByCache.DetailsDes = prdProductDb.DetailsDes.ToJsonObject<List<ImgSet>>();
                    productSkuByCache.BriefDes = prdProductDb.BriefDes.NullToEmpty();
                    productSkuByCache.SpecItems = prdProductDb.SpecItems.ToJsonObject<List<SpecItem>>();
                    productSkuByCache.SpecDes = productSkuByDb.SpecDes.ToJsonObject<List<SpecDes>>();
                    productSkuByCache.SpecIdx = productSkuByDb.SpecIdx;
                    productSkuByCache.IsTrgVideoService = prdProductDb.IsTrgVideoService;
                    productSkuByCache.CharTags = prdProductDb.CharTags.ToJsonObject<List<string>>();

                    if (!string.IsNullOrEmpty(productSkuByCache.BarCode))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SBR, merchId), productSkuByCache.BarCode.ToUpper(), productSkuId, StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(productSkuByCache.PinYinIndex))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SPY, merchId), productSkuByCache.PinYinIndex.ToUpper(), productSkuId, StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(productSkuByCache.Name))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SNA, merchId), productSkuByCache.Name, productSkuId, StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(productSkuByCache.CumCode))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SCC, merchId), productSkuByCache.CumCode.ToUpper(), productSkuId, StackExchange.Redis.When.Always);
                    }

                    var setting = new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    };

                    RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_INF, merchId), productSkuId, Newtonsoft.Json.JsonConvert.SerializeObject(productSkuByCache, setting), StackExchange.Redis.When.Always);
                }
            }

            return productSkuByCache;
        }

        public List<ProductSkuInfoBySearchModel> Search(string merchId, string type, string key)
        {

            List<ProductSkuInfoBySearchModel> searchModels = new List<ProductSkuInfoBySearchModel>();

            List<RedisValue> productSkuIds = new List<RedisValue>();

            if (type == "BarCode" || type == "All")
            {
                var search_Scan_BarCode = RedisManager.Db.HashScan(string.Format(RedisKeyS.PRD_SKU_SBR, merchId), string.Format("*{0}*", key.ToUpper()));
                foreach (var item in search_Scan_BarCode)
                {
                    productSkuIds.Add(item.Value);
                }
            }

            if (type == "PinYinIndex" || type == "All")
            {
                var search_Scan_PinYinIndex = RedisManager.Db.HashScan(string.Format(RedisKeyS.PRD_SKU_SPY, merchId), string.Format("*{0}*", key.ToUpper()));
                foreach (var item in search_Scan_PinYinIndex)
                {
                    productSkuIds.Add(item.Value);
                }
            }

            if (type == "CumCode" || type == "All")
            {
                var search_Scan_CumCode = RedisManager.Db.HashScan(string.Format(RedisKeyS.PRD_SKU_SCC, merchId), string.Format("*{0}*", key.ToUpper()));
                foreach (var item in search_Scan_CumCode)
                {
                    productSkuIds.Add(item.Value);
                }
            }

            if (type == "Name" || type == "All")
            {
                var search_Scan_Name = RedisManager.Db.HashScan(string.Format(RedisKeyS.PRD_SKU_SNA, merchId), string.Format("*{0}*", key));
                foreach (var item in search_Scan_Name)
                {
                    productSkuIds.Add(item.Value);
                }
            }

            productSkuIds = productSkuIds.Distinct().ToList();


            if (productSkuIds.Count > 0)
            {
                var productSkus = RedisManager.Db.HashGet(string.Format(RedisKeyS.PRD_SKU_INF, merchId), productSkuIds.ToArray());

                foreach (var productSku in productSkus)
                {
                    var productSkuModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductSkuInfoModel>(productSku);
                    var searchModel = new ProductSkuInfoBySearchModel();
                    searchModel.Id = productSkuModel.Id;
                    searchModel.Name = productSkuModel.Name;
                    searchModel.CumCode = productSkuModel.CumCode;
                    searchModel.BarCode = productSkuModel.BarCode;
                    searchModel.SpecDes = SpecDes.GetDescribe(productSkuModel.SpecDes);
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
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SKU_SBR, merch.Id));
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SKU_SPY, merch.Id));
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SKU_SNA, merch.Id));
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SKU_SCC, merch.Id));
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SKU_INF, merch.Id));
            }

            foreach (var merch in merchs)
            {
                var productSkus = CurrentDb.PrdProductSku.Where(m => m.MerchId == merch.Id).ToList();
                foreach (var productSku in productSkus)
                {

                    //List<object> a = new List<object>();

                    //a.Add(new { name = "单规格", value = productSku.SpecDes });

                    //productSku.SpecDes = a.ToJsonString();
                    //CurrentDb.SaveChanges();

                    GetSkuInfo(merch.Id, productSku.Id);
                }
            }
        }
    }
}
