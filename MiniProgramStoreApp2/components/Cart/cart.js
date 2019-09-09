const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const storeage = require('../../utils/storeageutil.js')

Component({
  options: {
    addGlobalClass: true
  },
  properties: {
    show: {
      type: Boolean,
      value: false
    }
  },
  data: {
    animationData: {},
    cart: {
      blocks: [],
      count: 0,
      sumPrice: 0,
      countBySelected: 0,
      sumPriceBySelected: 0
    }
  },
  methods: {
    open: function (e) {
      // 用that取代this，防止不必要的情况发生
      var that = this;
      if (!that.data.show) {
        // 创建一个动画实例
        var animation = wx.createAnimation({
          // 动画持续时间
          duration: 300,
          // 定义动画效果，当前是匀速
          timingFunction: 'linear'
        })
        // 将该变量赋值给当前动画
        that.animation = animation
        // 先在y轴偏移，然后用step()完成一个动画
        animation.translateY(300).step()
        // 用setData改变当前动画
        that.setData({
          // 通过export()方法导出数据
          animationData: animation.export(),
          // 改变view里面的Wx：if
          show: true
        })
        // 设置setTimeout来改变y轴偏移量，实现有感觉的滑动
        setTimeout(function () {
          animation.translateY(0).step()
          that.setData({
            animationData: animation.export()
          })
        }, 200)
      }



      that.setData({ cart: storeage.getCart() })


    },
    _close: function (e) {
      var that = this;
      var animation = wx.createAnimation({
        duration: 300,
        timingFunction: 'linear'
      })
      that.animation = animation
      animation.translateY(300).step()
      that.setData({
        animationData: animation.export()
      });

      setTimeout(function () {
        that.setData({
          show: false
        })
      }, 300)
    },
    _itemOperate(e) {
      console.log('cartBarListItemCheck');
      var _self = this
      var pIndex = e.currentTarget.dataset.replyPindex
      var cIndex = e.currentTarget.dataset.replyCindex
      var operate = e.currentTarget.dataset.replyOperate
      console.log('cartBarListItemCheck.pIndex:' + pIndex)
      console.log('cartBarListItemCheck.cIndex:' + cIndex)
      console.log('cartBarListItemCheck.operate' + operate)

      var productSku = _self.data.cart.blocks[pIndex].productSkus[cIndex];

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
        receptionMode: productSku.receptionMode
      });

      console.log('ownRequest.getCurrentStoreId():' + ownRequest.getCurrentStoreId())

      function _operate() {

        apiCart.operate({
          storeId: ownRequest.getCurrentStoreId(),
          operate: operate,
          productSkus: operateProductSkus
        }, {
            success: function (res) {

              _self.setData({ cart: storeage.getCart() })
            },
            fail: function () { }
          })
      }

      if (operate == 4) {
        wx.showModal({
          title: '提示',
          content: '确定要删除吗？',
          success: function (sm) {
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
    _immeBuy: function (e) {
      var _this = this

      var blocks = _this.data.cart.blocks

      var productSkus = []

      for (var i = 0; i < blocks.length; i++) {
        for (var j = 0; j < blocks[i].productSkus.length; j++) {
          if (blocks[i].productSkus[j].selected) {
            productSkus.push({
              cartId: blocks[i].productSkus[j].cartId,
              id: blocks[i].productSkus[j].id,
              quantity: blocks[i].productSkus[j].quantity,
              receptionMode: blocks[i].productSkus[j].receptionMode
            })
          }
        }
      }

      if (productSkus.length == 0) {
        console.log("至少选择一件商品")
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
    _itemTouchstart: function (e) {

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
    _itemTouchmove: function (e) {

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
      // console.log(JSON.stringify(_this.data.cart))
      _this.setData({
        cart: _this.data.cart
      })

    },
    angle: function (start, end) {

      var _X = end.X - start.X,

        _Y = end.Y - start.Y

      //返回角度 /Math.atan()返回数字的反正切值

      return 360 * Math.atan(_Y / _X) / (2 * Math.PI);

    }
  }
})