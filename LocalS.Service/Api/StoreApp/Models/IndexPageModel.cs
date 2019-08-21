using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class IndexPageModel
    {
        public IndexPageModel()
        {

        }
        public StoreModel Store { get; set; }

        public BannerModel Banner { get; set; }

        public PdAreaModel PdArea { get; set; }
    }
}
