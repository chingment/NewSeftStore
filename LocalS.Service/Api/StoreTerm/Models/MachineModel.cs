using LocalS.BLL.Biz;
using LocalS.Entity;
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

        public string DeviceId { get; set; }
        public string MerchName { get; set; }
        public string StoreName { get; set; }
        public string LogoImgUrl { get; set; }
        public string CsrQrCode { get; set; }
        public string CsrPhoneNumber { get; set; }
        public string CsrHelpTip { get; set; }
        public int CabinetId_1 { get; set; }
        public string CabinetName_1 { get; set; }
        public int[] CabinetRowColLayout_1 { get; set; }
        public int[] CabinetPendantRows_1 { get; set; }
        public bool IsHiddenKind { get; set; }
        public int KindRowCellSize { get; set; }
        public List<PayOption> PayOptions { get; set; }
        public bool IsOpenChkCamera { get; set; }
        public int MaxBuyNumber { get; set; }
        public bool ExIsHas { get; set; }
        public List<CabinetInfoModel> Cabinets { get; set; }
    }
}
