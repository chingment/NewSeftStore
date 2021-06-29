using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetDeviceInitData
    {
        public RetDeviceInitData()
        {
            this.Device = new DeviceModel();
            this.Ads = new Dictionary<string, AdModel>();
            this.Skus = new Dictionary<string, SkuModel>();
            this.Kinds = new List<KindModel>();
        }
        public DeviceModel Device { get; set; }
        public Dictionary<string, SkuModel> Skus { get; set; }
        public List<KindModel> Kinds { get; set; }
        public Dictionary<string, AdModel> Ads { get; set; }
    }
}
