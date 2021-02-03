using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class RetOperateStock
    {
        public RetOperateStock()
        {
            this.ChangeRecords = new List<StockChangeRecordModel>();
        }

        public List<StockChangeRecordModel> ChangeRecords { get; set; }

    }
}
