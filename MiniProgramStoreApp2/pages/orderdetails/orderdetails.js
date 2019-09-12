const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiOrder = require('../../api/order.js')
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

    apiOrder.details({
      id: orderId
    }, {
        success: function (res) {
          if (res.result == 1) {
            _this.setData(res.data)
          }
        },
        fail: function () { }
      })

  },
  goPay: function () {

    apiOrder.jsApiPaymentPms({
      orderId: orderId,
      payWay: 1,
      caller: 2
    }, {
        success: function (res) {
          if (res.result == 1) {

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
            toast.show({
              title: res.message
            })
          }
        },
        fail: function () { }
      })
  }

})