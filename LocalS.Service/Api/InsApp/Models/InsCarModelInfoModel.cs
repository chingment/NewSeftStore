using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class InsCarModelInfoModel
    {
        public string ModelName { get; set; }

        public string ModelCode { get; set; }

        public string MarketYear { get; set; }

        public int Seat { get; set; }

        public string Exhaust { get; set; }

        public string PurchasePrice { get; set; }
    }
}
