using LocalS.BLL;
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
        public CustomJsonResult<RetProductKindGetPageData> GetPageData(string operater, string clientUserId, string storeId)
        {

            var result = new CustomJsonResult<RetProductKindGetPageData>();

            var ret = new RetProductKindGetPageData();


            var productParentKindModels = new List<ProductParentKindModel>();

            var store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();

            var productKinds = CurrentDb.PrdKind.Where(m => m.MerchId == store.MerchId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();
            var top = productKinds.Where(m => m.Depth == 0).FirstOrDefault();
            var productParentKinds = productKinds.Where(m => m.PId == top.Id).ToList();
            foreach (var item in productParentKinds)
            {
                var productParentKindModel = new ProductParentKindModel();
                productParentKindModel.Id = item.Id;
                productParentKindModel.Name = item.Name;
                productParentKindModel.MainImgUrl = item.MainImgUrl;
                productParentKindModel.Selected = false;
                var productChildKinds = productKinds.Where(m => m.PId == item.Id).ToList();

                foreach (var item2 in productChildKinds)
                {
                    var productChildKindModel = new ProductChildKindModel();

                    productChildKindModel.Id = item2.Id;
                    productChildKindModel.Name = item2.Name;
                    productChildKindModel.MainImgUrl = item2.MainImgUrl;
                    productParentKindModel.Selected = false;

                    productParentKindModel.Child.Add(productChildKindModel);
                }

                productParentKindModels.Add(productParentKindModel);

            }

            var selectedCount = productParentKindModels.Where(m => m.Selected == true).Count();
            if (selectedCount == 0)
            {
                if (productParentKindModels.Count > 0)
                {
                    productParentKindModels[0].Selected = true;
                }
            }

            ret.List = productParentKindModels;

            result = new CustomJsonResult<RetProductKindGetPageData>(ResultType.Success, ResultCode.Success, "操作成功", ret);

            return result;
        }
    }
}
