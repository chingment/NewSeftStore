using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class PayRefund2CheckStatusModel
    {
        public string Id { get; set; }
        public string PayTransId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public E_PayPartner PayPartner { get; set; }
    }
}
