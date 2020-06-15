const ownRequest = require('../../own/ownRequest.js')
const apiKind = require('../../api/kind.js')
const apiProduct = require('../../api/product.js')
const apiCart = require('../../api/cart.js')
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
      }).then(function (res) {

      })
    },
    searchClick: function (e) {
      wx.navigateTo({
        url: '/pages/search/search'
      })
    },
    productLoadMore({
      detail
    }) {
      var _this = this
      var index = 0

      console.log("index:" + index)
      _this.data.tabs[index].list.pageIndex += 1
      _this.setData({
        tabs: _this.data.tabs
      })

      _this.productSearch().then(function (res) {
        detail.success();
      })

    },
    productRefesh({
      detail
    }) {

      var _this = this
      var index = 0

      console.log("productLoadMore.index:" + index)

      _this.data.tabs[index].list.pageIndex = 0
      _this.data.tabs[index].list.loading = false
      _this.data.tabs[index].list.allloaded = false

      _this.productSearch().then(function (res) {
        detail.success();
      })
    },
    getPageData: function () {
      var _this = this

      if (ownRequest.getCurrentStoreId() != undefined) {
        apiKind.pageData({
          storeId: ownRequest.getCurrentStoreId(),
          shopMode: app.globalData.currentShopMode
        }).then(function (res) {

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
        })
      }

    },
    onShow() {
      console.log("productKind.onShow")
      var _this = this;

      if (_this.data.currentShopMode != app.globalData.currentShopMode) {
        _this.setData({
          currentShopMode: app.globalData.currentShopMode
        })
        _this.getPageData()
      }

      const query = wx.createSelectorQuery().in(_this)
      query.select('.searchbox').boundingClientRect(function (rect) {

        var height = _this.data.height - rect.height
        _this.data["scrollHeight"] = height
        _this.setData(_this.data)

      }).exec()


    },
    productSearch: function () {
      var _this = this

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

      _this.data.tabs[currentTabIndex].list.loading = true

      _this.setData({ tabs: _this.data.tabs })

      var pageIndex = currentTab.list.pageIndex
      var pageSize = currentTab.list.pageSize
      var kindId = currentTab.id == undefined ? "" : currentTab.id

      return apiProduct.search({
        storeId: ownRequest.getCurrentStoreId(),
        pageIndex: pageIndex,
        pageSize: pageSize,
        kindId: kindId,
        shopMode: app.globalData.currentShopMode,
        name: ""
      }).then(function (res) {
        if (res.result == 1) {
          var d = res.data
          var items=[]
          var allloaded = false
          if (currentTab.list.pageIndex == 0) {
            items = d.items
          } else {
            items = _this.data.tabs[currentTabIndex].list.items.concat(d.items)
          }

          if ((d.pageIndex + 1) > d.pageCount) {
            allloaded = true
          }

          _this.data.tabs[currentTabIndex].list.allloaded=allloaded
          _this.data.tabs[currentTabIndex].list.total = d.total
          _this.data.tabs[currentTabIndex].list.pageSize = d.pageSize
          _this.data.tabs[currentTabIndex].list.pageCount = d.pageCount
          _this.data.tabs[currentTabIndex].list.pageIndex = d.pageIndex
          _this.data.tabs[currentTabIndex].list.items = items;

          _this.setData({
            tabs: _this.data.tabs
          })
        }
      })

    }
  }
})