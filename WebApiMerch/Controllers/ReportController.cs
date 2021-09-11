using LocalS.Service.Api.Merch;
using Lumos;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace WebApiMerch.Controllers
{
    public class ReportController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse DeviceStockRealDataInit()
        {
            var result = MerchServiceFactory.Report.DeviceStockRealDataInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeviceStockRealDataGet([FromBody]RopReportStoreStockRealDataGet rop)
        {
            var result = MerchServiceFactory.Report.DeviceStockRealDataGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeviceStockSumGet([FromBody]RopReportStoreStockRealDataGet rop)
        {
            var result = MerchServiceFactory.Report.DeviceStockRealDataGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse DeviceReplenishPlanInit()
        {
            var result = MerchServiceFactory.Report.DeviceReplenishPlanInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeviceReplenishPlanGet([FromBody]RopReportDeviceReplenishPlanGet rop)
        {
            var result = MerchServiceFactory.Report.DeviceReplenishPlanGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse DeviceStockDateHisInit()
        {
            var result = MerchServiceFactory.Report.DeviceStockDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeviceStockDateHisGet([FromBody]RopReporStoreStockDateHisGet rop)
        {
            var result = MerchServiceFactory.Report.DeviceStockDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse SkuSalesHisInit()
        {
            var result = MerchServiceFactory.Report.SkuSalesHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse<PageEntity<SkuSaleModel>> SkuSalesHisGet([FromBody]RopReportSkuSalesHisGet rop)
        {
            var result = MerchServiceFactory.Report.SkuSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse<PageEntity<SkuSaleModel>>(result);
        }

        [HttpPost]
        public HttpResponseMessage SkuSalesHisExport([FromBody]RopReportSkuSalesHisGet rop)
        {

            HSSFWorkbook workbook = new HSSFWorkbook();

            ISheet sheet = workbook.CreateSheet("sheet1");
            IRow titleRow = sheet.CreateRow(0);
            string[] titleNames = new string[] {
                "店铺",
                "门店",
                "设备编码",
                "提货方式",
                "订单号",
                "商品名称",
                "商品编码",
                "商品规格",
                "支付方式",
                "支付状态",
                "支付时间",
                "取货状态",
                "单价",
                "数量",
                "支付金额",
                "退款数量",
                "退款金额",
                "结算数量",
                "结算金额",
            };

            for (int i = 0; i < titleNames.Length; i++)
            {
                titleRow.CreateCell(i).SetCellValue(titleNames[i]);
            }

            rop.Page = 1;
            rop.Limit = int.MaxValue;

            var result_His = MerchServiceFactory.Report.SkuSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            if (result_His.Result == ResultType.Success)
            {
                var data = result_His.Data;
                var items = data.Items;
                for (int i = 0; i < items.Count; i++)
                {
                    IRow cellRow = sheet.CreateRow(i + 1);

                    cellRow.CreateCell(0, CellType.String).SetCellValue(items[i].StoreName);
                    cellRow.CreateCell(1, CellType.String).SetCellValue(items[i].ShopName);
                    cellRow.CreateCell(2, CellType.String).SetCellValue(items[i].DeviceCode);
                    cellRow.CreateCell(3, CellType.String).SetCellValue(items[i].ReceiveMode);
                    cellRow.CreateCell(4, CellType.String).SetCellValue(items[i].OrderId);
                    cellRow.CreateCell(5, CellType.String).SetCellValue(items[i].SkuName);
                    cellRow.CreateCell(6, CellType.String).SetCellValue(items[i].SkuCumCode);
                    cellRow.CreateCell(7, CellType.String).SetCellValue(items[i].SkuSpecDes);
                    cellRow.CreateCell(8, CellType.String).SetCellValue(items[i].PayWay);
                    cellRow.CreateCell(9, CellType.String).SetCellValue(items[i].PayStatus);
                    cellRow.CreateCell(10, CellType.String).SetCellValue(items[i].PayedTime);
                    cellRow.CreateCell(11, CellType.String).SetCellValue(items[i].PickupStatus);
                    cellRow.CreateCell(12, CellType.String).SetCellValue(items[i].SalePrice.ToF2Price());
                    cellRow.CreateCell(13, CellType.String).SetCellValue(items[i].Quantity);
                    cellRow.CreateCell(14, CellType.String).SetCellValue(items[i].ChargeAmount.ToF2Price());
                    cellRow.CreateCell(15, CellType.String).SetCellValue(items[i].RefundedQuantity);
                    cellRow.CreateCell(16, CellType.String).SetCellValue(items[i].RefundedAmount.ToF2Price());
                    cellRow.CreateCell(17, CellType.String).SetCellValue(items[i].TradeQuantity);
                    cellRow.CreateCell(18, CellType.String).SetCellValue(items[i].TradeAmount.ToF2Price());

                }
            }

            var fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".xls";
            //将Excel表格转化为流，输出
            //创建文件流
            MemoryStream bookStream = new MemoryStream();
            //文件写入流（向流中写入字节序列）
            workbook.Write(bookStream);
            //输出之前调用Seek（偏移量，游标位置) 把0位置指定为开始位置
            bookStream.Seek(0, SeekOrigin.Begin);

            //saveTofle(bookStream, "E:\\Web\\NewSeftStore\\WebApiMerch\\AAAA.xls");

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(bookStream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            response.Content.Headers.ContentLength = bookStream.Length;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            return response;
        }

        [HttpGet]
        public OwnApiHttpResponse OrderSalesHisInit()
        {
            var result = MerchServiceFactory.Report.OrderSalesHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse<PageEntity<OrderSaleModel>> OrderSalesHisGet([FromBody]RopReporOrderSalesHisGet rop)
        {
            var result = MerchServiceFactory.Report.OrderSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse<PageEntity<OrderSaleModel>>(result);
        }

        [HttpPost]
        public HttpResponseMessage OrderSalesHisExport([FromBody]RopReporOrderSalesHisGet rop)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            ISheet sheet = workbook.CreateSheet("sheet1");
            IRow titleRow = sheet.CreateRow(0);
            string[] titleNames = new string[] {
                "店铺",
                "门店",
                "设备编码",
                "提货方式",
                "订单号",
                "支付方式",
                "支付状态",
                "支付时间",
                "数量",
                "支付金额",
                "退款数量",
                "结算数量",
                "结算金额",
            };

            for (int i = 0; i < titleNames.Length; i++)
            {
                titleRow.CreateCell(i).SetCellValue(titleNames[i]);
            }

            rop.Page = 1;
            rop.Limit = int.MaxValue;

            var result_His = MerchServiceFactory.Report.OrderSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            if (result_His.Result == ResultType.Success)
            {
                var data = result_His.Data;
                var items = data.Items;
                for (int i = 0; i < items.Count; i++)
                {
                    IRow cellRow = sheet.CreateRow(i + 1);

                    cellRow.CreateCell(0, CellType.String).SetCellValue(items[i].StoreName);
                    cellRow.CreateCell(1, CellType.String).SetCellValue(items[i].ShopName);
                    cellRow.CreateCell(2, CellType.String).SetCellValue(items[i].DeviceCode);
                    cellRow.CreateCell(3, CellType.String).SetCellValue(items[i].ReceiveMode);
                    cellRow.CreateCell(4, CellType.String).SetCellValue(items[i].OrderId);
                    cellRow.CreateCell(5, CellType.String).SetCellValue(items[i].PayedTime);
                    cellRow.CreateCell(6, CellType.String).SetCellValue(items[i].PayWay);
                    cellRow.CreateCell(7, CellType.String).SetCellValue(items[i].PayStatus);
                    cellRow.CreateCell(8, CellType.String).SetCellValue(items[i].Quantity);
                    cellRow.CreateCell(9, CellType.String).SetCellValue(items[i].ChargeAmount.ToF2Price());
                    cellRow.CreateCell(10, CellType.String).SetCellValue(items[i].RefundedQuantity);
                    cellRow.CreateCell(11, CellType.String).SetCellValue(items[i].RefundedAmount.ToF2Price());
                    cellRow.CreateCell(12, CellType.String).SetCellValue(items[i].TradeQuantity);
                    cellRow.CreateCell(13, CellType.String).SetCellValue(items[i].TradeAmount.ToF2Price());

                }
            }

            var fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".xls";
            //将Excel表格转化为流，输出
            //创建文件流
            MemoryStream bookStream = new MemoryStream();
            //文件写入流（向流中写入字节序列）
            workbook.Write(bookStream);
            //输出之前调用Seek（偏移量，游标位置) 把0位置指定为开始位置
            bookStream.Seek(0, SeekOrigin.Begin);

            //saveTofle(bookStream, "E:\\Web\\NewSeftStore\\WebApiMerch\\AAAA.xls");

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(bookStream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            response.Content.Headers.ContentLength = bookStream.Length;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            return response;
        }

        [HttpGet]
        public OwnApiHttpResponse StoreSalesHisInit()
        {
            var result = MerchServiceFactory.Report.StoreSalesHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse StoreSalesHisGet([FromBody]RopReporStoreSalesHisGet rop)
        {
            var result = MerchServiceFactory.Report.StoreSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse CheckRightExport([FromBody]RopReportCheckRightExport rop)
        {
            var result = MerchServiceFactory.Report.CheckRightExport(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse DeviceSalesHisInit()
        {
            var result = MerchServiceFactory.Report.DeviceSalesHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeviceSalesHisGet([FromBody]RopReportDeviceSalesHisGet rop)
        {
            var result = MerchServiceFactory.Report.DeviceSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
