using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.StoreApp
{
    public class CartService : BaseDbContext
    {
        public CustomJsonResult GetPageData(string operater, string clientUserId, string storeId)
        {
            var result = new CustomJsonResult();

            var ret = new RetCartGetPageData();


            var clientCarts = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == storeId && m.Status == E_ClientCartStatus.WaitSettle).ToList();


            var cartProductSkuModels = new List<CartProductSkuModel>();

            foreach (var clientCart in clientCarts)
            {
                var productSkuByCache = CacheServiceFactory.ProductSku.GetModelById(clientCart.ProductSkuId);
                if (productSkuByCache != null)
                {
                    var cartProcudtSkuModel = new CartProductSkuModel();
                    cartProcudtSkuModel.CartId = clientCart.Id;
                    cartProcudtSkuModel.Id = productSkuByCache.Id;
                    cartProcudtSkuModel.Name = productSkuByCache.Name;
                    cartProcudtSkuModel.MainImgUrl = productSkuByCache.MainImgUrl;
                    cartProcudtSkuModel.SalePrice = productSkuByCache.SalePrice;
                    cartProcudtSkuModel.Quantity = clientCart.Quantity;
                    cartProcudtSkuModel.SumPrice = clientCart.Quantity * productSkuByCache.SalePrice;
                    cartProcudtSkuModel.Selected = clientCart.Selected;
                    cartProcudtSkuModel.ReceptionMode = clientCart.ReceptionMode;
                    cartProductSkuModels.Add(cartProcudtSkuModel);
                }
            }

            var receptionModes = (from c in clientCarts select new { c.ReceptionMode }).Distinct();

            foreach (var receptionMode in receptionModes)
            {

                var carBlock = new CartBlockModel();
                carBlock.ReceptionMode = receptionMode.ReceptionMode;
                carBlock.ProductSkus = cartProductSkuModels.Where(m => m.ReceptionMode == receptionMode.ReceptionMode).ToList();

                switch (receptionMode.ReceptionMode)
                {
                    case E_ReceptionMode.Machine:
                        carBlock.TagName = "自提商品";
                        break;
                    case E_ReceptionMode.Express:
                        carBlock.TagName = "快递商品";
                        break;
                }

                ret.Blocks.Add(carBlock);
            }

            ret.Count = cartProductSkuModels.Sum(m => m.Quantity);
            ret.SumPrice = cartProductSkuModels.Sum(m => m.SumPrice);
            ret.SumPriceBySelected = cartProductSkuModels.Where(m => m.Selected == true).Sum(m => m.SumPrice);
            ret.CountBySelected = cartProductSkuModels.Where(m => m.Selected == true).Count();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);

            return result;
        }



        private static readonly object lock_Operate = new object();
        public CustomJsonResult Operate(string operater, string clientUserId, RopCartOperate rop)
        {
            var result = new CustomJsonResult();

            lock (lock_Operate)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    foreach (var item in rop.ProductSkus)
                    {
                        var store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();

                        var clientCart = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == rop.StoreId && m.ProductSkuId == item.Id && m.ReceptionMode == item.ReceptionMode && m.Status == E_ClientCartStatus.WaitSettle).FirstOrDefault();

                        switch (rop.Operate)
                        {
                            case E_CartOperateType.Selected:
                                clientCart.Selected = item.Selected;
                                break;
                            case E_CartOperateType.Decrease:
                                if (clientCart.Quantity >= 2)
                                {
                                    clientCart.Quantity -= 1;
                                    clientCart.MendTime = DateTime.Now;
                                    clientCart.Mender = operater;
                                }
                                break;
                            case E_CartOperateType.Increase:
                                var productSkuModel = CacheServiceFactory.ProductSku.GetModelById(item.Id);

                                if (clientCart == null)
                                {
                                    clientCart = new ClientCart();
                                    clientCart.Id = GuidUtil.New();
                                    clientCart.ClientUserId = clientUserId;
                                    clientCart.MerchId = store.MerchId;
                                    clientCart.StoreId = rop.StoreId;
                                    clientCart.ProductSkuId = productSkuModel.Id;
                                    clientCart.ProductSkuName = productSkuModel.Name;
                                    clientCart.ProductSkuMainImgUrl = productSkuModel.MainImgUrl;
                                    clientCart.Selected = true;
                                    clientCart.CreateTime = DateTime.Now;
                                    clientCart.Creator = operater;
                                    clientCart.Quantity = 1;
                                    clientCart.ReceptionMode = item.ReceptionMode;
                                    clientCart.Status = E_ClientCartStatus.WaitSettle;
                                    CurrentDb.ClientCart.Add(clientCart);
                                }
                                else
                                {
                                    clientCart.Quantity += 1;
                                    clientCart.MendTime = DateTime.Now;
                                    clientCart.Mender = operater;
                                }
                                break;
                            case E_CartOperateType.Delete:
                                clientCart.Status = E_ClientCartStatus.Deleted;
                                clientCart.MendTime = DateTime.Now;
                                clientCart.Mender = operater;
                                break;
                        }
                    }

                    CurrentDb.SaveChanges();

                    ts.Complete();

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                }
            }

            return result;
        }
    }
}
