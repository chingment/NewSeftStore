using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using Lumos.Redis;
using NPinyin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class PrdProductService : BaseService
    {
        private List<TreeNode> GetKindTree(int id, List<PrdKind> prdKinds)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_PrdKinds = prdKinds.Where(t => t.PId == id).ToList();

            foreach (var p_productKind in p_PrdKinds)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_productKind.Id.ToString();
                treeNode.PId = p_productKind.PId.ToString();
                treeNode.Value = p_productKind.Id.ToString();
                treeNode.Label = p_productKind.Name;
                treeNode.IsDisabled = p_productKind.Depth <= 0 ? true : false;

                var children = GetKindTree(p_productKind.Id, prdKinds);
                if (children != null)
                {
                    if (children.Count > 0)
                    {
                        treeNode.Children = new List<TreeNode>();
                        treeNode.Children.AddRange(children);
                    }
                    else
                    {
                        treeNode.Children = null;
                    }
                }

                treeNodes.Add(treeNode);

            }

            return treeNodes;
        }

        public List<TreeNode> GetKindTree()
        {
            var prdKinds = CurrentDb.PrdKind.OrderBy(m => m.Priority).ToList();
            return GetKindTree(1, prdKinds);
        }

        public CustomJsonResult GetList(string operater, string merchId, RupPrdProductGetList rup)
        {
            var result = new CustomJsonResult();


            string[] spuIds = null;

            if (!string.IsNullOrEmpty(rup.Key))
            {
                List<string> l_SpuIds = new List<string>();
                var search1 = CacheServiceFactory.Product.SearchSpu(merchId, "All", rup.Key);
                var search2 = CacheServiceFactory.Product.SearchSku(merchId, "All", rup.Key);
                if (search1 != null)
                {
                    var l_SpuId = search1.Select(m => m.SpuId).Distinct().ToArray();

                    l_SpuIds.AddRange(l_SpuId);
                }

                if (search2 != null)
                {
                    var l_SpuId = search2.Select(m => m.SpuId).Distinct().ToArray();

                    l_SpuIds.AddRange(l_SpuId);
                }

                spuIds = l_SpuIds.Distinct().ToArray();
            }

            var query = (from u in CurrentDb.PrdSpu
                         where
                         u.MerchId == merchId
                         &&
                         u.IsDelete == rup.IsDelete
                         select new { u.Id, u.Name, u.SpuCode, u.BriefDes, u.KindIds, u.KindId1, u.KindId2, u.KindId3, u.CreateTime, u.DisplayImgUrls });

            if (spuIds != null)
            {
                query = query.Where(m => spuIds.Contains(m.Id));
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string str_prdKindNames = "";
                List<int> prdKindIds = new List<int>();
                if (!string.IsNullOrEmpty(item.KindIds))
                {
                    prdKindIds.Add(item.KindId1);
                    prdKindIds.Add(item.KindId2);
                    prdKindIds.Add(item.KindId3);
                    var prdKindNames = CurrentDb.PrdKind.Where(p => prdKindIds.Contains(p.Id)).OrderBy(m => m.Depth).Select(m => m.Name).ToArray();
                    str_prdKindNames = prdKindNames.Length != 0 ? string.Join("/", prdKindNames) : "";
                }

                var d_Skus = CurrentDb.PrdSku.Where(m => m.SpuId == item.Id).ToList();

                var list_Sku = new List<object>();

                foreach (var d_Sku in d_Skus)
                {
                    list_Sku.Add(new { Id = d_Sku.Id, CumCode = d_Sku.CumCode, BarCode = d_Sku.BarCode, SalePrice = d_Sku.SalePrice, SpecDes = d_Sku.SpecDes });
                }

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    SpuCode = item.SpuCode,
                    Skus = list_Sku,
                    BriefDes = item.BriefDes,
                    MainImgUrl = ImgSet.GetMain_S(item.DisplayImgUrls),
                    KindNames = str_prdKindNames,
                    CreateTime = item.CreateTime,
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult InitAdd(string operater, string merchId)
        {
            var result = new CustomJsonResult();
            var ret = new RetProductInitAdd();

            ret.Kinds = GetKindTree();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopProductAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            if (string.IsNullOrEmpty(rop.Name))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品名称不能为空");
            }

            if (rop.KindIds == null || rop.KindIds.Count != 3)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择三级商品分类");
            }

            if (rop.DisplayImgUrls == null || rop.DisplayImgUrls.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "至少上传一张商品图片");
            }

            if (rop.SpecItems == null || rop.SpecItems.Where(m => m.Value.Count > 0).ToList().Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "至少录入一种规格");
            }


            List<string> skuIds = new List<string>();


            using (TransactionScope ts = new TransactionScope())
            {
                var d_Spu = new PrdSpu();
                d_Spu.Id = IdWorker.Build(IdType.NewGuid);
                d_Spu.MerchId = merchId;
                d_Spu.Name = rop.Name.Trim2();
                d_Spu.SpuCode = rop.SpuCode.Trim2();
                d_Spu.KindIds = string.Join(",", rop.KindIds.ToArray());
                d_Spu.KindId1 = rop.KindIds[0];
                d_Spu.KindId2 = rop.KindIds[1];
                d_Spu.KindId3 = rop.KindIds[2];
                d_Spu.PinYinIndex = CommonUtil.GetPingYinIndex(d_Spu.Name);
                d_Spu.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                d_Spu.MainImgUrl = ImgSet.GetMain_O(d_Spu.DisplayImgUrls);
                d_Spu.DetailsDes = rop.DetailsDes.ToJsonString();
                d_Spu.BriefDes = rop.BriefDes.Trim2();
                d_Spu.IsTrgVideoService = rop.IsTrgVideoService;
                d_Spu.IsRevService = rop.IsRevService;
                d_Spu.IsSupRentService = rop.IsSupRentService;
                d_Spu.IsMavkBuy = rop.IsMavkBuy;
                d_Spu.SupReceiveMode = rop.SupReceiveMode;
                d_Spu.CharTags = rop.CharTags.ToJsonString();
                d_Spu.SpecItems = rop.SpecItems.Where(m => m.Value.Count > 0).ToJsonString();
                d_Spu.SupplierId = rop.SupplierId;
                d_Spu.Creator = operater;
                d_Spu.CreateTime = DateTime.Now;

                foreach (var sku in rop.Skus)
                {
                    if (string.IsNullOrEmpty(sku.CumCode))
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品编码不能为空");
                    }

                    if (sku.SalePrice <= 0)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品价格必须大于0");
                    }

                    if (sku.SpecDes == null || sku.SpecDes.Count <= 0)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品规格不能为空");
                    }

                    var isExtitSkuCode = CurrentDb.PrdSku.Where(m => m.MerchId == merchId && m.CumCode == sku.CumCode).FirstOrDefault();
                    if (isExtitSkuCode != null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品编码已经存在");
                    }

                    var d_Sku = new PrdSku();
                    d_Sku.Id = IdWorker.Build(IdType.NewGuid);
                    d_Sku.MerchId = d_Spu.MerchId;
                    d_Sku.SpuId = d_Spu.Id;
                    d_Sku.BarCode = sku.BarCode.Trim2();
                    d_Sku.CumCode = sku.CumCode.Trim2();
                    d_Sku.Name = BizFactory.ProductSku.GetSkuSpecCombineName(d_Spu.Name, sku.SpecDes);
                    d_Sku.PinYinIndex = CommonUtil.GetPingYinIndex(d_Sku.Name);
                    d_Sku.SpecDes = sku.SpecDes.ToJsonString();
                    d_Sku.SpecIdx = string.Join(",", sku.SpecDes.Select(m => m.Value));
                    d_Sku.SalePrice = sku.SalePrice;
                    d_Sku.Creator = operater;
                    d_Sku.CreateTime = DateTime.Now;
                    CurrentDb.PrdSku.Add(d_Sku);
                    CurrentDb.SaveChanges();

                    skuIds.Add(d_Sku.Id);
                }


                CurrentDb.PrdSpu.Add(d_Spu);
                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            if (result.Result == ResultType.Success)
            {
                Task.Factory.StartNew(() =>
                {
                    CacheServiceFactory.Product.GetSkuInfo(merchId, skuIds.ToArray());
                    MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.product_add, string.Format("新建商品（{0}）成功", rop.Name), rop);
                });
            }

            return result;
        }

        public CustomJsonResult InitEdit(string operater, string merchId, string spuId)
        {
            var ret = new RetProductInitEdit();
            var d_Spu = CurrentDb.PrdSpu.Where(m => m.MerchId == merchId && m.Id == spuId).FirstOrDefault();
            if (d_Spu != null)
            {
                ret.Id = d_Spu.Id;
                ret.Name = d_Spu.Name.NullToEmpty();
                ret.SpuCode = d_Spu.SpuCode.NullToEmpty();
                ret.SpecItems = d_Spu.SpecItems.ToJsonObject<List<SpecItem>>();
                ret.DetailsDes = d_Spu.DetailsDes.ToJsonObject<List<ImgSet>>();
                ret.BriefDes = d_Spu.BriefDes.NullToEmpty();
                ret.KindIds = string.IsNullOrEmpty(d_Spu.KindIds) ? new List<string>() : d_Spu.KindIds.Split(',').ToList();
                ret.CharTags = d_Spu.CharTags.ToJsonObject<List<string>>();
                ret.DisplayImgUrls = d_Spu.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                ret.Kinds = GetKindTree();
                ret.IsTrgVideoService = d_Spu.IsTrgVideoService;
                ret.IsRevService = d_Spu.IsRevService;
                ret.IsSupRentService = d_Spu.IsSupRentService;
                ret.SupReceiveMode = d_Spu.SupReceiveMode;
                ret.IsMavkBuy = d_Spu.IsMavkBuy;
                if (!string.IsNullOrEmpty(d_Spu.SupplierId))
                {
                    var supplier = CurrentDb.Supplier.Where(m => m.Id == d_Spu.SupplierId).FirstOrDefault();
                    if (supplier != null)
                    {
                        ret.SupplierId = supplier.Id;
                        ret.SupplierName = supplier.Name;
                    }
                }

                var d_Skus = CurrentDb.PrdSku.Where(m => m.SpuId == d_Spu.Id).OrderBy(m => m.SpecDes).ToList();

                foreach (var d_Sku in d_Skus)
                {
                    ret.Skus.Add(new RetProductInitEdit.Sku { Id = d_Sku.Id, SalePrice = d_Sku.SalePrice, BarCode = d_Sku.BarCode, CumCode = d_Sku.CumCode, IsOffSell = false, SpecDes = d_Sku.SpecDes.ToJsonObject<List<object>>() });
                }

            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult Edit(string operater, string merchId, RopProductEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.Id))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品Id不能为空");
            }

            if (string.IsNullOrEmpty(rop.Name))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品名称不能为空");
            }

            if (rop.KindIds == null || rop.KindIds.Count != 3)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择三级商品分类");
            }

            if (rop.DisplayImgUrls == null || rop.DisplayImgUrls.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品图片不能为空");
            }

            //先删除缓存

            CacheServiceFactory.Product.RemoveSpuInfo(merchId, rop.Id);


            List<string> skuIds = new List<string>();


            using (TransactionScope ts = new TransactionScope())
            {
                var d_Spu = CurrentDb.PrdSpu.Where(m => m.Id == rop.Id).FirstOrDefault();

                d_Spu.Name = rop.Name;
                d_Spu.SpuCode = rop.SpuCode;
                d_Spu.KindIds = string.Join(",", rop.KindIds.ToArray());
                d_Spu.KindId1 = rop.KindIds[0];
                d_Spu.KindId2 = rop.KindIds[1];
                d_Spu.KindId3 = rop.KindIds[2];
                d_Spu.PinYinIndex = CommonUtil.GetPingYinIndex(d_Spu.Name);
                d_Spu.BriefDes = rop.BriefDes;
                d_Spu.DetailsDes = rop.DetailsDes.ToJsonString();
                d_Spu.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                d_Spu.IsTrgVideoService = rop.IsTrgVideoService;
                d_Spu.IsRevService = rop.IsRevService;
                d_Spu.IsSupRentService = rop.IsSupRentService;
                d_Spu.IsMavkBuy = rop.IsMavkBuy;
                d_Spu.SupReceiveMode = rop.SupReceiveMode;
                d_Spu.CharTags = rop.CharTags.ToJsonString();
                d_Spu.SupplierId = rop.SupplierId;

                List<SpecItem> specItems = new List<BLL.SpecItem>();
                foreach (var item in rop.Skus)
                {
                    foreach (var item2 in item.SpecDes)
                    {
                        var specItem = specItems.Where(m => m.Name == item2.Name).FirstOrDefault();
                        if (specItem == null)
                        {
                            specItem = new SpecItem();
                            specItem.Name = item2.Name;
                            specItem.Value.Add(new SpecItemValue { Name = item2.Value });
                            specItems.Add(specItem);
                        }
                        else
                        {
                            var value = specItem.Value.Where(m => m.Name == item2.Value).FirstOrDefault();
                            if (value == null)
                            {
                                specItem.Value.Add(new SpecItemValue { Name = item2.Value });
                            }
                        }
                    }
                }

                d_Spu.SpecItems = specItems.ToJsonString();

                d_Spu.Mender = operater;
                d_Spu.MendTime = DateTime.Now;


                foreach (var sku in rop.Skus)
                {
                    if (CommonUtil.IsEmpty(sku.CumCode))
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品编码不能为空");
                    }

                    var isExtitSkuCode = CurrentDb.PrdSku.Where(m => m.MerchId == merchId && m.Id != sku.Id && m.CumCode == sku.CumCode).FirstOrDefault();
                    if (isExtitSkuCode != null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品编码已经存在");
                    }

                    var d_Sku = CurrentDb.PrdSku.Where(m => m.Id == sku.Id).FirstOrDefault();
                    if (d_Sku != null)
                    {
                        d_Sku.Name = BizFactory.ProductSku.GetSkuSpecCombineName(d_Spu.Name, d_Sku.SpecDes.ToJsonObject<List<SpecDes>>());
                        d_Sku.PinYinIndex = CommonUtil.GetPingYinIndex(d_Sku.Name);
                        d_Sku.SpecDes = sku.SpecDes.ToJsonString();
                        d_Sku.SpecIdx = string.Join(",", sku.SpecDes.Select(m => m.Value));
                        d_Sku.CumCode = sku.CumCode;
                        d_Sku.BarCode = sku.BarCode;
                        d_Sku.SalePrice = sku.SalePrice;
                        d_Sku.Mender = operater;
                        d_Sku.MendTime = DateTime.Now;


                        //统一修改销售价格
                        if (rop.IsUnifyUpdateSalePrice)
                        {
                            var d_SellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.SkuId == d_Sku.Id).ToList();
                            foreach (var d_SellChannelStock in d_SellChannelStocks)
                            {
                                d_SellChannelStock.SalePrice = sku.SalePrice;
                                d_SellChannelStock.IsOffSell = sku.IsOffSell;
                                d_SellChannelStock.Mender = operater;
                                d_SellChannelStock.MendTime = DateTime.Now;
                            }
                        }
                    }

                    skuIds.Add(sku.Id);
                }

                CurrentDb.SaveChanges();
                ts.Complete();


                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            if (result.Result == ResultType.Success)
            {
                Task.Factory.StartNew(() =>
                {
                    CacheServiceFactory.Product.RemoveSpuInfo(merchId, rop.Id);
                    CacheServiceFactory.Product.GetSkuInfo(merchId, skuIds.ToArray());
                    MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.product_edit, string.Format("保存商品（{0}）信息成功", rop.Name), rop);
                });
            }


            return result;
        }

        public CustomJsonResult Delete(string operater, string merchId, RopProductDelete rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.Id))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品Id不能为空");
            }

            //先删除缓存

            CacheServiceFactory.Product.RemoveSpuInfo(merchId, rop.Id);


            List<string> skuIds = new List<string>();


            using (TransactionScope ts = new TransactionScope())
            {
                var d_Spu = CurrentDb.PrdSpu.Where(m => m.Id == rop.Id).FirstOrDefault();

                d_Spu.IsDelete = true;
                d_Spu.Mender = operater;
                d_Spu.MendTime = DateTime.Now;

                var d_Skus = CurrentDb.PrdSku.Where(m => m.SpuId == d_Spu.Id).ToList();

                foreach (var d_Sku in d_Skus)
                {
                    d_Sku.IsDelete = true;
                    d_Sku.Mender = operater;
                    d_Sku.MendTime = DateTime.Now;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");
            }

            if (result.Result == ResultType.Success)
            {
                Task.Factory.StartNew(() =>
                {
                    CacheServiceFactory.Product.RemoveSpuInfo(merchId, rop.Id);
                });
            }

            return result;
        }

        public CustomJsonResult GetListBySale(string operater, string merchId, RupPrdProductGetListBySale rup)
        {
            string[] spuIds = null;

            if (!string.IsNullOrEmpty(rup.Key))
            {
                List<string> l_SpuIds = new List<string>();
                var search1 = CacheServiceFactory.Product.SearchSpu(merchId, "All", rup.Key);
                var search2 = CacheServiceFactory.Product.SearchSku(merchId, "All", rup.Key);
                if (search1 != null)
                {
                    var l_SpuId = search1.Select(m => m.SpuId).Distinct().ToArray();

                    l_SpuIds.AddRange(l_SpuId);
                }

                if (search2 != null)
                {
                    var l_SpuId = search2.Select(m => m.SpuId).Distinct().ToArray();

                    l_SpuIds.AddRange(l_SpuId);
                }

                spuIds = l_SpuIds.Distinct().ToArray();
            }

            var query = (from u in CurrentDb.SellChannelStock
                         where u.MerchId == merchId
                         group u by new
                         {
                             u.StoreId,
                             u.SpuId,
                             u.SkuId,
                             u.IsOffSell,
                             u.SalePrice
                         }
             into g
                         select new { g.Key });

            if (spuIds != null)
            {
                query = query.Where(m => spuIds.Contains(m.Key.SpuId));
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.Key.StoreId).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var r_Sku = CacheServiceFactory.Product.GetSkuInfo(merchId, item.Key.SkuId);
                var store = BizFactory.Store.GetOne(item.Key.StoreId);
                olist.Add(new
                {
                    StoreId = item.Key.StoreId,
                    StoreName = store.Name,
                    SpuId = item.Key.SpuId,
                    SkuId = item.Key.SkuId,
                    CumCode = r_Sku.CumCode,
                    Name = r_Sku.Name,
                    MainImgUrl = r_Sku.MainImgUrl,
                    IsOffSell = item.Key.IsOffSell,
                    SalePrice = item.Key.SalePrice
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);
        }

        public CustomJsonResult EditSale(string operater, string merchId, RopProductEditSale rop)
        {
            return BizFactory.ProductSku.AdjustStockSalePrice(operater, merchId, rop.StoreId, rop.SkuId, rop.SalePrice, rop.IsOffSell);
        }

        public CustomJsonResult SearchSpu(string operater, string merchId, string key)
        {
            var products = CacheServiceFactory.Product.SearchSpu(merchId, "All", key);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", products);
        }

        public CustomJsonResult SearchSku(string operater, string merchId, string key)
        {
            var r_Skus = CacheServiceFactory.Product.SearchSku(merchId, "All", key);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", r_Skus);
        }

        public CustomJsonResult GetSpecs(string operater, string merchId, string spuId)
        {

            var d_Skus = CurrentDb.PrdSku.Where(m => m.SpuId == spuId).ToList();

            List<object> objs = new List<object>();


            foreach (var d_Sku in d_Skus)
            {
                objs.Add(new { SkuId = d_Sku.Id, CumCode = d_Sku.CumCode, SumQuantity = 10000, SpecIdx = d_Sku.SpecIdx, SalePrice = d_Sku.SalePrice });
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", objs);
        }

    }
}
