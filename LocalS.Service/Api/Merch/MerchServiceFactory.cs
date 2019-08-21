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

        public static UserService User
        {
            get
            {
                return new UserService();
            }
        }

        public static ProductSkuService ProductSku
        {
            get
            {
                return new ProductSkuService();
            }
        }

        public static StoreService Store
        {
            get
            {
                return new StoreService();
            }
        }

        public static ProductKindService ProductKind
        {
            get
            {
                return new ProductKindService();
            }
        }

        public static ProductSubjectService ProductSubject
        {
            get
            {
                return new ProductSubjectService();
            }
        }
    }
}
