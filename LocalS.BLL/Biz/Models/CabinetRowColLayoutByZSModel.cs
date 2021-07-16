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
            this.Rows = new List<List<string>>();
        }

        public List<List<string>> Rows { get; set; }

    }
}
