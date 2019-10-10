/**
 * 小程序配置文件
 */

// 此处主机域名是腾讯云解决方案分配的域名
// 小程序后台服务解决方案：https://www.qcloud.com/solution/la

var host = "https://demo.res.17fanju.com/api"

var config = {
  // 下面的地址配合云端 Server 工作
  host,
  appId: `wx969a817779af7b53`,
  merchId: `d17df2252133478c99104180e8062230`,
  apiUrl: {
    ownLoginByMinProgram: `${host}/Own/LoginByMinProgram`,
    globalDataSet: `${host}/Global/DataSet`,
    indexPageData: `${host}/Index/PageData`,
    cartOperate: `${host}/Cart/Operate`,
    cartPageData: `${host}/Cart/PageData`,
    productList: `${host}/Product/List`,
    productDetails: `${host}/Product/Details`,
    deliveryAddressEdit: `${host}/DeliveryAddress/Edit`,
    deliveryAddressMy: `${host}/DeliveryAddress/My`,
    couponMy: `${host}/Coupon/My`,
    storeList: `${host}/Store/List`,
    operateResult: `${host}/Operate/Result`,
    orderConfirm: `${host}/Order/Confirm`,
    orderReserve: `${host}/Order/Reserve`,
    orderBuildPayParams: `${host}/Order/BuildPayParams`,
    orderList: `${host}/Order/List`,
    orderCancle: `${host}/Order/Cancle`,
    orderDetails: `${host}/Order/Details`,
    kindPageData: `${host}/Kind/PageData`,
    personalPageData: `${host}/Personal/PageData`
  }

};

module.exports = config
