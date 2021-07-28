using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class CabinetModel
    {
        public string CabinetId { get; set; }
        public string Name { get; set; }
        public string RowColLayout { get; set; }

        public int Priority { get; set; }

        public string ComId { get; set; }

        public int ComBaud { get; set; }
    }
}
