using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class OrderBlockModel
    {
        public OrderBlockModel()
        {
            this.Skus = new List<OrderConfirmProductSkuModel>();
            this.Delivery = new DeliveryModel();
            this.SelfTake = new SelfTakeModel();
            this.BookTime = new BookTimeModel();
        }

        public string TagName { get; set; }

        /// <summary>
        /// Tab选项卡模式 Delivery 仅支持配送方式，SelfTake 仅支持自提方式 DeliveryAndSelfTake 支持配送和自提 
        /// </summary>
        public E_TabMode TabMode { get; set; }

        /// <summary>
        /// 收货方式   Delivery SelfTake
        /// </summary>
        public E_ReceiveMode ReceiveMode { get; set; }
        /// <summary>
        /// 当ReceiveMode 为 Delivery 选择该方式
        /// </summary>
        public DeliveryModel Delivery { get; set; }
        /// <summary>
        /// 当当ReceiveMode 为 SelfTake 选择该方式
        /// </summary>
        public SelfTakeModel SelfTake { get; set; }
        public List<OrderConfirmProductSkuModel> Skus { get; set; }

        public BookTimeModel BookTime { get; set; }
    }

    public enum E_TabMode
    {

        Unknow = 0,
        Delivery = 1,
        StoreSelfTake = 2,
        DeliveryAndStoreSelfTake = 3,
        MachineSelfTake = 4,
        MemerbFee = 5
    }

    public class BookTimeModel
    {
        public string Text { get; set; }
        public string Week { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class OrderConfirmSubtotalItemModel
    {
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public bool IsDcrease { get; set; }
    }
}
