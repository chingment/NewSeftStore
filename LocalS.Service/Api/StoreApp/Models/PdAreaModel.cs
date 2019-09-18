using LocalS.BLL;
using Lumos;
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
                this.List = new PageEntity<PrdProductModel>();
            }

            public string Id { get; set; }
            public string MainImgUrl { get; set; }
            public string Name { get; set; }
            public PageEntity<PrdProductModel> List { get; set; }
        }
    }
}
