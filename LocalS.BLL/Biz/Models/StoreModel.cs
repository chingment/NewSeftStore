using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class AddressDetailsModel
    {
        public AddressDetailsModel()
        {
            this.AddressComponents = new AddressComponentsModel();
            this.Point = new PointModel();
        }
        public string Address { get; set; }
        public AddressComponentsModel AddressComponents { get; set; }
        public PointModel Point { get; set; }

        public class PointModel
        {
            public double Lng { get; set; }
            public double Lat { get; set; }

            public string Ye { get; set; }
        }

        public class AddressComponentsModel
        {
            public string StreetNumber { get; set; }
            public string Street { get; set; }
            public string District { get; set; }
            public string City { get; set; }
            public string Province { get; set; }

            public string Town { get; set; }
        }
    }

    public class StoreModel
    {
        public StoreModel()
        {

        }
        public string StoreId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsDelete { get; set; }
        public string MerchId { get; set; }
        public string MerchName { get; set; }
        public string BriefDes { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public bool IsTestMode { get; set; }
    }
}
