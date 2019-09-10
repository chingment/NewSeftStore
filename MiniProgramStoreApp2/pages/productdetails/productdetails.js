const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const wxparse = require("../../components/wxParse/wxParse.js")
const apiCart = require('../../api/cart.js')
const apiProduct = require('../../api/product.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "productdetails",
    isShowCart:false
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var id = options.id == undefined ? "0" : options.id

    //app.changeData("main", { cart: cart })

    apiProduct.details({
      id: id
    }, {
        success: function (res) {
          if (res.result == 1) {
            _this.setData({
              product: res.data,
              cart: storeage.getCart()
            })

            var detailsDes = res.data.detailsDes
            if (detailsDes==null){
              detailsDes=""
            }
            wxparse.wxParse('dkcontent', 'html', detailsDes, _this, 0);
          }
        },
        fail: function () { }
      })
  },
  goHome: function (e) {
    app.mainTabBarSwitch(0)
  },
  goCart: function (e) {
    //app.mainTabBarSwitch(2)

    this.selectComponent("#cart").open()
  },
  addToCart: function (e) {

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

    apiCart.operate({
      storeId: ownRequest.getCurrentStoreId(),
      operate: 2,
      productSkus: productSkus
    }, {
        success: function (res) {

        },
        fail: function () {

        }
      })

  },

  immeBuy: function (e) {
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
      success: function (res) {
        // success
      },
    })
  }

})