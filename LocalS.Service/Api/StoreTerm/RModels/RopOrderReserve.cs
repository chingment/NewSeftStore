using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{


    public class RopOrderReserve
    {
        public RopOrderReserve()
        {
            this.Skus = new List<SkuModel>();
        }

        public string MachineId { get; set; }
        public List<SkuModel> Skus { get; set; }

        public class SkuModel
        {
            public string SkuId { get; set; }
            public int Quantity { get; set; }
            public string SvcConsulterId { get; set; }
        }
    }
}
