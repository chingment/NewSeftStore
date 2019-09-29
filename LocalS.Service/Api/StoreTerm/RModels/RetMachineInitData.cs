using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetMachineInitData
    {
        public RetMachineInitData()
        {
            this.Machine = new MachineModel();
            this.Banners = new List<BannerModel>();
            this.Products = new Dictionary<string, PrdProductModel2>();
            this.ProductKinds = new List<ProductKindModel>();
        }
        public MachineModel Machine { get; set; }
        public List<BannerModel> Banners { get; set; }
        public Dictionary<string, PrdProductModel2> Products { get; set; }
        public List<ProductKindModel> ProductKinds { get; set; }
    }
}
