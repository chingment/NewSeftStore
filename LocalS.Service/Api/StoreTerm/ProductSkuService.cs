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
        public PageEntity<SkuModel> GetPageList(int pageIndex, int pageSize, string merchId, string storeId, string shopId, string machineId)
        {
            var pageEntiy = new PageEntity<SkuModel>();

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
m.ShopMode == Entity.E_ShopMode.Machine)
                         orderby m.CreateTime
                         select new { m.SkuId }).Distinct();

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderBy(m => m.SkuId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.Distinct().ToList();


            foreach (var item in list)
            {
                var r_Sku = CacheServiceFactory.Product.GetSkuStock(Entity.E_ShopMode.Machine, merchId, storeId, shopId, new string[] { machineId }, item.SkuId);

                var m_Sku = new SkuModel();
                m_Sku.SkuId = r_Sku.Id;
                m_Sku.SpuId = r_Sku.SpuId;
                m_Sku.Name = r_Sku.Name;
                m_Sku.MainImgUrl = ImgSet.Convert_B(r_Sku.MainImgUrl);
                m_Sku.DisplayImgUrls = r_Sku.DisplayImgUrls;
                m_Sku.DetailsDes = r_Sku.DetailsDes;
                m_Sku.BriefDes = r_Sku.BriefDes;
                m_Sku.SpecDes = SpecDes.GetDescribe(r_Sku.SpecDes);
                m_Sku.IsTrgVideoService = r_Sku.IsTrgVideoService;
                m_Sku.CharTags = r_Sku.CharTags;

                if (r_Sku.Stocks.Count > 0)
                {
                    m_Sku.IsShowPrice = false;
                    m_Sku.SalePrice = r_Sku.Stocks[0].SalePrice;
                    m_Sku.IsOffSell = r_Sku.Stocks[0].IsOffSell;
                    m_Sku.SellQuantity = r_Sku.Stocks.Sum(m => m.SellQuantity);
                }


                pageEntiy.Items.Add(m_Sku);

            }

            return pageEntiy;

        }

        public CustomJsonResult Search(RupSkuSearch rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetSkuSearch();

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


            ret.Skus = CacheServiceFactory.Product.SearchSku(machine.MerchId, "All", rup.Key);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
