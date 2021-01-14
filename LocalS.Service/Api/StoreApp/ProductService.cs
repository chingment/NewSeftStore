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
    public class ProductService : BaseService
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

            var pageEntiy = GetProducts(rup.PageIndex, rup.PageSize, rup.StoreId, "0", rup.ShopMode, rup.ShopMethod, rup.KindId);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntiy);

            return result;
        }

        public PageEntity<ProductSkuModel> GetProducts(int pageIndex, int pageSize, string storeId, string shopId, E_SellChannelRefType shopMode, E_OrderShopMethod shopMethod, string kindId)
        {
            var pageEntiy = new PageEntity<ProductSkuModel>();

            pageEntiy.PageIndex = pageIndex;
            pageEntiy.PageSize = pageSize;

            var store = BizFactory.Store.GetOne(storeId);

            var query = CurrentDb.SellChannelStock.Where(m =>
                m.MerchId == store.MerchId
                         && m.StoreId == storeId
                         && m.ShopId == shopId
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
                                          where d.StoreKindId == kindId && d.IsDelete == false
                                          select d.PrdProductId).Contains(p.PrdProductId));
            }

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderByDescending(r => r.PrdProductId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.ToList();

            foreach (var item in list)
            {
                LogUtil.Info("shopMode:" + shopMode);


                var r_productSku = CacheServiceFactory.Product.GetSkuStock(shopMode, store.MerchId, storeId, shopId, null, item.PrdProductSkuId);


                var m_productSku = new ProductSkuModel();

                m_productSku.Id = item.PrdProductSkuId;
                m_productSku.ProductId = item.PrdProductId;
                m_productSku.Name = r_productSku.Name;
                m_productSku.MainImgUrl = ImgSet.Convert_B(r_productSku.MainImgUrl);
                m_productSku.BriefDes = r_productSku.BriefDes;
                m_productSku.CharTags = r_productSku.CharTags;
                m_productSku.SpecDes = r_productSku.SpecDes;
                m_productSku.SpecItems = r_productSku.SpecItems;
                m_productSku.SpecIdx = r_productSku.SpecIdx;
                m_productSku.SpecIdxSkus = r_productSku.SpecIdxSkus;


                if (shopMethod == E_OrderShopMethod.Rent)
                {
                    m_productSku.IsMavkBuy = true;
                }
                else
                {
                    m_productSku.IsMavkBuy = r_productSku.IsMavkBuy;
                }


                if (shopMode == E_SellChannelRefType.Machine)
                {
                    m_productSku.SupReceiveMode = E_SupReceiveMode.SelfTakeByMachine;
                }
                else
                {
                    m_productSku.SupReceiveMode = r_productSku.SupReceiveMode;
                }

                if (r_productSku.Stocks != null)
                {
                    if (r_productSku.Stocks.Count > 0)
                    {
                        m_productSku.IsShowPrice = false;
                        m_productSku.SalePrice = r_productSku.Stocks[0].SalePrice;
                        m_productSku.IsOffSell = r_productSku.Stocks[0].IsOffSell;

                        pageEntiy.Items.Add(m_productSku);
                    }
                }
            }

            return pageEntiy;

        }

        public CustomJsonResult Details(string operater, string clientUserId, RupProductDetails rup)
        {
            var result = new CustomJsonResult();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            var r_productSku = CacheServiceFactory.Product.GetSkuStock(rup.ShopMode, store.MerchId, store.StoreId, rup.ShopId, null, rup.SkuId);

            var m_productSku = new ProductSkuModel();
            m_productSku.Id = r_productSku.Id;
            m_productSku.ProductId = r_productSku.ProductId;
            m_productSku.Name = r_productSku.Name;
            m_productSku.MainImgUrl = r_productSku.MainImgUrl;
            m_productSku.DisplayImgUrls = r_productSku.DisplayImgUrls;
            m_productSku.DetailsDes = r_productSku.DetailsDes;
            m_productSku.BriefDes = r_productSku.BriefDes;
            m_productSku.SpecItems = r_productSku.SpecItems;
            m_productSku.SpecIdx = r_productSku.SpecIdx;
            m_productSku.SpecIdxSkus = r_productSku.SpecIdxSkus;
            m_productSku.CharTags = r_productSku.CharTags;


            if (rup.ShopMethod == E_OrderShopMethod.Rent)
            {
                m_productSku.IsMavkBuy = true;
            }
            else
            {
                m_productSku.IsMavkBuy = r_productSku.IsMavkBuy;
            }

            if (rup.ShopMode == E_SellChannelRefType.Machine)
            {
                m_productSku.SupReceiveMode = E_SupReceiveMode.SelfTakeByMachine;
            }
            else
            {
                m_productSku.SupReceiveMode = r_productSku.SupReceiveMode;
            }

            m_productSku.KindId1 = r_productSku.KindId1;
            m_productSku.KindId2 = r_productSku.KindId2;
            m_productSku.KindId3 = r_productSku.KindId3;



            if (r_productSku.Stocks.Count > 0)
            {
                m_productSku.IsShowPrice = false;
                m_productSku.SalePrice = r_productSku.Stocks[0].SalePrice;
                m_productSku.IsOffSell = r_productSku.Stocks[0].IsOffSell;
                m_productSku.SellQuantity = r_productSku.Stocks.Sum(m => m.SellQuantity);
                m_productSku.IsUseRent = r_productSku.Stocks[0].IsUseRent;
                m_productSku.RentAmount = r_productSku.Stocks[0].RentMhPrice;
                m_productSku.RentTermUnit = E_RentTermUnit.Month;
                m_productSku.RentTermUnitText = "月";
                m_productSku.DepositAmount = r_productSku.Stocks[0].DepositPrice;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", m_productSku);

            return result;
        }

        public CustomJsonResult SkuStockInfo(string operater, string clientUserId, RupProductDetails rup)
        {
            var result = new CustomJsonResult();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            var r_productSku = CacheServiceFactory.Product.GetSkuStock(rup.ShopMode, store.MerchId, store.StoreId, rup.ShopId, null, rup.SkuId);

            bool isOffSell = true;
            bool isShowPrice = false;
            decimal salePrice = 0m;
            int sellQuantity = 0;
            if (r_productSku.Stocks.Count > 0)
            {
                isShowPrice = false;
                salePrice = r_productSku.Stocks[0].SalePrice;
                isOffSell = r_productSku.Stocks[0].IsOffSell;
                sellQuantity = r_productSku.Stocks.Sum(m => m.SellQuantity);
            }

            var data = new { skuId = rup.SkuId, name = r_productSku.Name, salePrice = salePrice, isShowPrice = isShowPrice, isOffSell = isOffSell, sellQuantity = sellQuantity };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", data);

            return result;
        }
    }
}
