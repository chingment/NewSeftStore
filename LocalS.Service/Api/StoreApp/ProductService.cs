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
    public class ProductService : BaseDbContext
    {
        public CustomJsonResult InitSearchPageData(string operater, string clientUserId, RupProductInitSearchPageData rup)
        {
            var result = new CustomJsonResult();


            var ret = new RetProductInitSearch();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            var prdKinds = CurrentDb.StoreKind.Where(m => m.MerchId == store.MerchId && m.StoreId == rup.StoreId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

            foreach (var prdKind in prdKinds)
            {
                var option = new Option();
                option.Id = prdKind.Id;
                option.Name = prdKind.Name;
                option.Selected = false;

                ret.Condition_Kinds.Add(option);
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Search(string operater, string clientUserId, RupProductSearch rup)
        {
            var result = new CustomJsonResult();

            var pageEntiy = GetProducts(rup.PageIndex, rup.PageSize, rup.StoreId, rup.ShopMode, rup.KindId);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntiy);

            return result;
        }

        public PageEntity<ProductSkuModel> GetProducts(int pageIndex, int pageSize, string storeId, E_SellChannelRefType shopMode, string kindId)
        {
            var pageEntiy = new PageEntity<ProductSkuModel>();

            pageEntiy.PageIndex = pageIndex;
            pageEntiy.PageSize = pageSize;

            var store = BizFactory.Store.GetOne(storeId);

            var query = CurrentDb.SellChannelStock.Where(m =>
                m.MerchId == store.MerchId
                         && m.StoreId == storeId
                         && m.SellChannelRefType == shopMode

                ).AsEnumerable()
                     .OrderBy(x => x.Id)
                     .GroupBy(x => x.PrdProductId)
                     .Select(g => new { g, count = g.Count() })
                     .SelectMany(t => t.g.Select(b => b)
                     .Zip(Enumerable.Range(1, t.count), (j, i) => new { j.PrdProductId, j.PrdProductSkuId, rn = i })).Where(m => m.rn == 1); ;

            if (!string.IsNullOrEmpty(kindId))
            {
                query = query.Where(p => (from d in CurrentDb.StoreKindSpu
                                          where d.StoreKindId == kindId
                                          select d.PrdProductId).Contains(p.PrdProductId));
            }

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderByDescending(r => r.PrdProductId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.ToList();

            foreach (var item in list)
            {
                var bizProductSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, storeId, store.GetSellChannelRefIds(shopMode), item.PrdProductSkuId);


                var productSkuModel = new ProductSkuModel();

                productSkuModel.Id = item.PrdProductSkuId;
                productSkuModel.ProductId = item.PrdProductId;
                productSkuModel.Name = bizProductSku.Name;
                productSkuModel.MainImgUrl = ImgSet.Convert_S(bizProductSku.MainImgUrl);
                productSkuModel.BriefDes = bizProductSku.BriefDes;
                productSkuModel.CharTags = bizProductSku.CharTags;
                productSkuModel.SpecDes = bizProductSku.SpecDes;
                productSkuModel.SpecItems = bizProductSku.SpecItems;
                productSkuModel.SpecIdx = bizProductSku.SpecIdx;
                productSkuModel.SpecIdxSkus = bizProductSku.SpecIdxSkus;
                if (bizProductSku.Stocks != null)
                {
                    if (bizProductSku.Stocks.Count > 0)
                    {
                        productSkuModel.IsShowPrice = false;
                        productSkuModel.SalePrice = bizProductSku.Stocks[0].SalePrice;
                        productSkuModel.SalePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
                        productSkuModel.IsOffSell = bizProductSku.Stocks[0].IsOffSell;

                        pageEntiy.Items.Add(productSkuModel);
                    }
                }
            }

            return pageEntiy;

        }

        public CustomJsonResult Details(string operater, string clientUserId, RupProductDetails rup)
        {
            var result = new CustomJsonResult();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            var bizProductSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.Id, store.GetSellChannelRefIds(rup.ShopMode), rup.SkuId);

            var productSkuDetailsModel = new ProductSkuDetailsModel();
            productSkuDetailsModel.Id = bizProductSku.Id;
            productSkuDetailsModel.ProductId = bizProductSku.ProductId;
            productSkuDetailsModel.Name = bizProductSku.Name;
            productSkuDetailsModel.MainImgUrl = bizProductSku.MainImgUrl;
            productSkuDetailsModel.DisplayImgUrls = bizProductSku.DisplayImgUrls;
            productSkuDetailsModel.DetailsDes = bizProductSku.DetailsDes;
            productSkuDetailsModel.BriefDes = bizProductSku.BriefDes;
            productSkuDetailsModel.SpecItems = bizProductSku.SpecItems;
            productSkuDetailsModel.SpecIdx = bizProductSku.SpecIdx;
            productSkuDetailsModel.SpecIdxSkus = bizProductSku.SpecIdxSkus;


            if (bizProductSku.Stocks.Count > 0)
            {
                productSkuDetailsModel.IsShowPrice = false;
                productSkuDetailsModel.SalePrice = bizProductSku.Stocks[0].SalePrice;
                productSkuDetailsModel.SalePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
                productSkuDetailsModel.IsOffSell = bizProductSku.Stocks[0].IsOffSell;
                productSkuDetailsModel.SellQuantity = bizProductSku.Stocks.Sum(m => m.SellQuantity);
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", productSkuDetailsModel);

            return result;
        }

        public CustomJsonResult SkuStockInfo(string operater, string clientUserId, RupProductDetails rup)
        {
            var result = new CustomJsonResult();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            var bizProductSku = CacheServiceFactory.Product.GetSkuStock(store.MerchId, store.Id, store.GetSellChannelRefIds(rup.ShopMode), rup.SkuId);

            bool isOffSell = true;
            bool isShowPrice = false;
            decimal salePrice = 0m;
            decimal salePriceByVip = 0m;
            int sellQuantity = 0;
            if (bizProductSku.Stocks.Count > 0)
            {
                isShowPrice = false;
                salePrice = bizProductSku.Stocks[0].SalePrice;
                salePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
                isOffSell = bizProductSku.Stocks[0].IsOffSell;
                sellQuantity = bizProductSku.Stocks.Sum(m => m.SellQuantity);
            }

            var data = new { skuId = rup.SkuId, name = bizProductSku.Name, salePrice = salePrice, isShowPrice = isShowPrice, salePriceByVip = salePriceByVip, isOffSell = isOffSell, sellQuantity = sellQuantity };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", data);

            return result;
        }
    }
}
