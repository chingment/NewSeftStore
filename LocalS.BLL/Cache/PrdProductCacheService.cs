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
        private static readonly string redis_key_all_spu_info_by_merchId = "info_spu_all:{0}";
        public void Remove(string merchId, string productId)
        {
            RedisHashUtil.Remove(string.Format(redis_key_all_spu_info_by_merchId, merchId), productId);
        }

        public ProductInfoModel GetInfo(string merchId, string productId)
        {
            //先从缓存信息读取商品信息
            ProductInfoModel2 productInfoModel2 = RedisHashUtil.Get<ProductInfoModel2>(string.Format(redis_key_all_spu_info_by_merchId, merchId), productId);

            //如商品信息从缓存取不到，读取数据库信息加载
            if (productInfoModel2 == null)
            {
                var prdProductByDb = CurrentDb.PrdProduct.Where(m => m.Id == productId).FirstOrDefault();
                if (prdProductByDb == null)
                    return null;

                var prdProductRefSkuByDb = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == prdProductByDb.Id && m.IsRef == true).FirstOrDefault();
                if (prdProductRefSkuByDb == null)
                    return null;

                productInfoModel2 = new ProductInfoModel2();

                productInfoModel2.Id = prdProductByDb.Id;
                productInfoModel2.Name = prdProductByDb.Name.NullToEmpty();
                productInfoModel2.DisplayImgUrls = prdProductByDb.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                productInfoModel2.MainImgUrl = ImgSet.GetMain(prdProductByDb.DisplayImgUrls);
                productInfoModel2.DetailsDes = prdProductByDb.DetailsDes.NullToEmpty();
                productInfoModel2.BriefDes = prdProductByDb.BriefDes.NullToEmpty();
                productInfoModel2.RefSkuId = prdProductRefSkuByDb.Id;
                RedisManager.Db.HashSetAsync(string.Format(redis_key_all_spu_info_by_merchId, prdProductByDb.MerchId), prdProductByDb.Id, Newtonsoft.Json.JsonConvert.SerializeObject(productInfoModel2), StackExchange.Redis.When.Always);
            }

            if (productInfoModel2.RefSkuId == null)
                return null;

            var productInfoModel = new ProductInfoModel();
            productInfoModel.Id = productInfoModel2.Id;
            productInfoModel.Name = productInfoModel2.Name.NullToEmpty();
            productInfoModel.DisplayImgUrls = productInfoModel2.DisplayImgUrls;
            productInfoModel.MainImgUrl = productInfoModel2.MainImgUrl;
            productInfoModel.DetailsDes = productInfoModel2.DetailsDes.NullToEmpty();
            productInfoModel.BriefDes = productInfoModel2.BriefDes.NullToEmpty();
            productInfoModel.RefSku.Id = productInfoModel2.RefSkuId;
            //从缓存中取店铺的商品库存信息

            var refSkuStock = CacheServiceFactory.ProductSku.GetStock(merchId, productInfoModel2.RefSkuId);

            if (refSkuStock == null)
            {
                LogUtil.Info(string.Format("库存,Product,SkuId:{0},数据为NULL", productInfoModel2.RefSkuId));
            }
            else
            {
                if (refSkuStock == null || refSkuStock.Count == 0)
                {
                    LogUtil.Info(string.Format("库存,Product,SkuId:{0},Stocks为NULL", productInfoModel2.RefSkuId));
                }
                else
                {
                    productInfoModel.RefSku.ReceptionMode = Entity.E_ReceptionMode.Machine;
                    productInfoModel.RefSku.SumQuantity = refSkuStock.Where(m => m.RefType == Entity.E_SellChannelRefType.Machine).Sum(m => m.SumQuantity);
                    productInfoModel.RefSku.LockQuantity = refSkuStock.Where(m => m.RefType == Entity.E_SellChannelRefType.Machine).Sum(m => m.LockQuantity);
                    productInfoModel.RefSku.SellQuantity = refSkuStock.Where(m => m.RefType == Entity.E_SellChannelRefType.Machine).Sum(m => m.SellQuantity);
                    productInfoModel.RefSku.SalePrice = refSkuStock[0].SalePrice;
                    productInfoModel.RefSku.SalePriceByVip = refSkuStock[0].SalePriceByVip;
                    productInfoModel.RefSku.IsOffSell = refSkuStock[0].IsOffSell;
                }
            }

            return productInfoModel;
        }


        

    }
}
