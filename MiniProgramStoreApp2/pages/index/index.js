const config = require('../../config')
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
        console.log("index.initdata")
        var _this = this
        _this.setData({
          shopModes:[
            {
              id:1,
              name:"线下机器",
              selected:true
            },
            {
              id:2,
              name:"线上商城",
              selected:false
            }
          ],
          singleStore: typeof config.storeId == undefined?false:true ,
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
    addToCart: function(e) {
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

    },
    goSelectStore:function(e){
 
      var _this=this

      if(_this.data.singleStore)
       return

      wx.navigateTo({
        url: '/pages/store/store',
      })
    },
    switchShopMode:function(e){
      var _this=this
      var shopModeId = e.currentTarget.dataset.replyShopmodeid //对应页面data-reply-index
      console.log("shopModeId:"+shopModeId)


      var shopModes=_this.data.shopModes

      for(var i=0;i<shopModes.length;i++){
        if(shopModes[i].id==shopModeId){
          shopModes[i].selected=true
        }
        else{
          shopModes[i].selected=false
        }
      }

      _this.setData({shopModes:shopModes})

    },
    onShow(){
      console.log("index.onShow")

      //this.getPageData()

    }
  }
})