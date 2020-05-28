using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class ProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public string BriefDes { get; set; }
        public List<string> CharTags { get; set; }
        public List<SpecItem> SpecItems { get; set; }
        public List<SpecSku> SpecSkus { get; set; }
    }
}
