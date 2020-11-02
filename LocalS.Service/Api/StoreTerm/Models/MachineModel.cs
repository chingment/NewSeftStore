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
        public MachineModel()
        {
            this.PayOptions = new List<PayOption>();
            this.Scanner = new ScannerModel();
            this.Im = new ImModel();
            this.Mqtt = new MqttModel();
            this.FingerVeinner = new FingerVeinnerModel();
            this.Cabinets = new Dictionary<string, CabinetInfoModel>();
        }

        public string MachineId { get; set; }
        public string Name { get; set; }

        public string DeviceId { get; set; }
        public string MerchName { get; set; }
        public string StoreName { get; set; }
        public string LogoImgUrl { get; set; }
        public string CsrQrCode { get; set; }
        public string CsrPhoneNumber { get; set; }
        public string CsrHelpTip { get; set; }
        public bool IsHiddenKind { get; set; }
        public int KindRowCellSize { get; set; }
        public List<PayOption> PayOptions { get; set; }
        public bool CameraByChkIsUse { get; set; }
        public bool CameraByJgIsUse { get; set; }
        public bool CameraByRlIsUse { get; set; }
        public int MaxBuyNumber { get; set; }
        public bool ExIsHas { get; set; }
        public Dictionary<string, CabinetInfoModel> Cabinets { get; set; }
        public ScannerModel Scanner { get; set; }
        public FingerVeinnerModel FingerVeinner { get; set; }
        public string MstVern { get; set; }
        public string OstVern { get; set; }
        public MqttModel Mqtt { get; set; }
        public ImModel Im { get; set; }
    }

}
