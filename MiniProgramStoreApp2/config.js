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
    cartOperate: `${host}/Cart/Operate`,
    cartGetPageData: `${host}/Cart/GetPageData`,
    productGetList: `${host}/Product/List`,
    productGetDetails: `${host}/Product/Details`,
    deliveryAddressEdit: `${host}/DeliveryAddress/Edit`,
    deliveryAddressMy: `${host}/DeliveryAddress/My`,
    orderConfirm: `${host}/Order/Confirm`,
    couponMy: `${host}/Coupon/My`,
    storeList: `${host}/Store/List`,
    operateGetResult: `${host}/Operate/GetResult`,
    orderReserve: `${host}/Order/Reserve`,
    orderGetJsApiPaymentPms: `${host}/Order/GetJsApiPaymentPms`,
    orderGetList: `${host}/Order/List`,
    orderCancle: `${host}/Order/Cancle`,
    orderGetDetails: `${host}/Order/Details`
  }

};

module.exports = config
