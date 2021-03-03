using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public static class StoreTermServiceFactory
    {
        public static OrderService Order
        {
            get
            {
                return new OrderService();
            }
        }

        public static MachineService Machine
        {
            get
            {
                return new MachineService();
            }
        }

        public static ProductService Product
        {
            get
            {
                return new ProductService();
            }
        }

        public static StockSettingService StockSetting
        {
            get
            {
                return new StockSettingService();
            }
        }

        public static ImService ImService
        {
            get
            {
                return new ImService();
            }
        }

    }
}
