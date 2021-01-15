using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetOrderConfirm
    {
        public RetOrderConfirm()
        {
            this.SubtotalItems = new List<SubtotalItemModel>();
            this.Blocks = new List<BlockModel>();
        }

        //购物优惠卷
        public CouponModel CouponByShop { get; set; }
        //租金优惠券
        public CouponModel CouponByRent { get; set; }
        //押金优惠券
        public CouponModel CouponByDeposit { get; set; }
        //订单块
        public List<BlockModel> Blocks { get; set; }
        //小计项目
        public List<SubtotalItemModel> SubtotalItems { get; set; }
        //实际支付金额
        public string ActualAmount { get; set; }
        //原金额
        public string OriginalAmount { get; set; }
        public List<string> OrderIds { get; set; }
        public E_ShopMethod ShopMethod { get; set; }

        public bool IsCanPay { get; set; }

        public class BlockModel
        {
            public BlockModel()
            {
                this.Skus = new List<BuildSku>();
                this.Delivery = new DeliveryModel();
                this.SelfTake = new SelfTakeModel();
            }

            public string TagName { get; set; }

            /// <summary>
            /// Tab选项卡模式 Delivery 仅支持配送方式，SelfTake 仅支持自提方式 DeliveryOrSelfTake 支持配送和自提 
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
            public List<BuildSku> Skus { get; set; }
        }


        public class SubtotalItemModel
        {
            public string ImgUrl { get; set; }
            public string Name { get; set; }
            public string Amount { get; set; }
            public bool IsDcrease { get; set; }
        }

        public class CouponModel
        {

            public CouponModel()
            {

            }

            public TipType TipType { get; set; }

            public string TipMsg { get; set; }

            //public int CanUseQuantity { get; set; }

            public List<string> SelectedCouponIds { get; set; }

            public decimal CouponAmount { get; set; }
        }
    }
}
