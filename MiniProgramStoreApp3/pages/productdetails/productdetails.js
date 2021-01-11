const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiProduct = require('../../api/product.js')
const config = require('../../config');
const pageMain = require('../../pages/main/main.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "productdetails",
    storeId: null,
    shopMode: null,
    shopMethod: 1,
    specsDialog: {
      isShow: false
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
    var shopMode = options.shopMode == undefined ? undefined : options.shopMode
    var shopMethod = options.shopMethod == undefined ? 1 : options.shopMethod
    var reffSign = options.reffSign == undefined ? '' : options.reffSign

    if (storeId == undefined) {
      storeId = ownRequest.getCurrentStoreId()
    }

    if (shopMode == undefined) {
      shopMode = storeage.getCurrentShopMode()
    }

    console.log("reffSign:" + reffSign)

    storeage.setReffSign(reffSign)

    storeage.setCurrentShopMode(shopMode)

    ownRequest.setCurrentStoreId(storeId)

    _this.setData({
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
      shopMethod: shopMethod
    }).then(function (res) {
      if (res.result == 1) {
        var d = res.data
        _this.setData({
          productSku: d
        })

        app.byPoint(_this.data.tag, 'browse_spu', {
          productSkuId: d.id,
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
  addToCart: function (e) {
    var _this = this

    if (_this.data.productSku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index

    var productSkus = new Array();
    productSkus.push({
      id: skuId,
      quantity: 1,
      selected: true,
      shopMode: _this.data.shopMode
    });

    apiCart.operate({
      storeId: _this.data.storeId,
      operate: 2,
      productSkus: productSkus
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
    var sku = _this.data.productSku
    _this.setData({
      specsDialog: {
        isShow: true,
        productSku: sku,
        shopMode: _this.data.shopMode,
        storeId: _this.data.storeId
      }
    })
  },
  immeBuy: function (e) {
    var _this = this

    if (_this.data.productSku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    var skuId = _this.data.productSku.id //对应页面data-reply-index
    var productSkus = []
    productSkus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      shopMode: _this.data.shopMode,
      shopMethod: 1
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus) + '&shopMethod=' + _this.data.shopMethod,
      success: function (res) {
        // success
      },
    })
  },
  immeRent: function (e) {
    var _this = this

    if (_this.data.productSku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    var skuId = _this.data.productSku.id //对应页面data-reply-index
    var productSkus = []
    productSkus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      shopMode: _this.data.shopMode,
      shopMethod: _this.data.shopMethod
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus) + '&shopMethod=' + _this.data.shopMethod,
      success: function (res) {
        // success
      },
    })
  },
  immeMavkBuy:function(e){
    var _this = this

    if (_this.data.productSku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    var skuId = _this.data.productSku.id //对应页面data-reply-index
    var productSkus = []
    productSkus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      shopMode: _this.data.shopMode,
      shopMethod: _this.data.shopMethod
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus) + '&shopMethod=' + _this.data.shopMethod,
      success: function (res) {
        // success
      },
    })
  },
  immeSawRent: function (e) {
    var _this = this
    var skuId = _this.data.productSku.id //对应页面data-reply-index

    wx.navigateTo({
      url: '/pages/productdetails/productdetails?skuId=' + skuId + '&shopMethod=2',
      success: function (res) {
        // success
      },
    })

  },
  updateProductSku: function (e) {
    var _this = this
    var selSku = e.detail.productSku

    console.log("updateParent:" + JSON.stringify(selSku))

    var productSku = _this.data.productSku
    productSku.id = selSku.id
    productSku.name = selSku.name
    productSku.isOffSell = selSku.isOffSell
    productSku.salePrice = selSku.salePrice
    productSku.isShowPrice = selSku.isShowPrice
    productSku.salePriceByVip = selSku.salePriceByVip
    productSku.sellQuantity = selSku.sellQuantity
    productSku.specIdx = selSku.specIdx

    _this.setData({
      productSku: productSku
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
    // 设置转发内容
    var shareObj = {
      title: _this.data.productSku.name,
      path: '/pages/productdetails/productdetails?reffSign=' + storeage.getOpenId() + '&skuId=' + _data.productSku.id + '&shopMode=' + _data.shopMode + '&shopMethod=' + _data.shopMethod + '&storeId=' + _data.storeId + "&merchId=" + storeage.getMerchId(), // 默认是当前页面，必须是以‘/’开头的完整路径
      imgUrl: '', //转发时显示的图片路径，支持网络和本地，不传则使用当前页默认截图。
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

  }
})