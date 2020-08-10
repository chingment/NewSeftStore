using System;
using LocalS.BLL;
using LocalS.Service.UI;
using Lumos;
using Lumos.DbRelay;
using Lumos.Session;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LocalS.Service.Api.Merch
{
    public class HomeService
    {
        public CustomJsonResult GetIndexPageData(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var retRptTodaySummary = new RetRptTodaySummary();

            StringBuilder sql1 = new StringBuilder();
            sql1.Append(" select count(*) from Store where IsDelete=0 and merchId='" + merchId + "' ");


            int storeCount = int.Parse(DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql1.ToString()).ToString());

            StringBuilder sql2 = new StringBuilder();
            sql2.Append(" select count(*) from MerchMachine where IsStopUse=0 and merchId='" + merchId + "' ");

            int machineCount = int.Parse(DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql2.ToString()).ToString());



            StringBuilder sql3 = new StringBuilder();
            sql3.Append(" select sum(ChargeAmount) from [Order] where payStatus='3' and merchId='" + merchId + "' ");

            string sumTradeAmount = DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql3.ToString()).ToString();

            StringBuilder sql4 = new StringBuilder();
            sql4.Append("  select count(*) from SellChannelStock where SellQuantity=0 or(SumQuantity < MaxQuantity) and merchId='" + merchId + "' ");

            string replenishCount = DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql4.ToString()).ToString();


            StringBuilder sql5 = new StringBuilder();
            sql5.Append(" select count(*) from Machine where (ExIsHas=1 or  datediff(MINUTE,LastRequestTime,GETDATE())>15) and CurUseMerchId='" + merchId + "' ");

            int machineExCount = int.Parse(DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql5.ToString()).ToString());

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { storeCount = storeCount, machineCount = machineCount, machineExCount = machineExCount, sumTradeAmount = sumTradeAmount, replenishCount = replenishCount });


            return result;
        }

        public CustomJsonResult GetTodaySummary(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var retRptTodaySummary = new RetRptTodaySummary();

            StringBuilder sql = new StringBuilder();
            sql.Append(" select COUNT(*) from dbo.[Order] where ReceiveMode=3 and ExIsHappen=1 and ExIsHandle=0 and merchId='" + merchId + "' ");


            int sumExHdByMachineSelfTake = int.Parse(DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql.ToString()).ToString());

            retRptTodaySummary.SumExHdByMachineSelfTake = sumExHdByMachineSelfTake;




            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", retRptTodaySummary);


            return result;
        }

        public CustomJsonResult Get7DayGmv(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var retRpt7DayGmvRl = new RetRpt7DayGmvRl();

            StringBuilder sql = new StringBuilder();
            sql.Append(" select a1.datef,isnull(sumCount,0) as sumCount, isnull(sumTradeAmount,0) as  sumTradeAmount from (  ");
            for (int i = 0; i < 7; i++)
            {
                string datef = DateTime.Now.AddDays(double.Parse((-i).ToString())).ToUnifiedFormatDate();

                sql.Append(" select '" + datef + "' datef union");
            }
            sql.Remove(sql.Length - 5, 5);

            sql.Append(" ) a1 left join ");
            sql.Append(" (    select datef, sum(sumCount) as sumCount ,sum(sumTradeAmount) as sumTradeAmount from ( select CONVERT(varchar(100),PayedTime, 23) datef,count(*) as sumCount ,sum(ChargeAmount) as sumTradeAmount from [Order] WITH(NOLOCK)  where  merchId='" + merchId + "' and PayStatus='3' and DateDiff(dd, PayedTime, getdate()) <= 7  group by PayedTime ) tb  group by datef ) b1 ");
            sql.Append(" on  a1.datef=b1.datef  ");
            sql.Append(" order by a1.datef desc  ");



            DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0];
            for (int r = 0; r < dtData.Rows.Count; r++)
            {
                var rpt7DayGmvRlModel = new Rpt7DayGmvRlModel();

                rpt7DayGmvRlModel.Datef = dtData.Rows[r]["datef"].ToString();
                rpt7DayGmvRlModel.SumCount = dtData.Rows[r]["sumCount"].ToString();
                rpt7DayGmvRlModel.SumTradeAmount = dtData.Rows[r]["sumTradeAmount"].ToString();

                retRpt7DayGmvRl.Days.Add(rpt7DayGmvRlModel);
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", retRpt7DayGmvRl);


            return result;
        }

        public CustomJsonResult GetTodayStoreGmvRl(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var retRptStoreGmvRl = new RetRptStoreGmvRl();

            string startTime = CommonUtil.ConverToStartTime(DateTime.Now.ToUnifiedFormatDateTime()).ToUnifiedFormatDateTime();
            string endTime = CommonUtil.ConverToEndTime(DateTime.Now.ToUnifiedFormatDateTime()).ToUnifiedFormatDateTime();

            StringBuilder sql = new StringBuilder("  select top 10 name,isnull(sumQuantity,0) as sumQuantity,isnull(sumTradeAmount,0) as sumTradeAmount from Store a left join ( ");
            sql.Append("  select StoreId ,sum(Quantity) as sumQuantity,sum(ChargeAmount) as sumTradeAmount  from [Order] WITH(NOLOCK) where merchId='" + merchId + "' and PayStatus='3' and PayedTime>='" + startTime + "'  and PayedTime<='" + endTime + "'  group by StoreId  )  b on a.id=b.storeId ");
            sql.Append("  where merchId='" + merchId + "' order by sumTradeAmount desc ");
            DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0];
            for (int r = 0; r < dtData.Rows.Count; r++)
            {
                var rptStoreGmvRlModel = new RptStoreGmvRlModel();

                rptStoreGmvRlModel.Name = dtData.Rows[r]["name"].ToString();
                rptStoreGmvRlModel.SumQuantity = dtData.Rows[r]["sumQuantity"].ToString();
                rptStoreGmvRlModel.SumTradeAmount = dtData.Rows[r]["sumTradeAmount"].ToString();

                retRptStoreGmvRl.Stores.Add(rptStoreGmvRlModel);
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", retRptStoreGmvRl);


            return result;
        }

        public CustomJsonResult GetStoreGmvRl(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var retRptStoreGmvRl = new RetRptStoreGmvRl();

            StringBuilder sql = new StringBuilder("  select top 10 name,isnull(sumQuantity,0) as sumQuantity,isnull(sumTradeAmount,0) as sumTradeAmount from Store a left join ( ");
            sql.Append("  select StoreId ,sum(Quantity) as sumQuantity,sum(chargeAmount) as sumTradeAmount  from [Order] WITH(NOLOCK) where merchId='" + merchId + "' and PayStatus='3' group by StoreId  )  b on a.id=b.storeId ");
            sql.Append("  where merchId='" + merchId + "' order by sumTradeAmount desc ");
            DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0];
            for (int r = 0; r < dtData.Rows.Count; r++)
            {
                var rptStoreGmvRlModel = new RptStoreGmvRlModel();

                rptStoreGmvRlModel.Name = dtData.Rows[r]["name"].ToString();
                rptStoreGmvRlModel.SumQuantity = dtData.Rows[r]["sumQuantity"].ToString();
                rptStoreGmvRlModel.SumTradeAmount = dtData.Rows[r]["sumTradeAmount"].ToString();

                retRptStoreGmvRl.Stores.Add(rptStoreGmvRlModel);
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", retRptStoreGmvRl);


            return result;
        }

        public CustomJsonResult GetProductSkuSaleRl(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var retRptProductSkuSaleRl = new RetRptProductSkuSaleRl();

            StringBuilder sql = new StringBuilder("  select top 10 prdProductSkuName ,sum(Quantity) as sumQuantity,sum(ChargeAmount) as sumTradeAmount  from OrderSub WITH(NOLOCK) where merchId='" + merchId + "' and PayStatus='3' group by PrdProductSkuName order by sumQuantity desc ");

            DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0];
            for (int r = 0; r < dtData.Rows.Count; r++)
            {
                var rptProductSkuSaleRlModel = new RptProductSkuSaleRlModel();

                rptProductSkuSaleRlModel.Name = dtData.Rows[r]["prdProductSkuName"].ToString();
                rptProductSkuSaleRlModel.SumQuantity = dtData.Rows[r]["sumQuantity"].ToString();
                rptProductSkuSaleRlModel.SumTradeAmount = dtData.Rows[r]["sumTradeAmount"].ToString();

                retRptProductSkuSaleRl.ProductSkus.Add(rptProductSkuSaleRlModel);
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", retRptProductSkuSaleRl);


            return result;
        }
    }
}
