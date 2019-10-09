using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq.MqMessageConentModel
{
    public enum StockOperateType
    {
        Unknow = 0,
        OrderReserveSuccess = 1,
        //OrderReserveFailure = 2,
        OrderPaySuccess = 3,
        //OrderPayFailure = 4,
        OrderCancle = 5
        //OrderPayTimeout = 6
    }

    public class StockOperateModel
    {
        public StockOperateModel()
        {

        }
        public StockOperateType OperateType { get; set; }

        public List<OperateStock> OperateStocks { get; set; }

        public class OperateStock
        {
            public string MerchId { get; set; }
            public string PrdProductSkuId { get; set; }
            public string SlotId { get; set; }
            public E_SellChannelRefType RefType { get; set; }
            public string RefId { get; set; }

            public int Quantity { get; set; }
        }
    }
}
