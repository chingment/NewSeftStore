using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.StoreTerm
{

    public class OrderService : BaseDbContext
    {
        public CustomJsonResult Reserve(RopOrderReserve rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            LocalS.BLL.Biz.RopOrderReserve bizRop = new LocalS.BLL.Biz.RopOrderReserve();
            bizRop.Source = E_OrderSource.Machine;
            bizRop.StoreId = machine.StoreId;
            bizRop.ReserveMode = E_ReserveMode.OffLine;
            bizRop.SellChannelRefId = machine.Id;
            bizRop.SellChannelRefType = E_StoreSellChannelRefType.Machine;

            foreach (var item in rop.Skus)
            {
                bizRop.ProductSkus.Add(new LocalS.BLL.Biz.RopOrderReserve.ProductSku() { Id = item.Id, Quantity = item.Quantity, ReceptionMode = E_ReceptionMode.Machine });
            }

            var bizResult = LocalS.BLL.Biz.BizFactory.Order.Reserve(machine.MerchId, bizRop);

            if (bizResult.Result == ResultType.Success)
            {
                RetOrderReserve ret = new RetOrderReserve();
                ret.OrderId = bizResult.Data.OrderId;
                ret.OrderSn = bizResult.Data.OrderSn;
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
            }
            else
            {
                result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, bizResult.Message);
            }


            return result;

        }

        public CustomJsonResult PayUrlBuild(RopOrderPayUrlBuild rop)
        {
            var result = new CustomJsonResult();

            var ret = new RetOrderPayUrlBuild();

            var order = CurrentDb.Order.Where(m => m.Id == rop.OrderId).FirstOrDefault();

            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单数据");
            }

            switch (rop.PayWay)
            {
                case PayWay.AliPay:
                    order.PayWay = E_OrderPayWay.AliPay;
                    break;
                case PayWay.Wechat:
                    order.PayWay = E_OrderPayWay.Wechat;

                    //var wxPaAppInfoConfig = BizFactory.Merchant.GetWxPaAppInfoConfig(order.MerchantId);

                    //var ret_UnifiedOrder = SdkFactory.Wx.UnifiedOrderByNative(wxPaAppInfoConfig, order.MerchantId, order.Sn, 0.01m, "", Common.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);

                    //if (string.IsNullOrEmpty(ret_UnifiedOrder.PrepayId))
                    //{
                    //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                    //}

                    //order.PayPrepayId = ret_UnifiedOrder.PrepayId;
                    //order.PayQrCodeUrl = ret_UnifiedOrder.CodeUrl;
                    CurrentDb.SaveChanges();

                    ret.OrderId = order.Id;
                    //ret.PayUrl = ret_UnifiedOrder.CodeUrl;

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
                    break;
            }


            return result;
        }

        public CustomJsonResult<RetOrderPayStatusQuery> PayStatusQuery(RupOrderPayStatusQuery rup)
        {
            CustomJsonResult<RetOrderPayStatusQuery> ret = new CustomJsonResult<RetOrderPayStatusQuery>();


            var ret_Biz = LocalS.BLL.Biz.BizFactory.Order.PayResultQuery(rup.MachineId, rup.OrderId);

            ret.Result = ret_Biz.Result;
            ret.Code = ret_Biz.Code;
            ret.Message = ret_Biz.Message;

            if (ret_Biz.Data != null)
            {
                ret.Data = new RetOrderPayStatusQuery();
                ret.Data.OrderId = ret_Biz.Data.OrderId;
                ret.Data.OrderSn = ret_Biz.Data.OrderSn;
                ret.Data.Status = ret_Biz.Data.Status;
                ret.Data.OrderDetails = GetOrderDetails(rup.MachineId, rup.OrderId);
            }

            return ret;
        }

        public CustomJsonResult Cancle(RopOrderCancle rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            result = LocalS.BLL.Biz.BizFactory.Order.Cancle(GuidUtil.Empty(), rop.OrderId, rop.Reason);

            return result;
        }

        private RetOrderDetails GetOrderDetails(string machineId, string orderId)
        {
            var ret = new RetOrderDetails();

            var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();
            var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == orderId).ToList();
            var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == orderId).ToList();

            ret.Sn = order.Sn;

            foreach (var orderDetailsChild in orderDetailsChilds)
            {
                var sku = new RetOrderDetails.Sku();
                sku.Id = orderDetailsChild.PrdProductSkuId;
                sku.Name = orderDetailsChild.PrdProductSkuName;
                sku.MainImgUrl = orderDetailsChild.PrdProductSkuMainImgUrl;
                sku.Quantity = orderDetailsChild.Quantity;


                var l_orderDetailsChildSons = orderDetailsChildSons.Where(m => m.PrdProductSkuId == orderDetailsChild.PrdProductSkuId).ToList();

                sku.QuantityBySuccess = l_orderDetailsChildSons.Where(m => m.Status == E_OrderDetailsChildSonStatus.Completed).Count();

                foreach (var orderDetailsChildSon in l_orderDetailsChildSons)
                {
                    var slot = new RetOrderDetails.Slot();
                    slot.UniqueId = orderDetailsChildSon.Id;
                    slot.SlotId = orderDetailsChildSon.SlotId;
                    slot.Status = orderDetailsChildSon.Status;

                    sku.Slots.Add(slot);
                }

                ret.Skus.Add(sku);
            }

            return ret;
        }

        public CustomJsonResult Details(RupOrderDetails rup)
        {
            CustomJsonResult result = new CustomJsonResult();
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", GetOrderDetails(rup.MachineId, rup.OrderId));
            return result;
        }

        public CustomJsonResult SkuPickupStatusQuery(RupOrderSkuPickupStatusQuery rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetOrderSkuPickupStatusQuery();

            var orderDetailsChildSon = CurrentDb.OrderDetailsChildSon.Where(m => m.Id == rup.UniqueId && m.OrderId == rup.OrderId && m.PrdProductSkuId == rup.SkuId && m.SlotId == rup.SlotId).FirstOrDefault();

            ret.Status = orderDetailsChildSon.Status;
            ret.Tips = orderDetailsChildSon.Status.ToString();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult SkuPickupEventNotify(RopOrderSkuPickupEventNotify rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            //result = BizFactory.Order.Cancle(operater, rop.OrderSn, rop.Reason);

            return result;
        }

    }
}
