const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiProduct = require('../../api/product.js')
const config = require('../../config');
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "productdetails",
    cartIsShow: false,
    storeId: null,
    shopMode: null,
    specsDialog: {
      isShow: false
    }
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var skuId = options.skuId == undefined ? "0" : options.skuId
    var storeId = options.storeId == undefined ? undefined : options.storeId
    var shopMode = options.shopMode == undefined ? undefined : options.shopMode

    if (storeId == undefined) {
      storeId = ownRequest.getCurrentStoreId()
    }

    if (shopMode == undefined) {
      shopMode = app.globalData.currentShopMode
    }

    app.globalData.currentShopMode = shopMode

    ownRequest.setCurrentStoreId(storeId)

    _this.setData({
      storeId: storeId,
      shopMode: shopMode
    })

    var cart = {
      count: 0
    }
    if (ownRequest.isLogin()) {
      cart = storeage.getCart()
    }

    apiProduct.details({
      storeId: storeId,
      skuId: skuId,
      shopMode: shopMode
    }).then(function (res) {
      if (res.result == 1) {
        _this.setData({
          productSku: res.data,
          cart: cart,
        })
      }
    })
  },
  goHome: function (e) {
    app.mainTabBarSwitch(0)
  },
  goCart: function (e) {
    var _this = this
    _this.setData({ cartIsShow: true })
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
      storeId: ownRequest.getCurrentStoreId(),
      operate: 2,
      productSkus: productSkus
    }).then(function (res) {
      if (res.result == 1) {
        toast.show({
          title: '加入购物车成功'
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
      shopMode: _this.data.shopMode
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus),
      success: function (res) {
        // success
      },
    })
  },
  onShareAppMessage: function (options) {
    var _this = this;
    // 设置转发内容
    var shareObj = {
      title: _this.data.productSku.name,
      path: '/pages/productdetails/productdetails?skuId=' + _this.data.productSku.id + '&shopMode=' + _this_data.shopMode + '&storeId=' + ownRequest.getCurrentStoreId() + "&merchId=" + config.merchId, // 默认是当前页面，必须是以‘/’开头的完整路径
      imgUrl: '', //转发时显示的图片路径，支持网络和本地，不传则使用当前页默认截图。
      success: function (res) { // 转发成功之后的回调　　　　　
        if (res.errMsg == 'shareAppMessage:ok') { }
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
      var dataid = options.target.dataset; //上方data-id=shareBtn设置的值
      // 此处可以修改 shareObj 中的内容
      shareObj.path = '/pages/btnname/btnname?id=' + dataid.id;
    }
    // 返回shareObj
    return shareObj;

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

    _this.setData({ productSku: productSku })

  }
})