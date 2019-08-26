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
   public class PrdSubjectService : BaseDbContext
    {
        private List<TreeNode> GetTree(string id, List<PrdSubject> productSubjects)
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

        public CustomJsonResult GetList(string operater, string merchId, RupPrdSubjectGetList rup)
        {
            var result = new CustomJsonResult();

            var prdSubjects = CurrentDb.PrdSubject.Where(m => m.MerchId == merchId).OrderBy(m => m.Priority).ToList();

            var topPrdSubject = prdSubjects.Where(m => m.Depth == 0).FirstOrDefault();

            var tree = GetTree(topPrdSubject.PId, prdSubjects);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", tree);

            return result;

        }

        public CustomJsonResult InitAdd(string operater, string merchId, string pSubjectId)
        {
            var result = new CustomJsonResult();

            var ret = new RetPrdSubjectInitAdd();

            var prdSubject = CurrentDb.PrdSubject.Where(m => m.Id == pSubjectId).FirstOrDefault();

            if (prdSubject != null)
            {
                ret.PId = prdSubject.Id;
                ret.PName = prdSubject.Name;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopPrdSubjectAdd rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var isExists = CurrentDb.PrdSubject.Where(m => m.Name == rop.Name).FirstOrDefault();
                if (isExists != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该名称已经存在");
                }

                var pPrdSubject = CurrentDb.PrdSubject.Where(m => m.Id == rop.PId).FirstOrDefault();
                if (pPrdSubject == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到上级节点");
                }

                var prdSubject = new PrdSubject();
                prdSubject.Id = GuidUtil.New();
                prdSubject.PId = rop.PId;
                prdSubject.Name = rop.Name;
                prdSubject.IconImgUrl = rop.IconImgUrl;
                prdSubject.MainImgUrl = rop.MainImgUrl;
                prdSubject.MerchId = merchId;
                prdSubject.Description = rop.Description;
                prdSubject.Depth = pPrdSubject.Depth + 1;
                prdSubject.CreateTime = DateTime.Now;
                prdSubject.Creator = operater;
                CurrentDb.PrdSubject.Add(prdSubject);

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

            var prdSubject = CurrentDb.PrdSubject.Where(m => m.Id == orgId).FirstOrDefault();

            if (prdSubject != null)
            {
                ret.Id = prdSubject.Id;
                ret.Name = prdSubject.Name;
                ret.IconImgUrl = prdSubject.IconImgUrl;
                ret.MainImgUrl = prdSubject.MainImgUrl;


                ret.Description = prdSubject.Description;

                var p_PrdSubject = CurrentDb.PrdSubject.Where(m => m.Id == prdSubject.PId).FirstOrDefault();

                if (p_PrdSubject != null)
                {
                    ret.PId = p_PrdSubject.Id;
                    ret.PName = p_PrdSubject.Name;
                }
                else
                {
                    ret.PName = "/";
                }
            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopPrdSubjectEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var prdSubject = CurrentDb.PrdSubject.Where(m => m.Id == rop.Id).FirstOrDefault();
                if (prdSubject == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }
                prdSubject.Name = rop.Name;
                prdSubject.IconImgUrl = rop.IconImgUrl;
                prdSubject.MainImgUrl = rop.MainImgUrl;
                prdSubject.Description = rop.Description;
                prdSubject.MendTime = DateTime.Now;
                prdSubject.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult Sort(string operater, string merchId, RopPrdSubjectSort rop)
        {

            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var productSubjects = CurrentDb.PrdSubject.Where(m => rop.Ids.Contains(m.Id)).ToList();

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
