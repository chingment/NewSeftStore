const config = require('../../config')
const storeage = require('../../utils/storeageutil.js')
const wxparse = require("../../wxParse/wxParse.js")
const cart = require('../../pages/cart/cart.js')
const ownRequest = require('../../own/ownRequest.js')
const lumos = require('../../utils/lumos.minprogram.js')
const toastUtil = require('../../utils/showtoastutil')
const app = getApp()

var orderId = null

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "productdetails",
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    orderId = options.id == undefined ? "" : options.id

    //app.changeData("main", { cart: cart })

    lumos.getJson({
      url: config.apiUrl.orderGetDetails,
      urlParams: {
        id: orderId
      },
      success: function (res) {
        _this.setData(res.data)
      }
    })
  },
  goPay: function () {
    console.log("orderconfirm.goPay->>>orderId:" + orderId);


    lumos.getJson({
      url: config.apiUrl.orderGetJsApiPaymentPms,
      dataParams: {
        orderId: orderId,
        payWay: 1,
        caller: 2
      },
      success: function (res) {
        if (res.result == lumos.resultType.success) {

          var data = res.data;
          wx.requestPayment({
            'timeStamp': data.timestamp,
            'nonceStr': data.nonceStr,
            'package': data.package,
            'signType': 'MD5',
            'paySign': data.paySign,
            'success': function (res) {
              wx.redirectTo({
                url: '/pages/operate/operate?id=' + data.orderId + '&type=1&caller=1'
              })
            },
            'fail': function (res) {
              wx.redirectTo({
                url: '/pages/operate/operate?id=' + data.orderId + '&type=2&caller=1'
              })
            }
          })


        } else {
          toastUtil.showToast({
            title: res.message
          })
        }
      }
    })

  }

})