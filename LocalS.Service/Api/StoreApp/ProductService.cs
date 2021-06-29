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

            //todo 
            if(rup.ShopMode== E_ShopMode.Mall)
            {
                rup.ShopId = "0";
            }

            var pageEntiy = GetProducts(rup.PageIndex, rup.PageSize, rup.StoreId, rup.ShopId, rup.ShopMode, rup.ShopMethod, rup.KindId);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntiy);

            return result;
        }

        public PageEntity<SkuModel> GetProducts(int pageIndex, int pageSize, string storeId, string shopId, E_ShopMode shopMode, E_ShopMethod shopMethod, string kindId)
        {
            var pageEntiy = new PageEntity<SkuModel>();

            pageEntiy.PageIndex = pageIndex;
            pageEntiy.PageSize = pageSize;

            var store = BizFactory.Store.GetOne(storeId);

            var query = CurrentDb.SellChannelStock.Where(m =>
                m.MerchId == store.MerchId
                         && m.StoreId == storeId
                         && m.ShopId == shopId
                         && m.ShopMode == shopMode

                ).AsEnumerable()
                     .OrderBy(x => x.Id)
                     .GroupBy(x => x.SpuId)
                     .Select(g => new { g, count = g.Count() })
                     .SelectMany(t => t.g.Select(b => b)
                     .Zip(Enumerable.Range(1, t.count), (j, i) => new { j.SpuId, j.SkuId, rn = i })).Where(m => m.rn == 1); ;

            if (!string.IsNullOrEmpty(kindId))
            {
                query = query.Where(p => (from d in CurrentDb.StoreKindSpu
                                          where d.StoreKindId == kindId && d.IsDelete == false
                                          select d.SpuId).Contains(p.SpuId));
            }

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderByDescending(r => r.SpuId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.ToList();

            foreach (var item in list)
            {
                LogUtil.Info("shopMode:" + shopMode);


                var r_Sku = CacheServiceFactory.Product.GetSkuStock(shopMode, store.MerchId, storeId, shopId, null, item.SkuId);


                var m_Sku = new SkuModel();

                m_Sku.Id = item.SkuId;
                m_Sku.SpuId = item.SpuId;
                m_Sku.Name = r_Sku.Name;
                m_Sku.MainImgUrl = ImgSet.Convert_B(r_Sku.MainImgUrl);
                m_Sku.BriefDes = r_Sku.BriefDes;
                m_Sku.CharTags = r_Sku.CharTags;
                m_Sku.SpecDes = r_Sku.SpecDes;
                m_Sku.SpecItems = r_Sku.SpecItems;
                m_Sku.SpecIdx = r_Sku.SpecIdx;
                m_Sku.SpecIdxSkus = r_Sku.SpecIdxSkus;


                if (shopMethod == E_ShopMethod.Rent)
                {
                    m_Sku.IsMavkBuy = true;
                }
                else
                {
                    m_Sku.IsMavkBuy = r_Sku.IsMavkBuy;
                }


                if (shopMode == E_ShopMode.Device)
                {
                    m_Sku.SupReceiveMode = E_SupReceiveMode.SelfTakeByDevice;
                }
                else
                {
                    m_Sku.SupReceiveMode = r_Sku.SupReceiveMode;
                }

                if (r_Sku.Stocks != null)
                {
                    if (r_Sku.Stocks.Count > 0)
                    {
                        m_Sku.IsShowPrice = false;
                        m_Sku.SalePrice = r_Sku.Stocks[0].SalePrice;
                        m_Sku.IsOffSell = r_Sku.Stocks[0].IsOffSell;

                        pageEntiy.Items.Add(m_Sku);
                    }
                }
            }

            return pageEntiy;

        }

        public CustomJsonResult Details(string operater, string clientUserId, RupProductDetails rup)
        {
            var result = new CustomJsonResult();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            var r_Sku = CacheServiceFactory.Product.GetSkuStock(rup.ShopMode, store.MerchId, store.StoreId, rup.ShopId, null, rup.SkuId);

            var m_Sku = new SkuModel();
            m_Sku.Id = r_Sku.Id;
            m_Sku.SpuId = r_Sku.SpuId;
            m_Sku.Name = r_Sku.Name;
            m_Sku.MainImgUrl = r_Sku.MainImgUrl;
            m_Sku.DisplayImgUrls = r_Sku.DisplayImgUrls;
            m_Sku.DetailsDes = r_Sku.DetailsDes;
            m_Sku.BriefDes = r_Sku.BriefDes;
            m_Sku.SpecItems = r_Sku.SpecItems;
            m_Sku.SpecIdx = r_Sku.SpecIdx;
            m_Sku.SpecIdxSkus = r_Sku.SpecIdxSkus;
            m_Sku.CharTags = r_Sku.CharTags;


            if (rup.ShopMethod == E_ShopMethod.Rent)
            {
                m_Sku.IsMavkBuy = true;
            }
            else
            {
                m_Sku.IsMavkBuy = r_Sku.IsMavkBuy;
            }

            if (rup.ShopMode == E_ShopMode.Device)
            {
                m_Sku.SupReceiveMode = E_SupReceiveMode.SelfTakeByDevice;
            }
            else
            {
                m_Sku.SupReceiveMode = r_Sku.SupReceiveMode;
            }

            m_Sku.KindId1 = r_Sku.KindId1;
            m_Sku.KindId2 = r_Sku.KindId2;
            m_Sku.KindId3 = r_Sku.KindId3;



            if (r_Sku.Stocks.Count > 0)
            {
                m_Sku.IsShowPrice = false;
                m_Sku.SalePrice = r_Sku.Stocks[0].SalePrice;
                m_Sku.IsOffSell = r_Sku.Stocks[0].IsOffSell;
                m_Sku.SellQuantity = r_Sku.Stocks.Sum(m => m.SellQuantity);
                m_Sku.IsUseRent = r_Sku.Stocks[0].IsUseRent;
                m_Sku.RentAmount = r_Sku.Stocks[0].RentMhPrice;
                m_Sku.RentTermUnit = E_RentTermUnit.Month;
                m_Sku.RentTermUnitText = "月";
                m_Sku.DepositAmount = r_Sku.Stocks[0].DepositPrice;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", m_Sku);

            return result;
        }

        public CustomJsonResult SkuStockInfo(string operater, string clientUserId, RupProductDetails rup)
        {
            var result = new CustomJsonResult();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            var r_Sku = CacheServiceFactory.Product.GetSkuStock(rup.ShopMode, store.MerchId, store.StoreId, rup.ShopId, null, rup.SkuId);

            bool isOffSell = true;
            bool isShowPrice = false;
            decimal salePrice = 0m;
            int sellQuantity = 0;
            if (r_Sku.Stocks.Count > 0)
            {
                isShowPrice = false;
                salePrice = r_Sku.Stocks[0].SalePrice;
                isOffSell = r_Sku.Stocks[0].IsOffSell;
                sellQuantity = r_Sku.Stocks.Sum(m => m.SellQuantity);
            }

            var data = new { skuId = rup.SkuId, name = r_Sku.Name, salePrice = salePrice, isShowPrice = isShowPrice, isOffSell = isOffSell, sellQuantity = sellQuantity };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", data);

            return result;
        }
    }
}
