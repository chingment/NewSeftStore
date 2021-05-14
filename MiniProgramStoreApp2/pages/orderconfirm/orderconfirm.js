const config = require('../../config')
const util = require('../../utils/util')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiOrder = require('../../api/order.js')
const skeletonData = require('./skeletonData')
const app = getApp()

Page({
  data: {
    tag: 'orderconfirm',
    skeletonLoadingTypes: ['spin', 'chiaroscuro', 'shine', 'null'],
    skeletonSelectedLoadingType: 'shine',
    skeletonIsDev: false,
    skeletonBgcolor: '#FFF',
    skeletonData,
    pageIsReady: false,
    tabShopModeByMachine: 1,
    curSelfPickAddressBlockIndex: -1,
    curBookTimeBlockIndex: -1,
    storeId: undefined,
    orderIds: [],
    blocks: [],
    couponIdsByShop: null,
    couponIdByRent: null,
    couponIdByDeposit: null,
    payOption: {
      title: '支付方式',
      options: []
    },
    curSelPayOption: null,
    booktimeDialog: {
      isShow: false
    },
    selfPickAddressDialog: {
      isShow: false,
      dataS: {
        curSelfPickAddressId: ''
      }
    },
    action: '',
    saleOutletId: '',
    shopMethod: 1,
    pOrderId: ''
  },
  onLoad: function (options) {
    var _this = this

    var _orderIds = options.orderIds == undefined ? null : options.orderIds
    var _action = options.action == undefined ? null : options.action
    var _saleOutletId = options.saleOutletId == undefined ? null : options.saleOutletId
    var _shopMethod = options.shopMethod == undefined ? 1 : options.shopMethod
    var _skus = options.skus == undefined ? null : JSON.parse(decodeURIComponent(options.skus))
    var _pOrderId = options.pOrderId == undefined ? null : options.pOrderId
    var orderIds = []
    if (_orderIds != null) {
      var arr_order = _orderIds.split(',')
      for (let i = 0; i < arr_order.length; i++) {
        orderIds.push(arr_order[i])
      }
    }

    _this.setData({
      storeId: storeage.getStoreId(),
      orderIds: orderIds,
      skus: _skus,
      action: _action,
      saleOutletId: _saleOutletId,
      shopMethod: _shopMethod,
      pOrderId: _pOrderId
    })

    _this.buildPayOptions()
    _this.getConfirmData()
  },
  onReady: function () {
    var _this = this

  },
  onShow: function () {
    var _this = this
    app.globalData.skeletonPage = _this
  },
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

    if (_this.data.orderIds.length > 0) {
      return
    }

    var faceTypes = e.currentTarget.dataset.replyFacetypes
    //console.log('faceTypes:' + faceTypes)
    var couponIds
    if (faceTypes == '1,2') {
      couponIds = _this.data.couponIdsByShop
    } else if (faceTypes == '3') {
      couponIds = [_this.data.couponIdByRent]
    } else if (faceTypes == '4') {
      couponIds = [_this.data.couponIdByDeposit]
    }

    var skus = _this.data.skus
    var shopMethod = _this.data.shopMethod
    var storeId = _this.data.storeId

    wx.navigateTo({
      url: "/pages/mycoupon/mycoupon?operate=2&isGetHis=false&skus=" + JSON.stringify(skus) + "&couponIds=" + JSON.stringify(couponIds) + '&shopMethod=' + shopMethod + "&storeId=" + storeId + '&faceTypes=' + faceTypes,
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
          shopMode: _skus[j].shopMode,
          shopMethod: _skus[j].shopMethod,
          shopId: ''
        })
      }


      var _delivery = _blocks[i].delivery
      var _selfTake = _blocks[i].selfTake

      if (_blocks[i].receiveMode == 1) {
        if (util.isEmptyOrNull(_delivery.contact.id)) {
          toast.show({
            title: '请选择快寄地址'
          })
          return
        }
      } else if (_blocks[i].receiveMode == 2) {

        if (util.isEmptyOrNull(_selfTake.mark.id)) {
          toast.show({
            title: '请选择自提地址'
          })
          return
        }

        if (util.isEmptyOrNull(_selfTake.bookTime.value)) {
          toast.show({
            title: '请选择预约时间'
          })
          return
        }

        if (util.isEmptyOrNull(_selfTake.contact.id)) {
          toast.show({
            title: '请选择联系人'
          })
          return
        }

        for (var j = 0; j < _skus.length; j++) {
          skus[j].shopId = _selfTake.mark.id
        }

      } else if (_blocks[i].receiveMode == 4) {

        if (util.isEmptyOrNull(_selfTake.mark.id)) {
          toast.show({
            title: '请选择自提地址'
          })
          return
        }

        // if (util.isEmptyOrNull(_selfTake.contact.id)) {
        //   toast.show({
        //     title: '请选择联系人'
        //   })
        //   return
        // }

        for (var j = 0; j < _skus.length; j++) {
          skus[j].shopId = _selfTake.mark.id
        }
      }

      blocks.push({
        shopMode: _blocks[i].shopMode,
        receiveMode: _blocks[i].receiveMode,
        delivery: _blocks[i].delivery,
        selfTake: _blocks[i].selfTake,
        skus: skus
      })

    }

    if (util.isEmptyOrNull(_this.data.orderIds)) {
      apiOrder.reserve({
        pOrderId: _this.data.pOrderId,
        storeId: _this.data.storeId,
        blocks: blocks,
        source: 3,
        reffSign: storeage.getReffSign(),
        saleOutletId: _this.data.saleOutletId,
        couponIdsByShop: _this.data.couponIdsByShop,
        couponIdByDeposit: _this.data.couponIdByDeposit,
        couponIdByRent: _this.data.couponIdByRent,
        shopMethod: _this.data.shopMethod
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



          if (_this.data.couponByShop != null) {
            if (_this.data.couponByShop.selectedCouponIds == null || _this.data.couponByShop.selectedCouponIds.length == 0) {
              _this.data.couponByShop.tipType = 3
              _this.data.couponByShop.tipMsg = "-0.0"
            }
          }

          if (_this.data.couponByDeposit != null) {
            if (_this.data.couponByDeposit.selectedCouponIds == null || _this.data.couponByDeposit.selectedCouponIds.length == 0) {
              _this.data.couponByDeposit.tipType = 3
              _this.data.couponByDeposit.tipMsg = "-0.0"
            }
          }

          if (_this.data.couponByRent != null) {

            if (_this.data.couponByRent.selectedCouponIds == null || _this.data.couponByRent.selectedCouponIds.length == 0) {
              _this.data.couponByRent.tipType = 3
              _this.data.couponByRent.tipMsg = "-0.0"
            }
          }

          _this.setData({
            orderIds: orderIds,
            couponByShop: _this.data.couponByShop,
            couponByDeposit: _this.data.couponByDeposit,
            couponByRent: _this.data.couponByRent
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
    if (parseInt(_this.data.actualAmount) == 0) {
      wx.showModal({
        title: '提示',
        content: '确定要支付吗？',
        success: function (sm) {
          if (sm.confirm) {
            _this.goPayCofirm(payOption, blocks)
          }
        }
      })
    } else {
      _this.goPayCofirm(payOption, blocks)
    }
  },
  goPayCofirm: function (payOption, blocks) {
    var _this = this
    var data = _this.data
    // console.log('_this.data.action:' + data.action)
    apiOrder.buildPayParams({
      orderIds: data.orderIds,
      payCaller: payOption.payCaller,
      blocks: blocks,
      payPartner: payOption.payPartner,
      couponIdsByShop: data.couponIdsByShop,
      couponIdByDeposit: data.couponIdByDeposit,
      couponIdByRent: data.couponIdByRent,
      shopMethod: data.shopMethod
    }).then(function (res) {
      if (res.result == 1) {

        var d = res.data

        if (res.code == '1040') {
          wx.redirectTo({
            url: '/pages/operate/operate?id=' + d.payTransId + '&type=1&caller=1&action=' + data.action
          })
        } else {
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
                url: '/pages/operate/operate?id=' + d.payTransId + '&type=1&caller=1&action=' + data.action
              })
            },
            'fail': function (res) {
              wx.redirectTo({
                url: '/pages/operate/operate?id=' + d.payTransId + '&type=2&caller=1&action=' + data.action
              })
            }
          })
        }
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

    //console.log("blockindex:" + blockindex)
    //console.log("tabmode:" + tabmode)
    //console.log("receivemode:" + receivemode)

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
      skus: _data.skus,
      couponIdsByShop: _data.couponIdsByShop,
      couponIdByRent: _data.couponIdByRent,
      couponIdByDeposit: _data.couponIdByDeposit,
      shopMethod: _data.shopMethod
    }).then(function (res) {
      if (res.result == 1) {
        var d = res.data

        var couponIdsByShop = []
        if (d.couponByShop != null) {
          couponIdsByShop = d.couponByShop.selectedCouponIds
        }

        var couponIdByRent = ''
        if (d.couponByRent != null) {
          if (d.couponByRent.selectedCouponIds != null && d.couponByRent.selectedCouponIds.length > 0)
            couponIdByRent = d.couponByRent.selectedCouponIds[0]
        }

        var couponIdByDeposit = ''
        if (d.couponByDeposit != null) {
          if (d.couponByDeposit.selectedCouponIds != null && d.couponByDeposit.selectedCouponIds.length > 0)
            couponIdByDeposit = d.couponByDeposit.selectedCouponIds[0]
        }

        _this.setData({
          blocks: d.blocks,
          subtotalItems: d.subtotalItems,
          actualAmount: d.actualAmount,
          originalAmount: d.originalAmount,
          couponByShop: d.couponByShop,
          couponByRent: d.couponByRent,
          couponByDeposit: d.couponByDeposit,
          couponIdsByShop: couponIdsByShop,
          couponIdByRent: couponIdByRent,
          couponIdByDeposit: couponIdByDeposit,
          shopMethod: d.shopMethod,
          pageIsReady: true
        })
      }
    })
  },
  clickToOpenBooktimeDialog: function (e) {

    var _this = this
    var blockIndex = e.currentTarget.dataset.replyBlockindex
    var booktime = _this.data.blocks[blockIndex].selfTake.bookTime

    _this.setData({
      curBookTimeBlockIndex: blockIndex,
      booktimeDialog: {
        isShow: true,
        dataS: {
          bookTime: booktime
        }
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


    _this.data.blocks[_this.data.curBookTimeBlockIndex].selfTake.bookTime = booktime

    _this.setData({
      blocks: _this.data.blocks
    })
  },
  clickToOpenSelfPickAddressDialog: function (e) {
    var _this = this
    var blockIndex = e.currentTarget.dataset.replyBlockindex

    _this.setData({
      curSelfPickAddressBlockIndex: blockIndex
    })

    var selfPickAddressDialog = _this.data.selfPickAddressDialog
    selfPickAddressDialog.isShow = true
    selfPickAddressDialog.dataS.curSelfPickAddressId = _this.data.blocks[blockIndex].selfTake.mark.id
    _this.setData({
      selfPickAddressDialog: selfPickAddressDialog
    })
  },
  selectSelfPickAddressItem: function (e) {
    var _this = this
    var selfPickAddress = e.detail.selfPickAddress

    var blockIndex = _this.data.curSelfPickAddressBlockIndex

    _this.data.blocks[blockIndex].selfTake.mark.id = selfPickAddress.id
    _this.data.blocks[blockIndex].selfTake.mark.name = selfPickAddress.name
    _this.data.blocks[blockIndex].selfTake.mark.address = selfPickAddress.address
    _this.data.blocks[blockIndex].selfTake.mark.areaCode = selfPickAddress.areaCode
    _this.data.blocks[blockIndex].selfTake.mark.areaName = selfPickAddress.areaName

    _this.setData({
      blocks: _this.data.blocks
    })

  }

})