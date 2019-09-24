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

            var store = CurrentDb.Store.Where(m => m.Id == rup.StoreId).FirstOrDefault();
            var storeModel = new StoreModel();
            storeModel.Id = store.Id;
            storeModel.Name = store.Name;
            storeModel.Address = store.Address;


            ret.Store = storeModel;

            var adSpaceContentIds = CurrentDb.AdSpaceContentBelong.Where(m => m.MerchId == store.MerchId && m.AdSpaceId == E_AdSpaceId.AppHomeTop && m.BelongType == E_AdSpaceBelongType.App && m.BelongId == store.Id).Select(m => m.AdSpaceContentId).ToArray();

            var adSpaceContents = CurrentDb.AdSpaceContent.Where(m => adSpaceContentIds.Contains(m.Id) && m.Status == E_AdSpaceContentStatus.Normal).ToList();

            BannerModel bannerModel = new BannerModel();
            bannerModel.Autoplay = true;
            bannerModel.CurrentSwiper = 0;

            foreach (var item in adSpaceContents)
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

            var prdKinds = CurrentDb.PrdKind.Where(m => m.MerchId == store.MerchId & m.IsDelete == false && m.Depth == 1).OrderBy(m => m.Priority).ToList();

            foreach (var prdKind in prdKinds)
            {
                var tab = new PdAreaModel.Tab();
                tab.Id = prdKind.Id;
                tab.Name = prdKind.Name;
                tab.MainImgUrl = prdKind.MainImgUrl;
                tab.List = StoreAppServiceFactory.Product.GetPageList(0, 6, rup.StoreId, prdKind.Id);
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
