using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetMachineInitManageBaseInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoImgUrl { get; set; }
        public string LastRequestTime { get; set; }
        public StatusModel Status { get; set; }
        public string AppVersion { get; set; }
        public string CtrlSdkVersion { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
    }
}
