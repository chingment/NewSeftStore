using LocalS.BLL.Biz;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopShopSave
    {
        public RopShopSave()
        {
            this.AddressDetails = new AddressDetailsModel();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public AddressDetailsModel AddressDetails { get; set; }
        public string AreaName { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public string BriefDes { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
    }
}
