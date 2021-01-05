using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.StoreApp
{
    public class DeliveryAddressService : BaseService
    {

        public CustomJsonResult My(string operater, string clientUserId, RupDeliveryAddressMy rup)
        {
            var result = new CustomJsonResult();

            var model = new List<DeliveryAddressModel>();

            var query = (from o in CurrentDb.ClientDeliveryAddress
                         where
                         o.ClientUserId == clientUserId &&
                         o.IsDelete == false
                         select new { o.Id, o.Consignee, o.PhoneNumber, o.MarkName, o.Address, o.AreaName, o.AreaCode, o.IsDefault, o.CreateTime }
              );


            query = query.OrderByDescending(r => r.CreateTime);

            var list = query.ToList();

            foreach (var m in list)
            {

                model.Add(new DeliveryAddressModel
                {
                    Id = m.Id,
                    Consignee = m.Consignee,
                    PhoneNumber = m.PhoneNumber,
                    Address = m.Address,
                    AreaName = m.AreaName,
                    AreaCode = m.AreaCode,
                    MarkName = m.MarkName,
                    IsDefault = m.IsDefault
                });
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", model);

            return result;

        }


        public CustomJsonResult Edit(string operater, string clientUserId, RopDeliveryAddressEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var l_userDeliveryAddress = CurrentDb.ClientDeliveryAddress.Where(m => m.Id == rop.Id).FirstOrDefault();
            if (l_userDeliveryAddress == null)
            {
                l_userDeliveryAddress = new ClientDeliveryAddress();
                l_userDeliveryAddress.Id = IdWorker.Build(IdType.NewGuid);
                l_userDeliveryAddress.ClientUserId = clientUserId;
                l_userDeliveryAddress.Consignee = rop.Consignee;
                l_userDeliveryAddress.PhoneNumber = rop.PhoneNumber;
                l_userDeliveryAddress.AreaName = rop.AreaName;
                l_userDeliveryAddress.AreaCode = rop.AreaCode;
                l_userDeliveryAddress.Address = rop.Address;
                l_userDeliveryAddress.IsDefault = rop.IsDefault;
                l_userDeliveryAddress.MarkName = rop.MarkName;
                l_userDeliveryAddress.CreateTime = DateTime.Now;
                l_userDeliveryAddress.Creator = operater;
                CurrentDb.ClientDeliveryAddress.Add(l_userDeliveryAddress);
                CurrentDb.SaveChanges();

            }
            else
            {
                l_userDeliveryAddress.Consignee = rop.Consignee;
                l_userDeliveryAddress.PhoneNumber = rop.PhoneNumber;
                l_userDeliveryAddress.AreaName = rop.AreaName;
                l_userDeliveryAddress.Address = rop.Address;
                l_userDeliveryAddress.IsDefault = rop.IsDefault;
                l_userDeliveryAddress.MarkName = rop.MarkName;
                l_userDeliveryAddress.MendTime = DateTime.Now;
                l_userDeliveryAddress.Creator = operater;
                CurrentDb.SaveChanges();
            }

            if (rop.IsDefault)
            {
                var list = CurrentDb.ClientDeliveryAddress.Where(m => m.ClientUserId == clientUserId).ToList();


                foreach (var item in list)
                {
                    if (item.Id != rop.Id)
                    {
                        item.IsDefault = false;
                        CurrentDb.SaveChanges();
                    }
                }
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
        }
    }
}
