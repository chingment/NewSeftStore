using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{

    public class OrderService : BaseService
    {
        public IResult2 Reserve(string merchId, RopOrderReserve rop)
        {
            var result = new CustomJsonResult2();

            if (string.IsNullOrEmpty(rop.device_id) && string.IsNullOrEmpty(rop.device_com_code))
            {
                return new CustomJsonResult2(ResultCode.Failure, "device_id,device_com_code不能同时为空");
            }

            MerchDevice d_MerchDevice = null;
            if (!string.IsNullOrEmpty(rop.device_id))
            {
                d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.DeviceId == rop.device_id).FirstOrDefault();
            }
            else
            {
                d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.CumCode == rop.device_com_code).FirstOrDefault();
            }

            if (d_MerchDevice == null)
            {
                return new CustomJsonResult2(ResultCode.Failure, "设备未登记");
            }


            if (string.IsNullOrEmpty(d_MerchDevice.CurUseStoreId))
            {
                return new CustomJsonResult2(ResultCode.Failure, "设备未绑定店铺");
            }

            if (string.IsNullOrEmpty(d_MerchDevice.CurUseShopId))
            {
                return new CustomJsonResult2(ResultCode.Failure, "设备未绑定门店");
            }

            if (string.IsNullOrEmpty(rop.low_order_id))
            {
                return new CustomJsonResult2(ResultCode.Failure, "商户订单号不能为空");
            }

            if (string.IsNullOrEmpty(rop.notify_url))
            {
                return new CustomJsonResult2(ResultCode.Failure, "通知URL不能为空");
            }

            var d_Shop = CurrentDb.Shop.Where(m => m.Id == d_MerchDevice.CurUseShopId).FirstOrDefault();

            if (d_Shop == null)
            {
                return new CustomJsonResult2(ResultCode.Failure, "门店信息异常");
            }

            var d_Device = CurrentDb.Device.Where(m => m.Id == d_MerchDevice.DeviceId).FirstOrDefault();

            LocalS.BLL.Biz.RopOrderReserve bizRop = new LocalS.BLL.Biz.RopOrderReserve();
            bizRop.AppId = AppId.STORETERM;
            bizRop.Source = E_OrderSource.Api;
            bizRop.StoreId = d_MerchDevice.CurUseStoreId;
            bizRop.ShopMethod = E_ShopMethod.Buy;
            bizRop.IsTestMode = d_Device.IsTestMode;
            bizRop.CumOrderId = rop.low_order_id;
            bizRop.IsPayed = true;
            LocalS.BLL.Biz.RopOrderReserve.BlockModel block = new LocalS.BLL.Biz.RopOrderReserve.BlockModel();

            block.ReceiveMode = E_ReceiveMode.SelfTakeByDevice;
            block.SelfTake.Mark.Id = d_MerchDevice.CurUseShopId;
            block.SelfTake.Mark.Name = d_Shop.Name;
            block.SelfTake.Mark.Address = d_Shop.Address;
            block.SelfTake.Mark.AreaCode = d_Shop.AreaCode;
            block.SelfTake.Mark.AreaName = d_Shop.AreaName;

            foreach (var detail in rop.detail)
            {

                if (string.IsNullOrEmpty(detail.sku_id) && string.IsNullOrEmpty(detail.sku_cum_code))
                    return new CustomJsonResult2(ResultCode.Failure, "sku_id,sku_cum_code 不能同时为空");

                if (string.IsNullOrEmpty(detail.sku_id))
                {
                    var d_Sku = CurrentDb.PrdSku.Where(m => m.CumCode == detail.sku_cum_code && m.MerchId == merchId).FirstOrDefault();
                    if (d_Sku == null)
                    {
                        return new CustomJsonResult2(ResultCode.Failure, "商品编码不存在");
                    }
                    else
                    {
                        detail.sku_id = d_Sku.Id;
                    }
                }

                block.Skus.Add(new LocalS.BLL.Biz.RopOrderReserve.BlockModel.SkuModel() { Id = detail.sku_id, Quantity = detail.quantity, ShopMode = E_ShopMode.Device, ShopId = d_Device.CurUseShopId, DeviceIds = new string[] { rop.device_id }, SvcConsulterId = "" });
            }

            bizRop.Blocks.Add(block);

            var bizResult = LocalS.BLL.Biz.BizFactory.Order.Reserve(IdWorker.Build(IdType.EmptyGuid), bizRop);

            if (bizResult.Result == ResultType.Success)
            {
                var order = bizResult.Data.Orders[0];

                result = new CustomJsonResult2(ResultCode.Success, "", new { low_order_id = order.CumId, up_order_id = order.Id });
            }
            else
            {
                result = new CustomJsonResult2(ResultCode.Failure, bizResult.Message);
            }


            return result;
        }

        public IResult2 Cancle(string merchId, RopOrderCancle rop)
        {
            var result = BizFactory.Order.Cancle(IdWorker.Build(IdType.EmptyGuid), null, rop.low_order_id, E_OrderCancleType.PayCancle, "订单支付取消");

            return new CustomJsonResult2(result.Code, result.Message, result.Data);
        }

        public IResult2 Query(string merchId, RopOrderQuery rop)
        {
            var result = new CustomJsonResult2();

            var d_Order = CurrentDb.Order.Where(m => m.CumId == rop.low_order_id).FirstOrDefault();

            if (d_Order == null)
            {
                return new CustomJsonResult2(ResultCode.Failure, "该商户订单号信息不存在");
            }

            var d_OrderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == d_Order.Id).ToList();

            var sku_Ships = new List<object>();

            foreach (var item in d_OrderSubs)
            {
                sku_Ships.Add(new
                {
                    unique_id = item.Id,
                    cabinet_id = item.CabinetId,
                    slot_id = item.SlotId,
                    sku_id = item.SkuId,
                    sku_cum_code = item.SkuCumCode,
                    status = item.PickupStatus,
                    tips = item.ExPickupReason,
                });
            }

            var ret = new
            {
                low_order_id = d_Order.CumId,
                up_order_id = d_Order.Id,
                business_type = "ship",
                detail = new
                {
                    is_trg = d_Order.PickupIsTrg,
                    sku_ships = sku_Ships
                }
            };

            result = new CustomJsonResult2(ResultCode.Success, "", ret);

            return result;
        }

        public IResult2 Ship(string merchId, RopOrderShip rop)
        {
            var result = new CustomJsonResult2();


            result = new CustomJsonResult2(ResultCode.Success, "");

            return result;
        }

        public IResult2 SaleRecords(string merchId, RopOrderSaleRecords rop)
        {
            var result = new CustomJsonResult2();

            if (!CommonUtil.IsDateTime(rop.sale_date))
                return new CustomJsonResult2(ResultCode.Failure, "日期格式不符合");


            DateTime? startTime = CommonUtil.ConverToStartTime(rop.sale_date);
            DateTime? endTime = CommonUtil.ConverToEndTime(rop.sale_date);

            var query = (from o in CurrentDb.Order
                         where
                         o.PayStatus == E_PayStatus.PaySuccess &&
                         o.MerchId == merchId &&
                         o.PayedTime >= startTime &&
                         o.PayedTime <= endTime &&
                         o.IsTestMode == false
                         select new { o.Id, o.CumId, o.PayedTime, o.PayWay, o.StoreId, o.IsTestMode, o.DeviceCumCode, o.DeviceId, o.StoreName, o.PickupIsTrg, o.ReceiverPhoneNumber, o.ReceiveModeName, o.ReceiveMode, o.ExIsHappen, o.ClientUserId, o.ExIsHandle, o.ClientUserName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });


            int total = query.Count();

            int pageIndex = rop.page;
            int pageSize = rop.limit;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == item.Id).OrderByDescending(m => m.PickupStartTime).ToList();

                var detail = new List<object>();

                foreach (var orderSub in orderSubs)
                {
                    detail.Add(new
                    {
                        sku_id = orderSub.SkuId,
                        sku_cum_code = orderSub.SkuCumCode,
                        sku_name = orderSub.SkuName,
                        quantity = orderSub.Quantity,
                        sale_price = orderSub.SalePrice,
                        trade_amount = orderSub.ChargeAmount
                    });
                }


                olist.Add(new
                {
                    up_order_id = item.Id,
                    low_order_id = item.CumId,
                    device_id = item.DeviceId,
                    device_cum_code = item.DeviceCumCode,
                    trade_time = item.PayedTime.ToUnifiedFormatDateTime(),
                    trade_amount = item.ChargeAmount,
                    quantity = item.Quantity,
                    pay_way = item.PayWay,
                    client_name = item.ClientUserName,
                    client_phonenumber = item.ReceiverPhoneNumber,
                    detail = detail
                });
            }

            var pageEntity = new { total = total, items = olist };

            result = new CustomJsonResult2(ResultCode.Success, "", pageEntity);

            return result;

        }
    }
}
