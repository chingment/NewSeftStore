//index.js
//获取应用实例
const config = require('../../config')
const storeage = require('../../utils/storeageutil.js')
const toastUtil = require('../../utils/showtoastutil')
const ownRequest = require('../../own/ownRequest.js')
const lumos = require('../../utils/lumos.minprogram.js')
const global = require('../../api/global.js')


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
        tabs: [{
          name: "热门推荐",
          selected: true,
          list: []
        }, {
          name: "休闲零食",
          selected: false,
          list: []
        }, {
          name: "营养食品",
          selected: false,
          list: []
        }, {
          name: "百货用品",
          selected: false,
          list: []
        }],
        tabsSliderIndex: 0
      }
    },
    productKind: {
      list: []
    },
    cart: {
      list: []
    }
  },
  bindgetuserinfo: function (e) {




    // if (e.target.userInfo) {
    ownRequest.login((params) => {
      // 登录成功后，返回
      // wx.redirectTo({
      //   url: '../main/main',
      // })


      lumos.postJson({
        dataParams: {
          merchId: config.merchId,
          appId: config.appId,
          code: params.code,
          iv: params.iv,
          encryptedData: params.encryptedData
        },
        success: function (res) {
          if (res.result == 1) {
            storeage.setAccessToken(res.data.token);
            console.log("token:" + storeage.getAccessToken())
            wx.reLaunch({ //关闭所有页面，打开到应用内的某个页面
              url: ownRequest.getReturnUrl()
            })
          } else {
            toastUtil.showToast({
              title: res.message
            })
          }
        }
      })

    })
    //}
  },
  changeData: function (data) {
    console.log("main.changeData")
    var _self = this;
    _self.setData(data)
  },
  onLoad: function () {
    console.log("main.onLoad")
    var _self = this;

    var isLogin = ownRequest.isLogin();

    if (!isLogin) {
      return;
    }

    _self.setData({ isLogin: isLogin })

    if (!ownRequest.isSelectedStore(true)) {
      return
    }

    var currentStore = ownRequest.getCurrentStore()
    var wHeight = wx.getSystemInfoSync().windowHeight;
    _self.setData({
      tabBarContentHeight: wHeight - ownRequest.rem2px(3.044)
    });
    wx.setNavigationBarTitle({
      title: _self.data.tabBar[0].navTitle
    })

    global.getDataSet({
      storeId: ownRequest.getCurrentStoreId(),
      datetime: '2018-03-30'
    }, {
        success: function (d) {
          if (d.result == 1) {
            var index = d.data.index
            var productKind = d.data.productKind
            var cart = d.data.cart
            var personal = d.data.personal

            index["currentStore"] = currentStore

            _self.setData({
              isLogin: isLogin,
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
  mainTabBarItemClick(e) {
    console.log('tabbar.tabBarItemClick')
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