const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiIndex = require('../../api/index.js')
const apiCart = require('../../api/cart.js')

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
    tag: "index",
    storeId: undefined,
    isOnLoad: false,
    pageIsReady: false,
    skeletonLoadingTypes: ['spin', 'chiaroscuro', 'shine', 'null'],
    skeletonSelectedLoadingType: 'shine',
    skeletonIsDev: false,
    skeletonBgcolor: '#FFF',
    skeletonData,
    shopMode: 0,
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
        }
        else {
          toast.show({
            title: res.message
          })
        }
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

          var shopMode
          d.shopModes.forEach(function (item, index) {
            if (item.selected) {
              shopMode = item.id
            }
          })
          app.globalData.currentShopMode = shopMode

          _this.setData({
            shopMode: shopMode,
            shopModes: d.shopModes,
            singleStore: typeof storeage.getStoreId() == "undefined" ? false : true,
            currentStore: d.store,
            banner: d.banner,
            pdArea: d.pdArea,
            pageIsReady: true
          })
        }
        else if (res.result == 2) {
          if (res.code == '2404') {//当前店铺无效，重新选择
            wx.navigateTo({ 
              url: "/pages/store/store"
            })
          }
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
      _this.setData({ shopMode: shopMode })
      _this.getPageData()

    },
    onReady: function () {
      console.log("index.onReady")
    },
    onShow() {
      console.log("index.onShow")
      var _this = this
      app.globalData.skeletonPage = _this;

      if (!_this.data.isOnLoad) {
        _this.setData({
          isOnLoad: true,
          storeId: ownRequest.getCurrentStoreId()
        })
        _this.getPageData()
      }

    },
    imageOnloadError: function (e) {
      console.log('das')
    }
  }
})