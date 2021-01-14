using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetIndexPageData
    {
        public RetIndexPageData()
        {
            this.Store = new StoreModel();
            this.Banner = new BannerModel();
            this.PdArea = new PdAreaModel();
        }

        public StoreModel Store { get; set; }
        public BannerModel Banner { get; set; }
        public PdAreaModel PdArea { get; set; }
        public bool IsSupMachineShop { get; set; }
    }
}
