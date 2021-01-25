using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class MerchServiceFactory
    {
        public static HomeService Home
        {
            get
            {
                return new HomeService();
            }
        }

        public static AdminUserService AdminUser
        {
            get
            {
                return new AdminUserService();
            }
        }

        public static ClientUserService ClientUser
        {
            get
            {
                return new ClientUserService();
            }
        }

        public static PrdProductService PrdProduct
        {
            get
            {
                return new PrdProductService();
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

        public static OrderService Order
        {
            get
            {
                return new OrderService();
            }
        }

        public static PayTransService PayTrans
        {
            get
            {
                return new PayTransService();
            }
        }

        public static PayRefundService PayRefund
        {
            get
            {
                return new PayRefundService();
            }
        }

        public static AdService Ad
        {
            get
            {
                return new AdService();
            }
        }

        public static ReportService Report
        {
            get
            {
                return new ReportService();
            }
        }

        public static LogService Log
        {
            get
            {
                return new LogService();
            }
        }

        public static CouponService Coupon
        {
            get
            {
                return new CouponService();
            }
        }

        public static SupplierService Supplier
        {
            get
            {
                return new SupplierService();
            }
        }

        public static MemberRightService MemberRight
        {
            get
            {
                return new MemberRightService();
            }
        }

        public static ShopService Shop
        {
            get
            {
                return new ShopService();
            }
        }

        
    }
}
