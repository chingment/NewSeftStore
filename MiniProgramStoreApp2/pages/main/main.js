const config = require('../../config')
const storeage = require('../../utils/storeageutil.js')
const toast = require('../../utils/toastutil')
const util = require('../../utils/util')
const ownRequest = require('../../own/ownRequest.js')
const apiGlobal = require('../../api/global.js')
const apiOwn = require('../../api/own.js')
const apiCart = require('../../api/cart.js')
var app = getApp()

Page({
  data: {
    scrollHeight: 500,
    tag: "main",
    tabBarContentHeight: 0,
    name: "index",
    tabBar: [{
      id: "cp_index",
      name: "index",
      pagePath: "/pages/index/index.wxml",
      iconPath: "/content/default/images/home.png",
      selectedIconPath: "/content/default/images/home_fill.png",
      text: "首页",
      navTitle: "贩聚社团",
      selected: true,
      number: 0
    }, {
      id: "cp_productkind",
      name: "productkind",
      pagePath: "/pages/productkind/productkind.wxml",
      iconPath: "/content/default/images/kind.png",
      selectedIconPath: "/content/default/images/kind_fill.png",
      text: "分类",
      navTitle: "分类",
      selected: false,
      number: 0
    }, {
      id: "cp_cart",
      name: "cart",
      pagePath: "/pages/cart/cart.wxml",
      iconPath: "/content/default/images/cart.png",
      selectedIconPath: "/content/default/images/cart_fill.png",
      text: "购物车",
      navTitle: "购物车",
      selected: false,
      number: 0
    }, {
      id: "cp_personal",
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
      store: {
        name: ""
      },
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
    },
    personal:{
      isLogin: false,
      userInfo: null
    }
  },
  onLoad: function (options) {
    var _this = this;
    //console.log("mainTabBarIndex:" + app.globalData.mainTabBarIndex)
    // if (!ownRequest.isSelectedStore(true)) {
    //   return
    // }

    // app.mainTabBarSwitch(app.globalData.mainTabBarIndex)

    // apiGlobal.dataSet({
    //   storeId: ownRequest.getCurrentStoreId(),
    //   datetime: util.formatTime(new Date())
    // }, {
    //   success: function (res) {
    //     if (res.result == 1) {
    //       var cart = res.data.cart
    //       var personal = res.data.personal

    //       if (personal.userInfo == null) {
    //         storeage.setAccessToken(null)
    //       }

    //       _this.setData({
    //         cart: cart,
    //         personal: personal
    //       })

    //       storeage.setCart(cart)
    //     }
    //   },
    //   fail: function () { }
    // })

    wx.createSelectorQuery().selectAll('.main-tabbar-nav').boundingClientRect(function (rect) {
      var wHeight = wx.getSystemInfoSync().windowHeight;
      _this.setData({
        tabBarContentHeight: wHeight - rect[0].height
      });
    }).exec()
  },
  onShow: function () {
    console.log("mian.onShow")
    var _this = this
    app.mainTabBarSwitch(app.globalData.mainTabBarIndex)
    if (!ownRequest.isSelectedStore(true)) {
      return
    }

    apiCart.pageData()
  },
  mainTabBarItemClick(e) {
    var _this = this
    var index = e.currentTarget.dataset.replyIndex
    app.mainTabBarSwitch(index)
  },
})