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
    public class ProductCacheService : BaseService
    {
        public void RemoveSpuInfo(string merchId, string spuId)
        {
            var r_spu = RedisHashUtil.Get<SpuInfoModel>(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), spuId.ToLower());
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

            RedisHashUtil.Remove(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), spuId.ToLower());
            RedisManager.Db.HashDelete(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("*:{0}", spuId.ToLower()));
        }

        public SpuInfoModel GetSpuInfo(string merchId, string spuId)
        {
            var r_Spu = RedisHashUtil.Get<SpuInfoModel>(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), spuId.ToLower());

            if (r_Spu == null)
            {
                r_Spu = new SpuInfoModel();
                r_Spu.Id = spuId;

                var d_Spu = CurrentDb.PrdSpu.Where(m => m.Id == spuId).FirstOrDefault();

                if (d_Spu != null)
                {
                    r_Spu.PinYinIndex = d_Spu.PinYinIndex;
                    r_Spu.Name = d_Spu.Name.NullToEmpty();
                    r_Spu.SpuCode = d_Spu.SpuCode.NullToEmpty();
                    r_Spu.DisplayImgUrls = d_Spu.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                    r_Spu.MainImgUrl = d_Spu.MainImgUrl;
                    r_Spu.DetailsDes = d_Spu.DetailsDes.ToJsonObject<List<ImgSet>>();
                    r_Spu.BriefDes = d_Spu.BriefDes.NullToEmpty();
                    r_Spu.SpecItems = d_Spu.SpecItems.ToJsonObject<List<SpecItem>>();
                    r_Spu.CharTags = d_Spu.CharTags.ToJsonObject<List<string>>();
                    r_Spu.IsTrgVideoService = d_Spu.IsTrgVideoService;
                    r_Spu.IsRevService = d_Spu.IsRevService;
                    r_Spu.IsMavkBuy = d_Spu.IsMavkBuy;
                    r_Spu.IsSupRentService = d_Spu.IsSupRentService;
                    r_Spu.SupReceiveMode = d_Spu.SupReceiveMode;
                    r_Spu.KindId1 = d_Spu.KindId1;
                    r_Spu.KindId2 = d_Spu.KindId2;
                    r_Spu.KindId3 = d_Spu.KindId3;

                    var d_Skus = CurrentDb.PrdSku.Where(m => m.SpuId == spuId).ToList();

                    foreach (var d_Sku in d_Skus)
                    {
                        r_Spu.SpecIdxSkus.Add(new SpecIdxSku { SkuId = d_Sku.Id, SpecIdx = d_Sku.SpecIdx });

                        if (!string.IsNullOrEmpty(d_Sku.CumCode))
                        {
                            RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("CC:{0}:{1}", d_Sku.CumCode.ToLower(), spuId.ToLower()), spuId.ToLower(), StackExchange.Redis.When.Always);
                        }
                    }


                    if (!string.IsNullOrEmpty(r_Spu.PinYinIndex))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("PY:{0}:{1}", r_Spu.PinYinIndex.ToLower(), spuId.ToLower()), spuId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_Spu.Name))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("NA:{0}:{1}", r_Spu.Name.ToLower(), spuId.ToLower()), spuId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_Spu.SpuCode))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("SC:{0}:{1}", r_Spu.SpuCode.ToLower(), spuId.ToLower()), spuId.ToLower(), StackExchange.Redis.When.Always);
                    }
                }

                RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), spuId.ToLower(), JsonConvertUtil.SerializeObject(r_Spu), StackExchange.Redis.When.Always);
            }

            return r_Spu;
        }

        public SkuInfoModel GetSkuStock(E_ShopMode shopMode, string merchId, string storeId, string shopId, string[] machineIds, string skuId)
        {
            var m_SkuInfo = GetSkuInfo(merchId, skuId);

            var m_SkuStocks = new List<SkuStockModel>();

            var sellChannelStocks = new List<SellChannelStock>();

            if (shopMode == E_ShopMode.Mall)
            {
                sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Mall && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == "0" && m.MachineId == "0" && m.SkuId == skuId).ToList();
            }
            else if (shopMode == E_ShopMode.Shop)
            {
                sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Shop && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == "0" && m.SkuId == skuId).ToList();
            }
            else if (shopMode == E_ShopMode.Machine)
            {
                if (machineIds == null || machineIds.Length == 0)
                {
                    machineIds = CurrentDb.Machine.Where(m => m.CurUseMerchId == merchId && m.CurUseStoreId == storeId && m.CurUseShopId == shopId).Select(m => m.Id).Distinct().ToArray();
                }
                if (machineIds != null && machineIds.Length > 0)
                {
                    sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Machine && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && machineIds.Contains(m.MachineId) && m.SkuId == skuId).ToList();
                }
            }

            foreach (var sellChannelStock in sellChannelStocks)
            {
                var m_SkuStock = new SkuStockModel();
                m_SkuStock.ShopMode = sellChannelStock.ShopMode;
                m_SkuStock.ShopId = sellChannelStock.ShopId;
                m_SkuStock.MachineId = sellChannelStock.MachineId;
                m_SkuStock.CabinetId = sellChannelStock.CabinetId;
                m_SkuStock.SlotId = sellChannelStock.SlotId;
                m_SkuStock.SumQuantity = sellChannelStock.SumQuantity;
                m_SkuStock.LockQuantity = sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity;
                m_SkuStock.SellQuantity = sellChannelStock.SellQuantity;
                m_SkuStock.IsOffSell = sellChannelStock.IsOffSell;
                m_SkuStock.SalePrice = sellChannelStock.SalePrice;
                m_SkuStock.IsUseRent = sellChannelStock.IsUseRent;
                m_SkuStock.RentMhPrice = sellChannelStock.RentMhPrice;
                m_SkuStock.DepositPrice = sellChannelStock.DepositPrice;
                m_SkuStocks.Add(m_SkuStock);
            }


            m_SkuInfo.Stocks = m_SkuStocks;

            return m_SkuInfo;
        }

        public List<SkuInfoModel> GetSkuInfo(string merchId, string[] skuIds)
        {
            var m_Skus = new List<SkuInfoModel>();

            foreach(var skuId in skuIds)
            {
                var m_Sku = GetSkuInfo(merchId, skuId);
                if (m_Sku != null)
                    m_Skus.Add(m_Sku);
            }

            return m_Skus;
        }

        public SkuInfoModel GetSkuInfo(string merchId, string skuId)
        {
            var r_Sku = RedisHashUtil.Get<SkuInfoModel>(string.Format(RedisKeyS.PRD_SKU_INF, merchId.ToLower()), skuId.ToLower());
            //判断商品信息在缓存数据库是否存在，不存在则加载数据到缓存中
            if (r_Sku == null)
            {
                r_Sku = new SkuInfoModel();
                r_Sku.Id = skuId;
                var d_Sku = CurrentDb.PrdSku.Where(m => m.Id == skuId).FirstOrDefault();
                if (d_Sku != null)
                {
                    var r_Spu = GetSpuInfo(merchId, d_Sku.SpuId);

                    r_Sku = new SkuInfoModel();
                    r_Sku.Id = d_Sku.Id;
                    r_Sku.BarCode = d_Sku.BarCode;
                    r_Sku.SpuCode = r_Spu.SpuCode;
                    r_Sku.CumCode = d_Sku.CumCode;
                    r_Sku.PinYinIndex = d_Sku.PinYinIndex;
                    r_Sku.SpuId = d_Sku.SpuId;
                    r_Sku.Name = d_Sku.Name.NullToEmpty();
                    r_Sku.DisplayImgUrls = r_Spu.DisplayImgUrls;
                    r_Sku.MainImgUrl = r_Spu.MainImgUrl;
                    r_Sku.DetailsDes = r_Spu.DetailsDes;
                    r_Sku.BriefDes = r_Spu.BriefDes;
                    r_Sku.SpecItems = r_Spu.SpecItems;
                    r_Sku.SalePrice = d_Sku.SalePrice;
                    r_Sku.CharTags = r_Spu.CharTags;
                    r_Sku.SpecDes = d_Sku.SpecDes.ToJsonObject<List<SpecDes>>();
                    r_Sku.SpecIdx = d_Sku.SpecIdx;
                    r_Sku.SpecIdxSkus = r_Spu.SpecIdxSkus;
                    r_Sku.IsTrgVideoService = r_Spu.IsTrgVideoService;
                    r_Sku.IsRevService = r_Spu.IsRevService;
                    r_Sku.IsSupRentService = r_Spu.IsSupRentService;
                    r_Sku.IsMavkBuy = r_Spu.IsMavkBuy;
                    r_Sku.SupReceiveMode = r_Spu.SupReceiveMode;
                    r_Sku.KindId1 = r_Spu.KindId1;
                    r_Sku.KindId2 = r_Spu.KindId2;
                    r_Sku.KindId3 = r_Spu.KindId3;
                    if (!string.IsNullOrEmpty(r_Sku.BarCode))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("BC:{0}:{1}", r_Sku.BarCode.ToLower(), skuId.ToLower()), skuId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_Sku.PinYinIndex))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("PY:{0}:{1}", r_Sku.PinYinIndex.ToLower(), skuId.ToLower()), skuId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_Sku.Name))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("NA:{0}:{1}", r_Sku.Name.ToLower(), skuId.ToLower()), skuId.ToLower(), StackExchange.Redis.When.Always);
                    }

                    if (!string.IsNullOrEmpty(r_Sku.CumCode))
                    {
                        RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("CC:{0}:{1}", r_Sku.CumCode.ToLower(), skuId.ToLower()), skuId.ToLower(), StackExchange.Redis.When.Always);
                    }
                }

                RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.PRD_SKU_INF, merchId.ToLower()), skuId.ToLower(), JsonConvertUtil.SerializeObject(r_Sku), StackExchange.Redis.When.Always);
            }

            return r_Sku;
        }

        public List<SkuInfoBySearchModel> SearchSku(string merchId, string type, string key)
        {
            List<SkuInfoBySearchModel> m_searchs = new List<SkuInfoBySearchModel>();

            if (string.IsNullOrEmpty(key))
                return m_searchs;

            List<RedisValue> r_skuIds = new List<RedisValue>();


            var search_Keys = RedisManager.Db.HashScan(string.Format(RedisKeyS.PRD_SKU_SKEY, merchId.ToLower()), string.Format("*{0}*", key.ToLower()));

            search_Keys = search_Keys.Distinct();

            foreach (var item in search_Keys)
            {
                r_skuIds.Add(item.Value);
            }

            r_skuIds = r_skuIds.Distinct().Take(10).ToList();

            LogUtil.Info("SkuIds.length:" + r_skuIds.Count);

            if (r_skuIds.Count > 0)
            {
                var r_skus = RedisManager.Db.HashGet(string.Format(RedisKeyS.PRD_SKU_INF, merchId.ToLower()), r_skuIds.ToArray());

                foreach (var r_sku in r_skus)
                {
                    try
                    {
                        var l_sku = Newtonsoft.Json.JsonConvert.DeserializeObject<SkuInfoModel>(r_sku);
                        var m_search = new SkuInfoBySearchModel();
                        m_search.SkuId = l_sku.Id;
                        m_search.SpuId = l_sku.SpuId;
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

        public List<SpuInfoBySearchModel> SearchSpu(string merchId, string type, string key)
        {
            List<SpuInfoBySearchModel> m_searchs = new List<SpuInfoBySearchModel>();

            if (string.IsNullOrEmpty(key))
                return m_searchs;

            List<RedisValue> r_spuIds = new List<RedisValue>();


            var search_Keys = RedisManager.Db.HashScan(string.Format(RedisKeyS.PRD_SPU_SKEY, merchId.ToLower()), string.Format("*{0}*", key.ToLower()));
            foreach (var item in search_Keys)
            {
                r_spuIds.Add(item.Value);
            }

            r_spuIds = r_spuIds.Distinct().Take(10).ToList();

            LogUtil.Info("r_spuIds.length:" + r_spuIds.Count);

            if (r_spuIds.Count > 0)
            {
                var r_spus = RedisManager.Db.HashGet(string.Format(RedisKeyS.PRD_SPU_INF, merchId.ToLower()), r_spuIds.ToArray());

                foreach (var r_spu in r_spus)
                {
                    try
                    {
                        var l_spu = Newtonsoft.Json.JsonConvert.DeserializeObject<SpuInfoModel>(r_spu);
                        var m_search = new SpuInfoBySearchModel();
                        m_search.SpuId = l_spu.Id;
                        m_search.Name = l_spu.Name;
                        m_search.SpuCode = l_spu.SpuCode;
                        m_search.MainImgUrl = ImgSet.Convert_S(l_spu.MainImgUrl);
                        m_search.IsSupRentService = l_spu.IsSupRentService;

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

                var d_Skus = CurrentDb.PrdSku.Where(m => m.MerchId == merch.Id).ToList();
                foreach (var d_Sku in d_Skus)
                {
                    GetSkuInfo(merch.Id, d_Sku.Id);
                }

            }
        }
    }
}
