const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const QRCode = require('../../utils/qrcode.js')
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
    payOption: {
      title: '支付方式',
      options: []
    },
    curSelPayOption: null
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function(options) {
    var _this = this
    orderId = options.id == undefined ? "" : options.id

    apiOrder.details({
      id: orderId
    }, {
      success: function(res) {
        if (res.result == 1) {
          _this.setData(res.data)

          for (var i = 0; i < res.data.blocks.length; i++) {
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
      },
      fail: function() {}
    })

    this.buildPayOptions()
  },
  goPay: function() {

    var _this = this;

    for (var i = 0; i < _this.data.payOption.options.length; i++) {
      if (_this.data.payOption.options[i].isSelect == true) {
        _this.data.curSelPayOption = _this.data.payOption.options[i]
        break
      }
    }



    if (_this.data.curSelPayOption == undefined || _this.data.curSelPayOption == null) {
      toast.show({
        title: '未选择支付方式'
      })
      return
    }


    apiOrder.buildPayParams({
      orderId: orderId,
      payCaller: _this.data.curSelPayOption.payCaller,
      payPartner: _this.data.curSelPayOption.payPartner
    }, {
      success: function(res) {
        if (res.result == 1) {

          var data = res.data;
          wx.requestPayment({
            'timeStamp': data.timestamp,
            'nonceStr': data.nonceStr,
            'package': data.package,
            'signType': data.signType,
            'paySign': data.paySign,
            'success': function(res) {
              wx.redirectTo({
                url: '/pages/operate/operate?id=' + data.orderId + '&type=1&caller=1'
              })
            },
            'fail': function(res) {
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
      fail: function() {}
    })
  },
  buildPayOptions: function() {
    var _this = this
    apiOrder.buildPayOptions({
      appCaller: 1
    }, {
      success: function(res) {
        if (res.result == 1) {
          _this.setData({
            payOption: res.data
          })

        }
      },
      fail: function() {}
    })

  }

})