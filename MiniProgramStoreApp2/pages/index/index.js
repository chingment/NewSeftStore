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
    initdata: {
      type: Object,
      observer: function (newVal, oldVal, changedPath) {
        console.log("index.initdata")
        var _this = this
        _this.getPageData()
      }
    },
    height: {
      type: Number
    }
  },
  data: {
    pageIsReady: false,
    skeletonLoadingTypes: ['spin', 'chiaroscuro', 'shine', 'null'],
    skeletonSelectedLoadingType: 'shine',
    skeletonIsDev: false,
    skeletonBgcolor: '#FFF',
    skeletonData,
    shopMode: 0
  },
  methods: {
    addToCart: function (e) {
      var _this = this
      var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index
      var productSkus = new Array();
      productSkus.push({
        id: skuId,
        quantity: 1,
        selected: true,
        shopMode: app.globalData.currentShopMode
      });

      apiCart.operate({
        storeId: ownRequest.getCurrentStoreId(),
        operate: 2,
        productSkus: productSkus
      }, {
        success: function (res) {

        },
        fail: function () { }
      })
    },
    getPageData: function () {
      var _this = this
      if (ownRequest.getCurrentStoreId() != undefined) {
        apiIndex.pageData({
          storeId: ownRequest.getCurrentStoreId(),
          shopMode: app.globalData.currentShopMode
        }, {
          success: function (res) {

            if (res.result === 1) {
              var d = res.data

              d.shopModes.forEach(function(item, index){
                if(item.selected){
                  app.globalData.currentShopMode=item.id
                }
              })



              _this.setData({
                shopModes: d.shopModes,
                singleStore: typeof config.storeId == "undefined" ? false : true,
                currentStore: d.store,
                banner: d.banner,
                pdArea: d.pdArea,
                pageIsReady:true
              })
            }

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
      console.log("shopModeId:" + shopMode)

      app.globalData.currentShopMode = shopMode

      this.getPageData()

    },
    onShow() {
      console.log("index.onShow")
      var _this=this
      app.globalData.skeletonPage=_this;
    }
  }
})