using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class CabinetRowColLayoutByDSModel
    {
        public CabinetRowColLayoutByDSModel()
        {
            this.Rows = new List<int>();
            this.PendantRows = new List<int>();
        }

        public List<int> Rows { get; set; }

        public List<int> PendantRows { get; set; }
    }
}
