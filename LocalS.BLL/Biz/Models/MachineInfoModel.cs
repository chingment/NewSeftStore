using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class MachineInfoModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoImgUrl { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string CsrQrCode { get; set; }
        public string MerchName { get; set; }
        public string StoreName { get; set; }
        public string JPushRegId { get; set; }
        public int CabinetId_1 { get; set; }
        public string CabinetName_1 { get; set; }
        public int[] CabinetRowColLayout_1 { get; set; }
        public string MainImgUrl { get; set; }
        public DateTime? LastRequestTime { get; set; }
        public E_MachineRunStatus RunStatus { get; set; }
        public string AppVersion { get; set; }
        public string CtrlSdkVersion { get; set; }
        public bool IsHiddenKind { get; set; }
        public int KindRowCellSize { get; set; }


    }
}
