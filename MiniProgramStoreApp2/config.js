/**
 * 小程序配置文件
 */

// 此处主机域名是腾讯云解决方案分配的域名
// 小程序后台服务解决方案：https://www.qcloud.com/solution/la

var host = "https://api.m.17fanju.com/api"

var config = {
  // 下面的地址配合云端 Server 工作
  host,
  appId: `wx969a817779af7b53`,
  merchId: `d17df2252133478c99104180e8062230`,
  // storeId: '21ae9399b1804dbc9ddd3c29e8b5c670',
  apiUrl: {
    ownLoginByMinProgram: `${host}/Own/LoginByMinProgram`,
    globalDataSet: `${host}/Global/DataSet`,
    indexPageData: `${host}/Index/PageData`,
    productKindPageData:`${host}/ProductKind/PageData`,
    cartOperate: `${host}/Cart/Operate`,
    cartPageData: `${host}/Cart/PageData`,
    productInitSearchPageData: `${host}/Product/InitSearchPageData`,
    productSearch: `${host}/Product/Search`,
    productDetails: `${host}/Product/Details`,
    productSkuStockInfo: `${host}/Product/SkuStockInfo`,
    deliveryAddressEdit: `${host}/DeliveryAddress/Edit`,
    deliveryAddressMy: `${host}/DeliveryAddress/My`,
    couponMy: `${host}/Coupon/My`,
    storeList: `${host}/Store/List`,
    operateResult: `${host}/Operate/Result`,
    orderConfirm: `${host}/Order/Confirm`,
    orderReserve: `${host}/Order/Reserve`,
    orderBuildPayParams: `${host}/Order/BuildPayParams`,
    orderBuildPayOptions: `${host}/Order/BuildPayOptions`,
    orderList: `${host}/Order/List`,
    orderCancle: `${host}/Order/Cancle`,
    orderDetails: `${host}/Order/Details`,
    orderReceiptTimeAxis: `${host}/Order/ReceiptTimeAxis`,
    kindPageData: `${host}/Kind/PageData`,
    personalPageData: `${host}/Personal/PageData`,
    tobeSearch: `${host}/Search/TobeSearch`,
  }

};

module.exports = config
