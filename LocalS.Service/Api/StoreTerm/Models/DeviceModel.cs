using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class DeviceModel
    {
        public DeviceModel()
        {
            this.PayOptions = new List<PayOption>();
            this.Scanner = new ScannerModel();
            this.Im = new ImModel();
            this.Mqtt = new MqttModel();
            this.FingerVeinner = new FingerVeinnerModel();
            this.Consult = new ConsultModel();
            this.Cabinets = new Dictionary<string, CabinetModel>();
        }
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string MerchName { get; set; }
        public string StoreName { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string LogoImgUrl { get; set; }
        public ConsultModel Consult { get; set; }
        public List<PayOption> PayOptions { get; set; }
        public bool CameraByChkIsUse { get; set; }
        public bool CameraByJgIsUse { get; set; }
        public bool CameraByRlIsUse { get; set; }
        public bool ExIsHas { get; set; }
        public Dictionary<string, CabinetModel> Cabinets { get; set; }
        public ScannerModel Scanner { get; set; }
        public FingerVeinnerModel FingerVeinner { get; set; }
        public string MstVern { get; set; }
        public string OstVern { get; set; }
        public MqttModel Mqtt { get; set; }
        public ImModel Im { get; set; }
        public int PicInSampleSize { get; set; }

        public Dictionary<string, string> Lights { get; set; }
    }

}
