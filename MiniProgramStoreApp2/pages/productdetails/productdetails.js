const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiProduct = require('../../api/product.js')
const apiOwn = require('../../api/own.js')
const pageMain = require('../../pages/main/main.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "productdetails",
    storeId: null,
    shopMode: 1,
    shopId: '0',
    shopMethod: 1,
    specsDialog: {
      isShow: false
    },
    shareDialog: {
      isShow: false,
      dataS: {}
    },
    proSign: '',
    cartDialog: {
      isShow: false,
      dataS: {
        blocks: [],
        count: 0,
        sumPrice: 0,
        countBySelected: 0,
        sumPriceBySelected: 0
      }
    },
    canvasPoster: {
      isShow: false,
      shareImage: '',
      painting: []
    }
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    console.log("==>child.onLoad==");
    var _this = this

    var skuId = options.skuId == undefined ? "0" : options.skuId
    var storeId = options.storeId == undefined ? undefined : options.storeId
    var shopMode = options.shopMode == undefined ? 1 : options.shopMode
    var shopMethod = options.shopMethod == undefined ? 1 : options.shopMethod
    var shopId = options.shopId == undefined ? '0' : options.shopId
    var reffSign = options.reffSign == undefined ? '' : options.reffSign

    if (storeId == undefined) {
      storeId = storeage.getStoreId()
    }


    console.log("reffSign:" + reffSign)

    storeage.setReffSign(reffSign)


    storeage.setStoreId(storeId)

    _this.setData({
      shopId: shopId,
      storeId: storeId,
      shopMode: shopMode,
      shopMethod: shopMethod
    })


    if (ownRequest.isLogin()) {

      apiCart.getCartData({
        storeId: storeId,
        shopMode: shopMode
      }).then(function (res) {
        if (res.result == 1) {
          var cartDialog = _this.data.cartDialog
          cartDialog.dataS = res.data
          _this.setData({
            cartDialog: cartDialog
          })
        }
      })
    }

    apiProduct.details({
      storeId: storeId,
      skuId: skuId,
      shopMode: shopMode,
      shopMethod: shopMethod,
      shopId: _this.data.shopId
    }).then(function (res) {
      if (res.result == 1) {
        var d = res.data
        _this.setData({
          sku: d
        })

        app.byPoint(_this.data.tag, 'browse_spu', {
          skuId: d.id,
          kindId1: d.kindId1,
          kindId2: d.kindId2,
          kindId3: d.kindId3
        })
      }
    })
  },
  goHome: function (e) {
    pageMain.mainTabBarSwitch(0)
    //app.mainTabBarSwitch(0)
  },
  goCart: function (e) {
    console.log("goCart")
    var _this = this
    var cartDialog = _this.data.cartDialog
    cartDialog.isShow = true
    _this.setData({
      cartDialog: cartDialog
    })
  },
  goShare: function () {
    var _this = this
    var shareDialog = _this.data.shareDialog
    shareDialog.isShow = true
    _this.setData({
      shareDialog: shareDialog
    })
  },
  addToCart: function (e) {
    var _this = this

    if (_this.data.sku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index

    var skus = new Array();
    skus.push({
      id: skuId,
      quantity: 1,
      selected: true,
      shopMode: _this.data.shopMode,
      shopId: _this.data.shopId
    });

    apiCart.operate({
      storeId: _this.data.storeId,
      operate: 2,
      skus: skus,
      shopId: _this.data.shopId
    }).then(function (res) {
      if (res.result == 1) {
        toast.show({
          title: '加入购物车成功'
        })

        var cartDialog = _this.data.cartDialog
        cartDialog.dataS = res.data
        cartDialog.isShow = false

        _this.setData({
          cartDialog: cartDialog
        })
      } else {
        toast.show({
          title: res.message
        })
      }
    })

  },
  selectSpecs: function (e) {
    var _this = this
    var sku = _this.data.sku
    _this.setData({
      specsDialog: {
        isShow: true,
        sku: sku,
        shopMode: _this.data.shopMode,
        storeId: _this.data.storeId
      }
    })
  },
  immeBuy: function (e) {
    var _this = this

    if (_this.data.sku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    var skuId = _this.data.sku.id //对应页面data-reply-index
    var skus = []
    skus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      shopMode: _this.data.shopMode,
      shopMethod: 1,
      shopId: _this.data.shopId
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?skus=' +  encodeURIComponent(JSON.stringify(skus)) + '&shopMethod=' + _this.data.shopMethod,
      success: function (res) {
        // success
      },
    })
  },
  immeRent: function (e) {
    var _this = this

    if (_this.data.sku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    var skuId = _this.data.sku.id //对应页面data-reply-index
    var skus = []
    skus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      shopMode: _this.data.shopMode,
      shopMethod: _this.data.shopMethod,
      shopId: _this.data.shopId
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?skus=' +  encodeURIComponent(JSON.stringify(skus)) + '&shopMethod=' + _this.data.shopMethod,
      success: function (res) {
        // success
      },
    })
  },
  immeMavkBuy: function (e) {
    var _this = this

    if (_this.data.sku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    var skuId = _this.data.sku.id //对应页面data-reply-index
    var skus = []
    skus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      shopMode: _this.data.shopMode,
      shopMethod: _this.data.shopMethod,
      shopId: _this.data.shopId
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?skus=' +  encodeURIComponent(JSON.stringify(skus)) + '&shopMethod=' + _this.data.shopMethod,
      success: function (res) {
        // success
      },
    })
  },
  immeSawRent: function (e) {
    var _this = this
    var skuId = _this.data.sku.id //对应页面data-reply-index

    wx.navigateTo({
      url: '/pages/productdetails/productdetails?skuId=' + skuId + '&shopMethod=2',
      success: function (res) {
        // success
      },
    })

  },
  updateSku: function (e) {
    var _this = this
    var selSku = e.detail.sku

    console.log("updateParent:" + JSON.stringify(selSku))

    var sku = _this.data.sku
    sku.id = selSku.id
    sku.name = selSku.name
    sku.isOffSell = selSku.isOffSell
    sku.salePrice = selSku.salePrice
    sku.isShowPrice = selSku.isShowPrice
    sku.salePriceByVip = selSku.salePriceByVip
    sku.sellQuantity = selSku.sellQuantity
    sku.specIdx = selSku.specIdx

    _this.setData({
      sku: sku
    })
  },
  cartdialogClose: function () {
    var _this = this
    var cartDialog = _this.data.cartDialog
    cartDialog.isShow = false
    _this.setData({
      cartDialog: cartDialog
    })
  },
  onShareAppMessage: function (options) {

    var _this = this
    var _data = _this.data

    console.log(_this.data.sku.mainImgUrl)
    // 设置转发内容
    var shareObj = {
      title: _this.data.sku.name,
      path: '/pages/productdetails/productdetails?reffSign=' + storeage.getOpenId() + '&skuId=' + _data.sku.id + '&shopMode=' + _data.shopMode + '&shopMethod=' + _data.shopMethod + '&storeId=' + _data.storeId + "&merchId=" + storeage.getMerchId(), // 默认是当前页面，必须是以‘/’开头的完整路径
      imageUrl: _this.data.sku.mainImgUrl, //转发时显示的图片路径，支持网络和本地，不传则使用当前页默认截图。
      success: function (res) { // 转发成功之后的回调　　　　　
        if (res.errMsg == 'shareAppMessage:ok') {}
      },
      fail: function () { // 转发失败之后的回调
        if (res.errMsg == 'shareAppMessage:fail cancel') {
          // 用户取消转发
        } else if (res.errMsg == 'shareAppMessage:fail') {
          // 转发失败，其中 detail message 为详细失败信息
        }
      },
      complete: function () {
        // 转发结束之后的回调（转发成不成功都会执行）
      }
    };
    // 来自页面内的按钮的转发
    if (options.from == 'button') {


    }
    // 返回shareObj
    return shareObj;

  },
  onHide: function () {
    console.log("==>onHide==");
  },
  onUnload: function () {

  },
  onShow: function () {

  },
  buildPoster: function () {
    var _this = this
    var _data = _this.data
    console.log('buildPoster')

    var accountInfo = wx.getAccountInfoSync()
    var appId = accountInfo.miniProgram.appId
    var wxacode_data = '/pages/productdetails/productdetails?reffSign=' + storeage.getOpenId() + '&skuId=' + _data.sku.id + '&shopMode=' + _data.shopMode + '&shopMethod=' + _data.shopMethod + '&storeId=' + _data.storeId + "&merchId=" + storeage.getMerchId()
    apiOwn.getWxACodeUnlimit({
      appId: appId,
      merchId: storeage.getMerchId(),
      openId: storeage.getOpenId(),
      data: wxacode_data,
      type: 'url',
      isGetAvatar: true,
    }).then(function (res) {
      if (res.result == 1) {
        var d = res.data

        _this.setData({
          canvasPoster: {
            isShow: true,
            painting: {
              width: 375,
              height: 555,
              clear: true,
              views: [{
                  type: 'image',
                  url: 'https://file.17fanju.com/upload/sharecode/1/bg_white.png',
                  top: 0,
                  left: 0,
                  width: 375,
                  height: 555
                },
                {
                  type: 'image',
                  url: d.avatar,
                  top: 27.5,
                  left: 29,
                  width: 55,
                  height: 55,
                  borderRadius: 100
                },
                {
                  type: 'text',
                  content: '您的好友【' + d.nickName + '】',
                  fontSize: 16,
                  color: '#402D16',
                  textAlign: 'left',
                  top: 33,
                  left: 96,
                  bolder: true,
                },
                {
                  type: 'text',
                  content: '发现一件好货，邀请你一起购买！',
                  fontSize: 15,
                  color: '#563D20',
                  textAlign: 'left',
                  top: 59.5,
                  left: 96
                },
                {
                  type: 'image',
                  url: _this.data.sku.mainImgUrl,
                  top: 110,
                  left: 375 / 2 - 186 / 2,
                  width: 186,
                  height: 186
                },
                {
                  type: 'image',
                  url: d.wxaCodeUrl,
                  top: 443,
                  left: 85,
                  width: 68,
                  height: 68
                },
                {
                  type: 'text',
                  content: _this.data.sku.name,
                  fontSize: 16,
                  lineHeight: 21,
                  color: '#383549',
                  textAlign: 'left',
                  top: 336,
                  left: 44,
                  width: 287,
                  MaxLineNumber: 2,
                  breakWord: true,
                  bolder: true
                },
                {
                  type: 'text',
                  content: '￥' + _this.data.sku.salePrice,
                  fontSize: 19,
                  color: '#E62004',
                  textAlign: 'left',
                  top: 387,
                  left: 44.5,
                  bolder: true
                },
                // {
                //   type: 'text',
                //   content: '原价:￥138.00',
                //   fontSize: 13,
                //   color: '#7E7E8B',
                //   textAlign: 'left',
                //   top: 391,
                //   left: 110,
                //   textDecoration: 'line-through'
                // },
                {
                  type: 'text',
                  content: '长按识别图中二维码立即试一试~',
                  fontSize: 14,
                  color: '#383549',
                  textAlign: 'left',
                  top: 460,
                  left: 165.5,
                  lineHeight: 20,
                  MaxLineNumber: 2,
                  breakWord: true,
                  width: 125
                }
              ]
            }
          }
        })

      } else {
        toast.show({
          title: res.message
        })
      }
    })
  },
  buildPosterEvent(event) {
    var _this = this
    console.log("event:" + JSON.stringify(event))
    const {
      tempFilePath,
      errMsg
    } = event.detail

    if (errMsg === 'canvasdrawer:build start') {
      wx.showLoading({
        title: '正在生成海报',
        mask: true
      })
    } else if (errMsg === 'canvasdrawer:ok') {
      wx.hideLoading()
      var shareDialog = _this.data.shareDialog
      shareDialog.isShow = false
      this.setData({
        shareDialog: shareDialog
      })
    } else if (errMsg === 'canvasdrawer:share ok') {
      wx.hideLoading()

      toast.show({
        title: '保存图片成功'
      })

    } else if (errMsg === 'canvasdrawer:download fail') {
      toast.show({
        title: '生成失败'
      })
    }
  },
})