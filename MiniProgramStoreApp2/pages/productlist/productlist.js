const util = require('../../utils/util.js')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiProduct = require('../../api/product.js')
const app = getApp()

var getList = function(_this) {
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
  var kindId = currentTab.kindId == undefined ? "" : currentTab.kindId
  var subjectId = currentTab.subjectId == undefined ? "" : currentTab.subjectId


  apiProduct.list({
    storeId: ownRequest.getCurrentStoreId(),
    pageIndex: pageIndex,
    pageSize: 10,
    kindId: kindId,
    subjectId: subjectId,
    name: ""
  }, {
    success: function(res) {
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
    fail: function() {}
  })

}

Page({
  data: {
    tag: "productlist",
    scrollHeight: 0
  },
  onLoad: function(options) {
    var _this = this

    var kindId = options.kindId == undefined ? "" : options.kindId
    var subjectId = options.subjectId == undefined ? "" : options.subjectId
    var navName = options.navName == undefined ? "" : options.navName

    wx.setNavigationBarTitle({
      title: navName
    })

    //加载tab数据，从缓存对象获取
    var productKinds = storeage.getProductKind().tabs
    // console.log("storeage.getProductKind():" +JSON.stringify( storeage.getProductKind()))
    var tabs = new Array()
    var tabsSliderIndex = -1 //默认未选择tab


    var deHeight = 2;
    if (kindId != "") {
      if (productKinds.length > 0) {
        for (var i = 0; i < productKinds.length; i++) {
          var selected = false
          if (productKinds[i].id == kindId) {
            selected = true
            tabsSliderIndex = i
          }
          var tab = {
            kindId: productKinds[i].id,
            subjectId: "",
            name: productKinds[i].name,
            selected: selected,
            list: {
              pageIndex: 0,
              pageSize: 10,
              item: [],
              total: 0,
              pageCount: 0
            },
            scrollTop: 0
          }
          tabs.push(tab)

        }
      }
    }

    if (subjectId != "") {
      deHeight = 0
      tabsSliderIndex = 0;
      var tab = {
        kindId: "",
        subjectId: subjectId,
        name: navName,
        pageIndex: 0,
        selected: true,
        list: null,
        scrollTop: 0
      }
      tabs.push(tab)
    }


    var wHeight = wx.getSystemInfoSync().windowHeight;
    _this.setData({
      scrollHeight: wHeight - util.rem2px(deHeight)
    });

    _this.setData({
      tabs: tabs,
      tabsSliderIndex: tabsSliderIndex,
      cart: storeage.getCart()
    })

    getList(_this)

  },
  //加载更多
  loadMore: function(e) {
    var _this = this

    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    _this.data.tabs[index].pageIndex += 1
    _this.setData({
      tabs: _this.data.tabs
    })
    getList(_this)
  },
  //刷新处理
  refesh: function(e) {
    var _this = this

    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var scrollTop = e.detail.scrollTop
    _this.data.tabs[index].pageIndex = 0
    _this.data.tabs[index].scrollTop = 0
    _this.setData({
      tabs: _this.data.tabs
    })
    getList(_this)
  },

  scroll: function(e) {
    var _this = this

    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var scrollTop = e.detail.scrollTop

    _this.data.tabs[index].scrollTop = scrollTop
    _this.setData({
      tabs: _this.data.tabs
    })

  },

  //tab点击
  tabBarClick: function(e) {

    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var kindId = e.currentTarget.dataset.replykindId //对应页面data-reply-index
    var _this = this

    for (var i = 0; i < _this.data.tabs.length; i++) {
      if (i == index) {
        _this.data.tabs[i].selected = true;
      } else {
        _this.data.tabs[i].selected = false;
      }
    }
    _this.setData({
      tabs: _this.data.tabs,
      tabsSliderIndex: index
    })

    getList(_this)
  },
  // 滚动切换标签样式
  swiperSwitchTab: function(e) {

    var index = e.detail.current //对应页面data-reply-index
    var _this = this

    for (var i = 0; i < _this.data.tabs.length; i++) {
      if (i == index) {
        _this.data.tabs[i].selected = true;
      } else {
        _this.data.tabs[i].selected = false;
      }
    }
    _this.setData({
      tabs: _this.data.tabs,
      tabsSliderIndex: index
    })

    getList(_this)

  },
  goCart: function(e) {
    //app.mainTabBarSwitch(2)
    this.selectComponent("#cart").open()
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
      fail: function() {

      }
    })

  }
})