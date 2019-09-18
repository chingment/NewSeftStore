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
    public class ProductKindService : BaseDbContext
    {
        public CustomJsonResult<RetProductKindPageData> PageData(string operater, string clientUserId, RupProductKindPageData rup)
        {

            var result = new CustomJsonResult<RetProductKindPageData>();

            var ret = new RetProductKindPageData();


            var prdKindModels = new List<PrdKindModel>();

            var store = CurrentDb.Store.Where(m => m.Id == rup.StoreId).FirstOrDefault();

            var prdKinds = CurrentDb.PrdKind.Where(m => m.MerchId == store.MerchId && m.Depth == 1 && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

            foreach (var prdKind in prdKinds)
            {
                var prdKindModel = new PrdKindModel();
                prdKindModel.Id = prdKind.Id;
                prdKindModel.Name = prdKind.Name;
                prdKindModel.MainImgUrl = prdKind.MainImgUrl;
                prdKindModel.Selected = false;

                var prdProdcutIds = CurrentDb.StoreSellChannelStock.Where(m => m.MerchId == store.MerchId && m.StoreId == rup.StoreId).Select(m => m.PrdProductId).Distinct<string>().ToArray();
                var prdProdcutKinds_query = (from o in CurrentDb.PrdProductKind where o.PrdKindId == prdKind.Id && prdProdcutIds.Contains(o.PrdProductId) select new { o.Id, o.PrdProductId, o.PrdKindId, o.CreateTime }).OrderByDescending(r => r.CreateTime);

                prdKindModel.List.PageIndex = 0;
                prdKindModel.List.PageSize = 10;
                prdKindModel.List.Total = prdProdcutKinds_query.Count();
                prdKindModel.List.PageCount = (prdKindModel.List.Total + prdKindModel.List.PageSize - 1) / prdKindModel.List.PageSize;
    
                var prdProdcutKinds = prdProdcutKinds_query.Take(prdKindModel.List.PageSize).ToList();

                foreach (var prdProdcutKind in prdProdcutKinds)
                {
                    var productModel = BizFactory.PrdProduct.GetProduct(rup.StoreId, prdProdcutKind.PrdProductId);
                    if (productModel != null)
                    {
                        prdKindModel.List.Items.Add(productModel);
                    }
                }

                prdKindModels.Add(prdKindModel);

            }

            var selectedCount = prdKindModels.Where(m => m.Selected == true).Count();
            if (selectedCount == 0)
            {
                if (prdKindModels.Count > 0)
                {
                    prdKindModels[0].Selected = true;
                }
            }

            ret.Tabs = prdKindModels;

            result = new CustomJsonResult<RetProductKindPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
