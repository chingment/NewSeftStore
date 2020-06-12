const ownRequest = require('../../own/ownRequest.js')
const apiKind = require('../../api/kind.js')
const apiProduct = require('../../api/product.js')
const apiCart = require('../../api/cart.js')
const app = getApp()

var getList = function (_this) {
  var currentTab;
  var currentTabIndex = -1;
  for (var i = 0; i < _this.data.tabs.length; i++) {
    if (_this.data.tabs[i].selected == true) {
      currentTab = _this.data.tabs[i];
      currentTabIndex = i;
    }
  }

  if (currentTabIndex == -1) {
    currentTabIndex = 0;
    currentTab = _this.data.tabs[currentTabIndex];
  }

  var pageIndex = currentTab.list.pageIndex
  var pageSize = currentTab.list.pageSize
  var kindId = currentTab.id == undefined ? "" : currentTab.id

  apiProduct.list({
    storeId: ownRequest.getCurrentStoreId(),
    pageIndex: pageIndex,
    pageSize: pageSize,
    kindId: kindId,
    shopMode: app.globalData.currentShopMode,
    name: ""
  }, {
    success: function (res) {
      if (res.result == 1) {
        var d = res.data
        var items
        if (currentTab.list.pageIndex == 0) {
          items = d.items
        } else {
          items = _this.data.tabs[currentTabIndex].list.items.concat(d.items)
        }

        _this.data.tabs[currentTabIndex].list.total = d.total
        _this.data.tabs[currentTabIndex].list.pageSize = d.pageSize
        _this.data.tabs[currentTabIndex].list.pageCount = d.pageCount
        _this.data.tabs[currentTabIndex].list.pageIndex = d.pageIndex
        _this.data.tabs[currentTabIndex].list.items = items;

        _this.setData({
          tabs: _this.data.tabs
        })
      }
    },
    fail: function () { }
  })

}

Component({
  options: {
    addGlobalClass: true,
    multipleSlots: true // 在组件定义时的选项中启用多slot支持
  },
  properties: {
    initdata: {
      type: Object,
      observer: function (newVal, oldVal, changedPath) {
        var _this = this
        console.log("productKind.initData")
      }
    },
    height: {
      type: Number
    }
  },
  data: {
    currentShopMode: 0
  },
  methods: {
    itemClick(e) {

      var _this = this
      var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
      var tabs = _this.data.tabs;
      for (var i = 0; i < tabs.length; i++) {
        if (i == index) {
          tabs[i].selected = true
        } else {
          tabs[i].selected = false
        }
      }
      _this.data.tabs = tabs
      _this.setData(_this.data)
    },
    addToCart: function (e) {
      var _this = this
      var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index
      var productSkus = new Array();
      productSkus.push({
        id: skuId,
        quantity: 1,
        selected: true,
        shopMode: 3
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
    searchClick: function (e) {
      wx.navigateTo({
        url: '/pages/search/search'
      })
    },
    productLoadMore: function (e) {
      var _this = this
      var index = e.currentTarget.dataset.replyIndex
      console.log("productLoadMore.index:" + index)
      console.log("_this.data.tabs[index].pageIndex:" + _this.data.tabs[index].list.pageIndex)
      console.log("_this.data.tabs[index].pageCount:" + _this.data.tabs[index].list.pageCountt - 1)
      if (_this.data.tabs[index].list.pageIndex != _this.data.tabs[index].list.pageCount - 1) {
        _this.data.tabs[index].list.pageIndex += 1
        _this.setData({
          tabs: _this.data.tabs
        })

        getList(_this)
      }
    },
    productRefesh: function (e) {
      var _this = this
      var index = e.currentTarget.dataset.replyIndex

      _this.data.tabs[index].list.pageIndex = 0

      console.log("productLoadMore.index:" + index)
      getList(_this)
    },
    getPageData: function () {
      var _this = this

      if (ownRequest.getCurrentStoreId() != undefined) {
        apiKind.pageData({
          storeId: ownRequest.getCurrentStoreId(),
          shopMode: app.globalData.currentShopMode
        }, {
          success: function (res) {

            if (res.result === 1) {
              var d = res.data
              var searchtips = [
                "商品搜索",
                "热销商品",
              ];


              _this.setData({
                searchtips: searchtips,
                tabs: d.tabs
              })

            }
          }
        })
      }

    },
    onShow() {
      console.log("productKind.onShow")
      var _this = this;

      if (_this.data.currentShopMode != app.globalData.currentShopMode) {
        _this.setData({
          currentShopMode:app.globalData.currentShopMode
        })
        _this.getPageData()
      }

      const query = wx.createSelectorQuery().in(_this)
      query.select('.searchbox').boundingClientRect(function (rect) {

        var height = _this.data.height - rect.height
        _this.data["scrollHeight"] = height
        _this.setData(_this.data)

      }).exec()


    }
  }
})