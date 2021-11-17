using LocalS.BLL;
using LocalS.Service.UI;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class OrgService : BaseService
    {
        private List<TreeNode> GetOrgTree(string id, List<MerchOrg> orgs)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            var p_Orgs = orgs.Where(t => t.PId == id).ToList();

            foreach (var p_Org in p_Orgs)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Id = p_Org.Id;
                treeNode.PId = p_Org.PId;
                treeNode.Label = p_Org.Name;
                treeNode.Description = p_Org.Description;
                if (p_Org.Depth == 0)
                {
                    treeNode.ExtAttr = new { CanDelete = false, CanEdit = false, CanAdd = true };
                }
                else
                {
                    treeNode.ExtAttr = new { CanDelete = true, CanEdit = true, CanAdd = true };
                }

                var children = GetOrgTree(p_Org.Id, orgs);
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

        public CustomJsonResult GetList(string operater, string merchId, RupOrgGetList rup)
        {
            var result = new CustomJsonResult();

            var d_Orgs = CurrentDb.MerchOrg.Where(m => m.MerchId == merchId).OrderBy(m => m.Priority).ToList();

            var topOrg = d_Orgs.Where(m => m.Depth == 0).FirstOrDefault();

            var orgTree = GetOrgTree(topOrg.PId, d_Orgs);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", orgTree);

            return result;

        }

        public CustomJsonResult InitAdd(string operater, string merchId, string pOrgId)
        {
            var result = new CustomJsonResult();

            var ret = new object();

            var d_Org = CurrentDb.MerchOrg.Where(m => m.Id == pOrgId).FirstOrDefault();

            if (d_Org != null)
            {
                ret = new { PId = d_Org.Id, PName = d_Org.Name };
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopOrgAdd rop)
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

        public CustomJsonResult InitEdit(string operater, string merchId, string orgId)
        {
            var result = new CustomJsonResult();

            var ret = new object();

            var d_Org = CurrentDb.MerchOrg.Where(m => m.Id == orgId).FirstOrDefault();

            if (d_Org != null)
            {
                string id = d_Org.Id;
                string name = d_Org.Name;
                string description = d_Org.Description;
                string pId = "";
                string pName = "";
                var l_Org = CurrentDb.MerchOrg.Where(m => m.Id == d_Org.PId).FirstOrDefault();

                if (l_Org != null)
                {
                    pId = l_Org.Id;
                    pName = l_Org.Name;
                }
                else
                {
                    pName = "/";
                }

                ret = new { PId = d_Org.Id, PName = d_Org.Name, Id = id, Name = name, Description = description };
            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopOrgEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var d_Org = CurrentDb.MerchOrg.Where(m => m.MerchId == merchId && m.Id == rop.Id).FirstOrDefault();
                if (d_Org == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }
                d_Org.Name = rop.Name;
                d_Org.Description = rop.Description;
                d_Org.MendTime = DateTime.Now;
                d_Org.Mender = operater;


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }

        public CustomJsonResult Sort(string operater, string merchId, RopOrgSort rop)
        {

            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_Orgs = CurrentDb.MerchOrg.Where(m => m.MerchId == merchId && rop.Ids.Contains(m.Id)).ToList();

                for (int i = 0; i < d_Orgs.Count; i++)
                {
                    int priority = rop.Ids.IndexOf(d_Orgs[i].Id);
                    d_Orgs[i].Priority = priority;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;

        }
    }
}
