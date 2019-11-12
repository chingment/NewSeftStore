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
using System.Transactions;

namespace LocalS.Service.Api.StoreTerm
{
    public class MachineService : BaseDbContext
    {
        public CustomJsonResult InitData(RopMachineInitData rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetMachineInitData();


            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            if (machine == null)
            {
                machine.Id = GuidUtil.New();
                machine.JPushRegId = rop.JPushRegId;
                machine.MacAddress = rop.MacAddress;
                machine.AppVersionCode = rop.AppVersionCode;
                machine.AppVersionName = rop.AppVersionName;
                machine.CtrlSdkVersionCode = rop.CtrlSdkVersionCode;
                machine.CreateTime = DateTime.Now;
                machine.Creator = GuidUtil.Empty();
                CurrentDb.Machine.Add(machine);
                CurrentDb.SaveChanges();
            }
            else
            {
                machine.JPushRegId = rop.JPushRegId;
                machine.MacAddress = rop.MacAddress;
                machine.AppVersionCode = rop.AppVersionCode;
                machine.AppVersionName = rop.AppVersionName;
                machine.CtrlSdkVersionCode = rop.CtrlSdkVersionCode;
                machine.MendTime = DateTime.Now;
                machine.Mender = GuidUtil.Empty();
                CurrentDb.SaveChanges();
            }

            if (string.IsNullOrEmpty(machine.CurUseMerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户");
            }

            if (string.IsNullOrEmpty(machine.CurUseStoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户店铺");
            }

            var machineInfo = BizFactory.Machine.GetOne(rop.MachineId);
            ret.Machine.Id = machineInfo.Id;
            ret.Machine.Name = machineInfo.Name;
            ret.Machine.LogoImgUrl = machineInfo.LogoImgUrl;
            ret.Machine.MerchName = machineInfo.MerchName;
            ret.Machine.StoreName = machineInfo.StoreName;
            ret.Machine.CsrQrCode = machineInfo.CsrQrCode;
            ret.Machine.CabinetId_1 = machineInfo.CabinetId_1;
            ret.Machine.CabinetName_1 = machineInfo.CabinetName_1;
            ret.Machine.CabinetRowColLayout_1 = machineInfo.CabinetRowColLayout_1;

            ret.Banners = StoreTermServiceFactory.Machine.GetBanners(machineInfo.MerchId, machineInfo.StoreId, machineInfo.Id);
            ret.ProductKinds = StoreTermServiceFactory.Machine.GetProductKinds(machineInfo.MerchId, machineInfo.StoreId, machineInfo.Id);
            ret.ProductSkus = StoreTermServiceFactory.Machine.GetProductSkus(machineInfo.MerchId, machineInfo.StoreId, machineInfo.Id);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public Dictionary<string, ProductSkuModel> GetProductSkus(string merchId, string storeId, string machineId)
        {
            var products = StoreTermServiceFactory.ProductSku.GetPageList(0, int.MaxValue, merchId, storeId, machineId);

            var dics = new Dictionary<string, ProductSkuModel>();

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

            var adContentIds = CurrentDb.AdContentBelong.Where(m => m.MerchId == merchId && m.AdSpaceId == E_AdSpaceId.MachineHome && m.BelongType == E_AdSpaceBelongType.Machine && m.BelongId == machineId).Select(m => m.AdContentId).ToArray();

            var adContents = CurrentDb.AdContent.Where(m => adContentIds.Contains(m.Id) && m.Status == E_AdContentStatus.Normal).ToList();


            foreach (var item in adContents)
            {
                bannerModels.Add(new BannerModel { Url = item.Url });
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

                var productIds = CurrentDb.PrdProductKind.Where(m => m.PrdKindId == prdKind.Id).Select(m => m.PrdProductId).Distinct().ToList();
                if (productIds.Count > 0)
                {
                    var productSkuIds = CurrentDb.SellChannelStock.Where(m => productIds.Contains(m.PrdProductId)).Select(m => m.PrdProductSkuId).Distinct().ToList();
                    if (productSkuIds.Count > 0)
                    {
                        prdKindModel.Childs = productSkuIds;
                        productKindModels.Add(prdKindModel);
                    }
                }

            }

            return productKindModels;
        }

        public CustomJsonResult GetCabinetSlots(string machineId, string cabinetId)
        {
            var ret = new RetMachineGetSlots();

            var machine = BizFactory.Machine.GetOne(machineId);

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未登记");
            }

            if (string.IsNullOrEmpty(machine.MerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户");
            }

            if (string.IsNullOrEmpty(machine.StoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户店铺");
            }

            if (machine.CabinetRowColLayout_1 == null || machine.CabinetRowColLayout_1.Length == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未识别到行列布局，请点击扫描按钮");
            }

            ret.RowColLayout = machine.CabinetRowColLayout_1;

            var machineStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == machine.MerchId && m.StoreId == machine.StoreId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId).ToList();

            foreach (var item in machineStocks)
            {
                var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(item.MerchId, item.StoreId, new string[] { machineId }, item.PrdProductSkuId);

                if (bizProductSku != null)
                {
                    var slot = new SlotModel();

                    slot.Id = item.SlotId;
                    slot.ProductSkuId = bizProductSku.Id;
                    slot.ProductSkuName = bizProductSku.Name;
                    slot.ProductSkuMainImgUrl = bizProductSku.MainImgUrl;
                    slot.SumQuantity = item.SumQuantity;
                    slot.LockQuantity = item.LockQuantity;
                    slot.SellQuantity = item.SellQuantity;
                    slot.MaxQuantity = 10;
                    ret.Slots.Add(item.SlotId, slot);
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult SaveCabinetSlot(RopMachineSaveCabinetSlot rop)
        {
            var machine = BizFactory.Machine.GetOne(rop.MachineId);

            if (string.IsNullOrEmpty(rop.ProductSkuId))
            {
                return BizFactory.ProductSku.OperateStock(GuidUtil.New(), OperateStockType.MachineSlotRemove, machine.MerchId, machine.StoreId, rop.MachineId, rop.Id, rop.ProductSkuId);
            }
            else
            {
                return BizFactory.ProductSku.OperateStock(GuidUtil.New(), OperateStockType.MachineSlotSave, machine.MerchId, machine.StoreId, rop.MachineId, rop.Id, rop.ProductSkuId, rop.SumQuantity);
            }


        }

        public CustomJsonResult SaveCabinetRowColLayout(RopMachineSaveCabinetRowColLayout rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

                machine.CabinetRowColLayout_1 = string.Join(",", rop.CabinetRowColLayout);

                int rowLength = rop.CabinetRowColLayout.Length;

                List<string> slotIds = new List<string>();
                for (int i = 0; i < rowLength; i++)
                {
                    int colLength = rop.CabinetRowColLayout[i];

                    for (var j = 0; j < colLength; j++)
                    {
                        slotIds.Add(string.Format("n{0}r{1}c{1}", rop.CabinetId, i, j));
                    }
                }

                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == machine.CurUseMerchId && m.StoreId == machine.CurUseStoreId && m.RefType == E_SellChannelRefType.Machine && m.RefId == rop.MachineId && !slotIds.Contains(m.SlotId)).ToList();
                foreach (var sellChannelStock in sellChannelStocks)
                {
                    CurrentDb.SellChannelStock.Remove(sellChannelStock);
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "扫描结果上传成功");
            }

            return result;
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

            if (sysMerchantUser.MerchId != machine.CurUseMerchId)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "帐号与商户不对应");
            }

            var ret = new RetMachineLogin();

            ret.UserId = sysMerchantUser.Id;
            ret.UserName = sysMerchantUser.UserName;
            ret.FullName = sysMerchantUser.FullName;

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);

        }

        public CustomJsonResult SendRunStatus(RopMachineSendRunStatus rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            if (machine != null)
            {
                switch (rop.Status)
                {
                    case "running":
                        machine.RunStatus = E_MachineRunStatus.Running;
                        machine.LastRequestTime = DateTime.Now;
                        break;
                    case "setting":
                        machine.RunStatus = E_MachineRunStatus.Setting;
                        machine.LastRequestTime = DateTime.Now;
                        break;
                }

                CurrentDb.SaveChanges();
            }

            return result;
        }
    }
}
