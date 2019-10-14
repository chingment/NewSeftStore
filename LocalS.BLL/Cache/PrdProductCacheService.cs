using LocalS.Entity;
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
        private static readonly string redis_key_all_spu_info_by_merchId = "info_sku_all:{0}";
        public void Remove(string merchId, string productId)
        {
            RedisHashUtil.Remove(string.Format(redis_key_all_spu_info_by_merchId, merchId), productId);
        }
        public ProductInfoModel GetInfo(string merchId, string productId)
        {
            //先从缓存信息读取商品信息
            ProductInfoModel prdProductByCache = RedisHashUtil.Get<ProductInfoModel>(string.Format(redis_key_all_spu_info_by_merchId, merchId), productId);

            //如商品信息从缓存取不到，读取数据库信息加载
            if (prdProductByCache == null)
            {
                var prdProductByDb = CurrentDb.PrdProduct.Where(m => m.Id == productId).FirstOrDefault();
                if (prdProductByDb == null)
                    return null;

                var prdProductRefSkuByDb = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == prdProductByDb.Id && m.IsRef == true).FirstOrDefault();
                if (prdProductRefSkuByDb == null)
                    return null;

                prdProductByCache = new ProductInfoModel();

                prdProductByCache.Id = prdProductByDb.Id;
                prdProductByCache.Name = prdProductByDb.Name.NullToEmpty();
                prdProductByCache.DisplayImgUrls = prdProductByDb.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                prdProductByCache.MainImgUrl = ImgSet.GetMain(prdProductByDb.DisplayImgUrls);
                prdProductByCache.DetailsDes = prdProductByDb.DetailsDes.NullToEmpty();
                prdProductByCache.BriefDes = prdProductByDb.BriefDes.NullToEmpty();


                prdProductByCache.RefSku = new ProductInfoModel.RefSkuModel { Id = prdProductRefSkuByDb.Id, SalePrice = prdProductRefSkuByDb.SalePrice, SpecDes = prdProductRefSkuByDb.SpecDes };


                RedisManager.Db.HashSetAsync(string.Format(redis_key_all_spu_info_by_merchId, prdProductByDb.MerchId), prdProductByCache.Id, Newtonsoft.Json.JsonConvert.SerializeObject(prdProductByCache), StackExchange.Redis.When.Always);
            }

            if (prdProductByCache.RefSku == null)
                return null;

            //从缓存中取店铺的商品库存信息

            var refSkuStock = CacheServiceFactory.ProductSku.GetStock(merchId, prdProductByCache.RefSku.Id);

            if (refSkuStock == null)
            {
                LogUtil.Info(string.Format("库存,Product,SkuId:{0},数据为NULL", prdProductByCache.RefSku.Id));
            }
            else
            {
                if (refSkuStock == null || refSkuStock.Count == 0)
                {
                    LogUtil.Info(string.Format("库存,Product,SkuId:{0},Stocks为NULL", prdProductByCache.RefSku.Id));
                }
                else
                {
                    prdProductByCache.RefSku.ReceptionMode = Entity.E_ReceptionMode.Machine;
                    prdProductByCache.RefSku.SumQuantity = refSkuStock.Where(m => m.RefType == Entity.E_SellChannelRefType.Machine).Sum(m => m.SumQuantity);
                    prdProductByCache.RefSku.LockQuantity = refSkuStock.Where(m => m.RefType == Entity.E_SellChannelRefType.Machine).Sum(m => m.LockQuantity);
                    prdProductByCache.RefSku.SellQuantity = refSkuStock.Where(m => m.RefType == Entity.E_SellChannelRefType.Machine).Sum(m => m.SellQuantity);
                    prdProductByCache.RefSku.SalePrice = refSkuStock[0].SalePrice;
                    prdProductByCache.RefSku.SalePriceByVip = refSkuStock[0].SalePriceByVip;
                    prdProductByCache.RefSku.IsOffSell = refSkuStock[0].IsOffSell;
                }
            }

            prdProductByCache.BriefDes = prdProductByCache.BriefDes.NullToEmpty();

            return prdProductByCache;
        }
    }
}
