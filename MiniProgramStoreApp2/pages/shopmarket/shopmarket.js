const ownRequest = require('../../own/ownRequest.js')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const apiShopMarket = require('../../api/shopmarket.js')
const apiProduct = require('../../api/product.js')
const apiCart = require('../../api/cart.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: 'shopmarket',
    isOnReady: false,
    shopMode: 3,
    storeId: '',
    shopId: '',
    specsDialog: {
      isShow: false
    },
    cartDialog: {
      isShow: false,
      dataS: {
        blocks: [],
        count: 0,
        sumPrice: 0,
        countBySelected: 0,
        sumPriceBySelected: 0
      }
    }
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {



  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

    var _this = this;

    const query = wx.createSelectorQuery().in(_this)
    var wHeight = wx.getSystemInfoSync().windowHeight;
    query.select('.searchbox').boundingClientRect(function (rect) {
      var height = wHeight - rect.height
      _this.setData({
        scrollHeight: height
      })

    }).exec()

    ownRequest.isSelectedShop(true)

    _this.setData({
      storeId: storeage.getStoreId(),
      shopId: storeage.getShopId()
    })

    _this.getPageData()
    _this.getCartData()
  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },
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
      shopId: _this.data.shopId,
    });

    apiCart.operate({
      storeId: _this.data.storeId,
      operate: 2,
      shopMode: _this.data.shopMode,
      shopId: _this.data.shopId,
      productSkus: productSkus
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

    apiShopMarket.pageData({
      storeId: _this.data.storeId,
      shopId: _this.data.shopId,
      shopMode: _this.data.shopMode
    }).then(function (res) {

      if (res.result === 1) {
        var d = res.data
        var searchtips = [
          "商品搜索",
          "热销商品",
        ];


        _this.setData({
          searchtips: searchtips,
          tabs: d.tabs,
          curShop: d.curShop
        })

      }
    })
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
      shopId: _this.data.shopId,
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

  },
  goCart: function (e) {
    var _this = this
    var cartDialog = _this.data.cartDialog
    cartDialog.isShow = true
    _this.setData({
      cartDialog: cartDialog
    })
  },
  getCartData: function () {
    var _this = this

    if (ownRequest.isLogin()) {

      apiCart.getCartData({
        shopMode: _this.data.shopMode,
        storeId: storeage.getStoreId(),
        shopId: _this.data.shopId
      }).then(function (res) {
        if (res.result == 1) {
          var cartDialog = _this.data.cartDialog
          cartDialog.dataS = res.data
          _this.setData({
            cartDialog: cartDialog
          })
        }
      })
    }
  },
  cartdialogClose: function () {
    var _this = this
    var cartDialog = _this.data.cartDialog
    cartDialog.isShow = false
    _this.setData({
      cartDialog: cartDialog
    })
  },
  goChoiceShop: function (e) {
    wx.navigateTo({
      url: '/pages/shopchoice/shopchoice',
    })
  },
})