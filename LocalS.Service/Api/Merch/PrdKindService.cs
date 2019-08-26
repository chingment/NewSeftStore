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
    public class PrdKindService : BaseDbContext
    {
        private List<TreeNode> GetTree(string id, List<PrdKind> productKinds)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_productKinds = productKinds.Where(t => t.PId == id).ToList();

            foreach (var p_productKind in p_productKinds)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_productKind.Id;
                treeNode.PId = p_productKind.PId;
                treeNode.Label = p_productKind.Name;
                treeNode.Description = p_productKind.Description;
                treeNode.Depth = p_productKind.Depth;

                if (p_productKind.Depth == 0)
                {
                    treeNode.ExtAttr = new { CanDelete = false, CanAdd = true };
                }
                else
                {
                    if (p_productKind.Depth >= 3)
                    {
                        treeNode.ExtAttr = new { CanDelete = true, CanAdd = false };
                    }
                    else
                    {
                        treeNode.ExtAttr = new { CanDelete = true, CanAdd = true };
                    }
                }

                var children = GetTree(p_productKind.Id, productKinds);
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

        public CustomJsonResult GetList(string operater, string merchId, RupPrdKindGetList rup)
        {
            var result = new CustomJsonResult();

            var prdKinds = CurrentDb.PrdKind.Where(m => m.MerchId == merchId).OrderBy(m => m.Priority).ToList();

            var toPrdKind = prdKinds.Where(m => m.Depth == 0).FirstOrDefault();

            var tree = GetTree(toPrdKind.PId, prdKinds);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", tree);

            return result;

        }

        public CustomJsonResult InitAdd(string operater, string merchId, string pKindId)
        {
            var result = new CustomJsonResult();

            var ret = new RetPrdKindInitAdd();

            var prdKind = CurrentDb.PrdKind.Where(m => m.Id == pKindId).FirstOrDefault();

            if (prdKind != null)
            {
                ret.PId = prdKind.Id;
                ret.PName = prdKind.Name;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopPrdKindAdd rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var isExists = CurrentDb.PrdKind.Where(m => m.Name == rop.Name).FirstOrDefault();
                if (isExists != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该名称已经存在");
                }

                var pPrdKind = CurrentDb.PrdKind.Where(m => m.Id == rop.PId).FirstOrDefault();
                if (pPrdKind == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到上级节点");
                }

                var productKind = new PrdKind();
                productKind.Id = GuidUtil.New();
                productKind.PId = rop.PId;
                productKind.Name = rop.Name;
                productKind.IconImgUrl = rop.IconImgUrl;
                productKind.MainImgUrl = rop.MainImgUrl;
                productKind.MerchId = merchId;
                productKind.Description = rop.Description;
                productKind.Depth = pPrdKind.Depth + 1;
                productKind.CreateTime = DateTime.Now;
                productKind.Creator = operater;
                CurrentDb.PrdKind.Add(productKind);

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult InitEdit(string operater, string merchId, string orgId)
        {
            var result = new CustomJsonResult();

            var ret = new RetPrdKindInitEdit();

            var prdKind = CurrentDb.PrdKind.Where(m => m.Id == orgId).FirstOrDefault();

            if (prdKind != null)
            {
                ret.Id = prdKind.Id;
                ret.Name = prdKind.Name;
                ret.IconImgUrl = prdKind.IconImgUrl;
                ret.MainImgUrl = prdKind.MainImgUrl;

 
                ret.Description = prdKind.Description;

                var p_ProductKind = CurrentDb.PrdKind.Where(m => m.Id == prdKind.PId).FirstOrDefault();

                if (p_ProductKind != null)
                {
                    ret.PId = p_ProductKind.Id;
                    ret.PName = p_ProductKind.Name;
                }
                else
                {
                    ret.PName = "/";
                }
            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopPrdKindEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var prdKind = CurrentDb.PrdKind.Where(m => m.Id == rop.Id).FirstOrDefault();
                if (prdKind == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }
                prdKind.Name = rop.Name;
                prdKind.IconImgUrl = rop.IconImgUrl;
                prdKind.MainImgUrl = rop.MainImgUrl;
                prdKind.Description = rop.Description;
                prdKind.MendTime = DateTime.Now;
                prdKind.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult Sort(string operater, string merchId, RopPrdKindSort rop)
        {

            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var prdKinds = CurrentDb.PrdKind.Where(m => rop.Ids.Contains(m.Id)).ToList();

                for (int i = 0; i < prdKinds.Count; i++)
                {
                    int priority = rop.Ids.IndexOf(prdKinds[i].Id);
                    prdKinds[i].Priority = priority;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }
    }
}
