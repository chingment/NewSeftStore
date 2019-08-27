using LocalS.BLL;
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
        public CustomJsonResult<RetIndexGetPageData> GetPageData(string operater, string clientUserId, string storeId)
        {

            var result = new CustomJsonResult<RetIndexGetPageData>();

            var ret = new RetIndexGetPageData();


            var store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();

            var storeModel = new StoreModel();
            storeModel.Id = store.Id;
            storeModel.Name = store.Name;
            storeModel.Address = store.Address;


            ret.Store = storeModel;

            var adSpaceContents = CurrentDb.AdSpaceContent.Where(m => m.AdSpaceId == E_AdSpaceId.AppHomeTop && m.StoreId == store.Id && m.BelongType == E_AdSpaceContentBelongType.App && m.BelongId == store.Id).ToList();

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

            var productSubjects = CurrentDb.PrdSubject.Where(m => m.MerchId == store.MerchId & m.IsDelete == false && m.Depth == 1).OrderBy(m => m.Priority).ToList();

            foreach (var productSubject in productSubjects)
            {

                var query = (from o in CurrentDb.PrdProductSubject where o.PrdSubjectId == productSubject.Id select new { o.Id, o.PrdProductId, o.PrdSubjectId, o.CreateTime });

                query = query.OrderByDescending(r => r.CreateTime).Take(6);

                var list = query.ToList();

                if (list.Count > 0)
                {
                    var tab = new PdAreaModel.Tab();
                    tab.Id = productSubject.Id;
                    tab.Name = productSubject.Name;
                    tab.MainImgUrl = productSubject.MainImgUrl;

                    foreach (var i in list)
                    {
                        var productByCache = CacheServiceFactory.PrdProduct.GetModelById(i.PrdProductId);

                        if (productByCache != null)
                        {
                            var procudtModel = new ProductModel();
                            procudtModel.Id = productByCache.Id;
                            procudtModel.Name = productByCache.Name;
                            procudtModel.DispalyImgUrls = productByCache.DispalyImgUrls;
                            procudtModel.MainImgUrl = productByCache.MainImgUrl;
                            procudtModel.SalePrice = productByCache.SalePrice;
                            procudtModel.ShowPrice = productByCache.ShowPrice;
                            procudtModel.BriefDes = productByCache.BriefDes;
                            procudtModel.DetailsDes = productByCache.DetailsDes;

                            tab.List.Add(procudtModel);
                        }

                    }

                    pdAreaModel.Tabs.Add(tab);
                }
            }


            ret.PdArea = pdAreaModel;

            result = new CustomJsonResult<RetIndexGetPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
