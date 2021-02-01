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
            this.Scanner = new ScannerModel();
            this.FingerVeinner = new FingerVeinnerModel();
        }

        public string MachineId { get; set; }
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string LogoImgUrl { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string ShopId { get; set; }
        public string CsrQrCode { get; set; }
        public string CsrPhoneNumber { get; set; }
        public string CsrHelpTip { get; set; }
        public string MerchName { get; set; }
        public string StoreName { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string MainImgUrl { get; set; }
        public DateTime? LastRequestTime { get; set; }
        public E_MachineRunStatus RunStatus { get; set; }
        public string AppVersion { get; set; }
        public string CtrlSdkVersion { get; set; }
        public bool KindIsHidden { get; set; }
        public int KindRowCellSize { get; set; }
        public List<PayOption> PayOptions { get; set; }
        public bool IsTestMode { get; set; }
        public bool CameraByChkIsUse { get; set; }
        public bool CameraByJgIsUse { get; set; }
        public bool CameraByRlIsUse { get; set; }
        public bool ExIsHas { get; set; }
        public Dictionary<string, CabinetInfoModel> Cabinets { get; set; }
        public string MstVern{ get; set; }
        public string OstVern { get; set; }
        public ScannerModel Scanner { get; set; }
        public FingerVeinnerModel FingerVeinner { get; set; }
        public bool ImIsUse { get; set; }
        public string ImPartner { get; set; }
        public string ImUserName { get; set; }
        public string ImPassword { get; set; }
        public int PicInSampleSize { get; set; }
    }
}
