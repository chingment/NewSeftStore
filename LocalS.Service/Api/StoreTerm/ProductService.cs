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
    public class ProductService : BaseDbContext
    {
        public PageEntity<PrdProductModel2> GetPageList(int pageIndex, int pageSize, string merchId, string storeId, string machineId, string kindId)
        {
            var pageEntiy = new PageEntity<PrdProductModel2>();

            pageEntiy.PageIndex = pageIndex;
            pageEntiy.PageSize = pageSize;

            var query = (from m in CurrentDb.SellChannelStock  where (
                         m.MerchId== merchId &&
                         m.RefId == machineId && 
                         m.RefType == Entity.E_SellChannelRefType.Machine)
                         orderby m.CreateTime
                         select new { m.PrdProductId }).Distinct();

            if (!string.IsNullOrEmpty(kindId))
            {
                query = query.Where(p => (from d in CurrentDb.PrdProductKind
                                          where d.PrdKindId == kindId
                                          select d.PrdProductId).Contains(p.PrdProductId));
            }

            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderBy(m => m.PrdProductId).Skip(pageSize * pageIndex).Take(pageSize);

            var list = query.Distinct().ToList();


            foreach (var item in list)
            {
                var productModel = CacheServiceFactory.Product.GetInfo(merchId,item.PrdProductId);
                if (productModel != null)
                {
                    var prdProductModel2 = new PrdProductModel2();

                    prdProductModel2.Id = productModel.Id;
                    prdProductModel2.Name = productModel.Name;
                    prdProductModel2.MainImgUrl = productModel.MainImgUrl;
                    prdProductModel2.DisplayImgUrls = productModel.DisplayImgUrls;
                    prdProductModel2.DetailsDes = productModel.DetailsDes;
                    prdProductModel2.BriefDes = productModel.BriefDes;
                    prdProductModel2.RefSku.Id = productModel.RefSku.Id;
                    prdProductModel2.RefSku.ReceptionMode = productModel.RefSku.ReceptionMode;
                    prdProductModel2.RefSku.SumQuantity = productModel.RefSku.SumQuantity;
                    prdProductModel2.RefSku.LockQuantity = productModel.RefSku.LockQuantity;
                    prdProductModel2.RefSku.SellQuantity = productModel.RefSku.SellQuantity;
                    prdProductModel2.RefSku.IsOffSell = productModel.RefSku.IsOffSell;
                    prdProductModel2.RefSku.SalePrice = productModel.RefSku.SalePrice;
                    prdProductModel2.RefSku.SalePriceByVip = productModel.RefSku.SalePriceByVip;
                    prdProductModel2.RefSku.ShowPrice = productModel.RefSku.ShowPrice;
                    prdProductModel2.RefSku.SpecDes = productModel.RefSku.SpecDes;
                    prdProductModel2.RefSku.IsShowPrice = productModel.RefSku.IsShowPrice;
                    pageEntiy.Items.Add(prdProductModel2);
                }
            }

            return pageEntiy;

        }

    }
}
