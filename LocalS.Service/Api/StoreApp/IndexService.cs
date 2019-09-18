﻿using LocalS.BLL;
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

            var prdKinds = CurrentDb.PrdKind.Where(m => m.MerchId == store.MerchId & m.IsDelete == false && m.Depth == 1).OrderBy(m => m.Priority).ToList();

            foreach (var prdKind in prdKinds)
            {

                var prdProdcutIds = CurrentDb.StoreSellChannelStock.Where(m => m.MerchId == store.MerchId && m.StoreId == rup.StoreId).Select(m => m.PrdProductId).Distinct<string>().ToArray();
                var prdProdcutKinds_query = (from o in CurrentDb.PrdProductKind where o.PrdKindId == prdKind.Id && prdProdcutIds.Contains(o.PrdProductId) select new { o.Id, o.PrdProductId, o.PrdKindId, o.CreateTime }).OrderByDescending(r => r.CreateTime);

                var list = prdProdcutKinds_query.OrderByDescending(r => r.CreateTime).Take(6).ToList();

                if (list.Count > 0)
                {
                    var tab = new PdAreaModel.Tab();
                    tab.Id = prdKind.Id;
                    tab.Name = prdKind.Name;
                    tab.MainImgUrl = prdKind.MainImgUrl;

                    foreach (var i in list)
                    {
                        var productModel = BizFactory.PrdProduct.GetProduct(rup.StoreId, i.PrdProductId);

                        if (productModel != null)
                        {
                            tab.List.Add(productModel);
                        }
                    }

                    if (tab.List.Count > 0)
                    {
                        pdAreaModel.Tabs.Add(tab);
                    }
                }
            }


            ret.PdArea = pdAreaModel;

            result = new CustomJsonResult<RetIndexPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
