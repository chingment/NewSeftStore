using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class PdAreaModel
    {
        public PdAreaModel()
        {

            this.Tabs = new List<Tab>();
        }
        public int TabsSliderIndex { get; set; }

        public List<Tab> Tabs { get; set; }

        public class Tab
        {
            public Tab()
            {
                this.List = new List<SkuModel>();
            }

            public string Id { get; set; }
            public string MainImgUrl { get; set; }
            public string Name { get; set; }
            public List<SkuModel> List { get; set; }
        }
    }
}
