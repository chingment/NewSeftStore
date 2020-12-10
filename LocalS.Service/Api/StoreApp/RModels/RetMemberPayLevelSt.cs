using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetMemberPayLevelSt
    {
        public RetMemberPayLevelSt()
        {
            this.SaleOutlets = new List<SaleOutletModel>();
            this.CurSaleOutlet = new SaleOutletModel();
        }

        public List<SaleOutletModel> SaleOutlets { get; set; }
        public bool IsOptSaleOutlet { get; set; }

        public SaleOutletModel CurSaleOutlet { get; set; }

        public LevelStModel LevelSt1 { get; set; }
        public LevelStModel LevelSt2 { get; set; }
        public class LevelStModel
        {
            public LevelStModel()
            {
                this.FeeSts = new List<FeeStModel>();
            }
            public string Id { get; set; }
            public string Tag { get; set; }
            public int Level { get; set; }
            public string DetailsDes { get; set; }
            public int CurFeeStIdx { get; set; }
            public List<FeeStModel> FeeSts { get; set; }
        }
        public class FeeStModel
        {
            public FeeStModel()
            {
                this.DesPoints = new List<FsField>();
            }
            public string Id { get; set; }
            public string Tag { get; set; }
            public FsText FeeValue { get; set; }
            public List<FsField> DesPoints { get; set; }
            public string LayoutWeight { get; set; }
        }

        public class SaleOutletModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
