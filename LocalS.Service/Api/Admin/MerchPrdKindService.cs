﻿using LocalS.BLL;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Admin
{
    public class MerchPrdKindService : BaseDbContext
    {
        private List<TreeNode> GetTree(int id, List<PrdKind> prdKinds)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_prdKinds = prdKinds.Where(t => t.PId == id).ToList();

            foreach (var p_prdKind in p_prdKinds)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_prdKind.Id.ToString();
                treeNode.PId = p_prdKind.PId.ToString();
                treeNode.Label = p_prdKind.Name;
                treeNode.Description = p_prdKind.Description;
                treeNode.Depth = p_prdKind.Depth;

                if (p_prdKind.Depth == 0)
                {
                    treeNode.ExtAttr = new { CanDelete = false, CanAdd = true, CanEdit = false };
                }
                else
                {
                    if (p_prdKind.Depth >= 3)
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

        public CustomJsonResult GetList(string operater, RupPrdKindGetList rup)
        {
            var result = new CustomJsonResult();

            var prdKinds = CurrentDb.PrdKind.OrderBy(m => m.Priority).ToList();

            prdKinds.Add(new PrdKind { Id = 1, Name = "商品分类", Depth = 0, IsDelete = false, Priority = 0, CreateTime = DateTime.Now, Creator = "" });

            var toPrdKind = prdKinds.Where(m => m.Depth == 0).FirstOrDefault();

            var tree = GetTree(toPrdKind.PId, prdKinds);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", tree);

            return result;

        }

        public CustomJsonResult InitAdd(string operater, int pKindId)
        {
            var result = new CustomJsonResult();

            var ret = new RetPrdKindInitAdd();

            var prdKind = CurrentDb.PrdKind.Where(m => m.Id == pKindId).FirstOrDefault();

            if (prdKind == null)
            {
                ret.PId = 1;
                ret.PName = "/";
            }
            else
            {
                ret.PId = prdKind.Id;
                ret.PName = prdKind.Name;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, RopPrdKindAdd rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                int depth = 0;
                var pPrdKind = CurrentDb.PrdKind.Where(m => m.Id == rop.PId).FirstOrDefault();
                if (pPrdKind == null && rop.PId != 1)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到上级节点");
                }


                if (pPrdKind != null)
                {
                    depth += pPrdKind.Depth + 1;
                }
                else
                {
                    depth = 1;
                }

                var count = CurrentDb.PrdKind.Where(m => m.PId == rop.PId).Count();

                if (count == 99)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "最大子节点99级");
                }

                int id = int.Parse(rop.PId.ToString() + (count + 1).ToString().PadLeft(2, '0'));

                LogUtil.Info("id:" + id.ToString());

                var productKind = new PrdKind();
                productKind.Id = id;
                productKind.PId = rop.PId;
                productKind.Name = rop.Name;
                productKind.IconImgUrl = rop.IconImgUrl;
                productKind.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                productKind.MainImgUrl = ImgSet.GetMain_O(productKind.DisplayImgUrls);
                productKind.Description = rop.Description;
                productKind.Depth = depth;
                productKind.CreateTime = DateTime.Now;
                productKind.Creator = operater;
                CurrentDb.PrdKind.Add(productKind);

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult InitEdit(string operater, int id)
        {
            var result = new CustomJsonResult();

            var ret = new RetPrdKindInitEdit();

            var prdKind = CurrentDb.PrdKind.Where(m => m.Id == id).FirstOrDefault();

            if (prdKind != null)
            {
                ret.Id = prdKind.Id;
                ret.Name = prdKind.Name;
                ret.DisplayImgUrls = prdKind.DisplayImgUrls.ToJsonObject<List<ImgSet>>();


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

        public CustomJsonResult Edit(string operater, RopPrdKindEdit rop)
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
                prdKind.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                prdKind.MainImgUrl = ImgSet.GetMain_O(prdKind.DisplayImgUrls);
                prdKind.Description = rop.Description;
                prdKind.MendTime = DateTime.Now;
                prdKind.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult Sort(string operater, RopPrdKindSort rop)
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
