const config = require('../../config')
const storeage = require('../../utils/storeageutil.js')
const wxparse = require("../../wxParse/wxParse.js")
const cart = require('../../pages/cart/cart.js')
const ownRequest = require('../../own/ownRequest.js')
const lumos = require('../../utils/lumos.minprogram.js')
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

    lumos.getJson({
      url: config.apiUrl.productGetDetails,
      urlParams: {
        id: id
      },
      success: function(res) {
        _this.setData({
          product: res.data,
          cart: storeage.getCart()
        })
        wxparse.wxParse('dkcontent', 'html', res.data.detailsDes, _this, 0);
      }
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
    var id = e.currentTarget.dataset.replyId //对应页面data-reply-index
    console.log("id:" + id)
    var products = new Array();
    products.push({
      id: id,
      quantity: 1,
      selected: true,
      receptionMode: 3
    });

    cart.operate({
      storeId: ownRequest.getCurrentStoreId(),
      operate: 2,
      products: products
    }, {
      success: function(res) {

      },
      fail: function() {

      }
    })

  },

  immeBuy: function(e) {
    var _this = this
    var products = []
    products.push({
      cartId: 0,
      id: _this.data.product.id,
      quantity: 1,
      receptionMode: 3
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?products=' + JSON.stringify(products),
      success: function(res) {
        // success
      },
    })
  }

})