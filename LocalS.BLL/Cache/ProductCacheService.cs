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
        public void RemoveSpuInfo(string merchId, string productId)
        {
            var productSpuInfoModel = RedisHashUtil.Get<ProductSpuInfoModel>(string.Format(RedisKeyS.PRD_SKU_INF, merchId), productId);
            if (productSpuInfoModel != null)
            {

                var specIdxSkus = productSpuInfoModel.SpecIdxSkus;
                if (specIdxSkus != null && specIdxSkus.Count > 0)
                {
                    foreach (var specIdxSku in specIdxSkus)
                    {
                        var productSkuInfoModel = GetSkuInfo(merchId, specIdxSku.SkuId);

                        if (productSkuInfoModel != null)
                        {
                            if (!string.IsNullOrEmpty(productSkuInfoModel.BarCode))
                            {
                                RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SKU_SBR, merchId), productSkuInfoModel.BarCode);
                            }

                            if (!string.IsNullOrEmpty(productSkuInfoModel.PinYinIndex))
                            {
                                RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SKU_SPY, merchId), productSkuInfoModel.PinYinIndex);
                            }

                            if (!string.IsNullOrEmpty(productSkuInfoModel.Name))
                            {
                                RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SKU_SNA, merchId), productSkuInfoModel.Name);
                            }

                            if (!string.IsNullOrEmpty(productSkuInfoModel.CumCode))
                            {
                                RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SKU_SCC, merchId), productSkuInfoModel.CumCode);
                            }

                            RedisHashUtil.Remove(string.Format(RedisKeyS.PRD_SPU_INF, merchId), productSkuInfoModel.Id);
                        }
                    }
                }
            }

            RedisHashUtil.Remove(string.Format(RedisKeyS.PRD_SPU_INF, merchId), productId);
        }

        private ProductSpuInfoModel GetSpuInfo(string merchId, string productId)
        {
            var productSpuByCache = RedisHashUtil.Get<ProductSpuInfoModel>(string.Format(RedisKeyS.PRD_SPU_INF, merchId), productId);

            if (productSpuByCache == null)
            {
                productSpuByCache = new ProductSpuInfoModel();
                productSpuByCache.Id = productId;

                var prdProductDb = CurrentDb.PrdProduct.Where(m => m.Id == productId).FirstOrDefault();

                if (prdProductDb != null)
                {
                    productSpuByCache.PinYinIndex = prdProductDb.PinYinIndex;
                    productSpuByCache.Name = prdProductDb.Name.NullToEmpty();
                    productSpuByCache.DisplayImgUrls = prdProductDb.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                    productSpuByCache.MainImgUrl = prdProductDb.MainImgUrl;
                    productSpuByCache.DetailsDes = prdProductDb.DetailsDes.ToJsonObject<List<ImgSet>>();
                    productSpuByCache.BriefDes = prdProductDb.BriefDes.NullToEmpty();
                    productSpuByCache.SpecItems = prdProductDb.SpecItems.ToJsonObject<List<SpecItem>>();
                    productSpuByCache.CharTags = prdProductDb.CharTags.ToJsonObject<List<string>>();
                    productSpuByCache.IsTrgVideoService = prdProductDb.IsTrgVideoService;

                    var productSkus = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == productId).ToList();

                    foreach (var productSku in productSkus)
                    {
                        productSpuByCache.SpecIdxSkus.Add(new SpecIdxSku { SkuId = productSku.Id, SpecIdx = productSku.SpecIdx });
                    }
                }

                RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_INF, merchId), productId, JsonConvertUtil.SerializeObject(productSpuByCache), StackExchange.Redis.When.Always);
            }

            return productSpuByCache;
        }

        public ProductSkuInfoModel GetSkuStock(string merchId, string storeId, string[] sellChannelRefIds, string productSkuId)
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
                productSkuByCache = new ProductSkuInfoModel();
                productSkuByCache.Id = productSkuId;
                var productSkuByDb = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                if (productSkuByDb != null)
                {
                    var productSpuByCache = GetSpuInfo(merchId, productSkuByDb.PrdProductId);

                    productSkuByCache = new ProductSkuInfoModel();
                    productSkuByCache.Id = productSkuByDb.Id;
                    productSkuByCache.BarCode = productSkuByDb.BarCode;
                    productSkuByCache.CumCode = productSkuByDb.CumCode;
                    productSkuByCache.PinYinIndex = productSkuByDb.PinYinIndex;
                    productSkuByCache.ProductId = productSkuByDb.PrdProductId;
                    productSkuByCache.Name = productSkuByDb.Name.NullToEmpty();
                    productSkuByCache.DisplayImgUrls = productSpuByCache.DisplayImgUrls;
                    productSkuByCache.MainImgUrl = productSpuByCache.MainImgUrl;
                    productSkuByCache.DetailsDes = productSpuByCache.DetailsDes;
                    productSkuByCache.BriefDes = productSpuByCache.BriefDes;
                    productSkuByCache.SpecItems = productSpuByCache.SpecItems;
                    productSkuByCache.CharTags = productSpuByCache.CharTags;
                    productSkuByCache.SpecDes = productSkuByDb.SpecDes.ToJsonObject<List<SpecDes>>();
                    productSkuByCache.SpecIdx = productSkuByDb.SpecIdx;
                    productSkuByCache.SpecIdxSkus = productSpuByCache.SpecIdxSkus;
                    productSkuByCache.IsTrgVideoService = productSpuByCache.IsTrgVideoService;

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
                }

                RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_INF, merchId), productSkuId, JsonConvertUtil.SerializeObject(productSkuByCache), StackExchange.Redis.When.Always);
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
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SPU_INF, merch.Id));
            }

            foreach (var merch in merchs)
            {
                var productSkus = CurrentDb.PrdProductSku.Where(m => m.MerchId == merch.Id).ToList();
                foreach (var productSku in productSkus)
                {
                    GetSkuInfo(merch.Id, productSku.Id);
                }
            }
        }
    }
}
