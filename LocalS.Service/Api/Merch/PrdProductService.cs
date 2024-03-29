﻿using LocalS.BLL;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
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
        private List<TreeNode> GetKindTree(string id, List<PrdKind> prdKinds)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_PrdKinds = prdKinds.Where(t => t.PId == id).ToList();

            foreach (var p_productKind in p_PrdKinds)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_productKind.Id;
                treeNode.PId = p_productKind.PId;
                treeNode.Value = p_productKind.Id;
                treeNode.Label = p_productKind.Name;
                treeNode.IsDisabled = p_productKind.Depth <= 0 ? true : false;

                var children = GetKindTree(treeNode.Id, prdKinds);
                if (children != null)
                {
                    if (children.Count > 0)
                    {
                        treeNode.Children = new List<TreeNode>();
                        treeNode.Children.AddRange(children);
                    }
                }
                treeNodes.Add(treeNode);
            }

            return treeNodes;
        }

        public List<TreeNode> GetKindTree(string merchId)
        {
            var prdKinds = CurrentDb.PrdKind.Where(m => m.MerchId == merchId).OrderBy(m => m.Priority).ToList();
            return GetKindTree(GuidUtil.Empty(), prdKinds);
        }

        private List<TreeNode> GetSubjectTree(string id, List<PrdSubject> productSubjects)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_productSubjects = productSubjects.Where(t => t.PId == id).ToList();

            foreach (var p_productSubject in p_productSubjects)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_productSubject.Id;
                treeNode.PId = p_productSubject.PId;
                treeNode.Value = p_productSubject.Id;
                treeNode.Label = p_productSubject.Name;
                treeNode.IsDisabled = p_productSubject.Depth <= 0 ? true : false;
                var children = GetSubjectTree(treeNode.Id, productSubjects);
                treeNode.Children.AddRange(children);

                treeNodes.Add(treeNode);
            }

            return treeNodes;
        }

        public List<TreeNode> GetSubjectTree(string merchId)
        {
            var prdSubjects = CurrentDb.PrdSubject.Where(m => m.MerchId == merchId).OrderBy(m => m.Priority).ToList();
            return GetSubjectTree(GuidUtil.Empty(), prdSubjects);
        }

        public CustomJsonResult GetList(string operater, string merchId, RupPrdProductGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.PrdProduct
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, u.Name, u.BriefDes, u.CreateTime, u.DispalyImgUrls });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var prdKindNames = CurrentDb.PrdKind.Where(p => (from d in CurrentDb.PrdProductKind
                                                                 where d.PrdProductId == item.Id
                                                                 select d.PrdKindId).Contains(p.Id)).Select(m => m.Name).ToArray();
                string str_prdKindNames = prdKindNames.Length != 0 ? string.Join(",", prdKindNames) : "";

                var prdProductSkus = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == item.Id).ToList();

                var list_Sku = new List<object>();

                foreach (var prdProductSku in prdProductSkus)
                {
                    list_Sku.Add(new { Id = prdProductSku.Id, SalePrice = prdProductSku.SalePrice, SpecDes = prdProductSku.SpecDes });
                }

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    BriefDes = item.BriefDes,
                    MainImgUrl = ImgSet.GetMain(item.DispalyImgUrls),
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


            ret.Kinds = GetKindTree(merchId);
            ret.Subjects = GetSubjectTree(merchId);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopPrdProductAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            if (string.IsNullOrEmpty(rop.Name))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品名称不能为空");
            }

            if (rop.KindIds == null || rop.KindIds.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "至少选择一个商品模块");
            }

            if (rop.SubjectIds == null || rop.SubjectIds.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "至少选择一个商品栏目");
            }

            if (rop.DispalyImgUrls == null || rop.DispalyImgUrls.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "至少上传一张商品图片");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var prdProduct = new PrdProduct();
                prdProduct.Id = GuidUtil.New();
                prdProduct.MerchId = merchId;
                prdProduct.Name = rop.Name;
                prdProduct.BarCode = rop.BarCode;
                prdProduct.PinYinName = Pinyin.ConvertEncoding(prdProduct.Name, Encoding.UTF8, Encoding.GetEncoding("GB2312"));
                prdProduct.PinYinIndex = Pinyin.GetInitials(prdProduct.PinYinName, Encoding.GetEncoding("GB2312"));
                prdProduct.DispalyImgUrls = rop.DispalyImgUrls.ToJsonString();
                prdProduct.MainImgUrl = ImgSet.GetMain(prdProduct.DispalyImgUrls);
                prdProduct.DetailsDes = rop.DetailsDes;
                prdProduct.BriefDes = rop.BriefDes;
                prdProduct.Creator = operater;
                prdProduct.CreateTime = DateTime.Now;

                foreach (var sku in rop.Skus)
                {
                    var prdProductSku = new PrdProductSku();
                    prdProductSku.Id = GuidUtil.New();
                    prdProductSku.MerchId = prdProduct.MerchId;
                    prdProductSku.PrdProductId = prdProduct.Id;
                    prdProductSku.Name = prdProduct.Name;
                    prdProductSku.SpecDes = sku.SpecDes;
                    prdProductSku.SalePrice = sku.SalePrice;
                    prdProductSku.Creator = operater;
                    prdProductSku.CreateTime = DateTime.Now;
                    CurrentDb.PrdProductSku.Add(prdProductSku);
                }

                if (rop.KindIds != null)
                {
                    prdProduct.PrdKindIds = string.Join(",", rop.KindIds.ToArray());

                    foreach (var kindId in rop.KindIds)
                    {
                        var productSkuKind = new PrdProductKind();
                        productSkuKind.Id = GuidUtil.New();
                        productSkuKind.PrdKindId = kindId;
                        productSkuKind.PrdProductId = prdProduct.Id;
                        productSkuKind.Creator = operater;
                        productSkuKind.CreateTime = DateTime.Now;
                        CurrentDb.PrdProductKind.Add(productSkuKind);
                    }
                }

                if (rop.SubjectIds != null)
                {
                    prdProduct.PrdSubjectIds = string.Join(",", rop.SubjectIds.ToArray());

                    foreach (var subjectId in rop.SubjectIds)
                    {
                        var productSkuSubject = new PrdProductSubject();
                        productSkuSubject.Id = GuidUtil.New();
                        productSkuSubject.PrdSubjectId = subjectId;
                        productSkuSubject.PrdProductId = prdProduct.Id;
                        productSkuSubject.Creator = operater;
                        productSkuSubject.CreateTime = DateTime.Now;
                        CurrentDb.PrdProductSubject.Add(productSkuSubject);
                    }
                }

                CurrentDb.PrdProduct.Add(prdProduct);
                CurrentDb.SaveChanges();
                ts.Complete();

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
                ret.DetailsDes = prdProduct.DetailsDes;
                ret.BriefDes = prdProduct.BriefDes;
                ret.KindIds = CurrentDb.PrdProductKind.Where(m => m.PrdProductId == prdProductId).Select(m => m.PrdKindId).ToList();
                ret.SubjectIds = CurrentDb.PrdProductSubject.Where(m => m.PrdProductId == prdProductId).Select(m => m.PrdSubjectId).ToList();
                ret.DispalyImgUrls = prdProduct.DispalyImgUrls.ToJsonObject<List<ImgSet>>();
                ret.Kinds = GetKindTree(merchId);
                ret.Subjects = GetSubjectTree(merchId);

                var prdProductSkus = CurrentDb.PrdProductSku.Where(m => m.PrdProductId == prdProduct.Id).ToList();

                foreach (var prdProductSku in prdProductSkus)
                {
                    ret.Skus.Add(new RetPrdProductInitEdit.Sku { Id = prdProductSku.Id, SalePrice = prdProductSku.SalePrice, SpecDes = prdProductSku.SpecDes });
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

            if (rop.KindIds == null || rop.KindIds.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品模块分类不能为空");
            }

            if (rop.DispalyImgUrls == null || rop.DispalyImgUrls.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品图片不能为空");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var prdProduct = CurrentDb.PrdProduct.Where(m => m.Id == rop.Id).FirstOrDefault();

                prdProduct.Name = rop.Name;
                prdProduct.PinYinName = Pinyin.ConvertEncoding(prdProduct.Name, Encoding.UTF8, Encoding.GetEncoding("GB2312"));
                prdProduct.PinYinIndex = Pinyin.GetInitials(prdProduct.PinYinName, Encoding.GetEncoding("GB2312"));
                prdProduct.BriefDes = rop.BriefDes;
                prdProduct.DetailsDes = rop.DetailsDes;
                prdProduct.DispalyImgUrls = rop.DispalyImgUrls.ToJsonString();
                prdProduct.Mender = operater;
                prdProduct.MendTime = DateTime.Now;

                foreach (var sku in rop.Skus)
                {
                    var prdProductSku = CurrentDb.PrdProductSku.Where(m => m.Id == sku.Id).FirstOrDefault();
                    if (prdProductSku != null)
                    {
                        prdProductSku.SpecDes = sku.SpecDes;
                        prdProductSku.SalePrice = sku.SalePrice;
                        prdProductSku.Mender = operater;
                        prdProductSku.MendTime = DateTime.Now;
                    }
                }

                var prdProductKinds = CurrentDb.PrdProductKind.Where(m => m.PrdProductId == prdProduct.Id).ToList();

                foreach (var prdProductKind in prdProductKinds)
                {
                    CurrentDb.PrdProductKind.Remove(prdProductKind);
                }

                if (rop.KindIds != null)
                {
                    foreach (var kindId in rop.KindIds)
                    {
                        var prdProductKind = new PrdProductKind();
                        prdProductKind.Id = GuidUtil.New();
                        prdProductKind.PrdKindId = kindId;
                        prdProductKind.PrdProductId = prdProduct.Id;
                        prdProductKind.Creator = operater;
                        prdProductKind.CreateTime = DateTime.Now;
                        CurrentDb.PrdProductKind.Add(prdProductKind);
                    }
                }

                var prdProductSubjects = CurrentDb.PrdProductSubject.Where(m => m.PrdProductId == prdProduct.Id).ToList();

                foreach (var prdProductSubject in prdProductSubjects)
                {
                    CurrentDb.PrdProductSubject.Remove(prdProductSubject);
                }


                if (rop.SubjectIds != null)
                {
                    foreach (var subjectId in rop.SubjectIds)
                    {
                        var prdProductSubject = new PrdProductSubject();
                        prdProductSubject.Id = GuidUtil.New();
                        prdProductSubject.PrdSubjectId = subjectId;
                        prdProductSubject.PrdProductId = prdProduct.Id;
                        prdProductSubject.Creator = operater;
                        prdProductSubject.CreateTime = DateTime.Now;
                        CurrentDb.PrdProductSubject.Add(prdProductSubject);
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;
        }

    }
}
