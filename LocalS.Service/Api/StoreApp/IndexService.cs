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


            var store = CurrentDb.Store.Where(m => m.Id == rup.StoreId).FirstOrDefault();

            if (store == null || store.IsDelete)
            {
                return new CustomJsonResult<RetIndexPageData>(ResultType.Failure, ResultCode.Failure2NoExsit, "无效店铺", null);
            }

            var ret = new RetIndexPageData();

            string storeId = rup.StoreId;


            if (store.SctMode.Contains("K"))
            {
                ret.IsSupMachineShop = true;
              
            }

            var storeModel = new StoreModel();
            storeModel.Id = store.Id;
            storeModel.Name = store.Name;

            ret.Store = storeModel;

            var adContentIds = CurrentDb.AdContentBelong.Where(m => m.MerchId == store.MerchId && m.AdSpaceId == E_AdSpaceId.AppHomeTopBanner && m.BelongType == E_AdSpaceBelongType.App && m.BelongId == store.Id).Select(m => m.AdContentId).ToArray();

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

            var d_sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == rup.MerchId && m.StoreId == rup.StoreId && m.ShopId == "0" && m.MachineId == "0" && m.ShopMode == E_ShopMode.Mall && m.IsUseRent == true).Take(2).ToList();

            var m_pdRent = new PdRentModel();

            m_pdRent.Name = "租用市场";

            foreach (var d_sellChannelStock in d_sellChannelStocks)
            {
                var r_productSku = CacheServiceFactory.Product.GetSkuStock(E_ShopMode.Mall, rup.MerchId, rup.StoreId, "0", null, d_sellChannelStock.PrdProductSkuId);
                if (r_productSku != null && r_productSku.Stocks != null && r_productSku.Stocks.Count > 0)
                {
                    var m_productSku = new ProductSkuModel();
                    m_productSku.Id = r_productSku.Id;
                    m_productSku.ProductId = r_productSku.ProductId;
                    m_productSku.Name = r_productSku.Name;
                    m_productSku.MainImgUrl = ImgSet.Convert_B(r_productSku.MainImgUrl);
                    m_productSku.BriefDes = r_productSku.BriefDes;
                    m_productSku.CharTags = r_productSku.CharTags;
                    m_productSku.SpecDes = r_productSku.SpecDes;
                    m_productSku.SpecItems = r_productSku.SpecItems;
                    m_productSku.SpecIdx = r_productSku.SpecIdx;
                    m_productSku.SpecIdxSkus = r_productSku.SpecIdxSkus;
                    m_productSku.IsMavkBuy = true;
                    m_productSku.IsShowPrice = false;
                    m_productSku.SalePrice = r_productSku.Stocks[0].SalePrice;
                    m_productSku.IsOffSell = r_productSku.Stocks[0].IsOffSell;
                    m_productSku.RentMhPrice = r_productSku.Stocks[0].RentMhPrice;
                    m_productSku.DepositPrice = r_productSku.Stocks[0].DepositPrice;
                    m_pdRent.List.Add(m_productSku);
                }

            }


            var m_pdArea = new PdAreaModel();

            var d_storeKinds = CurrentDb.StoreKind.Where(m => m.MerchId == rup.MerchId && m.StoreId == rup.StoreId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

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
