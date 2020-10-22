using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class IndexService : BaseDbContext
    {
        public CustomJsonResult<RetIndexPageData> PageData(string operater, string clientUserId, RupIndexPageData rup)
        {

            var result = new CustomJsonResult<RetIndexPageData>();

            var ret = new RetIndexPageData();

            string storeId = rup.StoreId;



            ret.ShopModes.Add(new ShopModeModel { Id = E_SellChannelRefType.Machine, Name = "线下机器", Selected = false });
            ret.ShopModes.Add(new ShopModeModel { Id = E_SellChannelRefType.Mall, Name = "线上商城", Selected = false });

            if (rup.ShopMode == E_SellChannelRefType.Unknow)
            {
                ret.ShopModes[0].Selected = true;
            }
            else
            {
                ret.ShopModes.Where(m => m.Id == rup.ShopMode).First().Selected = true;
            }


            rup.ShopMode = ret.ShopModes.Where(m => m.Selected == true).First().Id;


            var store = BizFactory.Store.GetOne(rup.StoreId);

            if (store == null || store.IsDelete)
            {
                return new CustomJsonResult<RetIndexPageData>(ResultType.Failure, ResultCode.Failure2NoExsit, "无效店铺", null);
            }

            var storeModel = new StoreModel();
            storeModel.Id = store.StoreId;
            storeModel.Name = store.Name;
            storeModel.Address = store.Address;


            ret.Store = storeModel;

            var adContentIds = CurrentDb.AdContentBelong.Where(m => m.MerchId == store.MerchId && m.AdSpaceId == E_AdSpaceId.AppHomeTopBanner && m.BelongType == E_AdSpaceBelongType.App && m.BelongId == store.StoreId).Select(m => m.AdContentId).ToArray();

            var adContents = CurrentDb.AdContent.Where(m => adContentIds.Contains(m.Id) && m.Status == E_AdContentStatus.Normal).ToList();

            BannerModel bannerModel = new BannerModel();

            foreach (var item in adContents)
            {
                var imgModel = new BannerModel.ImgModel();
                imgModel.Id = item.Id;
                imgModel.Title = item.Title;
                imgModel.Link = "";
                imgModel.Url = item.Url;
                bannerModel.Imgs.Add(imgModel);
            }

            ret.Banner = bannerModel;


            var pdAreaModel = new PdAreaModel();

            var prdKinds = CurrentDb.StoreKind.Where(m => m.MerchId == store.MerchId && m.StoreId == store.StoreId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

            foreach (var prdKind in prdKinds)
            {
                var tab = new PdAreaModel.Tab();
                tab.Id = prdKind.Id;
                tab.Name = prdKind.Name;
                tab.MainImgUrl = ImgSet.GetMain_O(prdKind.DisplayImgUrls);
                tab.List = StoreAppServiceFactory.Product.GetProducts(0, 6, rup.StoreId, rup.ShopMode, prdKind.Id);
                if (tab.List.Items.Count > 0)
                {
                    pdAreaModel.Tabs.Add(tab);
                }
            }

            ret.PdArea = pdAreaModel;

            result = new CustomJsonResult<RetIndexPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
