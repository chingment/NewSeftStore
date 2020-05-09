using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Admin
{
    public class RopMerchMachineEdit
    {
        public RopMerchMachineEdit()
        {
            this.Cabinets = new List<Cabinet>();
        }
        public string Id { get; set; }
        public bool CameraByChkIsUse { get; set; }
        public bool CameraByJgIsUse { get; set; }
        public bool CameraByRlIsUse { get; set; }
        public bool ExIsHas { get; set; }
        public bool SannerIsUse { get; set; }
        public string SannerComId { get; set; }
        public bool FingerVeinnerIsUse { get; set; }
        public string MstVern { get; set; }
        public string OstVern { get; set; }
        public bool KindIsHidden { get; set; }
        public int KindRowCellSize { get; set; }

        public List<Cabinet> Cabinets { get; set; }

        public class Cabinet
        {
            public string Id { get; set; }

            public string ComId { get; set; }

            public bool IsUse { get; set; }

            public string PendantRows { get; set; }

            public int SlotMaxQuantity { get; set; }
        }
    }
}
