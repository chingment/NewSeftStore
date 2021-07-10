using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{
    public class DeviceService : BaseService
    {
        public IResult2 List(string merchId, RopDeviceList rop)
        {
            var result = new CustomJsonResult2();

            var query = (from u in CurrentDb.MerchDevice
                         join m in CurrentDb.Device on u.DeviceId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where ((rop.device_id == null || u.DeviceId.Contains(rop.device_id)))
                         &&
                         u.MerchId == merchId
                         select new { u.DeviceId, u.Name, u.CumCode, tt.Lat, tt.Lng, tt.RunStatus });


            int total = query.Count();

            int pageIndex = rop.page;

            int pageSize = rop.limit;

            query = query.OrderByDescending(r => r.DeviceId).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> items = new List<object>();

            foreach (var r in list)
            {
                var d_cabinets = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == r.DeviceId && m.IsUse == true).Select(m => m.CabinetId).ToList();

                items.Add(new
                {
                    device_id = r.DeviceId,
                    name = r.Name,
                    cum_code = r.CumCode,
                    lat = r.Lat,
                    lng = r.Lng,
                    cabinets = d_cabinets,
                    status = r.RunStatus,
                });

            }


            var ret = new { Total = total, Items = items };

            result = new CustomJsonResult2(ResultCode.Success, "", ret);

            return result;
        }

        public IResult2 Stock(string merchId, RopDeviceStock rop)
        {
            var result = new CustomJsonResult2();

            var d_Device = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.DeviceId == rop.device_id).FirstOrDefault();

            if (d_Device == null)
                return new CustomJsonResult2(ResultCode.Failure, "设备不存在");


            if (string.IsNullOrEmpty(d_Device.CurUseStoreId))
                return new CustomJsonResult2(ResultCode.Failure, "该设备未绑定店铺");

            if (string.IsNullOrEmpty(d_Device.CurUseShopId))
                return new CustomJsonResult2(ResultCode.Failure, "该设备未绑定门店");


            var query = (from m in CurrentDb.SellChannelStock
                         where
                         m.MerchId == merchId && m.StoreId == d_Device.CurUseStoreId && m.ShopId == d_Device.CurUseShopId && m.DeviceId == rop.device_id
                         select new { m.CabinetId, m.SlotId, m.WarnQuantity, m.MaxQuantity, m.HoldQuantity, m.SkuId, m.WaitPayLockQuantity, m.WaitPickupLockQuantity, m.SumQuantity, m.SellQuantity, m.IsOffSell });

            if (!string.IsNullOrEmpty(rop.cabinet_id))
            {
                query = query.Where(m => m.CabinetId == rop.cabinet_id);
            }

            var list = query.ToList();

            List<object> items = new List<object>();

            if (rop.data_format == "slot")
            {
                foreach (var r in list)
                {
                    Dictionary<string, object> dics = new Dictionary<string, object>();
                    dics.Add("cabinet_id", r.CabinetId);
                    dics.Add("slot_id", r.SlotId);
                    dics.Add("sku_id", r.SkuId);

                    if (rop.is_need_detail)
                    {
                        var r_Sku = CacheServiceFactory.Product.GetSkuInfo(merchId, r.SkuId);
                        dics.Add("sku_cum_code", r_Sku.CumCode);
                        dics.Add("sku_name", r_Sku.Name);
                        dics.Add("sku_img_url", r_Sku.MainImgUrl);
                    }

                    dics.Add("sum_quantity", r.SumQuantity);
                    dics.Add("lock_quantity", r.WaitPayLockQuantity + r.WaitPickupLockQuantity);
                    dics.Add("sell_quantity", r.SellQuantity);
                    dics.Add("warn_quantity", r.WarnQuantity);
                    dics.Add("hold_quantity", r.HoldQuantity);
                    dics.Add("max_quantity", r.MaxQuantity);
                    dics.Add("is_off_sell", r.IsOffSell);

                    items.Add(dics);
                }
            }
            else if (rop.data_format == "sku")
            {
                var oList = (from u in list select new { u.SkuId, u.IsOffSell }).Distinct();

                foreach (var r in oList)
                {
                    Dictionary<string, object> dics = new Dictionary<string, object>();
                    dics.Add("sku_id", r.SkuId);

                    if (rop.is_need_detail)
                    {
                        var r_Sku = CacheServiceFactory.Product.GetSkuInfo(merchId, r.SkuId);
                        dics.Add("sku_cum_code", r_Sku.CumCode);
                        dics.Add("sku_name", r_Sku.Name);
                        dics.Add("sku_img_url", r_Sku.MainImgUrl);
                    }

                    var sku_Stocks = list.Where(m => m.SkuId == r.SkuId);

                    int sumQuantity = sku_Stocks.Sum(m => m.SumQuantity);
                    int waitPayLockQuantity = sku_Stocks.Sum(m => m.WaitPayLockQuantity);
                    int waitPickupLockQuantity = sku_Stocks.Sum(m => m.WaitPickupLockQuantity);
                    int sellQuantity = sku_Stocks.Sum(m => m.SellQuantity);
                    int warnQuantity = sku_Stocks.Sum(m => m.WarnQuantity);
                    int holdQuantity = sku_Stocks.Sum(m => m.HoldQuantity);
                    int maxQuantity = sku_Stocks.Sum(m => m.MaxQuantity);

                    dics.Add("sum_quantity", sumQuantity);
                    dics.Add("lock_quantity", waitPayLockQuantity + waitPickupLockQuantity);
                    dics.Add("sell_quantity", sellQuantity);
                    dics.Add("warn_quantity", warnQuantity);
                    dics.Add("hold_quantity", holdQuantity);
                    dics.Add("max_quantity", maxQuantity);

                    dics.Add("is_off_sell", r.IsOffSell);

                    List<object> slots = new List<object>();
                    foreach (var sku_Stock in sku_Stocks)
                    {
                        Dictionary<string, object> dic2s = new Dictionary<string, object>();

                        dic2s.Add("cabinet_id", sku_Stock.CabinetId);
                        dic2s.Add("slot_id", sku_Stock.SlotId);
                        dic2s.Add("sum_quantity", sku_Stock.SumQuantity);
                        dic2s.Add("lock_quantity", sku_Stock.WaitPayLockQuantity + sku_Stock.WaitPickupLockQuantity);
                        dic2s.Add("sell_quantity", sku_Stock.SellQuantity);
                        dic2s.Add("warn_quantity", sku_Stock.WarnQuantity);
                        dic2s.Add("hold_quantity", sku_Stock.HoldQuantity);
                        dic2s.Add("max_quantity", sku_Stock.MaxQuantity);

                        slots.Add(dic2s);
                    }

                    dics.Add("slots", slots);


                    items.Add(dics);
                }


            }

            var ret = new { Items = items };

            result = new CustomJsonResult2(ResultCode.Success, "", ret);

            return result;

        }
    }
}
