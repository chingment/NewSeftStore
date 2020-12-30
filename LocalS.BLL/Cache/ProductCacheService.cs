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
            var r_spu = RedisHashUtil.Get<ProductSpuInfoModel>(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), productId.ToLower());
            if (r_spu != null)
            {
                var specIdxSkus = r_spu.SpecIdxSkus;
                if (specIdxSkus != null && specIdxSkus.Count > 0)
                {
                    foreach (var specIdxSku in specIdxSkus)
                    {
                        var r_sku = GetSkuInfo(merchId, specIdxSku.SkuId);

                        if (r_sku != null)
                        {
                            RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("*:{0}", r_sku.Id.ToLower()));
                            RedisHashUtil.Remove(string.Format(RedisKeyS.PRD_SKU_INF, merchId.ToLower()), r_sku.Id.ToLower());
                        }
                    }
                }
            }

            RedisHashUtil.Remove(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), productId.ToLower());
            RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("*:{0}", productId.ToLower()));
        }

        public ProductSpuInfoModel GetSpuInfo(string merchId, string productId)
        {
            var r_spu = RedisHashUtil.Get<ProductSpuInfoModel>(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), productId.ToLower());

            if (r_spu == null)
            {
                r_spu = new ProductSpuInfoModel();
                r_spu.Id = productId;

                var d_spu = CurrentDb.PrdProduct.Where(m => m.Id == productId).FirstOrDefault();

                if (d_spu != null)
                {
                    r_spu.PinYinIndex = d_spu.PinYinIndex;
                    r_spu.Name = d_spu.Name.NullToEmpty();
                    r_spu.SpuCode = d_spu.SpuCode.NullToEmpty();
                    r_spu.DisplayImgUrls = d_spu.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                    r_spu.MainImgUrl = d_spu.MainImgUrl;
                    r_spu.DetailsDes = d_spu.DetailsDes.ToJsonObject<List<ImgSet>>();
                    r_spu.BriefDes = d_spu.BriefDes.NullToEmpty();
                    r_spu.SpecItems = d_spu.SpecItems.ToJsonObject<List<SpecItem>>();
                    r_spu.CharTags = d_spu.CharTags.ToJsonObject<List<string>>();
                    r_spu.IsTrgVideoService = d_spu.IsTrgVideoService;
                    r_spu.IsRevService = d_spu.IsRevService;
                    r_spu.IsHardware = d_spu.IsHardware;
                    r_spu.SupReceiveMode = d_spu.SupReceiveMode;
                    r_spu.KindId1 = d_spu.PrdKindId1;
                    r_spu.KindId2 = d_spu.PrdKindId2;
                    r_spu.KindId3 = d_spu.PrdKindId3;

                    var d_skus = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == productId).ToList();

                    foreach (var d_sku in d_skus)
                    {
                        r_spu.SpecIdxSkus.Add(new SpecIdxSku { SkuId = d_sku.Id, SpecIdx = d_sku.SpecIdx });

                        if (!string.IsNullOrEmpty(d_sku.CumCode))
                        {
                            RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("CC:{0}:{1}", d_sku.CumCode.ToLower(), productId.ToLower()), productId.ToLower(), StackExchange.Redis.When.Always);
                        }
                    }


                    if (!string.IsNullOrEmpty(r_spu.PinYinIndex))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("PY:{0}:{1}", r_spu.PinYinIndex.ToLower(), productId.ToLower()), productId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_spu.Name))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("NA:{0}:{1}", r_spu.Name.ToLower(), productId.ToLower()), productId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_spu.SpuCode))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("SC:{0}:{1}", r_spu.SpuCode.ToLower(), productId.ToLower()), productId.ToLower(), StackExchange.Redis.When.Always);
                    }
                }

                RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), productId.ToLower(), JsonConvertUtil.SerializeObject(r_spu), StackExchange.Redis.When.Always);
            }

            return r_spu;
        }

        public ProductSkuInfoModel GetSkuStock(string merchId, string storeId, string[] sellChannelRefIds, string productSkuId)
        {
            var productSkuInfo = GetSkuInfo(merchId, productSkuId);

            var productSkuStocks = new List<ProductSkuStockModel>();

            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && sellChannelRefIds.Contains(m.SellChannelRefId) && m.PrdProductSkuId == productSkuId).ToList();

            foreach (var sellChannelStock in sellChannelStocks)
            {
                var productSkuStock = new ProductSkuStockModel();
                productSkuStock.RefType = sellChannelStock.SellChannelRefType;
                productSkuStock.RefId = sellChannelStock.SellChannelRefId;
                productSkuStock.CabinetId = sellChannelStock.CabinetId;
                productSkuStock.SlotId = sellChannelStock.SlotId;
                productSkuStock.SumQuantity = sellChannelStock.SumQuantity;
                productSkuStock.LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;
                productSkuStock.SellQuantity = sellChannelStock.SellQuantity;
                productSkuStock.IsOffSell = sellChannelStock.IsOffSell;
                productSkuStock.SalePrice = sellChannelStock.SalePrice;
                productSkuStock.IsUseRent = sellChannelStock.IsUseRent;
                productSkuStock.RentMhPrice = sellChannelStock.RentMhPrice;
                productSkuStock.DepositPrice = sellChannelStock.DepositPrice;
                productSkuStocks.Add(productSkuStock);
            }


            productSkuInfo.Stocks = productSkuStocks;

            return productSkuInfo;
        }

        public ProductSkuInfoModel GetSkuInfo(string merchId, string productSkuId)
        {
            var r_sku = RedisHashUtil.Get<ProductSkuInfoModel>(string.Format(RedisKeyS.PRD_SKU_INF, merchId.ToLower()), productSkuId.ToLower());
            //判断商品信息在缓存数据库是否存在，不存在则加载数据到缓存中
            if (r_sku == null)
            {
                r_sku = new ProductSkuInfoModel();
                r_sku.Id = productSkuId;
                var d_sku = CurrentDb.PrdProductSku.Where(m => m.Id == productSkuId).FirstOrDefault();
                if (d_sku != null)
                {
                    var r_spu = GetSpuInfo(merchId, d_sku.PrdProductId);

                    r_sku = new ProductSkuInfoModel();
                    r_sku.Id = d_sku.Id;
                    r_sku.BarCode = d_sku.BarCode;
                    r_sku.SpuCode = r_spu.SpuCode;
                    r_sku.CumCode = d_sku.CumCode;
                    r_sku.PinYinIndex = d_sku.PinYinIndex;
                    r_sku.ProductId = d_sku.PrdProductId;
                    r_sku.Name = d_sku.Name.NullToEmpty();
                    r_sku.DisplayImgUrls = r_spu.DisplayImgUrls;
                    r_sku.MainImgUrl = r_spu.MainImgUrl;
                    r_sku.DetailsDes = r_spu.DetailsDes;
                    r_sku.BriefDes = r_spu.BriefDes;
                    r_sku.SpecItems = r_spu.SpecItems;
                    r_sku.CharTags = r_spu.CharTags;
                    r_sku.SpecDes = d_sku.SpecDes.ToJsonObject<List<SpecDes>>();
                    r_sku.SpecIdx = d_sku.SpecIdx;
                    r_sku.SpecIdxSkus = r_spu.SpecIdxSkus;
                    r_sku.IsTrgVideoService = r_spu.IsTrgVideoService;
                    r_sku.IsRevService = r_spu.IsRevService;
                    r_sku.IsHardware = r_spu.IsHardware;
                    r_sku.SupReceiveMode = r_spu.SupReceiveMode;
                    r_sku.KindId1 = r_spu.KindId1;
                    r_sku.KindId2 = r_spu.KindId2;
                    r_sku.KindId3 = r_spu.KindId3;
                    if (!string.IsNullOrEmpty(r_sku.BarCode))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("BC:{0}:{1}", r_sku.BarCode.ToLower(), productSkuId.ToLower()), productSkuId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_sku.PinYinIndex))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("PY:{0}:{1}", r_sku.PinYinIndex.ToLower(), productSkuId.ToLower()), productSkuId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_sku.Name))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("NA:{0}:{1}", r_sku.Name.ToLower(), productSkuId.ToLower()), productSkuId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_sku.CumCode))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("CC:{0}:{1}", r_sku.CumCode.ToLower(), productSkuId.ToLower()), productSkuId.ToLower(), StackExchange.Redis.When.Always);
                    }
                }

                RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_INF, merchId.ToLower()), productSkuId.ToLower(), JsonConvertUtil.SerializeObject(r_sku), StackExchange.Redis.When.Always);
            }

            return r_sku;
        }

        public List<ProductSkuInfoBySearchModel> SearchSku(string merchId, string type, string key)
        {
            List<ProductSkuInfoBySearchModel> m_searchs = new List<ProductSkuInfoBySearchModel>();

            if (string.IsNullOrEmpty(key))
                return m_searchs;

            List<RedisValue> r_skuIds = new List<RedisValue>();


            var search_Keys = RedisManager.Db.HashScan(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("*{0}*", key.ToLower()));
            foreach (var item in search_Keys)
            {
                r_skuIds.Add(item.Value);
            }

            r_skuIds = r_skuIds.Distinct().Take(10).ToList();

            LogUtil.Info("productSkuIds.length:" + r_skuIds.Count);

            if (r_skuIds.Count > 0)
            {
                var r_skus = RedisManager.Db.HashGet(string.Format(RedisKeyS.PRD_SKU_INF, merchId.ToLower()), r_skuIds.ToArray());

                foreach (var r_sku in r_skus)
                {
                    try
                    {
                        var l_sku = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductSkuInfoModel>(r_sku);
                        var m_search = new ProductSkuInfoBySearchModel();
                        m_search.ProductSkuId = l_sku.Id;
                        m_search.ProductId = l_sku.ProductId;
                        m_search.Name = l_sku.Name;
                        m_search.SpuCode = l_sku.SpuCode;
                        m_search.CumCode = l_sku.CumCode;
                        m_search.BarCode = l_sku.BarCode;
                        m_search.SpecDes = SpecDes.GetDescribe(l_sku.SpecDes);
                        m_search.MainImgUrl = ImgSet.Convert_S(l_sku.MainImgUrl);
                        m_searchs.Add(m_search);
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }

            return m_searchs;
        }

        public List<ProductSpuInfoBySearchModel> SearchSpu(string merchId, string type, string key)
        {
            List<ProductSpuInfoBySearchModel> m_searchs = new List<ProductSpuInfoBySearchModel>();

            if (string.IsNullOrEmpty(key))
                return m_searchs;

            List<RedisValue> r_spuIds = new List<RedisValue>();


            var search_Keys = RedisManager.Db.HashScan(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("*{0}*", key.ToLower()));
            foreach (var item in search_Keys)
            {
                r_spuIds.Add(item.Value);
            }

            r_spuIds = r_spuIds.Distinct().Take(10).ToList();

            LogUtil.Info("productIds.length:" + r_spuIds.Count);

            if (r_spuIds.Count > 0)
            {
                var r_spus = RedisManager.Db.HashGet(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), r_spuIds.ToArray());

                foreach (var r_spu in r_spus)
                {
                    try
                    {
                        var l_spu = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductSpuInfoModel>(r_spu);
                        var m_search = new ProductSpuInfoBySearchModel();
                        m_search.ProductId = l_spu.Id;
                        m_search.Name = l_spu.Name;
                        m_search.SpuCode = l_spu.SpuCode;
                        m_search.MainImgUrl = ImgSet.Convert_S(l_spu.MainImgUrl);
                        m_searchs.Add(m_search);
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }

            return m_searchs;
        }

        public void ReLoad()
        {
            var merchs = CurrentDb.Merch.ToList();
            foreach (var merch in merchs)
            {
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SKU_SKEY, merch.Id.ToLower()));
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SKU_INF, merch.Id.ToLower()));
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SPU_SKEY, merch.Id.ToLower()));
                RedisManager.Db.KeyDelete(string.Format(RedisKeyS.PRD_SPU_INF, merch.Id.ToLower()));

                var d_skus = CurrentDb.PrdProductSku.Where(m => m.MerchId == merch.Id).ToList();
                foreach (var d_sku in d_skus)
                {
                    GetSkuInfo(merch.Id, d_sku.Id);
                }

            }
        }
    }
}
