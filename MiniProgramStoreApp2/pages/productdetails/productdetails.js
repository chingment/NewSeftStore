const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiProduct = require('../../api/product.js')
const config = require('../../config');
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "productdetails",
    isShowCart: false,
    specSelectArr: [], //存放被选中的值
    specShopItemInfo: {}, //存放要和选中的值进行匹配的数据
    specSubIndex: [], //是否选中 因为不确定是多规格还是但规格，所以这里定义数组来判断
    specBoxArr: {},
    storeId: null,
    shopMode: null
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var skuId = options.skuId == undefined ? "0" : options.skuId
    var storeId = options.storeId == undefined ? undefined : options.storeId
    var shopMode = options.shopMode == undefined ? undefined : options.shopMode

    if (storeId == undefined) {
      storeId = ownRequest.getCurrentStoreId()
    }

    if (shopMode == undefined) {
      shopMode = app.globalData.currentShopMode
    }

    app.globalData.currentShopMode = shopMode

    ownRequest.setCurrentStoreId(storeId)

    _this.setData({
      storeId: storeId,
      shopMode: shopMode
    })

    var cart = {
      count: 0
    }
    if (ownRequest.isLogin()) {
      cart = storeage.getCart()
    }

    apiProduct.details({
      storeId: storeId,
      skuId: skuId,
      shopMode: shopMode
    }).then(function (res) {
      if (res.result == 1) {

        var productSku = res.data

        var specIdxSkus = productSku.specIdxSkus

        var specShopItemInfo = _this.data.specShopItemInfo

        var specItems = productSku.specItems

        for (var i in specIdxSkus) {

          specShopItemInfo[specIdxSkus[i].specIdx] = specIdxSkus[i]; //修改数据结构格式，改成键值对的方式，以方便和选中之后的值进行匹配

        }



        for (var i = 0; i < specItems.length; i++) {

          for (var o = 0; o < specItems[i].value.length; o++) {

            specItems[i].value[o].isShow = true

          }

        }


        productSku.specItems = specItems


        _this.setData({
          productSku: productSku,
          cart: cart,
          specShopItemInfo: specShopItemInfo,
          specSubIndex: productSku.specIdx.split(',')
        })
      }
    })
  },
  goHome: function (e) {
    app.mainTabBarSwitch(0)
  },
  goCart: function (e) {
    this.selectComponent("#cart").open()
  },
  addToCart: function (e) {
    var _this = this

    if (_this.data.productSku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    var skuId = e.currentTarget.dataset.replySkuid //对应页面data-reply-index

    var productSkus = new Array();
    productSkus.push({
      id: skuId,
      quantity: 1,
      selected: true,
      shopMode: _this.data.shopMode
    });

    apiCart.operate({
      storeId: ownRequest.getCurrentStoreId(),
      operate: 2,
      productSkus: productSkus
    }).then(function (res) {
      if (res.result == 1) {

      } else {
        toast.show({
          title: res.message
        })
      }
    })

  },

  immeBuy: function (e) {
    var _this = this

    if (_this.data.productSku.isOffSell) {
      toast.show({
        title: '商品已下架'
      })
      return
    }

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    var skuId = _this.data.productSku.id //对应页面data-reply-index
    var productSkus = []
    productSkus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      shopMode: _this.data.shopMode
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus),
      success: function (res) {
        // success
      },
    })
  },
  specificationBtn(e) {
    var _this = this;
    var n = e.currentTarget.dataset.n

    var index = e.currentTarget.dataset.index

    var item = e.currentTarget.dataset.name



    var productSku = _this.data.productSku

    var specSubIndex = _this.data.specSubIndex

    var specBoxArr = _this.data.specBoxArr

    var specShopItemInfo = _this.data.specShopItemInfo

    if (specSubIndex[n] != item) {

      specSubIndex[n] = item;

      specSubIndex[n] = item;

    } else {

      // self.selectArr[n] = "";

      // self.subIndex[n] = -1; //去掉选中的颜色

    }

    _this.checkItem();

    var arr = specShopItemInfo[specSubIndex];

    //选中修改当前SKU.ID
    if (arr) {

      productSku.id = arr.skuId;
      productSku.specIdx = arr.specIdx;

      apiProduct.skuStockInfo({
        storeId: _this.data.storeId,
        skuId: arr.skuId,
        shopMode: _this.data.shopMode
      }).then(function (res) {

        var d = res.data

        productSku.name = d.name
        productSku.isOffSell = d.isOffSell
        productSku.salePrice = d.salePrice
        productSku.isShowPrice = d.isShowPrice
        productSku.salePriceByVip = d.salePriceByVip
        productSku.sellQuantity = d.sellQuantity

        // console.log("arr:" + JSON.stringify(arr))
        // console.log("specSubIndex:" + JSON.stringify(specSubIndex))
        // console.log("specShopItemInfo:" + JSON.stringify(specShopItemInfo))
        _this.setData({
          productSku: productSku,
          specSubIndex: specSubIndex,
          specShopItemInfo: specShopItemInfo

        })

      })

    }

  },

  checkItem() {

    var _this = this;

    var productSku = _this.data.productSku

    var option = _this.data.productSku.specItems;

    var result = []; //定义数组存储被选中的值

    for (var i in option) {

      result[i] = _this.data.specSelectArr[i] ? _this.data.specSelectArr[i] : "";

    }


    for (var i in option) {

      var last = result[i]; //把选中的值存放到字符串last去

      for (var k in option[i].item) {

        result[i] = option[i].item[k].name; //赋值，存在直接覆盖，不存在往里面添加name值

        option[i].item[k].isShow = _this.isMay(result); //在数据里面添加字段isShow来判断是否可以选择

      }

      result[i] = last; //还原，目的是记录点下去那个值，避免下一次执行循环时避免被覆盖

    }

    productSku.specItems = option

    _this.setData({

      productSku: productSku

    })

  },

  isMay(result) {
    var _this=this
    for (var i in result) {

      if (result[i] == "") {

        return true; //如果数组里有为空的值，那直接返回true

      }

    }

    return !_this.data.specShopItemInfo[result] ?

      false :

      _this.data.specShopItemInfo[result].stock == 0 ?

        false :

        true; //匹配选中的数据的库存，若不为空返回true反之返回false

  },
  onShareAppMessage: function (options) {
    var _this = this;
    // 设置转发内容
    var shareObj = {
      title: _this.data.productSku.name,
      path: '/pages/productdetails/productdetails?skuId=' + _this.data.productSku.id + '&shopMode=' + app.globalData.currentShopMode + '&storeId=' + ownRequest.getCurrentStoreId() + "&merchId=" + config.merchId, // 默认是当前页面，必须是以‘/’开头的完整路径
      imgUrl: '', //转发时显示的图片路径，支持网络和本地，不传则使用当前页默认截图。
      success: function (res) { // 转发成功之后的回调　　　　　
        if (res.errMsg == 'shareAppMessage:ok') { }
      },
      fail: function () { // 转发失败之后的回调
        if (res.errMsg == 'shareAppMessage:fail cancel') {
          // 用户取消转发
        } else if (res.errMsg == 'shareAppMessage:fail') {
          // 转发失败，其中 detail message 为详细失败信息
        }
      },
      complete: function () {
        // 转发结束之后的回调（转发成不成功都会执行）
      }
    };
    // 来自页面内的按钮的转发
    if (options.from == 'button') {
      var dataid = options.target.dataset; //上方data-id=shareBtn设置的值
      // 此处可以修改 shareObj 中的内容
      shareObj.path = '/pages/btnname/btnname?id=' + dataid.id;
    }
    // 返回shareObj
    return shareObj;

  }

})