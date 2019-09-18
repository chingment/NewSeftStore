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

        public static PrdKindService PrdKind
        {
            get
            {
                return new PrdKindService();
            }
        }

        public static PrdSubjectService PrdSubject
        {
            get
            {
                return new PrdSubjectService();
            }
        }

        public static MachineService Machine
        {
            get
            {
                return new MachineService();
            }
        }
    }
}
