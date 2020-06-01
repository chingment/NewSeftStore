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
m.StoreId == storeId &&
m.SellChannelRefId == machineId &&
m.SellChannelRefType == Entity.E_SellChannelRefType.Machine)
                         orderby m.CreateTime
                         select new { m.PrdProductSkuId }).Distinct();

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderBy(m => m.PrdProductSkuId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.Distinct().ToList();


            foreach (var item in list)
            {
                var bizProductSku = CacheServiceFactory.Product.GetSkuStock(merchId, storeId, new string[] { machineId }, item.PrdProductSkuId);
                if (bizProductSku != null)
                {
                    var productSkuModel = new ProductSkuModel();
                    productSkuModel.Id = bizProductSku.Id;
                    productSkuModel.ProductId = bizProductSku.ProductId;
                    productSkuModel.Name = bizProductSku.Name;
                    productSkuModel.MainImgUrl = ImgSet.Convert_B(bizProductSku.MainImgUrl);
                    productSkuModel.DisplayImgUrls = bizProductSku.DisplayImgUrls;
                    productSkuModel.DetailsDes = bizProductSku.DetailsDes;
                    productSkuModel.BriefDes = bizProductSku.BriefDes;
                    productSkuModel.SpecDes = SpecDes.GetDescribe(bizProductSku.SpecDes);
                    productSkuModel.IsTrgVideoService = bizProductSku.IsTrgVideoService;
                    productSkuModel.CharTags = bizProductSku.CharTags;
                    if (bizProductSku.Stocks != null)
                    {
                        if (bizProductSku.Stocks.Count > 0)
                        {
                            productSkuModel.IsShowPrice = false;
                            productSkuModel.SalePrice = bizProductSku.Stocks[0].SalePrice;
                            productSkuModel.SalePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
                            productSkuModel.IsOffSell = bizProductSku.Stocks[0].IsOffSell;
                            productSkuModel.SumQuantity = bizProductSku.Stocks.Sum(m => m.SumQuantity);
                            productSkuModel.LockQuantity = bizProductSku.Stocks.Sum(m => m.LockQuantity);
                            productSkuModel.SellQuantity = bizProductSku.Stocks.Sum(m => m.SellQuantity);
                        }
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


            ret.ProductSkus = CacheServiceFactory.Product.Search(machine.MerchId, "All", rup.Key);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
