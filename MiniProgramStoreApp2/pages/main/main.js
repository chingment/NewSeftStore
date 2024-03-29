const config = require('../../config')
const storeage = require('../../utils/storeageutil.js')
const toast = require('../../utils/toastutil')
const util = require('../../utils/util')
const ownRequest = require('../../own/ownRequest.js')
const apiGlobal = require('../../api/global.js')
const apiOwn = require('../../api/own.js')

var app = getApp()

Page({
  data: {
    isLogin: false,
    scrollHeight: 500,
    tag: "main",
    tabBarContentHeight: 0,
    name: "index",
    tabBar: [{
      name: "index",
      pagePath: "/pages/index/index.wxml",
      iconPath: "/content/default/images/home.png",
      selectedIconPath: "/content/default/images/home_fill.png",
      text: "首页",
      navTitle: "贩聚社团",
      selected: true,
      number: 0
    }, {
      name: "productkind",
      pagePath: "/pages/productkind/productkind.wxml",
      iconPath: "/content/default/images/kind.png",
      selectedIconPath: "/content/default/images/kind_fill.png",
      text: "分类",
      navTitle: "分类",
      selected: false,
      number: 0
    }, {
      name: "cart",
      pagePath: "/pages/cart/cart.wxml",
      iconPath: "/content/default/images/cart.png",
      selectedIconPath: "/content/default/images/cart_fill.png",
      text: "购物车",
      navTitle: "购物车",
      selected: false,
      number: 0
    }, {
      name: "personal",
      pagePath: "/pages/personal/personal.wxml",
      iconPath: "/content/default/images/personal.png",
      selectedIconPath: "/content/default/images/personal_fill.png",
      text: "个人",
      navTitle: "个人",
      selected: false,
      number: 0
    }],
    index: {
      banner: {
        imgs: [],
        currentSwiper: 0,
        autoplay: true
      },
      pdArea: {
        tabs: [],
        tabsSliderIndex: 0
      }
    },
    productKind: {
      list: []
    },
    cart: {
      blocks: [],
      count: 0,
      sumPrice: 0,
      countBySelected: 0,
      sumPriceBySelected: 0
    }
  },
  changeData: function (data) {
    var _self = this;
    _self.setData(data)
  },
  onLoad: function (options) {
    var _self = this;

    if (!ownRequest.isSelectedStore(true)) {
      return
    }

    var currentStore = ownRequest.getCurrentStore()
    var wHeight = wx.getSystemInfoSync().windowHeight;
    _self.setData({
      tabBarContentHeight: wHeight - util.rem2px(3.044)
    });
    wx.setNavigationBarTitle({
      title: _self.data.tabBar[0].navTitle
    })

    apiGlobal.dataSet({
      storeId: ownRequest.getCurrentStoreId(),
      datetime: util.formatTime(new Date())
    }, {
        success: function (res) {
          if (res.result == 1) {
            var index = res.data.index
            var productKind = res.data.productKind
            var cart = res.data.cart
            var personal = res.data.personal

            if (personal.userInfo== null) {
              storeage.setAccessToken(null)
            }

            _self.setData({
              isLogin: ownRequest.isLogin(),
              index: index,
              productKind: productKind,
              cart: cart,
              personal: personal
            })

            storeage.setProductKind(productKind)
            storeage.setCart(cart)
          }
        },
        fail: function () { }
      })

  },
  onShow: function () {
    if (!ownRequest.isSelectedStore(true)) {
      return
    }
  },
  mainTabBarItemClick(e) {
    var _self = this
    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var tabBar = _self.data.tabBar;
    for (var i = 0; i < tabBar.length; i++) {
      if (i == index) {
        tabBar[i].selected = true
        //设置页面标题
        wx.setNavigationBarTitle({
          title: tabBar[i].navTitle
        })
      } else {
        tabBar[i].selected = false
      }
    }
    this.setData({
      tabBar: tabBar
    })
  }
})