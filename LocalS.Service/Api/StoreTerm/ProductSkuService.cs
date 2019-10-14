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
    public class ProductSkuService : BaseDbContext
    {
        public PageEntity<ProductSkuModel> GetPageList(int pageIndex, int pageSize, string merchId, string storeId, string machineId)
        {
            var pageEntiy = new PageEntity<ProductSkuModel>();

            pageEntiy.PageIndex = pageIndex;
            pageEntiy.PageSize = pageSize;

            var query = (from m in CurrentDb.SellChannelStock
                         where (
m.MerchId == merchId &&
m.RefId == machineId &&
m.RefType == Entity.E_SellChannelRefType.Machine)
                         orderby m.CreateTime
                         select new { m.PrdProductSkuId }).Distinct();

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderBy(m => m.PrdProductSkuId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.Distinct().ToList();


            foreach (var item in list)
            {
                var productSkuInfoAndStockModel = CacheServiceFactory.ProductSku.GetInfoAndStock(merchId, item.PrdProductSkuId);
                if (productSkuInfoAndStockModel != null)
                {
                    var productSkuModel = new ProductSkuModel();
                    productSkuModel.Id = productSkuInfoAndStockModel.Id;
                    productSkuModel.ProductId = productSkuInfoAndStockModel.ProductId;
                    productSkuModel.Name = productSkuInfoAndStockModel.Name;
                    productSkuModel.MainImgUrl = productSkuInfoAndStockModel.MainImgUrl;
                    productSkuModel.DisplayImgUrls = productSkuInfoAndStockModel.DisplayImgUrls;
                    productSkuModel.DetailsDes = productSkuInfoAndStockModel.DetailsDes;
                    productSkuModel.BriefDes = productSkuInfoAndStockModel.BriefDes;
                    productSkuModel.SpecDes = productSkuInfoAndStockModel.SpecDes;

                    var stocks = productSkuInfoAndStockModel.Stocks.Where(m => m.RefType == Entity.E_SellChannelRefType.Machine && m.RefId == machineId).ToList();

                    if (stocks.Count > 0)
                    {
                        productSkuModel.IsShowPrice = false;
                        productSkuModel.SalePrice = stocks[0].SalePrice;
                        productSkuModel.SalePriceByVip = stocks[0].SalePriceByVip;
                        productSkuModel.IsOffSell = stocks[0].IsOffSell;
                        productSkuModel.SumQuantity = stocks.Sum(m => m.SumQuantity);
                        productSkuModel.LockQuantity = stocks.Sum(m => m.LockQuantity);
                        productSkuModel.SellQuantity = stocks.Sum(m => m.SellQuantity);
                    }


                    pageEntiy.Items.Add(productSkuModel);
                }
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

            ret.ProductSkus = CacheServiceFactory.ProductSku.Search(machine.MerchId, rup.Key);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
