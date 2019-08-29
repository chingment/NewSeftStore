using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class ProductService : BaseDbContext
    {
        public CustomJsonResult List(string operater, string clientUserId, RupProductList rup)
        {
            var result = new CustomJsonResult();

            var olist = new List<object>();

            var query = (from o in CurrentDb.PrdProduct

                         select new { o.Id, o.Name, o.PrdKindIds, o.DispalyImgUrls, o.BriefDes, o.MainImgUrl, o.CreateTime }
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
                query = query.Where(p => (from d in CurrentDb.PrdProductKind
                                          where d.PrdKindId == rup.KindId
                                          select d.PrdProductId).Contains(p.Id));
            }

            if (!string.IsNullOrEmpty(rup.SubjectId))
            {
                query = query.Where(p => (from d in CurrentDb.PrdProductSubject
                                          where d.PrdSubjectId == rup.SubjectId
                                          select d.PrdProductId).Contains(p.Id));
            }

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (rup.PageIndex)).Take(pageSize);

            var list = query.ToList();


            foreach (var item in list)
            {
                var productModel = BizFactory.PrdProduct.GetProduct(rup.StoreId, item.Id);
                if (productModel != null)
                {
                    olist.Add(productModel);
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;
        }


        public CustomJsonResult Details(string operater, string clientUserId, RupProductDetails rup)
        {


            var result = new CustomJsonResult();


            var productModel = BizFactory.PrdProduct.GetProduct(rup.StoreId, rup.Id);


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", productModel);

            return result;
        }
    }
}
