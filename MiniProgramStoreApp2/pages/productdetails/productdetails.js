const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
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
    var skuId = options.skuId == undefined ? "0" : options.skuId

    //app.changeData("main", { cart: cart })

    apiProduct.details({
      storeId: ownRequest.getCurrentStoreId(),
      skuId: skuId
    }, {
        success: function (res) {
          if (res.result == 1) {
            _this.setData({
              productSku: res.data,
              cart: storeage.getCart()
            })
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

    if (_self.data.productSku.isOffSell) {
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
      receptionMode: 3
    });

    apiCart.operate({
      storeId: ownRequest.getCurrentStoreId(),
      operate: 2,
      productSkus: productSkus
    }, {
        success: function (res) {
            if(res.result==1)
            {

            }
            else{
              toast.show({
                title: res.message
              })
            }
        },
        fail: function () {

        }
      })

  },

  immeBuy: function (e) {
    var _this = this

    if(_this.data.productSku.isOffSell){
      toast.show({
        title:'商品已下架'
      })
      return
    }

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }
    
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