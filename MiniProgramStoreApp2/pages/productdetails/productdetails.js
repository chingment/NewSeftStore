const storeage = require('../../utils/storeageutil.js')
const wxparse = require("../../wxParse/wxParse.js")
const cart = require('../../api/cart.js')
const product = require('../../api/product.js')
const ownRequest = require('../../own/ownRequest.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "productdetails",
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function(options) {
    var _this = this
    var id = options.id == undefined ? "0" : options.id

    //app.changeData("main", { cart: cart })

    product.details({
      id: id
    }, {
        success: function (res) {
          _this.setData({
            product: res.data,
            cart: storeage.getCart()
          })
          wxparse.wxParse('dkcontent', 'html', res.data.detailsDes, _this, 0);
        },
        fail: function () { }
      })
  },
  goHome: function(e) {
    app.mainTabBarSwitch(0)
  },
  goCart: function(e) {
    app.mainTabBarSwitch(2)
  },
  addToCart: function(e) {

    var _self = this
    var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index
    console.log("skuId:" + skuId)
    var productSkus = new Array();
    productSkus.push({
      id: skuId,
      quantity: 1,
      selected: true,
      receptionMode: 3
    });

    cart.operate({
      storeId: ownRequest.getCurrentStoreId(),
      operate: 2,
      productSkus: productSkus
    }, {
      success: function(res) {

      },
      fail: function() {

      }
    })

  },

  immeBuy: function(e) {
    var _this = this
    var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index
    var productSkus = []
    productSkus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      receptionMode: 3
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus),
      success: function(res) {
        // success
      },
    })
  }

})