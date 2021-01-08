const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const QRCode = require('../../utils/qrcode.js')
const ownRequest = require('../../own/ownRequest.js')
const apiOrder = require('../../api/order.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "orderdetails"
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var orderId = options.id == undefined ? "" : options.id

    apiOrder.details({
      id: orderId
    }).then(function (res) {
      if (res.result == 1) {
        _this.setData(res.data)

        for (var i = 0; i < res.data.blocks.length; i++) {
          if (res.data.blocks[i].qrcode != null) {
            if (res.data.blocks[i].qrcode.code != null && res.data.blocks[i].qrcode.code != '') {
              new QRCode('qrcode-' + i, {
                // usingIn: this,
                text: res.data.blocks[i].qrcode.code,
                width: 130,
                height: 130,
                colorDark: "#000000",
                colorLight: "white",
                correctLevel: QRCode.CorrectLevel.H,
              });
            }
          }
        }
      }
    })
  },
  onShow: function () {},
  onUnload: function () {},
})