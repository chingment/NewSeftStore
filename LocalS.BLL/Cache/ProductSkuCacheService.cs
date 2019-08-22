using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ProductSkuCacheService : BaseDbContext
    {
        private static readonly string redis_key_productsku_list = "entity:LocalS.Entity.ProductSku";

        public ProductSkuModel GetModelById(string id)
        {
            var model = new ProductSkuModel();

            model = RedisHashUtil.Get<ProductSkuModel>(redis_key_productsku_list, id);

            if (model == null)
            {
                var productSku = CurrentDb.ProductSku.Where(m => m.Id == id).FirstOrDefault();
                if (productSku == null)
                    return null;


                model = new ProductSkuModel();

                model.Id = productSku.Id;
                model.Name = productSku.Name;
                model.SalePrice = productSku.SalePrice;
                model.ShowPrice = productSku.ShowPrice;
                model.DispalyImgUrls = productSku.DispalyImgUrls.ToJsonObject<List<ImgSet>>();
                model.MainImgUrl = ImgSet.GetMain(productSku.DispalyImgUrls);
                model.DetailsDes = productSku.DetailsDes;
                model.BriefDes = productSku.BriefDes;
                model.SpecDes = productSku.SpecDes;

                RedisManager.Db.HashSetAsync(redis_key_productsku_list, model.Id, Newtonsoft.Json.JsonConvert.SerializeObject(model), StackExchange.Redis.When.Always);

            }

            return model;
        }
    }
}
