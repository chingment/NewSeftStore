using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetShopMarketPageData
    {
        public RetShopMarketPageData()
        {
            this.Tabs = new List<PrdKindModel>();
            this.CurShop = new ShopModel();
        }
        public List<PrdKindModel> Tabs { get; set; }

        public ShopModel CurShop { get; set; }
    }
}
