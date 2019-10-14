using LocalS.BLL;
using LocalS.BLL.Biz;
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
        public CustomJsonResult<RetCartPageData> PageData(string operater, string clientUserId, RupCartPageData rup)
        {
            var result = new CustomJsonResult<RetCartPageData>();

            var ret = new RetCartPageData();


            var clientCarts = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == rup.StoreId && m.Status == E_ClientCartStatus.WaitSettle).ToList();


            //构建购物车商品信息
            var cartProductSkuModels = new List<CartProductSkuModel>();

            foreach (var clientCart in clientCarts)
            {
                var productSkuModel = CacheServiceFactory.ProductSku.GetInfoAndStock(clientCart.MerchId, clientCart.PrdProductSkuId);
                if (productSkuModel != null)
                {
                    var cartProcudtSkuModel = new CartProductSkuModel();
                    cartProcudtSkuModel.CartId = clientCart.Id;
                    cartProcudtSkuModel.Id = clientCart.PrdProductSkuId;
                    cartProcudtSkuModel.ProductId = clientCart.PrdProductId;
                    cartProcudtSkuModel.Name = productSkuModel.Name;
                    cartProcudtSkuModel.MainImgUrl = productSkuModel.MainImgUrl;
                    cartProcudtSkuModel.SalePrice = productSkuModel.SalePrice;
                    cartProcudtSkuModel.Quantity = clientCart.Quantity;
                    cartProcudtSkuModel.SumPrice = clientCart.Quantity * cartProcudtSkuModel.SalePrice;
                    cartProcudtSkuModel.Selected = clientCart.Selected;
                    cartProcudtSkuModel.ReceptionMode = clientCart.ReceptionMode;
                    cartProductSkuModels.Add(cartProcudtSkuModel);
                }
            }

            //分类块，自取或快递 各构建
            var receptionModes = (from c in clientCarts select c.ReceptionMode).Distinct().ToList();

            foreach (var receptionMode in receptionModes)
            {

                var carBlock = new CartBlockModel();
                carBlock.ReceptionMode = receptionMode;
                carBlock.ProductSkus = cartProductSkuModels.Where(m => m.ReceptionMode == receptionMode).ToList();

                switch (receptionMode)
                {
                    case E_ReceptionMode.Express:
                        carBlock.TagName = "快递外送";
                        break;
                    case E_ReceptionMode.SelfTake:
                        carBlock.TagName = "店内自取";
                        break;
                    case E_ReceptionMode.Machine:
                        carBlock.TagName = "机器自提";
                        break;
                }

                ret.Blocks.Add(carBlock);
            }

            ret.Count = cartProductSkuModels.Sum(m => m.Quantity);
            ret.SumPrice = cartProductSkuModels.Sum(m => m.SumPrice);
            ret.SumPriceBySelected = cartProductSkuModels.Where(m => m.Selected == true).Sum(m => m.SumPrice);
            ret.CountBySelected = cartProductSkuModels.Where(m => m.Selected == true).Count();

            result = new CustomJsonResult<RetCartPageData>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }



        private static readonly object lock_Operate = new object();
        public CustomJsonResult Operate(string operater, string clientUserId, RopCartOperate rop)
        {
            var result = new CustomJsonResult();

            if (rop.ProductSkus == null || rop.ProductSkus.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "选择商品为空");
            }

           
            lock (lock_Operate)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var storeInfo = BizFactory.Store.GetInfo(rop.StoreId);

                    foreach (var item in rop.ProductSkus)
                    {
                        var clientCart = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == rop.StoreId && m.PrdProductSkuId == item.Id && m.ReceptionMode == item.ReceptionMode && m.Status == E_ClientCartStatus.WaitSettle).FirstOrDefault();

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

                                var productSku = CacheServiceFactory.ProductSku.GetInfoAndStock(storeInfo.MerchId, item.Id);

                                if (clientCart == null)
                                {
                                    clientCart = new ClientCart();
                                    clientCart.Id = GuidUtil.New();
                                    clientCart.ClientUserId = clientUserId;
                                    clientCart.MerchId = storeInfo.MerchId;
                                    clientCart.StoreId = rop.StoreId;
                                    clientCart.PrdProductId = productSku.PrdProductId;
                                    clientCart.PrdProductSkuId = item.Id;
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
