const config = require('../../config')
const util = require('../../utils/util')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const QRCode = require('../../utils/qrcode.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCoupon = require('../../api/coupon.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "mycoupondetails"
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var id = options.id == undefined ? "" : options.id

    apiCoupon.details({
      id: id
    }).then(function (res) {
      if (res.result == 1) {
        var d=res.data

        _this.setData(d)

        if (!util.isEmptyOrNull(d.wtCode)) {
          new QRCode('qrcode-0', {
            // usingIn: this,
            text: d.wtCode,
            width: 130,
            height: 130,
            colorDark: "#000000",
            colorLight: "white",
            correctLevel: QRCode.CorrectLevel.H,
          });
        }

      }
    })
  }
})