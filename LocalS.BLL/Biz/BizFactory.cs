using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class BizFactory
    {
        public static OrderService Order
        {
            get
            {
                return new OrderService();
            }
        }

        public static MerchService Merch
        {
            get
            {
                return new MerchService();
            }
        }

        public static PrdProductService PrdProduct
        {
            get
            {
                return new PrdProductService();
            }
        }
    }
}
