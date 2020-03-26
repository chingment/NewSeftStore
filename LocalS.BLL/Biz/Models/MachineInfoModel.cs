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
        public MachineInfoModel()
        {
            this.Cabinets = new Dictionary<string, CabinetInfoModel>();
            this.ScanCtrl = new ScanCtrlModel();
            this.FingerVeinCtrl = new FingerVeinCtrlModel();
        }

        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string LogoImgUrl { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string CsrQrCode { get; set; }
        public string CsrPhoneNumber { get; set; }
        public string CsrHelpTip { get; set; }
        public string MerchName { get; set; }
        public string StoreName { get; set; }
        public string JPushRegId { get; set; }

        public string MainImgUrl { get; set; }
        public DateTime? LastRequestTime { get; set; }
        public E_MachineRunStatus RunStatus { get; set; }
        public string AppVersion { get; set; }
        public string CtrlSdkVersion { get; set; }
        public bool IsHiddenKind { get; set; }
        public int KindRowCellSize { get; set; }
        public List<PayOption> PayOptions { get; set; }
        public bool IsTestMode { get; set; }
        public bool IsOpenChkCamera { get; set; }
        public bool ExIsHas { get; set; }
        public ScanCtrlModel ScanCtrl { get; set; }
        public FingerVeinCtrlModel FingerVeinCtrl { get; set; }
        public Dictionary<string, CabinetInfoModel> Cabinets { get; set; }
    }
}
