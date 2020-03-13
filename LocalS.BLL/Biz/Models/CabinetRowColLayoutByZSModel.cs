using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class CabinetRowColLayoutByZSModel
    {

        public CabinetRowColLayoutByZSModel()
        {
            this.Rows = new List<RowLayout>();
        }

        public List<RowLayout> Rows { get; set; }

        public class RowLayout
        {
            public int Index { get; set; }
            public string Id { get; set; }
            public List<ColLayout> Cols { get; set; }
        }

        public class ColLayout
        {
            public int Index { get; set; } 
            public string Id { get; set; }
            public bool CanUse { get; set; }
        }
    }
}
