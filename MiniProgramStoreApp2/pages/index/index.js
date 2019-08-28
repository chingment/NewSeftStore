const ownRequest = require('../../own/ownRequest.js')
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

        var _self = this
        var currentStore = ownRequest.getCurrentStore()
        _self.setData({
          currentStore: currentStore,
          banner: newVal.banner,
          pdArea: newVal.pdArea
        })
      }
    },
    height:{
      type:Number
    }
  },
  data: {},
  methods: {
    topBannerSwiperChange: function (e) {
      var _self = this
      _self.data.banner.currentSwiper = e.detail.current;
      this.setData({
        banner: _self.data.banner
      })
    },
    addToCart: function (e) {
      var _self = this
      var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index
      console.log('skuId：' + skuId)
      var productSkus = new Array();
      productSkus.push({
        id: skuId,
        quantity: 1,
        selected: true,
        receptionMode: 3
      });
      console.log('ownRequest.getCurrentStoreId():' + ownRequest.getCurrentStoreId())
      apiCart.operate({
        storeId: ownRequest.getCurrentStoreId(),
        operate: 2,
        productSkus: productSkus
      }, {
          success: function (res) {

          },
          fail: function () { }
        })
    }
  }
})