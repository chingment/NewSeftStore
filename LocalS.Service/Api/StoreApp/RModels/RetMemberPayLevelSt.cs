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
   
        }

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
    }
}
