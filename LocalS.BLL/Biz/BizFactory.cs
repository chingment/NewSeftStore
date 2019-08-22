using Lumos.BLL.Biz;
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
    }
}
