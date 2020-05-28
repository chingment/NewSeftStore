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
    public class ProductService : BaseDbContext
    {
        public CustomJsonResult List(string operater, string clientUserId, RupProductList rup)
        {
            var result = new CustomJsonResult();

            var pageEntiy = GetProducts(rup.PageIndex, rup.PageSize, rup.StoreId, rup.KindId);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntiy);

            return result;
        }

        public PageEntity<ProductModel> GetProducts(int pageIndex, int pageSize, string storeId, string kindId)
        {
            var pageEntiy = new PageEntity<ProductModel>();

            pageEntiy.PageIndex = pageIndex;
            pageEntiy.PageSize = pageSize;

            var store = BizFactory.Store.GetOne(storeId);

            var query = (from m in CurrentDb.SellChannelStock
                         where m.MerchId == store.MerchId
                         && store.SellMachineIds.Contains(m.SellChannelRefId)
                         && m.SellChannelRefType == Entity.E_SellChannelRefType.Machine
                         select new { m.PrdProductId }).Distinct();

            if (!string.IsNullOrEmpty(kindId))
            {
                query = query.Where(p => (from d in CurrentDb.PrdProductKind
                                          where d.PrdKindId == kindId
                                          select d.PrdProductId).Contains(p.PrdProductId));
            }

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderByDescending(r => r.PrdProductId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.ToList();

            foreach (var item in list)
            {
                var prdProductSkuIds = CurrentDb.SellChannelStock.Where(m => m.MerchId == store.MerchId
                && store.SellMachineIds.Contains(m.SellChannelRefId)
                && m.SellChannelRefType == Entity.E_SellChannelRefType.Machine && m.PrdProductId == item.PrdProductId
                ).Select(m => m.PrdProductSkuId).Distinct().ToList();

                if (prdProductSkuIds.Count > 0)
                {
                    var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(store.MerchId, store.Id, store.SellMachineIds, prdProductSkuIds[0]);
                    if (bizProductSku != null)
                    {
                        var productModel = new ProductModel();
                        productModel.Id = item.PrdProductId;
                        productModel.Name = bizProductSku.Name;
                        productModel.MainImgUrl = ImgSet.Convert_S(bizProductSku.MainImgUrl);
                        productModel.BriefDes = bizProductSku.BriefDes;
                        productModel.CharTags = bizProductSku.CharTags;
                        productModel.SpecItems = bizProductSku.SpecItems;
                        if (bizProductSku.Stocks.Count > 0)
                        {
                            //productModel.RefSkuId = bizProductSku.Id;
                            //productModel.IsShowPrice = false;
                            //productModel.SalePrice = bizProductSku.Stocks[0].SalePrice;
                            //productModel.SalePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
                            //productModel.IsOffSell = bizProductSku.Stocks[0].IsOffSell;

                            //productModel.SpecSkus.Add();

                            pageEntiy.Items.Add(productModel);
                        }
                    }
                }
            }

            return pageEntiy;

        }

        public CustomJsonResult Details(string operater, string clientUserId, RupProductDetails rup)
        {
            var result = new CustomJsonResult();

            //var store = BizFactory.Store.GetOne(rup.StoreId);
            //var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(store.MerchId, store.Id, store.SellMachineIds, rup.ProductId);
            //if (bizProductSku != null)
            //{
            //    var productSkuModel = new ProductModel();
            //    productSkuModel.Id = bizProductSku.Id;
            //    productSkuModel.ProductId = bizProductSku.ProductId;
            //    productSkuModel.Name = bizProductSku.Name;
            //    productSkuModel.MainImgUrl = bizProductSku.MainImgUrl;
            //    productSkuModel.DisplayImgUrls = bizProductSku.DisplayImgUrls;
            //    productSkuModel.DetailsDes = bizProductSku.DetailsDes;
            //    productSkuModel.BriefDes = bizProductSku.BriefDes;
            //    productSkuModel.SpecDes = bizProductSku.SpecDes;

            //    if (bizProductSku.Stocks.Count > 0)
            //    {
            //        productSkuModel.IsShowPrice = false;
            //        productSkuModel.SalePrice = bizProductSku.Stocks[0].SalePrice;
            //        productSkuModel.SalePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
            //        productSkuModel.IsOffSell = bizProductSku.Stocks[0].IsOffSell;
            //    }

            //    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", productSkuModel);
            //}

            return result;
        }
    }
}
