﻿using LocalS.BLL;
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
    public class PrdProductService : BaseDbContext
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

            string[] productSkuIds = null;
            string[] productIds = null;
            if (!string.IsNullOrEmpty(rup.Key))
            {
                var search = CacheServiceFactory.Product.Search(merchId, "All", rup.Key);
                if (search != null)
                {
                    productSkuIds = search.Select(m => m.Id).Distinct().ToArray();

                    productIds = CurrentDb.PrdProductSku.Where(m => productSkuIds.Contains(m.Id)).Select(m => m.PrdProductId).Distinct().ToArray();
                }
            }



            var query = (from u in CurrentDb.PrdProduct
                         where
                         u.MerchId == merchId
                         select new { u.Id, u.Name, u.BriefDes, u.PrdKindIds, u.PrdKindId1, u.PrdKindId2, u.PrdKindId3, u.CreateTime, u.DisplayImgUrls });

            if (productIds != null)
            {
                query = query.Where(m => productIds.Contains(m.Id));
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
                if (!string.IsNullOrEmpty(item.PrdKindIds))
                {
                    prdKindIds.Add(item.PrdKindId1.Value);
                    prdKindIds.Add(item.PrdKindId2.Value);
                    prdKindIds.Add(item.PrdKindId3.Value);
                    var prdKindNames = CurrentDb.PrdKind.Where(p => prdKindIds.Contains(p.Id)).OrderBy(m => m.Depth).Select(m => m.Name).ToArray();
                    str_prdKindNames = prdKindNames.Length != 0 ? string.Join("/", prdKindNames) : "";
                }

                var prdProductSkus = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == item.Id).ToList();

                var list_Sku = new List<object>();

                foreach (var prdProductSku in prdProductSkus)
                {
                    list_Sku.Add(new { Id = prdProductSku.Id, CumCode = prdProductSku.CumCode, BarCode = prdProductSku.BarCode, SalePrice = prdProductSku.SalePrice, SpecDes = prdProductSku.SpecDes });
                }

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    BriefDes = item.BriefDes,
                    MainImgUrl = ImgSet.GetMain_S(item.DisplayImgUrls),
                    KindNames = str_prdKindNames,
                    Skus = list_Sku,
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
            var ret = new RetPrdProductInitAdd();

            ret.Kinds = GetKindTree();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public string GetSkuSpecCombineName(string productName, List<SpecDes> specDess)
        {
            string str_Spec = "";
            foreach (var spec in specDess)
            {
                str_Spec += spec.Value + " ";
            }

            string name = productName + " " + str_Spec.Trim2();

            return name;
        }

        public CustomJsonResult Add(string operater, string merchId, RopPrdProductAdd rop)
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


            using (TransactionScope ts = new TransactionScope())
            {
                var prdProduct = new PrdProduct();
                prdProduct.Id = IdWorker.Build(IdType.NewGuid);
                prdProduct.MerchId = merchId;
                prdProduct.Name = rop.Name.Trim2();
                prdProduct.PrdKindIds = string.Join(",", rop.KindIds.ToArray());
                prdProduct.PrdKindId1 = rop.KindIds[0];
                prdProduct.PrdKindId2 = rop.KindIds[1];
                prdProduct.PrdKindId3 = rop.KindIds[2];
                prdProduct.PinYinIndex = CommonUtil.GetPingYinIndex(prdProduct.Name);
                prdProduct.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                prdProduct.MainImgUrl = ImgSet.GetMain_O(prdProduct.DisplayImgUrls);
                prdProduct.DetailsDes = rop.DetailsDes.ToJsonString();
                prdProduct.BriefDes = rop.BriefDes.Trim2();
                prdProduct.IsTrgVideoService = rop.IsTrgVideoService;
                prdProduct.CharTags = rop.CharTags.ToJsonString();
                prdProduct.SpecItems = rop.SpecItems.Where(m => m.Value.Count > 0).ToJsonString();
                prdProduct.Creator = operater;
                prdProduct.CreateTime = DateTime.Now;

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

                    var isExtitSkuCode = CurrentDb.PrdProductSku.Where(m => m.MerchId == merchId && m.CumCode == sku.CumCode).FirstOrDefault();
                    if (isExtitSkuCode != null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品编码已经存在");
                    }

                    var prdProductSku = new PrdProductSku();
                    prdProductSku.Id = IdWorker.Build(IdType.NewGuid);
                    prdProductSku.MerchId = prdProduct.MerchId;
                    prdProductSku.PrdProductId = prdProduct.Id;
                    prdProductSku.BarCode = sku.BarCode.Trim2();
                    prdProductSku.CumCode = sku.CumCode.Trim2();
                    prdProductSku.Name = GetSkuSpecCombineName(prdProduct.Name, sku.SpecDes);
                    prdProductSku.PinYinIndex = CommonUtil.GetPingYinIndex(prdProductSku.Name);
                    prdProductSku.SpecDes = sku.SpecDes.ToJsonString();
                    prdProductSku.SpecIdx = string.Join(",", sku.SpecDes.Select(m => m.Value));
                    prdProductSku.SalePrice = sku.SalePrice;
                    prdProductSku.Creator = operater;
                    prdProductSku.CreateTime = DateTime.Now;
                    CurrentDb.PrdProductSku.Add(prdProductSku);
                    CurrentDb.SaveChanges();
                }


                CurrentDb.PrdProduct.Add(prdProduct);
                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.PrdProductAdd, string.Format("新建商品（{0}）成功", rop.Name));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;
        }

        public CustomJsonResult InitEdit(string operater, string merchId, string prdProductId)
        {
            var ret = new RetPrdProductInitEdit();
            var prdProduct = CurrentDb.PrdProduct.Where(m => m.MerchId == merchId && m.Id == prdProductId).FirstOrDefault();
            if (prdProduct != null)
            {
                ret.Id = prdProduct.Id;
                ret.Name = prdProduct.Name;
                ret.DetailsDes = prdProduct.DetailsDes.ToJsonObject<List<ImgSet>>();
                ret.BriefDes = prdProduct.BriefDes;
                ret.KindIds = string.IsNullOrEmpty(prdProduct.PrdKindIds) ? new List<string>() : prdProduct.PrdKindIds.Split(',').ToList();
                ret.CharTags = prdProduct.CharTags.ToJsonObject<List<string>>();
                ret.DisplayImgUrls = prdProduct.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                ret.Kinds = GetKindTree();
                ret.IsTrgVideoService = prdProduct.IsTrgVideoService;
                var prdProductSkus = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == prdProduct.Id).OrderBy(m => m.SpecDes).ToList();

                foreach (var prdProductSku in prdProductSkus)
                {
                    ret.Skus.Add(new RetPrdProductInitEdit.Sku { Id = prdProductSku.Id, SalePrice = prdProductSku.SalePrice, BarCode = prdProductSku.BarCode, CumCode = prdProductSku.CumCode, SpecDes = prdProductSku.SpecDes.ToJsonObject<List<object>>() });
                }

            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult Edit(string operater, string merchId, RopPrdProductEdit rop)
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

            using (TransactionScope ts = new TransactionScope())
            {
                var prdProduct = CurrentDb.PrdProduct.Where(m => m.Id == rop.Id).FirstOrDefault();

                prdProduct.Name = rop.Name;

                prdProduct.PrdKindIds = string.Join(",", rop.KindIds.ToArray());
                prdProduct.PrdKindId1 = rop.KindIds[0];
                prdProduct.PrdKindId2 = rop.KindIds[1];
                prdProduct.PrdKindId3 = rop.KindIds[2];
                prdProduct.PinYinIndex = CommonUtil.GetPingYinIndex(prdProduct.Name);
                prdProduct.BriefDes = rop.BriefDes;
                prdProduct.DetailsDes = rop.DetailsDes.ToJsonString();
                prdProduct.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                prdProduct.IsTrgVideoService = rop.IsTrgVideoService;
                prdProduct.CharTags = rop.CharTags.ToJsonString();
                prdProduct.Mender = operater;
                prdProduct.MendTime = DateTime.Now;


                foreach (var sku in rop.Skus)
                {
                    if (CommonUtil.IsEmpty(sku.CumCode))
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品编码不能为空");
                    }

                    var isExtitSkuCode = CurrentDb.PrdProductSku.Where(m => m.MerchId == merchId && m.Id != sku.Id && m.CumCode == sku.CumCode).FirstOrDefault();
                    if (isExtitSkuCode != null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品编码已经存在");
                    }

                    var prdProductSku = CurrentDb.PrdProductSku.Where(m => m.Id == sku.Id).FirstOrDefault();
                    if (prdProductSku != null)
                    {
                        prdProductSku.Name = GetSkuSpecCombineName(prdProduct.Name, prdProductSku.SpecDes.ToJsonObject<List<SpecDes>>());
                        prdProductSku.PinYinIndex = CommonUtil.GetPingYinIndex(prdProductSku.Name);
                        prdProductSku.CumCode = sku.CumCode;
                        prdProductSku.BarCode = sku.BarCode;
                        prdProductSku.SalePrice = sku.SalePrice;
                        prdProductSku.Mender = operater;
                        prdProductSku.MendTime = DateTime.Now;


                        //统一修改销售价格
                        if (rop.IsUnifyUpdateSalePrice)
                        {
                            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.PrdProductSkuId == prdProductSku.Id).ToList();
                            foreach (var sellChannelStock in sellChannelStocks)
                            {
                                sellChannelStock.SalePrice = sku.SalePrice;
                                sellChannelStock.Mender = operater;
                                sellChannelStock.MendTime = DateTime.Now;
                            }
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.PrdProductEdit, string.Format("保存商品（{0}）信息成功", rop.Name));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }


            if (result.Result == ResultType.Success)
            {
                var sellChannelStocks = (from m in CurrentDb.SellChannelStock where m.MerchId == merchId && m.PrdProductId == rop.Id select new { m.StoreId, m.SellChannelRefId, m.PrdProductSkuId }).Distinct();

                foreach (var sellChannelStock in sellChannelStocks)
                {
                    var bizProductSku = CacheServiceFactory.Product.GetSkuStock(merchId, sellChannelStock.StoreId, new string[] { sellChannelStock.SellChannelRefId }, sellChannelStock.PrdProductSkuId);
                    if (bizProductSku.Stocks != null)
                    {
                        if (bizProductSku.Stocks.Count > 0)
                        {
                            var updateProdcutSkuStock = new UpdateMachineProdcutSkuStockModel();
                            updateProdcutSkuStock.Id = bizProductSku.Id;
                            updateProdcutSkuStock.IsOffSell = bizProductSku.Stocks[0].IsOffSell;
                            updateProdcutSkuStock.SalePrice = bizProductSku.Stocks[0].SalePrice;
                            updateProdcutSkuStock.SalePriceByVip = bizProductSku.Stocks[0].SalePriceByVip;
                            updateProdcutSkuStock.LockQuantity = bizProductSku.Stocks.Sum(m => m.LockQuantity);
                            updateProdcutSkuStock.SellQuantity = bizProductSku.Stocks.Sum(m => m.SellQuantity);
                            updateProdcutSkuStock.SumQuantity = bizProductSku.Stocks.Sum(m => m.SumQuantity);
                            updateProdcutSkuStock.IsTrgVideoService = bizProductSku.IsTrgVideoService;
                            BizFactory.Machine.SendUpdateProductSkuStock(operater, AppId.MERCH, merchId, sellChannelStock.SellChannelRefId, updateProdcutSkuStock);
                        }
                    }
                }
            }


            return result;
        }

        public CustomJsonResult GetOnSaleStores(string operater, string merchId, string productId)
        {

            var query = (from u in CurrentDb.SellChannelStock
                         where u.PrdProductId == productId
                         && u.MerchId == merchId
                         select new { u.StoreId, u.PrdProductId, u.PrdProductSkuId, u.IsOffSell, u.SalePrice, u.SalePriceByVip }).Distinct();


            int total = query.Count();

            int pageIndex = 0;
            int pageSize = int.MaxValue;

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var productSku = CacheServiceFactory.Product.GetSkuInfo(merchId, item.PrdProductSkuId);
                var store = BizFactory.Store.GetOne(item.StoreId);
                olist.Add(new
                {
                    StoreId = item.StoreId,
                    StoreName = store.Name,
                    ProductId = item.PrdProductId,
                    ProductSkuId = item.PrdProductSkuId,
                    ProductSkuCumCode = productSku.CumCode,
                    ProductSkuName = productSku.Name,
                    ProductSkuMainImgUrl = productSku.MainImgUrl,
                    ProductSkuIsOffSell = item.IsOffSell,
                    ProductSkuSalePrice = item.SalePrice,
                    ProductSkuSalePriceByVip = item.SalePriceByVip,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);
        }

        public CustomJsonResult EditSalePriceOnStore(string operater, string merchId, RopPrdProductEditSalePriceOnStore rop)
        {
            return BizFactory.ProductSku.AdjustStockSalePrice(operater, AppId.MERCH, merchId, rop.StoreId, rop.ProductSkuId, rop.ProductSkuSalePrice, rop.ProductSkuIsOffSell);
        }

        public CustomJsonResult Search(string operater, string merchId, string key)
        {
            var productSkus = CacheServiceFactory.Product.Search(merchId, "All", key);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", productSkus);
        }
    }
}
