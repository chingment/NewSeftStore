using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
using System;
using System.Collections.Generic;
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

            return result;
        }

        public CustomJsonResult MachineStockGet(string operater, string merchId, RupReportMachineStockGet rup)
        {

            var result = new CustomJsonResult();


            return result;
        }
    }
}
