using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetStoreGetProductSkuList
    {
        public RetStoreGetProductSkuList()
        {
            this.StoreSellChannels = new List<StoreSellChannelModel>();
        }
        public List<StoreSellChannelModel> StoreSellChannels { get; set; }
    }
}
