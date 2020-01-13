using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class ReportService : BaseDbContext
    {
        public CustomJsonResult MachineStockInit(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetReportMachineStockInit();

            var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).ToList();


            foreach (var merchMachine in merchMachines)
            {
                string storeName = "未绑定店铺";
                var machie = BizFactory.Machine.GetOne(merchMachine.MachineId);
                if (!string.IsNullOrEmpty(machie.StoreName))
                {
                    storeName = machie.StoreName;
                }

                ret.Machines.Add(new MachineModel { Id = merchMachine.MachineId, Name = merchMachine.Name, StoreName = storeName });
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult MachineStockGet(string operater, string merchId, RupReportMachineStockGet rup)
        {

            var result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rup.MachineId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择机器");
            }

            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.SellChannelRefType == Entity.E_SellChannelRefType.Machine && m.SellChannelRefId == rup.MachineId).ToList();

            var machine = BizFactory.Machine.GetOne(rup.MachineId);

            List<object> olist = new List<object>();

            foreach (var sellChannelStock in sellChannelStocks)
            {
                var productSku = CacheServiceFactory.ProductSku.GetInfo(sellChannelStock.MerchId, sellChannelStock.PrdProductSkuId);
                if (productSku != null)
                {
                    olist.Add(new
                    {
                        StoreName = machine.StoreName,
                        MachineName = machine.Name,
                        ProductSkuId = productSku.Id,
                        ProductSkuName = productSku.Name,
                        ProductSkuSpecDes = productSku.SpecDes,
                        ProductSkuBarCode = productSku.BarCode,
                        SlotId = sellChannelStock.SlotId,
                        SellQuantity = sellChannelStock.SellQuantity,
                        WaitPayLockQuantity = sellChannelStock.WaitPayLockQuantity,
                        WaitPickupLockQuantity = sellChannelStock.WaitPickupLockQuantity,
                        LockQuantity = sellChannelStock.WaitPickupLockQuantity + sellChannelStock.WaitPayLockQuantity,
                        SumQuantity = sellChannelStock.SumQuantity,
                        IsOffSell = sellChannelStock.IsOffSell
                    });
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;

        }
    }
}
