
using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetOrderPayStatusQuery
    {
        public string PayTransId { get; set; }
        public E_PayStatus PayStatus { get; set; }
        public List<OrderSkuByPickupModel> Skus { get; set; }
        public string OrderId { get; set; }

    }
}
