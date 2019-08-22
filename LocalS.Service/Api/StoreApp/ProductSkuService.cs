using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class ProductSkuService : BaseDbContext
    {
        public List<SkuModel> List(string operater, string clientId, RupProductSkuList rup)
        {
            var olist = new List<SkuModel>();

            var query = (from o in CurrentDb.ProductSku

                         select new { o.Id, o.Name, o.KindIds, o.DispalyImgUrls, o.BriefDes, o.SalePrice, o.ShowPrice, o.MainImgUrl, o.CreateTime }
             );

            if (rup.Name != null && rup.Name.Length > 0)
            {
                query = query.Where(p => p.Name.Contains(rup.Name));
            }

            //if (type != Enumeration.ProductType.Unknow)
            //{
            //    //query = query.Where(p => p.ProductCategoryId.ToString().StartsWith(categoryId.ToString()));
            //}

            //if (categoryId != 0)
            //{
            //    query = query.Where(p => p.ProductCategoryId.ToString().StartsWith(categoryId.ToString()));
            //}


            if (!string.IsNullOrEmpty(rup.KindId))
            {
                query = query.Where(p => (from d in CurrentDb.ProductSkuKind
                                          where d.ProductKindId == rup.KindId
                                          select d.ProductSkuId).Contains(p.Id));
            }

            if (!string.IsNullOrEmpty(rup.SubjectId))
            {
                query = query.Where(p => (from d in CurrentDb.ProductSkuSubject
                                          where d.ProductSubjectId == rup.SubjectId
                                          select d.ProductSkuId).Contains(p.Id));
            }

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (rup.PageIndex)).Take(pageSize);

            var list = query.ToList();


            foreach (var item in list)
            {
                olist.Add(new SkuModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    MainImgUrl = ImgSet.GetMain(item.DispalyImgUrls),
                    SalePrice = item.SalePrice,
                    ShowPrice = item.ShowPrice,
                    BriefDes = item.BriefDes
                });
            }

            return olist;


        }

        public SkuModel Details(string skuId)
        {
            //var skuModel = BizFactory.ProductSku.GetModel(skuId);
            //return skuModel;

            return null;
        }
    }
}
