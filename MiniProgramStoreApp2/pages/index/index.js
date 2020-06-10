const config = require('../../config')
const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const apiIndex = require('../../api/index.js')
const apiCart = require('../../api/cart.js')

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
        // _this.setData({
        //   shopModes:newVal.shopModes,
        //   singleStore: typeof config.storeId == "undefined"?false:true ,
        //   currentStore: newVal.store,
        //   banner: newVal.banner,
        //   pdArea: newVal.pdArea
        // })
      }
    },
    height: {
      type: Number
    }
  },
  data: {
    shopMode: 0
  },
  ready: function () {

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
        receptionMode: 3
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
          shopMode: _this.data.shopMode
        }, {
          success: function (res) {

            var d = res.data
            _this.setData({
              shopModes: d.shopModes,
              singleStore: typeof config.storeId == "undefined" ? false : true,
              currentStore: d.store,
              banner: d.banner,
              pdArea: d.pdArea
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
      console.log("shopModeId:" + shopMode)

      _this.setData({ shopMode: shopMode })

      this.getPageData()

    },
    onShow() {
      console.log("index.onShow")


      //this.getPageData()

    }
  }
})