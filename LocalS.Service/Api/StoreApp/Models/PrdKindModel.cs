using LocalS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class PrdKindModel
    {
        public PrdKindModel()
        {
            this.List = new List<PrdProductModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string MainImgUrl { get; set; }

        public bool Selected { get; set; }

        public List<PrdProductModel> List { get; set; }

        public int PageIndex { get; set; }

        public int Total { get; set; }
    }
}
