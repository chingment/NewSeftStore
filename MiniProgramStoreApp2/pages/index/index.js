const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiIndex = require('../../api/index.js')
const apiCart = require('../../api/cart.js')
const apiGlobal = require('../../api/global.js')
const skeletonData = require('./skeletonData');

const app = getApp()
Component({
  options: {
    addGlobalClass: true,
    multipleSlots: true // 在组件定义时的选项中启用多slot支持
  },
  properties: {
    height: {
      type: Number
    }
  },
  data: {
    tag: "main-index",
    storeId: undefined,
    isOnReady: false,
    pageIsReady: false,
    skeletonLoadingTypes: ['spin', 'chiaroscuro', 'shine', 'null'],
    skeletonSelectedLoadingType: 'shine',
    skeletonIsDev: false,
    skeletonBgcolor: '#FFF',
    skeletonData,
    shopMode: 1,
    specsDialog: {
      isShow: false
    }
  },
  methods: {
    addToCart: function (e) {
      var _this = this
      var skuId = e.currentTarget.dataset.replySkuid
      var sku = e.currentTarget.dataset.replySku

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
        } else {
          toast.show({
            title: res.message
          })
        }
      })

      apiGlobal.byPoint(_this.data.tag, "op_cart", {
        operate: 2,
        productSkus: productSkus
      })
    },
    selectSpecs: function (e) {
      var _this = this
      var sku = e.currentTarget.dataset.replySku
      _this.setData({
        specsDialog: {
          isShow: true,
          productSku: sku,
          shopMode: _this.data.shopMode,
          storeId: _this.data.storeId,
        }
      })
    },
    getPageData: function () {
      var _this = this
      apiIndex.pageData({
        storeId: _this.data.storeId,
        shopMode: _this.data.shopMode
      }).then(function (res) {

        if (res.result === 1) {
          var d = res.data

          _this.setData({
            isSupMachineShop: d.isSupMachineShop,
            currentStore: d.store,
            banner: d.banner,
            pageIsReady: true
          })

          _this.getSugProducts()

        }
      })
    },
    getSugProducts: function () {
      var _this = this
      apiIndex.sugProducts({
        storeId: _this.data.storeId,
        shopMode: _this.data.shopMode
      }).then(function (res) {

        if (res.result === 1) {
          var d = res.data
          _this.setData({
            pdRent: d.pdRent,
            pdArea: d.pdArea
          })

        }
      })
    },
    goSelectStore: function (e) {

      var _this = this

      if (_this.data.singleStore)
        return

      wx.navigateTo({
        url: '/pages/store/store',
      })
    },
    switchShopMode: function (e) {
      var _this = this
      var shopMode = e.currentTarget.dataset.replyShopmodeid //对应页面\
      _this.setData({
        shopMode: shopMode
      })
      _this.getPageData()

    },
    onReady: function () {
      console.log("index.onReady")
    },
    onShow() {
      console.log("index.onShow")
      var _this = this
      app.globalData.skeletonPage = _this;

      _this.setData({
        storeId: ownRequest.getCurrentStoreId()
      })
      _this.getPageData()

    },
    imageOnloadError: function (e) {
      console.log('das')
    },
    clickToRent: function (e) {

      var _this = this

      var sku = e.currentTarget.dataset.replySku

      if (sku.isOffSell) {
        toast.show({
          title: '商品已下架'
        })
        return
      }

      if (!ownRequest.isLogin()) {
        ownRequest.goLogin()
        return
      }

      var skuId = sku.id //对应页面data-reply-index
      var productSkus = []
      productSkus.push({
        cartId: 0,
        id: skuId,
        quantity: 1,
        shopMode: _this.data.shopMode,
        shopMethod: 2
      })
      wx.navigateTo({
        url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus) + '&shopMethod=2',
        success: function (res) {
          // success
        },
      })

    },
    clickToFc: function (e) {
      var url = e.currentTarget.dataset.url
      var ischecklogin = e.currentTarget.dataset.ischecklogin
      if (ischecklogin == "true") {
        if (!ownRequest.isLogin()) {
          ownRequest.goLogin(url)
          return
        }
      }

      wx.navigateTo({
        url: url
      })
    },
  }
})