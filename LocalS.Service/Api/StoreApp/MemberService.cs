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
        public CustomJsonResult GetPayLevelSt(string operater)
        {
            var result = new CustomJsonResult();

            var ret = new RetMemberPayLevelSt();

            var d_memberLevelSts = CurrentDb.MemberLevelSt.Where(m => m.MerchId == "35129159f53249efabd4f0bc9a65810c").ToList();
            var d_memberFeeSts = CurrentDb.MemberFeeSt.Where(m => m.MerchId == "35129159f53249efabd4f0bc9a65810c").ToList();

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
                        m_feeSt.DesPoints = d_memberLevelSt_2_FeeSt.ToJsonObject<List<FsField>>();
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
