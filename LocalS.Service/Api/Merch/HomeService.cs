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
            sql2.Append(" select count(*) from MerchDevice where IsStopUse=0 and merchId='" + merchId + "' ");

            int deviceCount = int.Parse(DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql2.ToString()).ToString());

            StringBuilder sql3 = new StringBuilder();
            sql3.Append(" select ISNULL(sum(ChargeAmount),0)-ISNULL(sum(RefundedAmount),0) from [Order] where  payStatus='3' and IsTestMode=0 and merchId='" + merchId + "' ");

            decimal sumTradeAmount = decimal.Parse(DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql3.ToString()).ToString());

            StringBuilder sql4 = new StringBuilder();
            sql4.Append("  select COUNT( distinct skuId) from SellChannelStock where (SellQuantity=0 or (SumQuantity < MaxQuantity)) and merchId='" + merchId + "' ");

            int replenishCount = int.Parse(DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql4.ToString()).ToString());


            StringBuilder sql5 = new StringBuilder();
            sql5.Append(" select count(*) from Device where (ExIsHas=1 or  datediff(MINUTE,LastRequestTime,GETDATE())>15) AND CurUseStoreId IS NOT NULL and CurUseMerchId='" + merchId + "' ");

            int deviceExCount = int.Parse(DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql5.ToString()).ToString());

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { storeCount = storeCount, deviceCount = deviceCount, deviceExCount = deviceExCount, sumTradeAmount = sumTradeAmount, replenishCount = replenishCount });


            return result;
        }

        public CustomJsonResult GetTodaySummary(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var retRptTodaySummary = new RetRptTodaySummary();

            StringBuilder sql = new StringBuilder();
            sql.Append(" select COUNT(*) from dbo.[Order] where ReceiveMode=4 and ExIsHappen=1 and ExIsHandle=0 and merchId='" + merchId + "' ");


            int sumExHdByDeviceSelfTake = int.Parse(DatabaseFactory.GetIDBOptionBySql().ExecuteScalar(sql.ToString()).ToString());

            retRptTodaySummary.SumExHdByDeviceSelfTake = sumExHdByDeviceSelfTake;

            string date = DateTime.Now.ToString("yyyy-MM") + "-01";

            StringBuilder sql2 = new StringBuilder();
            sql2.Append(" select COUNT(*) as sumCount,ISNULL(SUM(Quantity),0) as sumQuantity , ISNULL(SUM(ChargeAmount),0)-ISNULL(SUM(RefundedAmount),0) as sumTradeAmount  from dbo.[Order] where  PayStatus=3 and  DATEDIFF(MONTH, PayedTime, '" + date + "') = 0 and merchId='" + merchId + "' ");
            DataTable dtData2 = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql2.ToString()).Tables[0];


            if (dtData2.Rows.Count > 0)
            {
                retRptTodaySummary.NowMonthGmvRl.SumCount = dtData2.Rows[0]["sumCount"].ToString();
                retRptTodaySummary.NowMonthGmvRl.SumQuantity = dtData2.Rows[0]["sumQuantity"].ToString();
                retRptTodaySummary.NowMonthGmvRl.SumTradeAmount = dtData2.Rows[0]["sumTradeAmount"].ToString();
            }

            StringBuilder sql3 = new StringBuilder();
            sql3.Append(" select COUNT(*) as sumCount,ISNULL(SUM(Quantity),0) as sumQuantity , ISNULL(SUM(ChargeAmount),0)-ISNULL(SUM(RefundedAmount),0) as sumTradeAmount  from dbo.[Order] where  PayStatus=3 and  DATEDIFF(MONTH, PayedTime, '" + date + "') = 1 and merchId='" + merchId + "' ");
            DataTable dtData3 = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql2.ToString()).Tables[0];


            if (dtData3.Rows.Count > 0)
            {
                retRptTodaySummary.LastMonthGmvRl.SumCount = dtData3.Rows[0]["sumCount"].ToString();
                retRptTodaySummary.LastMonthGmvRl.SumQuantity = dtData3.Rows[0]["sumQuantity"].ToString();
                retRptTodaySummary.LastMonthGmvRl.SumTradeAmount = dtData3.Rows[0]["sumTradeAmount"].ToString();
            }




            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", retRptTodaySummary);


            return result;
        }

        public CustomJsonResult Get7DayGmv(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var retRpt7DayGmvRl = new RetRpt7DayGmvRl();

            StringBuilder sql = new StringBuilder();
            sql.Append(" select a1.datef,isnull(sumCount,0) as sumCount,isnull(sumQuantity,0) as sumQuantity, isnull(sumTradeAmount,0) as  sumTradeAmount from (  ");
            for (int i = 0; i < 10; i++)
            {
                string datef = DateTime.Now.AddDays(double.Parse((-i).ToString())).ToUnifiedFormatDate();

                sql.Append(" select '" + datef + "' datef union");
            }
            sql.Remove(sql.Length - 5, 5);

            sql.Append(" ) a1 left join ");
            sql.Append(" (    select datef, sum(sumCount) as sumCount , sum(sumQuantity) as sumQuantity,sum(sumTradeAmount) as sumTradeAmount from ( select CONVERT(varchar(100),PayedTime, 23) datef,count(*) as sumCount ,sum(Quantity) as sumQuantity,sum(ChargeAmount)-sum(RefundedAmount) as sumTradeAmount from [Order] WITH(NOLOCK)  where  merchId='" + merchId + "' and PayStatus='3' and IsTestMode=0 and DateDiff(dd, PayedTime, getdate()) <= 10  group by PayedTime ) tb  group by datef ) b1 ");
            sql.Append(" on  a1.datef=b1.datef  ");
            sql.Append(" order by a1.datef desc  ");



            DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0];
            for (int r = 0; r < dtData.Rows.Count; r++)
            {
                var rpt7DayGmvRlModel = new
                {

                    Datef = dtData.Rows[r]["datef"].ToString(),
                    SumCount = dtData.Rows[r]["sumCount"].ToString(),
                    SumQuantity = dtData.Rows[r]["sumQuantity"].ToString(),
                    SumTradeAmount = dtData.Rows[r]["sumTradeAmount"].ToString()
                };

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

            StringBuilder sql = new StringBuilder("  select top 10 name,isnull(sumCount,0) as sumCount ,isnull(sumQuantity,0) as sumQuantity,isnull(sumTradeAmount,0) as sumTradeAmount from Store a left join ( ");
            sql.Append("  select StoreId ,count(*) as sumCount, sum(Quantity) as sumQuantity,sum(ChargeAmount)-sum(RefundedAmount) as sumTradeAmount  from [Order] WITH(NOLOCK) where merchId='" + merchId + "' and PayStatus='3' and IsTestMode=0 and PayedTime>='" + startTime + "'  and PayedTime<='" + endTime + "'  group by StoreId  )  b on a.id=b.storeId ");
            sql.Append("  where merchId='" + merchId + "' and a.IsDelete=0 order by sumTradeAmount desc ");
            DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0];
            for (int r = 0; r < dtData.Rows.Count; r++)
            {
                var rptStoreGmvRlModel = new
                {

                    Name = dtData.Rows[r]["name"].ToString(),
                    SumCount = dtData.Rows[r]["sumCount"].ToString(),
                    SumQuantity = dtData.Rows[r]["sumQuantity"].ToString(),
                    SumTradeAmount = dtData.Rows[r]["sumTradeAmount"].ToString()
                };

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
            sql.Append("  select StoreId ,sum(Quantity) as sumQuantity,sum(chargeAmount)-sum(RefundedAmount) as sumTradeAmount  from [Order] WITH(NOLOCK) where merchId='" + merchId + "' and IsTestMode=0 and PayStatus='3' group by StoreId  )  b on a.id=b.storeId ");
            sql.Append("  where merchId='" + merchId + "' and a.IsDelete=0 order by sumTradeAmount desc ");
            DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0];
            for (int r = 0; r < dtData.Rows.Count; r++)
            {
                var rptStoreGmvRlModel = new
                {
                    Name = dtData.Rows[r]["name"].ToString(),
                    SumQuantity = dtData.Rows[r]["sumQuantity"].ToString(),
                    SumTradeAmount = dtData.Rows[r]["sumTradeAmount"].ToString()
                };

                retRptStoreGmvRl.Stores.Add(rptStoreGmvRlModel);
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", retRptStoreGmvRl);


            return result;
        }

        public CustomJsonResult GetSkuSaleRl(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret_RptSkuSaleRl = new RetRptSkuSaleRl();

            StringBuilder sql = new StringBuilder("  select top 10 skuName ,sum(Quantity-RefundedQuantity) as sumQuantity,sum(ChargeAmount-RefundedQuantity) as sumTradeAmount  from OrderSub WITH(NOLOCK) where merchId='" + merchId + "' and IsTestMode=0 and PayStatus='3' group by SkuName order by sumQuantity desc ");

            DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0];
            for (int r = 0; r < dtData.Rows.Count; r++)
            {
                var rptSkuSaleRlModel = new
                {

                    Name = dtData.Rows[r]["skuName"].ToString(),
                    SumQuantity = dtData.Rows[r]["sumQuantity"].ToString(),
                    SumTradeAmount = dtData.Rows[r]["sumTradeAmount"].ToString()
                };

                ret_RptSkuSaleRl.Skus.Add(rptSkuSaleRlModel);
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret_RptSkuSaleRl);


            return result;
        }
    }
}
