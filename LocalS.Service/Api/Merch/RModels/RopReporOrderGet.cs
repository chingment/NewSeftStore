using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopReporOrderGet
    {
        public List<string[]> SellChannels { get; set; }

        public string[] TradeDateTimeArea { get; set; }
    }
}
