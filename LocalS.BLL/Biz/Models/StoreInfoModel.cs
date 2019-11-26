using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class StoreInfoModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsDelete { get; set; }
        public string MerchId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string[] AllMachineIds { get; set; }
        public string[] SellMachineIds { get; set; }
        public string BriefDes { get; set; }

        public bool IsOpen { get; set; }

        public List<ImgSet> DisplayImgUrls { get; set; }
    }
}
