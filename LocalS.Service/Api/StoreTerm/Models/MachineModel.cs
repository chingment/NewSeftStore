using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class MachineModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MerchName { get; set; }
        public string StoreName { get; set; }
        public string LogoImgUrl { get; set; }
        public string CsrQrCode { get; set; }
        public string CabinetId_1 { get; set; }
        public string CabinetName_1 { get; set; }
        public CabineRowColLayoutModel CabineRowColLayout_1 { get; set; }

    }
}
