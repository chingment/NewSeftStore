using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{
    public class IotTermServiceFactory
    {
        public static OrderService Order
        {
            get
            {
                return new OrderService();
            }
        }
        public static DeviceService Device
        {
            get
            {
                return new DeviceService();
            }
        }

        public static ProductService Product
        {
            get
            {
                return new ProductService();
            }
        }

    }
}
