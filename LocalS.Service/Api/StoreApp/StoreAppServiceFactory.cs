using LocalS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class StoreAppServiceFactory : BaseDbContext
    {
        public static GlobalService Global
        {
            get
            {
                return new GlobalService();
            }
        }

        public static IndexService Index
        {
            get
            {
                return new IndexService();
            }
        }

        public static ProductService Product
        {
            get
            {
                return new ProductService();
            }
        }

        public static CartService Cart
        {
            get
            {
                return new CartService();
            }
        }

        public static PersonalService Personal
        {
            get
            {
                return new PersonalService();
            }
        }

        public static DeliveryAddressService DeliveryAddress
        {
            get
            {
                return new DeliveryAddressService();
            }
        }

        public static OrderService Order
        {
            get
            {
                return new OrderService();
            }
        }

        public static CouponService Coupon
        {
            get
            {
                return new CouponService();
            }
        }
        public static ProductKindService ProductKind
        {
            get
            {
                return new ProductKindService();
            }
        }

        public static StoreService Store
        {
            get
            {
                return new StoreService();
            }
        }

        public static OperateService Operate
        {
            get
            {
                return new OperateService();
            }
        }

        public static SearchService Search
        {
            get
            {
                return new SearchService();
            }
        }

    }
}
