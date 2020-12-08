const config = require('../../config')
const util = require('../../utils/util')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiOrder = require('../../api/order.js')
const app = getApp()

Page({
  data: {
    tag: 'orderconfirm',
    tabShopModeByMall: 0,
    tabShopModeByMachine: 1,
    storeId: undefined,
    orderIds: null,
    blocks: [],
    couponId: [],
    payOption: {
      title: '支付方式',
      options: []
    },
    curSelPayOption: null,
    booktimeDialog: {
      isShow: false
    },
    booktimeSelectBlockIndex: -1
  },
  onLoad: function (options) {
    var _this = this

    var _this = this
    if (!ownRequest.isSelectedStore(true)) {
      return
    }

    var _orderIds = options.orderIds == undefined ? null : options.orderIds
    console.log('orderIds:' + _orderIds)
    var orderIds = []
    if (_orderIds != null) {
      var arr_order = _orderIds.split(',')

      for (let i = 0; i < arr_order.length; i++) {
        orderIds.push(arr_order[i])
      }

      console.log("orderIds:" + JSON.stringify(orderIds))
    }

    var productSkus = options.productSkus == undefined ? null : JSON.parse(options.productSkus)
    _this.setData({
      storeId: ownRequest.getCurrentStoreId(),
      orderIds: orderIds,
      productSkus: productSkus
    })
    _this.buildPayOptions()
    _this.getConfirmData()
  },
  onReady: function () {
    var _this = this

  },
  onShow: function () {},
  onHide: function () {

  },
  onUnload: function () {

  },
  onPullDownRefresh: function () {

  },
  onReachBottom: function () {

  },
  onShareAppMessage: function () {

  },
  booktimeSelect: function (e) {
    console.log('booktimeSelect')
    var _this = this
    var index = e.currentTarget.dataset.replyIndex
    _this.setData({
      booktimeSelectBlockIndex: index,
      booktimeDialog: {
        isShow: true
      }
    })
  },
  deliveryAddressSelect: function (e) {
    var _this = this
    var index = e.currentTarget.dataset.replyIndex
    var deliveryaddressid = e.currentTarget.dataset.replyDeliveryaddressid
    wx.navigateTo({
      url: "/pages/deliveryaddress/deliveryaddress?operate=2&orderBlockIndex=" + index + "&currentSelectId=" + deliveryaddressid,
      success: function (res) {
        // success
      },
    })
  },
  couponSelect: function (e) {
    var _this = this

    var couponId = _this.data.couponId
    var productSkus = _this.data.productSkus
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

    if (util.isEmptyOrNull(_this.data.curSelPayOption)) {
      toast.show({
        title: '未选择支付方式'
      })
      return
    }

    var tabShopModeByMall = _this.data.tabShopModeByMall

    var blocks = []
    var _blocks = _this.data.blocks
    for (var i = 0; i < _blocks.length; i++) {
      var skus = []
      var _skus = _blocks[i].skus
      for (var j = 0; j < _skus.length; j++) {
        skus.push({
          cartId: _skus[j].cartId,
          id: _skus[j].id,
          quantity: _skus[j].quantity,
          shopMode: _skus[j].shopMode
        })
      }


      var _delivery = _blocks[i].delivery
      var _selfTake = _blocks[i].selfTake

      var delivery = null
      var selfTake = null
      var bookTime = null
      if (_blocks[i].shopMode == 1) {
        if (_blocks[i].receiveMode == 1) {
          if (util.isEmptyOrNull(_delivery.id)) {
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
        } else if (_blocks[i].receiveMode == 2) {

          if (util.isEmptyOrNull(_blocks[i].bookTime.value)) {
            toast.show({
              title: '请选择预约时间'
            })
            return
          }

          bookTime = {
            type: _blocks[i].bookTime.type,
            value: _blocks[i].bookTime.value
          }

          selfTake = {
            storeName: _selfTake.storeName,
            storeAddress: _selfTake.storeAddress,
            areaCode: _selfTake.areaCode,
            address: _selfTake.address
          }
        }
      } else if (_blocks[i].shopMode == 3) {
        selfTake = {
          storeName: _selfTake.storeName,
          storeAddress: _selfTake.storeAddress,
          areaCode: _selfTake.areaCode,
          address: _selfTake.address
        }
      }

      blocks.push({
        shopMode: _blocks[i].shopMode,
        receiveMode: _blocks[i].receiveMode,
        delivery: delivery,
        selfTake: selfTake,
        bookTime: bookTime,
        skus: skus
      })

    }

    if (_this.data.orderIds == undefined || _this.data.orderIds == null || _this.data.orderIds.length == 0) {


      apiOrder.reserve({
        storeId: _this.data.storeId,
        blocks: blocks,
        source: 3
      }).then(function (res) {
        if (res.result == 1) {
          var d = res.data
          apiCart.pageData({
            success: function (res) {}
          })

          var orderIds = []
          for (var i = 0; i < d.orders.length; i++) {
            orderIds.push(d.orders[i].id)
          }

          _this.setData({
            orderIds: orderIds
          })
          _this.goPay(_this.data.curSelPayOption, null)
        } else {
          toast.show({
            title: res.message
          })
        }
      })

    } else {
      _this.goPay(_this.data.curSelPayOption, blocks)
    }
  },
  goPay: function (payOption, blocks) {
    var _this = this
    apiOrder.buildPayParams({
      orderIds: _this.data.orderIds,
      payCaller: payOption.payCaller,
      blocks: blocks,
      payPartner: payOption.payPartner
    }).then(function (res) {
      if (res.result == 1) {

        var d = res.data;

        _this.setData({
          payTransId: d.payTransId
        })

        wx.requestPayment({
          'timeStamp': d.timestamp,
          'nonceStr': d.nonceStr,
          'package': d.package,
          'signType': d.signType,
          'paySign': d.paySign,
          'success': function (res) {
            wx.redirectTo({
              url: '/pages/operate/operate?id=' + d.payTransId + '&type=1&caller=1'
            })
          },
          'fail': function (res) {
            wx.redirectTo({
              url: '/pages/operate/operate?id=' + d.payTransId + '&type=2&caller=1'
            })
          }
        })
      } else {
        toast.show({
          title: res.message
        })
      }
    })
  },
  buildPayOptions: function () {
    var _this = this
    apiOrder.buildPayOptions({
      appCaller: 1
    }).then(function (res) {
      if (res.result == 1) {
        _this.setData({
          payOption: res.data
        })

      }
    })

  },
  tabMallReceiveModeClick(e) {
    var _this = this
    var receivemode = e.currentTarget.dataset.replyReceivemode
    var blockindex = e.currentTarget.dataset.replyBlockindex
    var tabmode = e.currentTarget.dataset.replyTabmode

    console.log("blockindex:" + blockindex)
    console.log("tabmode:" + tabmode)
    console.log("receivemode:" + receivemode)

    _this.data.blocks[blockindex].receiveMode = receivemode


    if (tabmode == 3) {
      _this.setData({
        blocks: _this.data.blocks
      })
    }


  },
  getConfirmData: function (_this) {
    var _this = this
    var _data = _this.data
    apiOrder.confirm({
      orderIds: _data.orderIds,
      storeId: _data.storeId,
      productSkus: _data.productSkus,
      couponId: _data.couponId
    }).then(function (res) {
      if (res.result == 1) {
        var d = res.data
        var blocks = d.blocks
        var tabShopModeByMall = 0
        for (var i = 0; i < blocks.length; i++) {
          if (blocks[i].shopMode == 1) {
            if (blocks[i].tabMode == 1 || blocks[i].tabMode == 3) {
              tabShopModeByMall = 0
            } else if (blocks[i].tabMode == 2 || blocks[i].tabMode == 3) {
              tabShopModeByMall = 1
            }
          }
        }

        console.log("tabShopModeByMall:" + tabShopModeByMall)

        _this.setData({
          tabShopModeByMall: tabShopModeByMall,
          blocks: d.blocks,
          subtotalItems: d.subtotalItems,
          actualAmount: d.actualAmount,
          originalAmount: d.originalAmount,
          coupon: d.coupon
        })
      }
    })
  },
  getSelectBookTime: function (e) {
    var _this = this;
    var d = e.detail.params

    var booktime = {
      text: d.text,
      value: d.value,
      type: d.type
    }

    _this.data.blocks[_this.data.booktimeSelectBlockIndex].bookTime = booktime

    _this.setData({
      blocks: _this.data.blocks
    })
  }

})