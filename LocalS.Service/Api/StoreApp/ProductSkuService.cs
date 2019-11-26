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
    public class ProductSkuService : BaseDbContext
    {
        public CustomJsonResult List(string operater, string clientUserId, RupProductList rup)
        {
            var result = new CustomJsonResult();

            var pageEntiy = GetPageList(rup.PageIndex, rup.PageSize, rup.StoreId, rup.KindId);


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntiy);

            return result;
        }


        public PageEntity<ProductSkuModel> GetPageList(int pageIndex, int pageSize, string storeId, string kindId)
        {
            var pageEntiy = new PageEntity<ProductSkuModel>();

            pageEntiy.PageIndex = pageIndex;
            pageEntiy.PageSize = pageSize;

            var store = BizFactory.Store.GetOne(storeId);

            var query = (from m in CurrentDb.SellChannelStock
                         where m.MerchId == store.MerchId
                         && store.SellMachineIds.Contains(m.RefId)
                         && m.RefType == Entity.E_SellChannelRefType.Machine
                         select new { m.PrdProductId, m.PrdProductSkuId }).Distinct();

            //var query = CurrentDb.SellChannelStock.Where(m =>
            //m.MerchId == store.MerchId
            //&& (store.MachineIds.Contains(m.RefId) && m.RefType == Entity.E_SellChannelRefType.Machine)
            //);

            if (!string.IsNullOrEmpty(kindId))
            {
                query = query.Where(p => (from d in CurrentDb.PrdProductKind
                                          where d.PrdKindId == kindId
                                          select d.PrdProductId).Contains(p.PrdProductId));
            }

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderByDescending(r => r.PrdProductSkuId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.ToList();


            foreach (var item in list)
            {
                var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(store.MerchId, store.Id, store.SellMachineIds, item.PrdProductSkuId);
                if (bizProductSku != null)
                {
                    var productSkuModel = new ProductSkuModel();
                    productSkuModel.Id = bizProductSku.Id;
                    productSkuModel.ProductId = bizProductSku.ProductId;
                    productSkuModel.Name = bizProductSku.Name;
                    productSkuModel.MainImgUrl = bizProductSku.MainImgUrl;
                    productSkuModel.DisplayImgUrls = bizProductSku.DisplayImgUrls;
                    productSkuModel.DetailsDes = bizProductSku.DetailsDes;
                    productSkuModel.BriefDes = bizProductSku.BriefDes;
                    productSkuModel.SpecDes = bizProductSku.SpecDes;

                    if (bizProductSku.Stocks.Count > 0)
                    {
                        productSkuModel.IsShowPrice = false;
                        productSkuModel.SalePrice = bizProductSku.Stocks[0].SalePrice;
                        productSkuModel.SalePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
                        productSkuModel.IsOffSell = bizProductSku.Stocks[0].IsOffSell;
                    }


                    pageEntiy.Items.Add(productSkuModel);
                }
            }

            return pageEntiy;

        }

        public CustomJsonResult Details(string operater, string clientUserId, RupProductDetails rup)
        {
            var result = new CustomJsonResult();

            var store = BizFactory.Store.GetOne(rup.StoreId);
            var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(store.MerchId, store.Id, store.SellMachineIds, rup.SkuId);
            if (bizProductSku != null)
            {
                var productSkuModel = new ProductSkuModel();
                productSkuModel.Id = bizProductSku.Id;
                productSkuModel.ProductId = bizProductSku.ProductId;
                productSkuModel.Name = bizProductSku.Name;
                productSkuModel.MainImgUrl = bizProductSku.MainImgUrl;
                productSkuModel.DisplayImgUrls = bizProductSku.DisplayImgUrls;
                productSkuModel.DetailsDes = bizProductSku.DetailsDes;
                productSkuModel.BriefDes = bizProductSku.BriefDes;
                productSkuModel.SpecDes = bizProductSku.SpecDes;

                if (bizProductSku.Stocks.Count > 0)
                {
                    productSkuModel.IsShowPrice = false;
                    productSkuModel.SalePrice = bizProductSku.Stocks[0].SalePrice;
                    productSkuModel.SalePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
                    productSkuModel.IsOffSell = bizProductSku.Stocks[0].IsOffSell;
                }

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", productSkuModel);
            }

            return result;
        }
    }
}
