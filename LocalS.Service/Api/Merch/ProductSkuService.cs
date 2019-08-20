using LocalS.BLL;
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
    public class ProductSkuService : BaseDbContext
    {

        private List<TreeNode> GetKindTree(string id, List<ProductKind> productKinds)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_productKinds = productKinds.Where(t => t.PId == id).ToList();

            foreach (var p_productKind in p_productKinds)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_productKind.Id;
                treeNode.PId = p_productKind.PId;
                treeNode.Value = p_productKind.Id;
                treeNode.Label = p_productKind.Name;
                treeNode.IsDisabled = p_productKind.Depth <= 1 ? true : false;

                var children = GetKindTree(treeNode.Id, productKinds);
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
            var productKinds = CurrentDb.ProductKind.Where(m => m.MerchId == merchId).OrderBy(m => m.Priority).ToList();
            return GetKindTree(GuidUtil.Empty(), productKinds);
        }

        private List<TreeNode> GetSubjectTree(string id, List<ProductSubject> productSubjects)
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
            var productSubjects = CurrentDb.ProductSubject.Where(m => m.MerchId == merchId).OrderBy(m => m.Priority).ToList();
            return GetSubjectTree(GuidUtil.Empty(), productSubjects);
        }

        public CustomJsonResult GetList(string operater, string merchId, RupProductSkuGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.ProductSku
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, u.Name, u.CreateTime, u.SalePrice, u.ShowPrice, u.DispalyImgUrls });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    MainImgUrl = ImgSet.GetMain(item.DispalyImgUrls),
                    KindNames = "",
                    SubjectNames = "",
                    SalePrice = item.SalePrice.ToF2Price(),
                    ShowPrice = item.ShowPrice.ToF2Price(),
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
            var ret = new RetProductSkuInitAdd();


            ret.Kinds = GetKindTree(merchId);
            ret.Subjects = GetSubjectTree(merchId);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopProductSkuAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();


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

            if (rop.ShowPrice < rop.SalePrice)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "展示价格不能小于销售价");
            }

            using (TransactionScope ts = new TransactionScope())
            {

                var productSku = new ProductSku();
                productSku.Id = GuidUtil.New();
                productSku.MerchId = merchId;
                productSku.Name = rop.Name;
                productSku.BarCode = rop.BarCode;
                Encoding gb2312 = Encoding.GetEncoding("GB2312");
                string pinyinName = Pinyin.ConvertEncoding(productSku.Name, Encoding.UTF8, gb2312);
                string simpleCode = Pinyin.GetInitials(pinyinName, gb2312);
                productSku.SimpleCode = simpleCode;
                productSku.DispalyImgUrls = rop.DispalyImgUrls.ToJsonString();
                productSku.MainImgUrl = ImgSet.GetMain(productSku.DispalyImgUrls);
                productSku.ShowPrice = rop.ShowPrice;
                productSku.SalePrice = rop.SalePrice;
                productSku.DetailsDes = rop.DetailsDes;
                productSku.SpecDes = rop.SpecDes;
                productSku.BriefDes = rop.BriefDes;
                productSku.Creator = operater;
                productSku.CreateTime = DateTime.Now;

                if (rop.KindIds != null)
                {
                    productSku.KindIds = string.Join(",", rop.KindIds.ToArray());

                    foreach (var kindId in rop.KindIds)
                    {
                        var productSkuKind = new ProductSkuKind();
                        productSkuKind.Id = GuidUtil.New();
                        productSkuKind.ProductKindId = kindId;
                        productSkuKind.ProductSkuId = productSku.Id;
                        productSkuKind.Creator = operater;
                        productSkuKind.CreateTime = DateTime.Now;
                        CurrentDb.ProductSkuKind.Add(productSkuKind);
                    }
                }

                if (rop.SubjectIds != null)
                {
                    productSku.SubjectIds = string.Join(",", rop.SubjectIds.ToArray());

                    foreach (var subjectId in rop.SubjectIds)
                    {
                        var productSkuSubject = new ProductSkuSubject();
                        productSkuSubject.Id = GuidUtil.New();
                        productSkuSubject.ProductSubjectId = subjectId;
                        productSkuSubject.ProductSkuId = productSku.Id;
                        productSkuSubject.Creator = operater;
                        productSkuSubject.CreateTime = DateTime.Now;
                        CurrentDb.ProductSkuSubject.Add(productSkuSubject);
                    }
                }

                CurrentDb.ProductSku.Add(productSku);
                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
            }

            return result;
        }

        public CustomJsonResult InitEdit(string operater, string merchId, string productSkuId)
        {
            var ret = new RetProductSkuInitEdit();
            var productSku = CurrentDb.ProductSku.Where(m => m.MerchId == merchId && m.Id == productSkuId).FirstOrDefault();
            if (productSku != null)
            {
                ret.ProductSkuId = productSku.Id;
                ret.Name = productSku.Name;
                ret.SalePrice = productSku.SalePrice;
                ret.ShowPrice = productSku.ShowPrice;
                ret.DetailsDes = productSku.DetailsDes;
                ret.BriefDes = productSku.BriefDes;
                ret.KindIds = CurrentDb.ProductSkuKind.Where(m => m.ProductSkuId == productSkuId).Select(m => m.ProductKindId).ToList();
                ret.SubjectIds = CurrentDb.ProductSkuSubject.Where(m => m.ProductSkuId == productSkuId).Select(m => m.ProductSubjectId).ToList();
                ret.DispalyImgUrls = productSku.DispalyImgUrls.ToJsonObject<List<ImgSet>>();
                ret.Kinds = GetKindTree(merchId);
                ret.Subjects = GetSubjectTree(merchId);
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
        }

        public CustomJsonResult Edit(string operater, string merchId, RopProductSkuEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.ProductSkuId))
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

            if (rop.ShowPrice < rop.SalePrice)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "展示价格不能小于销售价");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var productSku = CurrentDb.ProductSku.Where(m => m.Id == rop.ProductSkuId).FirstOrDefault();

                productSku.Name = rop.Name;

                Encoding gb2312 = Encoding.GetEncoding("GB2312");
                string s = Pinyin.ConvertEncoding(productSku.Name, Encoding.UTF8, gb2312);
                string simpleCode = Pinyin.GetInitials(s, gb2312);
                productSku.SimpleCode = simpleCode;
                productSku.ShowPrice = rop.ShowPrice;
                productSku.SalePrice = rop.SalePrice;
                productSku.BriefDes = rop.BriefDes;
                productSku.DetailsDes = rop.DetailsDes;
                productSku.DispalyImgUrls = rop.DispalyImgUrls.ToJsonString();
                productSku.Mender = operater;
                productSku.MendTime = DateTime.Now;


                var productSkuKinds = CurrentDb.ProductSkuKind.Where(m => m.ProductSkuId == productSku.Id).ToList();

                foreach (var productSkuKind in productSkuKinds)
                {
                    CurrentDb.ProductSkuKind.Remove(productSkuKind);
                }

                if (rop.KindIds != null)
                {
                    foreach (var kindId in rop.KindIds)
                    {
                        var productKindSku = new ProductSkuKind();
                        productKindSku.Id = GuidUtil.New();
                        productKindSku.ProductKindId = kindId;
                        productKindSku.ProductSkuId = productSku.Id;
                        productKindSku.Creator = operater;
                        productKindSku.CreateTime = DateTime.Now;
                        CurrentDb.ProductSkuKind.Add(productKindSku);
                    }
                }

                var productSkuSubjects = CurrentDb.ProductSkuSubject.Where(m => m.ProductSkuId == productSku.Id).ToList();

                foreach (var productSkuSubject in productSkuSubjects)
                {
                    CurrentDb.ProductSkuSubject.Remove(productSkuSubject);
                }


                if (rop.SubjectIds != null)
                {
                    foreach (var subjectId in rop.SubjectIds)
                    {
                        var productSkuSubject = new ProductSkuSubject();
                        productSkuSubject.Id = GuidUtil.New();
                        productSkuSubject.ProductSubjectId = subjectId;
                        productSkuSubject.ProductSkuId = productSku.Id;
                        productSkuSubject.Creator = operater;
                        productSkuSubject.CreateTime = DateTime.Now;
                        CurrentDb.ProductSkuSubject.Add(productSkuSubject);
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
            }

            return result;
        }

    }
}
