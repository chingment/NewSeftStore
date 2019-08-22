using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetCartGetPageData
    {
        public RetCartGetPageData()
        {
            this.Blocks = new List<CartBlockModel>();
        }

        public List<CartBlockModel> Blocks { get; set; }

        public int Count { get; set; }

        public decimal SumPrice { get; set; }

        public int CountBySelected { get; set; }

        public decimal SumPriceBySelected { get; set; }
    }
}
