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

        public static DeviceService Device
        {
            get
            {
                return new DeviceService();
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

        public static EventService Event
        {
            get
            {
                return new EventService();
            }
        }

        public static ByPointService ByPoint
        {
            get
            {
                return new ByPointService();
            }
        }

        public static BackgroundJobService BackgroundJob
        {
            get
            {
                return new BackgroundJobService();
            }
        }

        public static CouponService Coupon
        {
            get
            {
                return new CouponService();
            }
        }

        public static ErpService Erp
        {
            get
            {
                return new ErpService();
            }
        }
    }
}
