const util = require('../../utils/util')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiGlobal = require('../../api/global.js')


Component({
  options: {
    addGlobalClass: true,
    multipleSlots: true // 在组件定义时的选项中启用多slot支持
  },
  properties: {
    height: {
      type: Number
    }
  },
  data: {
    tag: 'main-cart',
    isOnReady: false,
    isLogin: false,
    storeId: undefined,
    cartData: {
      blocks: [],
      count: 0,
      sumPrice: 0,
      countBySelected: 0,
      sumPriceBySelected: 0
    }
  },
  methods: {
    itemOperate: util.throttle(function (e) {

      var _this = this
      var pIndex = e.currentTarget.dataset.replyPindex
      var cIndex = e.currentTarget.dataset.replyCindex
      var operate = e.currentTarget.dataset.replyOperate


      var productSku = _this.data.cartData.blocks[pIndex].productSkus[cIndex];

      switch (operate) {
        case "1":
          if (productSku.selected) {
            productSku.selected = false
          } else {
            productSku.selected = true
          }
          break;
      }

      var operateProductSkus = new Array();
      operateProductSkus.push({
        id: productSku.id,
        quantity: 1,
        selected: productSku.selected,
        shopMode: productSku.shopMode,
        shopId: productSku.shopId
      });

      function _operate() {

        apiCart.operate({
          storeId: _this.data.storeId,
          operate: operate,
          productSkus: operateProductSkus
        }).then(function (res) {
          toast.show({
            title: res.message
          })

        })

        apiGlobal.byPoint(_this.data.tag, "op_cart", {
          operate: operate,
          productSkus: operateProductSkus
        })
      }

      if (operate == 4) {
        wx.showModal({
          title: '提示',
          content: '确定要删除吗？',
          success: function (sm) {
            if (sm.confirm) {
              _operate()
            } else if (sm.cancel) {}
          }
        })

      } else {
        _operate()
      }
    }, 1000),
    itemTouchstart: function (e) {

      //开始触摸时 重置所有删除
      var _this = this

      var cartData = _this.data.cartData

      for (var i = 0; i < cartData.blocks.length; i++) {
        for (var j = 0; j < cartData.blocks[i].productSkus.length; j++) {
          if (cartData.blocks[i].productSkus[j].isTouchMove) {
            cartData.blocks[i].productSkus[j].isTouchMove = false;
          }
        }
      }

      this.setData({
        startX: e.changedTouches[0].clientX,
        startY: e.changedTouches[0].clientY,
        cartData: cartData
      })

    },
    itemTouchmove: function (e) {

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

      var cartData = _this.data.cartData

      for (var i = 0; i < cartData.blocks.length; i++) {
        for (var j = 0; j < cartData.blocks[i].productSkus.length; j++) {

          cartData.blocks[i].productSkus[j].isTouchMove = false

          //滑动超过30度角 return

          if (Math.abs(angle) > 30) return;

          if (cartId == cartData.blocks[i].productSkus[j].cartId) {

            if (touchMoveX > startX) //右滑

              cartData.blocks[i].productSkus[j].isTouchMove = false

            else //左滑

              cartData.blocks[i].productSkus[j].isTouchMove = true

          }
        }
      }

      //更新数据
      _this.setData({
        cartData: cartData
      })

    },
    angle: function (start, end) {

      var _X = end.X - start.X,

        _Y = end.Y - start.Y

      //返回角度 /Math.atan()返回数字的反正切值

      return 360 * Math.atan(_Y / _X) / (2 * Math.PI);

    },
    getPageData: function (e) {
      var _this = this
      apiCart.pageData().then(function (res) {
        if (res.result == 1) {
          var d = res.data


          var d_cartData = d.cartData
          var p_cartData = _this.data.cartData
          p_cartData.blocks = d_cartData.blocks
          p_cartData.count = d_cartData.count
          p_cartData.sumPrice = d_cartData.sumPrice
          p_cartData.countBySelected = d_cartData.countBySelected
          p_cartData.sumPriceBySelected = d_cartData.sumPriceBySelected


          _this.setData({
            cartData: p_cartData
          })

        }
      })
    },
    onReady: function () {
      var _this = this
      // console.log("cart.onReady")
      // if(!_this.data.isOnReady){
      //   _this.setData({isOnReady:true})
      // }
    },
    onShow() {
      console.log("cart.onShow")
      var _this = this

      _this.setData({
        storeId: storeage.getStoreId(),
        isLogin: ownRequest.isLogin()
      })

      const query = wx.createSelectorQuery().in(_this)

      query.select('.cart-bottom').boundingClientRect(function (rect) {
        if (rect != null) {
          if (rect.height != null) {
            var height = _this.data.height - rect.height
            _this.setData({
              scrollHeight: height
            })
          }
        }

      }).exec()

      _this.getPageData()

    },
    clickToBuy: function (e) {
      var _this = this

      var blocks = _this.data.cartData.blocks

      var productSkus = []

      for (var i = 0; i < blocks.length; i++) {
        for (var j = 0; j < blocks[i].productSkus.length; j++) {
          if (blocks[i].productSkus[j].selected) {
            productSkus.push({
              cartId: blocks[i].productSkus[j].cartId,
              id: blocks[i].productSkus[j].id,
              quantity: blocks[i].productSkus[j].quantity,
              shopMode: blocks[i].productSkus[j].shopMode,
              shopMethod: 1,
              shopId: blocks[i].productSkus[j].shopId
            })
          }
        }
      }

      if (productSkus.length == 0) {
        toast.show({
          title: '至少选择一件商品'
        })
        return
      }

      wx.navigateTo({
        url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus),
        success: function (res) {
          // success
        },
      })
    },
    clickToGoShop: function (e) {
      e.currentTarget.dataset.replyIndex = 1
      this.triggerEvent('callSomeFun', {
        "dataset": {
          "replyIndex": 1
        }
      })
    },
    clickToGoLogin: function (e) {
      ownRequest.goLogin()
    }
  }
})