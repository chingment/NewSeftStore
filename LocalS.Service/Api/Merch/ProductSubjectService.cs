using LocalS.BLL;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
   public class ProductSubjectService : BaseDbContext
    {
        private List<TreeNode> GetTree(string id, List<ProductSubject> productSubjects)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_productSubjects = productSubjects.Where(t => t.PId == id).ToList();

            foreach (var p_productSubject in p_productSubjects)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_productSubject.Id;
                treeNode.PId = p_productSubject.PId;
                treeNode.Label = p_productSubject.Name;
                treeNode.Description = p_productSubject.Description;
                treeNode.Depth = p_productSubject.Depth;

                if (p_productSubject.Depth == 0)
                {
                    treeNode.ExtAttr = new { CanDelete = false, CanAdd = true };
                }
                else
                {
                    if (p_productSubject.Depth >= 3)
                    {
                        treeNode.ExtAttr = new { CanDelete = true, CanAdd = false };
                    }
                    else
                    {
                        treeNode.ExtAttr = new { CanDelete = true, CanAdd = true };
                    }
                }

                var children = GetTree(p_productSubject.Id, p_productSubjects);
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

        public CustomJsonResult GetList(string operater, string merchId, RupProductSubjectGetList rup)
        {
            var result = new CustomJsonResult();

            var productSubjects = CurrentDb.ProductSubject.Where(m => m.MerchId == merchId).OrderBy(m => m.Priority).ToList();

            var topProductSubject = productSubjects.Where(m => m.Depth == 0).FirstOrDefault();

            var tree = GetTree(topProductSubject.PId, productSubjects);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", tree);

            return result;

        }

        public CustomJsonResult InitAdd(string operater, string merchId, string pSubjectId)
        {
            var result = new CustomJsonResult();

            var ret = new RetProductSubjectInitAdd();

            var productSubject = CurrentDb.ProductSubject.Where(m => m.Id == pSubjectId).FirstOrDefault();

            if (productSubject != null)
            {
                ret.PId = productSubject.Id;
                ret.PName = productSubject.Name;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopProductSubjectAdd rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var isExists = CurrentDb.ProductSubject.Where(m => m.Name == rop.Name).FirstOrDefault();
                if (isExists != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该名称已经存在");
                }

                var pProductSubject = CurrentDb.ProductSubject.Where(m => m.Id == rop.PId).FirstOrDefault();
                if (pProductSubject == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到上级节点");
                }

                var productSubject = new ProductSubject();
                productSubject.Id = GuidUtil.New();
                productSubject.PId = rop.PId;
                productSubject.Name = rop.Name;
                productSubject.IconImgUrl = rop.IconImgUrl;
                productSubject.MainImgUrl = rop.MainImgUrl;
                productSubject.MerchId = merchId;
                productSubject.Description = rop.Description;
                productSubject.Depth = pProductSubject.Depth + 1;
                productSubject.CreateTime = DateTime.Now;
                productSubject.Creator = operater;
                CurrentDb.ProductSubject.Add(productSubject);

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult InitEdit(string operater, string merchId, string orgId)
        {
            var result = new CustomJsonResult();

            var ret = new RetProductSubjectInitEdit();

            var productSubject = CurrentDb.ProductSubject.Where(m => m.Id == orgId).FirstOrDefault();

            if (productSubject != null)
            {
                ret.Id = productSubject.Id;
                ret.Name = productSubject.Name;
                ret.IconImgUrl = productSubject.IconImgUrl;
                ret.MainImgUrl = productSubject.MainImgUrl;


                ret.Description = productSubject.Description;

                var p_ProductSubject = CurrentDb.ProductSubject.Where(m => m.Id == productSubject.PId).FirstOrDefault();

                if (p_ProductSubject != null)
                {
                    ret.PId = p_ProductSubject.Id;
                    ret.PName = p_ProductSubject.Name;
                }
                else
                {
                    ret.PName = "/";
                }
            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopProductSubjectEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var productSubject = CurrentDb.ProductSubject.Where(m => m.Id == rop.Id).FirstOrDefault();
                if (productSubject == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }
                productSubject.Name = rop.Name;
                productSubject.IconImgUrl = rop.IconImgUrl;
                productSubject.MainImgUrl = rop.MainImgUrl;
                productSubject.Description = rop.Description;
                productSubject.MendTime = DateTime.Now;
                productSubject.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult Sort(string operater, string merchId, RopProductSubjectSort rop)
        {

            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var productSubjects = CurrentDb.ProductSubject.Where(m => rop.Ids.Contains(m.Id)).ToList();

                for (int i = 0; i < productSubjects.Count; i++)
                {
                    int priority = rop.Ids.IndexOf(productSubjects[i].Id);
                    productSubjects[i].Priority = priority;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }
    }
}
