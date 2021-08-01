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

        public static ReplenishService Replenish
        {
            get
            {
                return new ReplenishService();
            }
        }

    }
}
