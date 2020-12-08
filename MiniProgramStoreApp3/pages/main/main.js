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
    tabBarIndex:0,
    tabBar: [{
      id: "cp_index",
      name: "index",
      pagePath: "/pages/index/index.wxml",
      iconPath: "/content/default/images/home.png",
      selectedIconPath: "/content/default/images/home_fill.png",
      text: "首页",
      navTitle: "首页",
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
    personal: {
      isLogin: false,
      userInfo: null
    }
  },
  onLoad: function (options) {
    var _this = this;
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
    if (!ownRequest.isSelectedStore(true)) {
      return
    }

    var tabBarIndex= wx.getStorageSync('main_tabbar_index') || 0
    mainTabBarSwitch(tabBarIndex)
    apiCart.pageData()
  },
  mainTabBarItemClick(e) {
    var _this = this
    var index = e.currentTarget.dataset.replyIndex
    mainTabBarSwitch(index)
  },
})

function mainTabBarSwitch(index) {
  console.log('mainTabBarSwitch')

  var _this = this

  var pages = getCurrentPages();
  var isHasMain = false;
  for (var i = 0; i < pages.length; i++) {
    if (pages[i].data.tag == "main") {
      isHasMain = true
      var tabBar = pages[i].data.tabBar;

      for (var j = 0; j < tabBar.length; j++) {
        if (j == index) {
          tabBar[j].selected = true
          var s = tabBar[j];
          setTimeout(function () {
            wx.setNavigationBarTitle({
              title: s.navTitle
            })
          }, 1)

          wx.setStorageSync('main_tabbar_index',index)
          pages[i].setData({tabBarIndex:index})
        } else {
          tabBar[j].selected = false
        }
      }

      let cp = pages[i].selectComponent('#' + tabBar[index].id);
      cp.onReady()

      pages[i].setData({
        tabBar: tabBar
      })

      cp = pages[i].selectComponent('#' + tabBar[index].id);
      cp.onShow()
      break
    }
  }

  if (isHasMain) {
    if (pages.length > 1) {
      wx.navigateBack({
        delta: pages.length
      })
    }
  } else {
    wx.reLaunch({
      url: '/pages/main/main'
    })
  }

}

module.exports = {
  mainTabBarSwitch: mainTabBarSwitch
}