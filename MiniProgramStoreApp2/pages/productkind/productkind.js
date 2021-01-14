const ownRequest = require('../../own/ownRequest.js')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
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
    tag: 'main-productkind',
    isOnReady: false,
    shopMode: 1,
    shopId: '0',
    specsDialog: {
      isShow: false
    }
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
      _this.setData({
        tabs: tabs
      })
    },
    addToCart: function (e) {
      var _this = this
      var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index
      var productSkus = new Array();
      productSkus.push({
        id: skuId,
        quantity: 1,
        selected: true,
        shopMode: _this.data.shopMode,
        shopId:_this.data.shopId
      });

      apiCart.operate({
        storeId: _this.data.storeId,
        operate: 2,
        productSkus: productSkus,
        shopId:_this.data.shopId
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
    searchClick: function (e) {
      var _this = this
      wx.navigateTo({
        url: '/pages/search/search?shopId=' + _this.data.shopId + '&shopMode=' + _this.data.shopMode + '&shopMethod=1'
      })
    },
    productLoadMore: function (e) {
      var _this = this


      var index = e.currentTarget.dataset.replyIndex

      // console.log("index:" + index)
      _this.data.tabs[index].list.pageIndex += 1
      _this.setData({
        tabs: _this.data.tabs
      })

      _this.productSearch().then(function (res) {
        e.detail.success();
      })

    },
    productRefesh: function (e) {

      var _this = this
      var index = e.currentTarget.dataset.replyIndex

      //  console.log("productLoadMore.index:" + index)

      _this.data.tabs[index].list.pageIndex = 0
      _this.data.tabs[index].list.loading = false
      _this.data.tabs[index].list.allloaded = false

      _this.productSearch().then(function (res) {
        e.detail.success();
      })
    },
    getPageData: function () {
      var _this = this

      apiKind.pageData({
        storeId: _this.data.storeId,
        shopMode: _this.data.shopMode,
        shopId:_this.data.shopId
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
    },
    onReady: function () {
      var _this = this;
      console.log("productkind.onReady")
    },
    onShow() {
      console.log("productKind.onShow")
      var _this = this;

      const query = wx.createSelectorQuery().in(_this)
      query.select('.searchbox').boundingClientRect(function (rect) {

        var height = _this.data.height - rect.height

        _this.setData({
          scrollHeight: height
        })

      }).exec()


      _this.setData({
        storeId: storeage.getStoreId()
      })
      _this.getPageData()



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

      _this.setData({
        tabs: _this.data.tabs
      })

      var pageIndex = currentTab.list.pageIndex
      var pageSize = currentTab.list.pageSize
      var kindId = currentTab.id == undefined ? "" : currentTab.id

      return apiProduct.search({
        storeId: _this.data.storeId,
        pageIndex: pageIndex,
        pageSize: pageSize,
        kindId: kindId,
        shopMode: _this.data.shopMode,
        shopId:_this.data.shopId,
        name: ""
      }, false).then(function (res) {
        if (res.result == 1) {
          var d = res.data
          var items = []
          var allloaded = false
          var isEmpty = false
          var list = _this.data.tabs[currentTabIndex].list
          if (d.pageIndex == 0) {
            items = d.items
          } else {
            items = list.items.concat(d.items)
          }

          if (d.total == 0) {
            isEmpty = true
          }

          if ((d.pageIndex + 1) >= d.pageCount) {
            allloaded = true
          }

          list.isEmpty = isEmpty
          list.allloaded = allloaded
          list.total = d.total
          list.pageSize = d.pageSize
          list.pageCount = d.pageCount
          list.pageIndex = d.pageIndex


          list.items = items;

          _this.data.tabs[currentTabIndex].list = list
          _this.setData({
            tabs: _this.data.tabs
          })
        }
      })

    }
  }
})