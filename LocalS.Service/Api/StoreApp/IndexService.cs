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
    public class IndexService : BaseService
    {
        public CustomJsonResult<RetIndexPageData> PageData(string operater, string clientUserId, RupIndexPageData rup)
        {

            var result = new CustomJsonResult<RetIndexPageData>();


            var d_Store = CurrentDb.Store.Where(m => m.Id == rup.StoreId).FirstOrDefault();

            if (d_Store == null || d_Store.IsDelete)
            {
                return new CustomJsonResult<RetIndexPageData>(ResultType.Failure, ResultCode.Failure2NoExsit, "无效店铺", null);
            }

            var ret = new RetIndexPageData();

            string storeId = rup.StoreId;


            ret.Store.Id = d_Store.Id;
            ret.Store.Name = d_Store.Name;

            var adContentIds = CurrentDb.AdContentBelong.Where(m => m.MerchId == d_Store.MerchId && m.AdSpaceId == E_AdSpaceId.AppHomeTopBanner && m.BelongType == E_AdSpaceBelongType.App && m.BelongId == d_Store.Id).Select(m => m.AdContentId).ToArray();

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

            result = new CustomJsonResult<RetIndexPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }


        public CustomJsonResult SugProducts(string operater, string clientUserId, RupIndexSugProducts rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetIndexSugProducts();

            var d_sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.StoreId == rup.StoreId && m.ShopId == "0" && m.DeviceId == "0" && m.ShopMode == E_ShopMode.Mall && m.IsUseRent == true).Take(2).ToList();

            var m_pdRent = new PdRentModel();

            m_pdRent.Name = "租用市场";

            foreach (var d_sellChannelStock in d_sellChannelStocks)
            {
                var r_Sku = CacheServiceFactory.Product.GetSkuStock(E_ShopMode.Mall, d_sellChannelStock.MerchId, rup.StoreId, "0", null, d_sellChannelStock.SkuId);
                if (r_Sku != null && r_Sku.Stocks != null && r_Sku.Stocks.Count > 0)
                {
                    var m_Sku = new SkuModel();
                    m_Sku.Id = r_Sku.Id;
                    m_Sku.SpuId = r_Sku.SpuId;
                    m_Sku.Name = r_Sku.Name;
                    m_Sku.MainImgUrl = ImgSet.Convert_B(r_Sku.MainImgUrl);
                    m_Sku.BriefDes = r_Sku.BriefDes;
                    m_Sku.CharTags = r_Sku.CharTags;
                    m_Sku.SpecDes = r_Sku.SpecDes;
                    m_Sku.SpecItems = r_Sku.SpecItems;
                    m_Sku.SpecIdx = r_Sku.SpecIdx;
                    m_Sku.SpecIdxSkus = r_Sku.SpecIdxSkus;
                    m_Sku.IsMavkBuy = true;
                    m_Sku.IsShowPrice = false;
                    m_Sku.SalePrice = r_Sku.Stocks[0].SalePrice;
                    m_Sku.IsOffSell = r_Sku.Stocks[0].IsOffSell;
                    m_Sku.RentMhPrice = r_Sku.Stocks[0].RentMhPrice;
                    m_Sku.DepositPrice = r_Sku.Stocks[0].DepositPrice;
                    m_pdRent.List.Add(m_Sku);
                }

            }


            var m_pdArea = new PdAreaModel();

            var d_storeKinds = CurrentDb.StoreKind.Where(m => m.StoreId == rup.StoreId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

            foreach (var d_storeKind in d_storeKinds)
            {
                var tab = new PdAreaModel.Tab();
                tab.Id = d_storeKind.Id;
                tab.Name = d_storeKind.Name;
                tab.MainImgUrl = ImgSet.GetMain_O(d_storeKind.DisplayImgUrls);
                tab.List = StoreAppServiceFactory.Product.GetProducts(0, 6, rup.StoreId, "0", rup.ShopMode, E_ShopMethod.Buy, d_storeKind.Id);
                if (tab.List.Items.Count > 0)
                {
                    m_pdArea.Tabs.Add(tab);
                }
            }

            ret.PdRent = m_pdRent;
            ret.PdArea = m_pdArea;

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
