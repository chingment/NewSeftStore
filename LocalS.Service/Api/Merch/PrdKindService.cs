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
        private List<TreeNode> GetTree(string id, List<PrdKind> prdKinds)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_prdKinds = prdKinds.Where(t => t.PId == id).ToList();

            foreach (var p_prdKind in p_prdKinds)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_prdKind.Id;
                treeNode.PId = p_prdKind.PId;
                treeNode.Label = p_prdKind.Name;
                treeNode.Description = p_prdKind.Description;
                treeNode.Depth = p_prdKind.Depth;

                if (p_prdKind.Depth == 0)
                {
                    treeNode.ExtAttr = new { CanDelete = false, CanAdd = true, CanEdit = false };
                }
                else
                {
                    if (p_prdKind.Depth >= 1)
                    {
                        treeNode.ExtAttr = new { CanDelete = true, CanAdd = false, CanEdit = true };
                    }
                    else
                    {
                        treeNode.ExtAttr = new { CanDelete = true, CanAdd = true, CanEdit = true };
                    }
                }

                var children = GetTree(p_prdKind.Id, prdKinds);
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

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

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
                productKind.DispalyImgUrls = rop.DispalyImgUrls.ToJsonString();
                productKind.MainImgUrl = ImgSet.GetMain(productKind.DispalyImgUrls);
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
                ret.DispalyImgUrls = prdKind.DispalyImgUrls.ToJsonObject<List<ImgSet>>();


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



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

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
                prdKind.DispalyImgUrls = rop.DispalyImgUrls.ToJsonString();
                prdKind.MainImgUrl = ImgSet.GetMain(prdKind.DispalyImgUrls);
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
