using LocalS.BLL;
using LocalS.BLL.UI;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Admin
{
    public class AdminOrgService : BaseService
    {
        private List<TreeNode> GetOrgTree(string id, List<SysOrg> sysOrgs)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_sysOrgs = sysOrgs.Where(t => t.PId == id).ToList();

            foreach (var p_sysOrg in p_sysOrgs)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_sysOrg.Id;
                treeNode.PId = p_sysOrg.PId;
                treeNode.Label = p_sysOrg.Name;
                treeNode.Description = p_sysOrg.Description;
                if (p_sysOrg.Depth == 0)
                {
                    treeNode.ExtAttr = new { CanDelete = false, CanEdit = false, CanAdd = true };
                }
                else
                {
                    treeNode.ExtAttr = new { CanDelete = true, CanEdit = true, CanAdd = true };
                }

                var children = GetOrgTree(p_sysOrg.Id, sysOrgs);
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

        public CustomJsonResult GetList(string operater, RupAdminOrgGetList rup)
        {
            var result = new CustomJsonResult();

            var sysOrgs = CurrentDb.SysOrg.Where(m => m.BelongSite == Enumeration.BelongSite.Admin).OrderBy(m => m.Priority).ToList();

            var topOrg = sysOrgs.Where(m => m.Depth == 0).FirstOrDefault();

            var orgTree = GetOrgTree(topOrg.PId, sysOrgs);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", orgTree);

            return result;

        }

        public CustomJsonResult InitAdd(string operater, string pOrgId)
        {
            var result = new CustomJsonResult();

            var ret = new RetAdminOrgInitAdd();

            var sysOrg = CurrentDb.SysOrg.Where(m => m.Id == pOrgId).FirstOrDefault();

            if (sysOrg != null)
            {
                ret.PId = sysOrg.Id;
                ret.PName = sysOrg.Name;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, RopAdminOrgAdd rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var isExists = CurrentDb.SysOrg.Where(m => m.Name == rop.Name).FirstOrDefault();
                if (isExists != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该名称已经存在");
                }

                var sysOrg = new SysOrg();
                sysOrg.Id = IdWorker.Build(IdType.NewGuid);
                sysOrg.Name = rop.Name;
                sysOrg.Description = rop.Description;
                sysOrg.PId = rop.PId;
                sysOrg.BelongSite = Enumeration.BelongSite.Admin;
                sysOrg.ReferenceId = IdWorker.Build(IdType.EmptyGuid);
                sysOrg.Depth = 0;
                sysOrg.CreateTime = DateTime.Now;
                sysOrg.Creator = operater;
                CurrentDb.SysOrg.Add(sysOrg);

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult InitEdit(string operater, string orgId)
        {
            var result = new CustomJsonResult();

            var ret = new RetAdminOrgInitEdit();

            var sysOrg = CurrentDb.SysOrg.Where(m => m.Id == orgId).FirstOrDefault();

            if (sysOrg != null)
            {
                ret.Id = sysOrg.Id;
                ret.Name = sysOrg.Name;
                ret.Description = sysOrg.Description;

                var p_sysOrg = CurrentDb.SysOrg.Where(m => m.Id == sysOrg.PId).FirstOrDefault();

                if (p_sysOrg != null)
                {
                    ret.PId = p_sysOrg.Id;
                    ret.PName = p_sysOrg.Name;
                }
                else
                {
                    ret.PName = "/";
                }
            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, RopAdminOrgEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var sysOrg = CurrentDb.SysOrg.Where(m => m.Id == rop.Id).FirstOrDefault();
                if (sysOrg == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }
                sysOrg.Name = rop.Name;
                sysOrg.Description = rop.Description;
                sysOrg.MendTime = DateTime.Now;
                sysOrg.Mender = operater;


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult Sort(string operater, RopAdminOrgSort rop)
        {

            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var sysOrgs = CurrentDb.SysOrg.Where(m => rop.Ids.Contains(m.Id)).ToList();

                for (int i = 0; i < sysOrgs.Count; i++)
                {
                    int priority = rop.Ids.IndexOf(sysOrgs[i].Id);
                    sysOrgs[i].Priority = priority;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }
    }
}
