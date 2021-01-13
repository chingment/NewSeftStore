using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class ProductSkuService : BaseService
    {
        public PageEntity<ProductSkuModel> GetPageList(int pageIndex, int pageSize, string merchId, string storeId, string shopId, string machineId)
        {
            var pageEntiy = new PageEntity<ProductSkuModel>();

            pageEntiy.PageIndex = pageIndex;
            pageEntiy.PageSize = pageSize;


            LogUtil.Info("merchId:" + merchId);
            LogUtil.Info("storeId:" + storeId);
            LogUtil.Info("shopId:" + shopId);
            LogUtil.Info("MachineId:" + machineId);
            var query = (from m in CurrentDb.SellChannelStock
                         where (
m.MerchId == merchId &&
m.StoreId == storeId &&
m.ShopId == shopId &&
m.MachineId == machineId &&
m.SellChannelRefType == Entity.E_SellChannelRefType.Machine)
                         orderby m.CreateTime
                         select new { m.PrdProductSkuId }).Distinct();

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderBy(m => m.PrdProductSkuId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.Distinct().ToList();


            foreach (var item in list)
            {
                var r_productSku = CacheServiceFactory.Product.GetSkuStock(Entity.E_SellChannelRefType.Machine, merchId, storeId, shopId, new string[] { machineId }, item.PrdProductSkuId);

                var m_productSku = new ProductSkuModel();
                m_productSku.ProductSkuId = r_productSku.Id;
                m_productSku.ProductId = r_productSku.ProductId;
                m_productSku.Name = r_productSku.Name;
                m_productSku.MainImgUrl = ImgSet.Convert_B(r_productSku.MainImgUrl);
                m_productSku.DisplayImgUrls = r_productSku.DisplayImgUrls;
                m_productSku.DetailsDes = r_productSku.DetailsDes;
                m_productSku.BriefDes = r_productSku.BriefDes;
                m_productSku.SpecDes = SpecDes.GetDescribe(r_productSku.SpecDes);
                m_productSku.IsTrgVideoService = r_productSku.IsTrgVideoService;
                m_productSku.CharTags = r_productSku.CharTags;

                if (r_productSku.Stocks.Count > 0)
                {
                    m_productSku.IsShowPrice = false;
                    m_productSku.SalePrice = r_productSku.Stocks[0].SalePrice;
                    m_productSku.IsOffSell = r_productSku.Stocks[0].IsOffSell;
                    m_productSku.SellQuantity = r_productSku.Stocks.Sum(m => m.SellQuantity);
                }


                pageEntiy.Items.Add(m_productSku);

            }

            return pageEntiy;

        }

        public CustomJsonResult Search(RupProductSkuSearch rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetProductSkuSearch();

            var machine = BizFactory.Machine.GetOne(rup.MachineId);

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未登记");
            }

            if (string.IsNullOrEmpty(machine.MerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户");
            }

            if (string.IsNullOrEmpty(machine.StoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户店铺");
            }


            ret.ProductSkus = CacheServiceFactory.Product.SearchSku(machine.MerchId, "All", rup.Key);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
