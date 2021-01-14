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
    public class ShopMarketService : BaseService
    {
        public CustomJsonResult PageData(string operater, string clientUserId, RupShopMartketPageData rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetShopMarketPageData();

            var d_Shop = (from s in CurrentDb.StoreShop
                          join m in CurrentDb.Shop on s.ShopId equals m.Id into temp
                          from u in temp.DefaultIfEmpty()
                          where
                          s.MerchId == rup.MerchId
                          && s.StoreId == rup.StoreId
                          select new { u.Id, u.Name, u.Address, u.Lat, u.Lng, u.MainImgUrl, u.IsOpen, u.AreaCode, u.AreaName, u.MerchId, s.StoreId, u.ContactName, u.ContactPhone, u.ContactAddress, u.CreateTime }).ToList();

            if (d_Shop.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, "2304", "请先选择店铺");
            }

            string shopId = rup.ShopId == null ? "" : rup.ShopId.ToLower();

            var curShop = d_Shop.Where(m => m.Id == shopId).FirstOrDefault();
            if (curShop == null)
            {
                curShop = d_Shop[0];
            }

            ret.CurShop.Id = curShop.Id;
            ret.CurShop.Name = curShop.Name;
            ret.CurShop.Address = curShop.Address;


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
                prdKindModel.List = StoreAppServiceFactory.Product.GetProducts(0, 10, rup.StoreId, curShop.Id, Entity.E_SellChannelRefType.Machine, Entity.E_OrderShopMethod.Shop, prdKind.Id);

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


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }
    }
}
