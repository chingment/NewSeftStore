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
      id:"cp_index",
      name: "index",
      pagePath: "/pages/index/index.wxml",
      iconPath: "/content/default/images/home.png",
      selectedIconPath: "/content/default/images/home_fill.png",
      text: "首页",
      navTitle: "贩聚社团",
      selected: true,
      number: 0
    }, {
      id:"cp_productkind",
      name: "productkind",
      pagePath: "/pages/productkind/productkind.wxml",
      iconPath: "/content/default/images/kind.png",
      selectedIconPath: "/content/default/images/kind_fill.png",
      text: "分类",
      navTitle: "分类",
      selected: false,
      number: 0
    }, {
      id:"cp_cart",
      name: "cart",
      pagePath: "/pages/cart/cart.wxml",
      iconPath: "/content/default/images/cart.png",
      selectedIconPath: "/content/default/images/cart_fill.png",
      text: "购物车",
      navTitle: "购物车",
      selected: false,
      number: 0
    }, {
      id:"cp_personal",
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
      store:{
        name:""
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
    }
  },
  changeData: function (data) {
    var _self = this;
    _self.setData(data)
  },
  onLoad: function (options) {
    var _self = this;
    console.log("getMainTabbarIndex():" +storeage.getMainTabbarIndex())
    if (!ownRequest.isSelectedStore(true)) {
      return
    }
    

    wx.setNavigationBarTitle({
      title: _self.data.tabBar[0].navTitle
    })

    // this.setMainTabBar(storeage.getMainTabbarIndex())

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

      wx.createSelectorQuery().selectAll('.main-tabbar-nav').boundingClientRect(function (rect) {
        var wHeight = wx.getSystemInfoSync().windowHeight;
        _self.setData({
          tabBarContentHeight: wHeight - rect[0].height
        });
    }).exec() 

  //   wx.createSelectorQuery().select('.searchbox').boundingClientRect(function (rect) {
  //     console.log("_self."+JSON.stringify(rect))
  // }).exec() 

//     　setTimeout(function () {
//     var query= _self.selectComponent("#cp_productkind").createSelectorQuery()
//     query.select('.searchbox').boundingClientRect(function (rect) {
//       console.log("_self.lenght1:"+JSON.stringify(rect))
//     }
//       //console.log("_self.lenght2:"+rect[0].height)
//       //var height=_self.height-rect.length;
//       //console.log("rect.lenght:"+height)

//       //newVal["contentHeight"]=height
//   ).exec()
// }, 3000)
  

  },
  onShow: function () {
    if (!ownRequest.isSelectedStore(true)) {
      return
    }
  },
  setMainTabBar:function(index){
    var _self = this
    var tabBar = _self.data.tabBar;
    for (var i = 0; i < tabBar.length; i++) {
      if (i == index) {

        tabBar[i].selected = true
        //设置页面标题
        wx.setNavigationBarTitle({
          title: tabBar[i].navTitle
        })
        
        let cp = this.selectComponent('#'+ tabBar[i].id);
        cp.onShow()
        
        storeage.setMainTabbarIndex(index)
      } else {
        tabBar[i].selected = false
      }
    }
    this.setData({
      tabBar: tabBar
    })
  },
  mainTabBarItemClick(e) {
    var _self = this
    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    this.setMainTabBar(index)
  },
  
})