using LocalS.BLL;
using Lumos;
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
            this.List = new PageEntity<PrdProductModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string MainImgUrl { get; set; }

        public bool Selected { get; set; }

        public PageEntity<PrdProductModel> List { get; set; }


    }
}
