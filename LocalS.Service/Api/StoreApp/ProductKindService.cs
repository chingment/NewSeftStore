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
    public class ProductKindService : BaseService
    {
        public CustomJsonResult<RetProductKindPageData> PageData(string operater, string clientUserId, RupProductKindPageData rup)
        {

            var result = new CustomJsonResult<RetProductKindPageData>();

            var ret = new RetProductKindPageData();


            var prdKindModels = new List<PrdKindModel>();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            var prdKinds = CurrentDb.StoreKind.Where(m => m.MerchId == store.MerchId && m.StoreId == rup.StoreId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

            foreach (var prdKind in prdKinds)
            {
                var prdKindModel = new PrdKindModel();
                prdKindModel.Id = prdKind.Id;
                prdKindModel.Name = prdKind.Name;
                prdKindModel.MainImgUrl = ImgSet.GetMain_O(prdKind.DisplayImgUrls);
                prdKindModel.Selected = false;
                prdKindModel.List = StoreAppServiceFactory.Product.GetProducts(0, 10, rup.StoreId, rup.ShopId, rup.ShopMode, Entity.E_OrderShopMethod.Shop, prdKind.Id);

                if (prdKindModel.List.Items.Count > 0)
                {
                    prdKindModels.Add(prdKindModel);
                }

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
