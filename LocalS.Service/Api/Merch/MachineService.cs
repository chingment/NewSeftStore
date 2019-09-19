using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class MachineService : BaseDbContext
    {

        public StatusModel GetStatus()
        {
            var status = new StatusModel();
            return status;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupMachineGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.MerchMachine
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, u.MachineId, u.Name, u.StoreId, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == item.MachineId).FirstOrDefault();

                olist.Add(new
                {
                    Id = item.MachineId,
                    Name = item.Name,
                    MainImgUrl = machine.MainImgUrl,
                    Status = GetStatus(),
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }


        public CustomJsonResult InitStock(string operater, string merchId, string machineId)
        {
            var ret = new RetMachineInitStock();

            var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).ToList();


            foreach (var merchMachine in merchMachines)
            {
                if (merchMachine.MachineId == machineId)
                {
                    ret.CurMachine.Id = merchMachine.MachineId;
                    ret.CurMachine.Name = merchMachine.Name;
                }

                ret.Machines.Add(new MachineModel { Id = merchMachine.MachineId, Name = merchMachine.Name });
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }


        public CustomJsonResult GetStockList(string operater, string merchId, RupMachineGetStockList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SellChannelStock
                         where
                         u.MerchId == merchId &&
                         u.RefType == E_SellChannelRefType.Machine &&
                         u.RefId == rup.MachineId
                         select new { u.Id, u.PrdProductSkuId, u.MerchId, u.RefType, u.RefId, u.SalePrice, u.IsOffSell, u.LockQuantity, u.SumQuantity, u.SellQuantity });

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.PrdProductSkuId).Skip(pageSize * (pageIndex)).Take(pageSize);

            List<object> olist = new List<object>();

            var list = query.ToList();
            foreach (var item in list)
            {
                var prdProductSku = BizFactory.PrdProduct.GetProductSku(item.PrdProductSkuId);
                if (prdProductSku != null)
                {
                    var productSkuModel = new ProductSkuModel();
                    productSkuModel.Id = prdProductSku.Id;
                    productSkuModel.Name = prdProductSku.Name;
                    productSkuModel.DispalyImgUrls = prdProductSku.DispalyImgUrls.ToJsonObject<List<ImgSet>>();
                    productSkuModel.MainImgUrl = prdProductSku.MainImgUrl;
                    productSkuModel.BriefDes = prdProductSku.BriefDes;
                    productSkuModel.DetailsDes = prdProductSku.DetailsDes;
                    productSkuModel.SumQuantity = item.SumQuantity;
                    productSkuModel.LockQuantity = item.LockQuantity;
                    productSkuModel.SellQuantity = item.SellQuantity;
                    productSkuModel.SalePrice = item.SalePrice;
                    productSkuModel.IsOffSell = item.IsOffSell;
                    olist.Add(productSkuModel);
                }
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

    }
}
