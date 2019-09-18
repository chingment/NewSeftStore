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

            var pageEntiy = GetPageList(rup.PageIndex, rup.PageSize, rup.StoreId, rup.KindId);


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntiy);

            return result;
        }


        public PageEntity<PrdProductModel> GetPageList(int pageIndex, int pageSize, string storeId, string kindId)
        {
            var pageEntiy = new PageEntity<PrdProductModel>();

            pageEntiy.PageIndex = pageIndex;
            pageEntiy.PageSize = pageSize;

            var store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();

            var query = CurrentDb.StoreSellChannelStock.Where(m => m.MerchId == store.MerchId && m.StoreId == storeId);


            if (!string.IsNullOrEmpty(kindId))
            {
                query = query.Where(p => (from d in CurrentDb.PrdProductKind
                                          where d.PrdKindId == kindId
                                          select d.PrdProductId).Contains(p.PrdProductId));
            }

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.ToList();


            foreach (var item in list)
            {
                var productModel = BizFactory.PrdProduct.GetProduct(storeId, item.PrdProductId);
                if (productModel != null)
                {
                    pageEntiy.Items.Add(productModel);
                }
            }

            return pageEntiy;

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
