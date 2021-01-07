﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{

    public class RopOrderReserve
    {
        public RopOrderReserve()
        {
            this.Blocks = new List<BlockModel>();
        }

        public string ReffSign { get; set; }
        public string AppId { get; set; }
        public string StoreId { get; set; }
        public string ClientUserId { get; set; }
        public string SvcAnswererId { get; set; }
        public string SaleOutletId { get; set; }
        public List<string> CouponIdsByShop { get; set; }
        public string CouponIdByRent { get; set; }
        public string CouponIdByDeposit { get; set; }
        public E_OrderShopMethod ShopMethod { get; set; }
        public E_OrderSource Source { get; set; }
        public List<BlockModel> Blocks { get; set; }
        public bool IsTestMode { get; set; }

        public class BlockModel
        {
            public BlockModel()
            {
                this.Delivery = new DeliveryModel();
                this.SelfTake = new SelfTakeModel();
                this.Skus = new List<ProductSkuModel>();

            }
            public E_ReceiveMode ReceiveMode { get; set; }
            public DeliveryModel Delivery { get; set; }
            public SelfTakeModel SelfTake { get; set; }
            public List<ProductSkuModel> Skus { get; set; }
            public class DeliveryModel
            {
                public ContactModel Contact { get; set; }
            }
            public class SelfTakeModel
            {
                public MarkModel Mark { get; set; }
                public ContactModel Contact { get; set; }
                public BookTimeModel BookTime { get; set; }
            }

            public class ProductSkuModel
            {
                public string CartId { get; set; }
                public string Id { get; set; }
                public int Quantity { get; set; }
                public E_SellChannelRefType ShopMode { get; set; }
                public string[] SellChannelRefIds { get; set; }
                public string SvcConsulterId { get; set; }
            }
            public class BookTimeModel
            {
                public string Value { get; set; }
                public int Type { get; set; }
            }
            public class ContactModel
            {
                public string Id { get; set; }
                public string Consignee { get; set; }
                public string PhoneNumber { get; set; }
                public string AreaName { get; set; }
                public string AreaCode { get; set; }
                public string Address { get; set; }
                public string MarkName { get; set; }
            }
            public class MarkModel
            {
                public string Id { get; set; }
                public string Name { get; set; }
                public string Address { get; set; }
                public string AreaName { get; set; }
                public string AreaCode { get; set; }
            }
        }
    }
}
