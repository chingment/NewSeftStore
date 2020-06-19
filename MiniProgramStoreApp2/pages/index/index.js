const config = require('../../config')
const toast = require('../../utils/toastutil')
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
      var sku= e.currentTarget.dataset.replySku

      var productSkus = new Array();
      productSkus.push({
        id: skuId,
        quantity: 1,
        selected: true,
        shopMode: app.globalData.currentShopMode
      });

      // apiCart.operate({
      //   storeId: ownRequest.getCurrentStoreId(),
      //   operate: 2,
      //   productSkus: productSkus
      // })

      _this.setData({
        specsDialog: {
          isShow: true,
          productSku:sku
        }
      })

    },
    getPageData: function () {
      var _this = this

      if (ownRequest.getCurrentStoreId() != undefined) {
        apiIndex.pageData({
          storeId: ownRequest.getCurrentStoreId(),
          shopMode: app.globalData.currentShopMode
        }).then(function (res) {

          if (res.result === 1) {
            var d = res.data

            d.shopModes.forEach(function (shopMode, index) {
              if (shopMode.selected) {
                app.globalData.currentShopMode = shopMode.id
              }
            })

            _this.setData({
              shopModes: d.shopModes,
              singleStore: typeof config.storeId == "undefined" ? false : true,
              currentStore: d.store,
              banner: d.banner,
              pdArea: d.pdArea,
              pageIsReady: true
            })
          }
        })

      }

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
      var shopMode = e.currentTarget.dataset.replyShopmodeid //对应页面data-reply-index

      app.globalData.currentShopMode = shopMode

      this.getPageData()

    },
    onReady: function () {
      console.log("index.onReady")
    },
    onShow() {
      console.log("index.onShow")
      var _this = this
      app.globalData.skeletonPage = _this;

      if (!_this.data.isOnLoad) {
        _this.setData({ isOnLoad: true })
        _this.getPageData()
      }

    }
  }
})