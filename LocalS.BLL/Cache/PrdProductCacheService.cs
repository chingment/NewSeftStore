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
        private static readonly string redis_key_productsku_list = "entity:LocalS.Entity.ProductSku";

        public PrdProductModel GetModelById(string id)
        {
            var model = new PrdProductModel();

            model = RedisHashUtil.Get<PrdProductModel>(redis_key_productsku_list, id);

            if (model == null)
            {
                var prdProduct = CurrentDb.PrdProduct.Where(m => m.Id == id).FirstOrDefault();
                if (prdProduct == null)
                    return null;

                var prdProductSkus = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == prdProduct.Id).ToList();
                if (prdProductSkus == null)
                    return null;


                model = new PrdProductModel();

                model.Id = prdProduct.Id;
                model.Name = prdProduct.Name;
                model.DispalyImgUrls = prdProduct.DispalyImgUrls.ToJsonObject<List<ImgSet>>();
                model.MainImgUrl = ImgSet.GetMain(prdProduct.DispalyImgUrls);
                model.DetailsDes = prdProduct.DetailsDes;
                model.BriefDes = prdProduct.BriefDes;

                for (var i = 0; i < prdProductSkus.Count; i++)
                {
                    if (prdProductSkus[i].IsRef)
                    {
                        model.RefSkuIndex = i;
                    }
                    model.Skus.Add(new PrdProductModel.Sku { Id = prdProductSkus[i].Id, SalePrice = prdProductSkus[i].SalePrice, SpecDes = prdProductSkus[i].SpecDes });
                }

                RedisManager.Db.HashSetAsync(redis_key_productsku_list, model.Id, Newtonsoft.Json.JsonConvert.SerializeObject(model), StackExchange.Redis.When.Always);

            }

            return model;
        }
    }
}
