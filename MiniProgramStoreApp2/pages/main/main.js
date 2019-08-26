//index.js
//获取应用实例
const config = require('../../config')
const storeage = require('../../utils/storeageutil.js')
const cart = require('../../pages/cart/cart.js')
const index = require('../../pages/index/index.js')
const productkind = require('../../pages/productkind/productkind.js')
const personal = require('../../pages/personal/personal.js')
const toastUtil = require('../../utils/showtoastutil')
const ownRequest = require('../../own/ownRequest.js')
const lumos = require('../../utils/lumos.minprogram.js')
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
  bindgetuserinfo: function(e) {




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
        success: function(res) {
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
  loadMore: function(e) {
    console.log("main.loadMore")
    var _self = this
    var _dataset = e.currentTarget.dataset
    var index = _dataset.replyIndex //对应页面data-reply-index
    var name = _dataset.replyName //对应页面data-reply-name
    console.log("main.loadMore.index:" + index)
    console.log("main.loadMore.name:" + name)
  },
  refresh: function(e) {
    console.log("main.refresh")
    var _self = this
    var _dataset = e.currentTarget.dataset
    var index = _dataset.replyIndex //对应页面data-reply-index
    var name = _dataset.replyName //对应页面data-reply-name
    console.log("main.loadMore.index:" + index)
    console.log("main.loadMore.name:" + name)
  },
  changeData: function(data) {
    console.log("main.changeData")
    var _self = this;
    _self.setData(data)
  },
  onLoad: function() {
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

    lumos.getJson({
      url: config.apiUrl.globalDataSet,
      urlParams: {
        storeId: ownRequest.getCurrentStoreId(),
        datetime: '2018-03-30'
      },
      success: function(d) {
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
      }
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
  },
  indexBarBannerSwiperChange: function(e) {
    var _index = this.data.index;
    _index.banner.currentSwiper = e.detail.current;
    this.setData({
      index: _index
    })
  },
  kindBarItemClick(e) {
    console.log('kindBarItemClick');
    var _self = this
    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    console.log('kindBarItemClick.index' + index)
    var list = _self.data.productKind.list;
    for (var i = 0; i < list.length; i++) {
      if (i == index) {
        list[i].selected = true
      } else {
        list[i].selected = false
      }
    }
    _self.data.productKind.list = list
    this.setData({
      productKind: _self.data.productKind
    })
  },
  cartBarListItemOperate(e) {
    console.log('cartBarListItemCheck');
    var _self = this
    var pIndex = e.currentTarget.dataset.replyPindex
    var cIndex = e.currentTarget.dataset.replyCindex
    var operate = e.currentTarget.dataset.replyOperate
    console.log('cartBarListItemCheck.pIndex:' + pIndex)
    console.log('cartBarListItemCheck.cIndex:' + cIndex)
    console.log('cartBarListItemCheck.operate' + operate)

    var sku = _self.data.cart.blocks[pIndex].productSkus[cIndex];

    switch (operate) {
      case "1":
        if (sku.selected) {
          sku.selected = false
        } else {
          sku.selected = true
        }
        break;
    }

    var operateSkus = new Array();
    operateSkus.push({
      id: sku.id,
      quantity: 1,
      selected: sku.selected,
      receptionMode: sku.receptionMode
    });

    console.log('ownRequest.getCurrentStoreId():' + ownRequest.getCurrentStoreId())

    function _operate() {

      cart.operate({
        storeId: ownRequest.getCurrentStoreId(),
        operate: operate,
        productSkus: operateSkus
      }, {
        success: function(res) {
          
        },
        fail: function() {}
      })
    }

    if (operate == 4) {
      wx.showModal({
        title: '提示',
        content: '确定要删除吗？',
        success: function(sm) {
          if (sm.confirm) {
            _operate()
          } else if (sm.cancel) {
            console.log('用户点击取消')
          }
        }
      })

    } else {
      _operate()
    }


  },
  cartBarImmeBuy: function(e) {
    var _this = this

    var blocks = _this.data.cart.blocks

    var skus = []

    for (var i = 0; i < blocks.length; i++) {
      for (var j = 0; j < blocks[i].productSkus.length; j++) {
        if (blocks[i].productSkus[j].selected) {
          skus.push({
            cartId: blocks[i].productSkus[j].cartId,
            id: blocks[i].productSkus[j].id,
            quantity: blocks[i].productSkus[j].quantity,
            receptionMode: blocks[i].productSkus[j].receptionMode
          })
        }
      }
    }

    if (skus.length == 0) {
      toastUtil.showToast({
        title: '至少选择一件商品'
      })
      return
    }

    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?skus=' + JSON.stringify(skus),
      success: function(res) {
        // success
      },
    })
  },
  addToCart: function(e) {
    var _self = this
    var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index
    console.log("skuId:" + skuId)
    var skus = new Array();
    skus.push({
      id: skuId,
      quantity: 1,
      selected: true,
      receptionMode: 1
    });
    console.log('ownRequest.getCurrentStoreId():' + ownRequest.getCurrentStoreId())
    cart.operate({
      storeId: ownRequest.getCurrentStoreId(),
      operate: 2,
      productSkus: skus
    }, {
      success: function(res) {},
      fail: function() {}
    })
  },


  //手指触摸动作开始 记录起点X坐标
  cartBarTouchstart: function(e) {

    //开始触摸时 重置所有删除
    var _this = this

    for (var i = 0; i < _this.data.cart.blocks.length; i++) {
      for (var j = 0; j < _this.data.cart.blocks[i].productSkus.length; j++) {
        if (_this.data.cart.blocks[i].productSkus[j].isTouchMove) {
          _this.data.cart.blocks[i].productSkus[j].isTouchMove = false;
        }
      }
    }

    this.setData({
      startX: e.changedTouches[0].clientX,
      startY: e.changedTouches[0].clientY,
      cart: _this.data.cart
    })

  },

  //滑动事件处理

  cartBarTouchmove: function(e) {

    var _this = this,

      cartId = e.currentTarget.dataset.cartid, //当前索引

      startX = _this.data.startX, //开始X坐标

      startY = _this.data.startY, //开始Y坐标

      touchMoveX = e.changedTouches[0].clientX, //滑动变化坐标

      touchMoveY = e.changedTouches[0].clientY, //滑动变化坐标

      //获取滑动角度

      angle = _this.angle({
        X: startX,
        Y: startY
      }, {
        X: touchMoveX,
        Y: touchMoveY
      });

    console.log("cartId:" + cartId);


    for (var i = 0; i < _this.data.cart.blocks.length; i++) {
      for (var j = 0; j < _this.data.cart.blocks[i].productSkus.length; j++) {

        _this.data.cart.blocks[i].productSkus[j].isTouchMove = false

        //滑动超过30度角 return

        if (Math.abs(angle) > 30) return;

        if (cartId == _this.data.cart.blocks[i].productSkus[j].cartId) {

          if (touchMoveX > startX) //右滑

            _this.data.cart.blocks[i].productSkus[j].isTouchMove = false

          else //左滑

            _this.data.cart.blocks[i].productSkus[j].isTouchMove = true

        }
      }
    }

    //更新数据
    console.log(JSON.stringify(_this.data.cart))
    _this.setData({
      cart: _this.data.cart
    })

  },

  /**
  
  * 计算滑动角度
  
  * @param {Object} start 起点坐标
  
  * @param {Object} end 终点坐标
  
  */

  angle: function(start, end) {

    var _X = end.X - start.X,

      _Y = end.Y - start.Y

    //返回角度 /Math.atan()返回数字的反正切值

    return 360 * Math.atan(_Y / _X) / (2 * Math.PI);

  }
})