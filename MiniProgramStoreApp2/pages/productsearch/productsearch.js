const util = require('../../utils/util.js')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiProduct = require('../../api/product.js')
const app = getApp()

Page({
  data: {
    tag: "productlist",
    dataList: {
      isEmpty: false,
      allloaded: false,
      loading: false,
      total: 0,
      pageIndex: 0,
      pageCount: 0,
      items: []
    },
    scrollHeight: 0
  },
  onLoad: function (options) {
    var _this = this

    var kindId = options.kindId == undefined ? "" : options.kindId
    var subjectId = options.subjectId == undefined ? "" : options.subjectId
    var navName = options.navName == undefined ? "" : options.navName

    wx.setNavigationBarTitle({
      title: navName
    })

    apiProduct.initSearchPageData({
      storeId: ownRequest.getCurrentStoreId(),
      kindId: kindId,
      subjectId: subjectId,
      shopMode: app.globalData.currentShopMode
    }).then(function (res) {
      if (res.result == 1) {
        var d = res.data

        var condition_Kinds_index = 0
        for (var i = 0; i < d.condition_Kinds.length; i++) {
          if (d.condition_Kinds[i].id == kindId) {
            condition_Kinds_index = i
          }
        }

        _this.setData({
          condition_Kinds: d.condition_Kinds,
          condition_Kinds_index: condition_Kinds_index,
          cart: storeage.getCart()
        })

        _this.search()

      }
    })


    wx.createSelectorQuery().selectAll('.search-condition').boundingClientRect(function (rect) {
      var wHeight = wx.getSystemInfoSync().windowHeight;

      var height = 0
      if (typeof rect != 'undefined' && rect.length > 0) {
        height = rect[0].height
      }

      _this.setData({
        scrollHeight: wHeight - height
      });
    }).exec()


  },
  //加载更多
  loadmore: function (e) {
    var _this = this

    if (!_this.data.dataList.loading) {
      _this.data.dataList.pageIndex += 1
      _this.setData({
        dataList: _this.data.dataList
      })
      _this.search().then(res => {
        e.detail.success();
      });
    }
  },
  //刷新处理
  refresh: function (e) {
    var _this = this
    _this.data.dataList.pageIndex = 0
    _this.search().then(res => {
      e.detail.success();
    });
  },
  goCart: function (e) {
    this.selectComponent("#cart").open()
  },
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
    }).then(function (res) {

    })

  },
  //tab点击
  tabBarClickByKind: function (e) {
    var _this = this
    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    _this.data.dataList.pageIndex = 0
    _this.setData({
      condition_Kinds_index: index,
      dataList: _this.data.dataList
    })
    _this.search(_this)
  },
  onShow() {
    var _this = this
    app.globalData.skeletonPage = _this;
  },
  search: function () {
    var _this = this

    _this.setData({ loading: true })

    var pageIndex = _this.data.dataList.pageIndex
    var kindId = _this.data.condition_Kinds[_this.data.condition_Kinds_index].id

    return apiProduct.search({
      storeId: ownRequest.getCurrentStoreId(),
      pageIndex: pageIndex,
      pageSize: 10,
      kindId: kindId,
      subjectId: undefined,
      shopMode: app.globalData.currentShopMode,
      name: ""
    }).then(function (res) {
      if (res.result == 1) {
        var d = res.data
        var items = []
        var allloaded = false
        var isEmpty = false
        var list = _this.data.dataList
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

        list.loading = false
        list.allloaded = allloaded
        list.isEmpty = isEmpty
        list.total = d.total
        list.pageSize = d.pageSize
        list.pageCount = d.pageCount
        list.pageIndex = d.pageIndex
        list.items = items;

        _this.setData({
          dataList: list
        })
      }
    })
  }
})