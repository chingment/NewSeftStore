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

        public static StoreService Store
        {
            get
            {
                return new StoreService();
            }
        }

        public static MachineService Machine
        {
            get
            {
                return new MachineService();
            }
        }

        public static BackgroundJobProvider BackgroundJob
        {
            get
            {
                return new BackgroundJobProvider();
            }
        }

        public static ProductSkuService ProductSku
        {
            get
            {
                return new ProductSkuService();
            }
        }

        public static AppSoftwareService AppSoftware
        {
            get
            {
                return new AppSoftwareService();
            }
        }
        public static OperateLogService OperateLog
        {
            get
            {
                return new OperateLogService();
            }
        }
    }
}
