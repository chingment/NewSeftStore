using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public static class InsAppServiceFactory
    {
        public static HomeService Home
        {
            get
            {
                return new HomeService();
            }
        }

        public static InsCarService InsCar
        {
            get
            {
                return new InsCarService();
            }
        }

        public static OwnService Own
        {
            get
            {
                return new OwnService();
            }
        }
    }
}
