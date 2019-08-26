using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{

    public class CartProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ShowPrice { get; set; }
        public string BriefInfo { get; set; }
        public List<Lumos.ImgSet> DispalyImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public string SpecDes { get; set; }
        public string CartId { get; set; }
        public int Quantity { get; set; }
        public bool Selected { get; set; }
        public decimal SumPrice { get; set; }
        public E_ReceptionMode ReceptionMode { get; set; }

    }
}
