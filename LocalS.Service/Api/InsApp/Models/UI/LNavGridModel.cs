using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class LNavGridModel
    {

        public LNavGridModel()
        {
            this.Items = new List<LNavGridItemModel>();
        }

        public string Title { get; set; }

        public List<LNavGridItemModel> Items { get; set; }
    }
}
