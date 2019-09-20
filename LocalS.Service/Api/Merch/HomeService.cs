using System;
using LocalS.BLL;
using LocalS.Service.UI;
using Lumos;
using Lumos.DbRelay;
using Lumos.Session;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class HomeService
    {
        public CustomJsonResult GetIndexPageData(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            return result;
        }

        public CustomJsonResult GetTodaySummary(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            return result;
        }

        public CustomJsonResult GetStoreGmvRl(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            return result;
        }

        public CustomJsonResult GetProductSkuSaleRl(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            return result;
        }
    }
}
