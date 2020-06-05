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
      observer: function(newVal, oldVal, changedPath) {

        var _self = this
        _self.setData({
          currentStore: newVal.store,
          banner: newVal.banner,
          pdArea: newVal.pdArea
        })
      }
    },
    height: {
      type: Number
    }
  },
  data: {},
  ready: function() {
    
  },
  methods: {
    topBannerSwiperChange: function(e) {
      var _self = this
      _self.data.banner.currentSwiper = e.detail.current;
      this.setData({
        banner: _self.data.banner
      })
    },
    addToCart: function(e) {
      var _self = this
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
        success: function(res) {

        },
        fail: function() {}
      })
    },
    getPageData: function() {
      if (ownRequest.getCurrentStoreId() != undefined) {
        var self = this
        apiIndex.pageData({
          storeId: ownRequest.getCurrentStoreId()
        }, {
          success: function(res) {

            self.setData({
              initdata: res.data
            })
          }
        })
      }

    }
  }
})