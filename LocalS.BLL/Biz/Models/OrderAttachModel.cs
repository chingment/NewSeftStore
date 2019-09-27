using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class OrderAttachModel
    {
        public string MerchId { get; set; }

        public string StoreId { get; set; }

        public E_OrderPayCaller PayCaller { get; set; }
    }
}
