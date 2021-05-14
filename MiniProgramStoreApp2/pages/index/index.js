const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiIndex = require('../../api/index.js')
const apiCart = require('../../api/cart.js')
const apiGlobal = require('../../api/global.js')
const skeletonData = require('./skeletonData')

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

      var skus = new Array();
      skus.push({
        id: skuId,
        quantity: 1,
        selected: true,
        shopMode: _this.data.shopMode,
        shopId: '0',
      });

      apiCart.operate({
        storeId: _this.data.storeId,
        operate: 2,
        skus: skus
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
        skus: skus
      })
    },
    selectSpecs: function (e) {
      var _this = this
      var sku = e.currentTarget.dataset.replySku
      _this.setData({
        specsDialog: {
          isShow: true,
          sku: sku,
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
    onReady: function () {
      console.log("index.onReady")
    },
    onShow() {
      console.log("index.onShow")
      var _this = this
      app.globalData.skeletonPage = _this

      _this.setData({
        storeId: storeage.getStoreId()
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
      var skus = []
      skus.push({
        cartId: 0,
        id: skuId,
        quantity: 1,
        shopMode: _this.data.shopMode,
        shopMethod: 2,
        shopId: '0'
      })
      wx.navigateTo({
        url: '/pages/orderconfirm/orderconfirm?skus=' +  encodeURIComponent(JSON.stringify(skus)) + '&shopMethod=2',
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