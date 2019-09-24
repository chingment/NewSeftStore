using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class MachineService : BaseDbContext
    {
        public CustomJsonResult InitData(RupMachineInitData rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetMachineInitData();

            var machine = CurrentDb.Machine.Where(m => m.Id == rup.MachineId).FirstOrDefault();

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未登记");
            }

            var merch = CurrentDb.Merch.Where(m => m.Id == machine.MerchId).FirstOrDefault();

            if (merch == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户");
            }

            var store = CurrentDb.Store.Where(m => m.Id == machine.StoreId).FirstOrDefault();

            if (store == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户店铺");
            }

            ret.Machine.Id = machine.Id;
            ret.Machine.Name = machine.Name;
            ret.Machine.LogoImgUrl = machine.LogoImgUrl;
            ret.Machine.MerchName = merch.Name;
            ret.Machine.StoreName = store.Name;


            ret.Banners = StoreTermServiceFactory.Machine.GetBanners(machine.MerchId, machine.StoreId, machine.Id);

            ret.ProductKinds = StoreTermServiceFactory.Machine.GetProductKinds(machine.MerchId, machine.StoreId, machine.Id);

            ret.Products = StoreTermServiceFactory.Machine.GetProducts(machine.MerchId, machine.StoreId, machine.Id);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public Dictionary<string, PrdProductModel2> GetProducts(string merchId, string storeId, string machineId)
        {
            var products = StoreTermServiceFactory.Product.GetPageList(0, int.MaxValue, merchId, storeId, machineId, "");

            var dics = new Dictionary<string, PrdProductModel2>();

            if (products == null)
            {
                return dics;
            }

            if (products.Items == null)
            {
                return dics;
            }

            if (products.Items.Count == 0)
            {
                return dics;
            }

            foreach (var item in products.Items)
            {
                dics.Add(item.Id, item);
            }

            return dics;
        }

        public List<BannerModel> GetBanners(string merchId, string storeId, string machineId)
        {
            var bannerModels = new List<BannerModel>();

            var adSpaceContentIds = CurrentDb.AdSpaceContentBelong.Where(m => m.MerchId == merchId && m.AdSpaceId == E_AdSpaceId.MachineHome && m.BelongType == E_AdSpaceBelongType.Machine && m.BelongId == machineId).Select(m => m.AdSpaceContentId).ToArray();

            var adSpaceContents = CurrentDb.AdSpaceContent.Where(m => adSpaceContentIds.Contains(m.Id) && m.Status == E_AdSpaceContentStatus.Normal).ToList();

 
            foreach (var item in adSpaceContents)
            {
                bannerModels.Add(new BannerModel { ImgUrl = item.Url });
            }

            return bannerModels;
        }

        public List<ProductKindModel> GetProductKinds(string merchId, string storeId, string machineId)
        {
            var productKindModels = new List<ProductKindModel>();

            var prdKinds = CurrentDb.PrdKind.Where(m => m.MerchId == merchId && m.Depth == 1 && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

            foreach (var prdKind in prdKinds)
            {
                var prdKindModel = new ProductKindModel();
                prdKindModel.Id = prdKind.Id;
                prdKindModel.Name = prdKind.Name;

                var products = StoreTermServiceFactory.Product.GetPageList(0, 10, merchId, storeId, machineId, prdKind.Id);

                if (products != null)
                {
                    if (products.Items != null)
                    {
                        if (products.Items.Count > 0)
                        {
                            prdKindModel.Childs = products.Items.Select(m => m.Id).ToList();
                        }
                    }
                }

            }

            return productKindModels;
        }

        public CustomJsonResult GetSlotSkusStock(string merchId, string machineId)
        {
            var slotProductSkuModels = new List<SlotProductSkuModel>();

            var machineStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId && m.IsOffSell == false).ToList();

            foreach (var item in machineStocks)
            {
                var productSkuModel = BizFactory.PrdProduct.GetProductSku(item.PrdProductSkuId);

                if (productSkuModel != null)
                {
                    var slotProductSkuModel = new SlotProductSkuModel();

                    slotProductSkuModel.Id = productSkuModel.Id;
                    slotProductSkuModel.SlotId = item.SlotId;
                    slotProductSkuModel.Name = productSkuModel.Name;
                    slotProductSkuModel.MainImgUrl = productSkuModel.MainImgUrl;
                    slotProductSkuModel.SumQuantity = item.SumQuantity;
                    slotProductSkuModel.LockQuantity = item.LockQuantity;
                    slotProductSkuModel.SellQuantity = item.SellQuantity;

                    slotProductSkuModels.Add(slotProductSkuModel);
                }
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", slotProductSkuModels);
        }

        public CustomJsonResult UpdateInfo(RopMachineUpdateInfo rop)
        {
            var result = new CustomJsonResult();

            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "更新失败，找不到机器信息");
            }

            if (rop.Lat > 0)
            {
                machine.Lat = rop.Lat;
            }

            if (rop.Lng > 0)
            {
                machine.Lng = rop.Lng;
            }

            if (string.IsNullOrEmpty(rop.JPushRegId))
            {
                machine.JPushRegId = rop.JPushRegId;
            }


            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "更新成功");
        }

        public CustomJsonResult Login(RopMachineLogin rop)
        {

            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未登记");
            }

            var sysMerchantUser = CurrentDb.SysMerchUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();

            if (sysMerchantUser == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败,用户名不存在");
            }

            var isPasswordCorrect = PassWordHelper.VerifyHashedPassword(sysMerchantUser.PasswordHash, rop.Password);

            if (!isPasswordCorrect)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败,用户密码错误");
            }

            if (sysMerchantUser.MerchId != machine.MerchId)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "帐号与商户不对应");
            }

            var ret = new RetMachineLogin();

            ret.UserId = sysMerchantUser.Id;
            ret.UserName = sysMerchantUser.UserName;
            ret.FullName = sysMerchantUser.FullName;

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);

        }
    }
}
