const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiOrder = require('../../api/order.js')
const app = getApp()

var productSkus = null
var orderId = null
var getData = function (_this) {

  var couponId = _this.data.couponId


  apiOrder.confirm({
    orderId: orderId,
    storeId: ownRequest.getCurrentStoreId(),
    productSkus: productSkus,
    couponId: couponId
  }, {
      success: function (res) {
        if (res.result == 1) {
          var d = res.data
          _this.setData({
            orderId: orderId,
            block: d.block,
            subtotalItem: d.subtotalItem,
            actualAmount: d.actualAmount,
            originalAmount: d.originalAmount,
            coupon: d.coupon
          })
        }
      },
      fail: function () { }
    })
}

Page({

  /**
   * 页面的初始数据
   */
  data: {
    orderId: null,
    block: [],
    couponId: [],
    payOption:{
      title:'支付方式',
      options:[]
    },
    curSelPayOption:null
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    orderId = options.orderId == undefined ? null : options.orderId
    productSkus = JSON.parse(options.productSkus)
    this.buildPayOptions()
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {
    var _this = this
    getData(_this)
  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () { },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },
  deliveryAddressSelect: function (e) {
    var _this = this
    var index = e.currentTarget.dataset.replyIndex
    var deliveryAddress = _this.data.block[index].deliveryAddress
    if (!deliveryAddress.canSelectElse)
      return
    wx.navigateTo({
      url: "/pages/deliveryaddress/deliveryaddress?operate=2&operateIndex=" + index,
      success: function (res) {
        // success
      },
    })
  },
  couponSelect: function (e) {
    var _this = this

    var couponId = _this.data.couponId

    wx.navigateTo({
      url: "/pages/mycoupon/mycoupon?operate=2&isGetHis=false&productSkus=" + JSON.stringify(productSkus) + "&couponId=" + JSON.stringify(couponId),
      success: function (res) {
        // success
      },
    })
  },
  unifiedOrder: function (e) {
    var _this = this
    for (var i = 0; i < _this.data.block.length; i++) {
      if (_this.data.block[i].receptionMode == 1) {
        if (_this.data.block[i].deliveryAddress.id == "") {
          toast.show({
            title: '请选择快寄地址'
          })
          return
        }
      }
    }


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
    
  

    if (orderId == undefined || orderId == null) {


      apiOrder.reserve({
        storeId: ownRequest.getCurrentStoreId(),
        productSkus: productSkus,
        source: 3
      }, {
          success: function (res) {
            if (res.result == 1) {
              orderId = res.data.orderId
              apiCart.pageData({
                success: function (res) { }
              })
              _this.goPay(_this.data.curSelPayOption)
            } else {
              toast.show({
                title: res.message
              })
            }
          },
          fail: function () { }
        })

    } else {
      _this.goPay(_this.data.curSelPayOption)
    }
  },
  goPay: function (payOption) {
   
    apiOrder.buildPayParams({
      orderId: orderId,
      payCaller: payOption.payCaller,
      payPartner: payOption.payPartner
    }, {
        success: function (res) {
          if (res.result == 1) {

            var data = res.data;
            wx.requestPayment({
              'timeStamp': data.timestamp,
              'nonceStr': data.nonceStr,
              'package': data.package,
              'signType': data.signType,
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
  },
  buildPayOptions: function(){
    var _self = this
    apiOrder.buildPayOptions({
      appCaller: 1
    }, {
        success: function (res) {
          if (res.result == 1) {
            _self.setData({ payOption:res.data})
            
          }
        },
        fail: function () { }
      })

  }


})