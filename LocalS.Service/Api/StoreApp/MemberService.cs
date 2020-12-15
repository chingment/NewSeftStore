using LocalS.BLL;
using LocalS.Service.UI;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class MemberService : BaseDbContext
    {
        public CustomJsonResult GetPromSt(string operater, RupMemberGetPromSt rup)
        {

            var result = new CustomJsonResult();

            var ret = new RetMemberPromSt();

            var d_clientUser = CurrentDb.SysClientUser.Where(m => m.WxMpOpenId == rup.OpenId).FirstOrDefault();

            if (d_clientUser != null)
            {
                ret.MemberLevel = d_clientUser.MemberLevel;


                if (d_clientUser.MemberLevel == 1)
                {
                    ret.MemberExpireTip = string.Format("{0}天后过期", Convert.ToInt16((d_clientUser.MemberExpireTime.Value - DateTime.Now).TotalDays));
                }
                else if (d_clientUser.MemberLevel == 2)
                {
                    ret.MemberExpireTip = "永久";
                }


                var memberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.MerchId == rup.MerchId && m.Level == d_clientUser.MemberLevel).FirstOrDefault();
                if (memberLevelSt != null)
                {
                    ret.MemberTag = memberLevelSt.Name;
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetPayLevelSt(string operater, RupMemberGetPayLevelSt rup)
        {
            var merchId = "35129159f53249efabd4f0bc9a65810c";
            var result = new CustomJsonResult();

            var ret = new RetMemberPayLevelSt();

            ret.IsOptSaleOutlet = true;


            var d_saleOutlet = CurrentDb.SaleOutlet.Where(m => m.MerchId == merchId && m.Id == rup.SaleOutletId).FirstOrDefault();
            if (d_saleOutlet == null)
            {
                ret.CurSaleOutlet = new RetMemberPayLevelSt.SaleOutletModel { Id = "", TagName = "服务网点", TagTip = "地址信息", ContentBm = "请选择", ContentSm = "" };
            }
            else
            {
                ret.CurSaleOutlet = new RetMemberPayLevelSt.SaleOutletModel { Id = d_saleOutlet.Id, TagName = "服务网点", TagTip = "地址信息", ContentBm = d_saleOutlet.Name, ContentSm = d_saleOutlet.ContactAddress };
            }


            var d_memberLevelSts = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId).ToList();
            var d_memberFeeSts = CurrentDb.MemberFeeSt.Where(m => m.MerchId == merchId).ToList();

            var d_memberLevelSt_1 = d_memberLevelSts.Where(m => m.Level == 1).FirstOrDefault();

            if (d_memberLevelSt_1 != null)
            {
                LogUtil.Info("A1");
                var d_memberLevelSt_1_FeeSts = d_memberFeeSts.Where(m => m.LevelStId == d_memberLevelSt_1.Id).ToList();
                if (d_memberLevelSt_1_FeeSts.Count > 0)
                {
                    LogUtil.Info("A2");
                    var m_levelSt1 = new RetMemberPayLevelSt.LevelStModel();
                    m_levelSt1.Id = d_memberLevelSt_1.Id;
                    m_levelSt1.Tag = d_memberLevelSt_1.Tag;
                    m_levelSt1.Level = d_memberLevelSt_1.Level;
                    m_levelSt1.DetailsDes = d_memberLevelSt_1.DetailsDes;
                    m_levelSt1.CurFeeStIdx = 0;
                    foreach (var d_memberLevelSt_1_FeeSt in d_memberLevelSt_1_FeeSts)
                    {
                        LogUtil.Info("A3");
                        var m_feeSt = new RetMemberPayLevelSt.FeeStModel();
                        m_feeSt.Id = d_memberLevelSt_1_FeeSt.Id;
                        m_feeSt.Tag = d_memberLevelSt_1_FeeSt.Tag;
                        m_feeSt.FeeValue = new UI.FsText(d_memberLevelSt_1_FeeSt.FeeValue.ToF2Price(), "");
                        m_feeSt.DesPoints = d_memberLevelSt_1_FeeSt.DesPoints.ToJsonObject<List<FsField>>();
                        m_feeSt.LayoutWeight = d_memberLevelSt_1_FeeSt.LayoutWeight;

                        m_levelSt1.FeeSts.Add(m_feeSt);
                    }

                    LogUtil.Info("A4");
                    ret.LevelSt1 = m_levelSt1;


                }
            }

            var d_memberLevelSt_2 = d_memberLevelSts.Where(m => m.Level == 2).FirstOrDefault();

            if (d_memberLevelSt_2 != null)
            {
                var d_memberLevelSt_2_FeeSts = d_memberFeeSts.Where(m => m.LevelStId == d_memberLevelSt_2.Id).ToList();
                if (d_memberLevelSt_2_FeeSts.Count > 0)
                {
                    var m_levelSt2 = new RetMemberPayLevelSt.LevelStModel();
                    m_levelSt2.Id = d_memberLevelSt_2.Id;
                    m_levelSt2.Tag = d_memberLevelSt_2.Tag;
                    m_levelSt2.Level = d_memberLevelSt_2.Level;
                    m_levelSt2.DetailsDes = d_memberLevelSt_2.DetailsDes;
                    m_levelSt2.CurFeeStIdx = 0;
                    foreach (var d_memberLevelSt_2_FeeSt in d_memberLevelSt_2_FeeSts)
                    {

                        var m_feeSt = new RetMemberPayLevelSt.FeeStModel();
                        m_feeSt.Id = d_memberLevelSt_2_FeeSt.Id;
                        m_feeSt.Tag = d_memberLevelSt_2_FeeSt.Tag;
                        m_feeSt.FeeValue = new UI.FsText(d_memberLevelSt_2_FeeSt.FeeValue.ToF2Price(), "");
                        m_feeSt.DesPoints = d_memberLevelSt_2_FeeSt.DesPoints.ToJsonObject<List<FsField>>();
                        m_feeSt.LayoutWeight = d_memberLevelSt_2_FeeSt.LayoutWeight;

                        m_levelSt2.FeeSts.Add(m_feeSt);
                    }

                    ret.LevelSt2 = m_levelSt2;
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
