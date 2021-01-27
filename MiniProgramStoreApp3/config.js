/**
 * 小程序配置文件
 */

// 此处主机域名是腾讯云解决方案分配的域名
// 小程序后台服务解决方案：https://www.qcloud.com/solution/la

var host = "https://api.m.17fanju.com/api"

var config = {
  // 下面的地址配合云端 Server 工作
  host,
  // appId: `wx969a817779af7b53`,
  // merchId: `d17df2252133478c99104180e8062230`,
  //storeId: '21ae9399b1804dbc9ddd3c29e8b5c670',
  apiUrl: {
    ownLoginByMinProgram: `${host}/Own/LoginByMinProgram`,
    ownWxApiCode2Session: `${host}/Own/WxApiCode2Session`,
    ownConfig: `${host}/Own/Config`,
    ownWxPhoneNumber: `${host}/Own/WxPhoneNumber`,
    ownBindPhoneNumberByWx: `${host}/Own/BindPhoneNumberByWx`,
    ownGetWxACodeUnlimit: `${host}/Own/GetWxACodeUnlimit`,
    globalDataSet: `${host}/Global/DataSet`,
    globalByPoint: `${host}/Global/ByPoint`,
    globalMsgTips: `${host}/Global/MsgTips`,
    indexPageData: `${host}/Index/PageData`,
    indexSugProducts: `${host}/Index/SugProducts`,
    productKindPageData: `${host}/ProductKind/PageData`,
    cartOperate: `${host}/Cart/Operate`,
    cartPageData: `${host}/Cart/PageData`,
    cartGetCartData: `${host}/Cart/GetCartData`,
    productInitSearchPageData: `${host}/Product/InitSearchPageData`,
    productSearch: `${host}/Product/Search`,
    productDetails: `${host}/Product/Details`,
    productSkuStockInfo: `${host}/Product/SkuStockInfo`,
    deliveryAddressEdit: `${host}/DeliveryAddress/Edit`,
    deliveryAddressMy: `${host}/DeliveryAddress/My`,
    couponMy: `${host}/Coupon/My`,
    couponDetails: `${host}/Coupon/Details`,
    couponRevCenterSt: `${host}/Coupon/RevCenterSt`,
    couponReceive: `${host}/Coupon/Receive`,
    shopList: `${host}/Shop/List`,
    operateResult: `${host}/Operate/Result`,
    orderConfirm: `${host}/Order/Confirm`,
    orderReserve: `${host}/Order/Reserve`,
    orderBuildPayParams: `${host}/Order/BuildPayParams`,
    orderBuildPayOptions: `${host}/Order/BuildPayOptions`,
    orderBuildBookTimeArea: `${host}/Order/BuildBookTimeArea`,
    orderList: `${host}/Order/List`,
    orderCancle: `${host}/Order/Cancle`,
    orderDetails: `${host}/Order/Details`,
    orderReceiptTimeAxis: `${host}/Order/ReceiptTimeAxis`,
    kindPageData: `${host}/Kind/PageData`,
    personalPageData: `${host}/Personal/PageData`,
    tobeSearch: `${host}/Search/TobeSearch`,
    memberGetPayLevelSt: `${host}/Member/GetPayLevelSt`,
    memberGetPromSt: `${host}/Member/GetPromSt`,
    memberGetRightDescSt: `${host}/Member/GetRightDescSt`,
    saleOutletList: `${host}/SaleOutlet/List`,
    selfPickAddressList: `${host}/SelfPickAddress/List`,
    serviceFunScanCodeResult: `${host}/ServiceFun/ScanCodeResult`,
    serviceFunGetMyReffSkus: `${host}/ServiceFun/GetMyReffSkus`,
    smCfSelfTakeOrderCfTake: `${host}/SmCfSelfTakeOrder/CfTake`,
    smCfSelfTakeOrderDetails: `${host}/SmCfSelfTakeOrder/Details`,
    shopMarketPageData: `${host}/ShopMarket/PageData`,

  }

};

module.exports = config