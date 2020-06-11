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



        var blocks = d.blocks
        var tabShopModeByMall = 0
        for (var i = 0; i < blocks.length; i++) {

          if (blocks[i].shopMode == 1) {
            if (blocks[i].tabMode == 1 || blocks[i].tabMode == 3) {
              tabShopModeByMall = 0
            }
            else if (blocks[i].tabMode == 2 || blocks[i].tabMode == 3) {
              tabShopModeByMall = 1
            }
          }
        }

        console.log("tabShopModeByMall:" + tabShopModeByMall)

        _this.setData({
          tabShopModeByMall: tabShopModeByMall,
          orderId: orderId,
          blocks: d.blocks,
          subtotalItems: d.subtotalItems,
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
    tabShopModeByMall: 0,
    tabShopModeByMachine: 1,
    orderId: null,
    blocks: [],
    couponId: [],
    payOption: {
      title: '支付方式',
      options: []
    },
    curSelPayOption: null
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


    var tabShopModeByMall = _this.data.tabShopModeByMall

    if (orderId == undefined || orderId == null) {

      var blocks = []
      var _blocks = _this.data.blocks
      for (var i = 0; i < _blocks.length; i++) {
        var skus = []
        var _skus = _blocks[i].skus
        for (var j = 0; j < _skus.length; j++) {
          skus.push({ cartId: _skus[j].cartId, id: _skus[j].id, quantity: _skus[j].quantity, shopMode: _skus[j].shopMode })
        }


        var _delivery = _blocks[i].delivery
        var _selfTake = _blocks[i].selfTake

        var delivery = null;
        var selfTake = null;

        var receiveMode = 0
        if (_blocks[i].shopMode == 1) {
          if (tabShopModeByMall == 0) {
            receiveMode = 1

            if (_delivery.id == "") {
              toast.show({
                title: '请选择快寄地址'
              })
              return
            }

            delivery = {
              id: _delivery.id,
              consignee: _delivery.consignee,
              phoneNumber: _delivery.phoneNumber,
              areaName: _delivery.areaName,
              areaCode: _delivery.areaCode,
              address: _delivery.address
            }
          }
          else if (tabShopModeByMall == 1) {
            receiveMode = 2

            selfTake = {
              storeName: _selfTake.storeName,
              storeAddress: _selfTake.storeAddress,
            }

          }
        }
        else if (_blocks[i].shopMode == 3) {
          receiveMode = 3

          selfTake = {
            storeName: _selfTake.storeName,
            storeAddress: _selfTake.storeAddress,
          }
        }

        blocks.push({ shopMode: _blocks[i].shopMode, receiveMode: receiveMode, delivery: delivery, selfTake: selfTake, skus: skus })

      }




      apiOrder.reserve({
        storeId: ownRequest.getCurrentStoreId(),
        blocks: blocks,
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
  buildPayOptions: function () {
    var _this = this
    apiOrder.buildPayOptions({
      appCaller: 1
    }, {
      success: function (res) {
        if (res.result == 1) {
          _this.setData({ payOption: res.data })

        }
      },
      fail: function () { }
    })

  },
  tabShopModeByMallClick(e) {
    var _this = this
    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var tabmode = e.currentTarget.dataset.replyTabmode //对应页面data-reply-index

    var isFalg = false
    if (index == 0) {
      if (tabmode == 1 || tabmode == 3) {
        isFalg = true
      }
    }
    else if (index == 1) {
      if (tabmode == 2 || tabmode == 3) {
        isFalg = true
      }
    }
    if (isFalg) {
      _this.setData({ tabShopModeByMall: index })
    }
  }


})