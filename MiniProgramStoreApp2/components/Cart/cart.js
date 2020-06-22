const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const storeage = require('../../utils/storeageutil.js')

Component({
  options: {
    addGlobalClass: true
  },
  properties: {
    isShow: {
      type: Boolean,
      value: false,
      observer:function(newVal,oldVal){
        if(newVal){
          this._dialogOpen()
        }
        else{
          this._dialogClose();
        }
      }
    }
  },
  data: {
    myAnimationData: {},
    myShow:false,
    myStop: true,
    myCart: {
      blocks: [],
      count: 0,
      sumPrice: 0,
      countBySelected: 0,
      sumPriceBySelected: 0
    }
  },
  methods: {
    _dialogOpen: function (e) {
      var _this = this;
      if (!_this.data.myStop)
      return

        var animation = wx.createAnimation({
          duration: 200,
          timingFunction: 'linear'
        })
        animation.translateY(500).step()
        _this.setData({
          myAnimationData: animation.export(),
          myShow: true,
          isShow:true,
          myStop: false
        })
        setTimeout(function () {
          animation.translateY(0).step()
          _this.setData({
            myAnimationData: animation.export()
          })
        }, 200)
      
      _this.setData({ myCart: storeage.getCart() })
    },
    _dialogClose: function (e) {
      var _this = this;
      var animation = wx.createAnimation({
        duration: 500,
        timingFunction: 'linear'
      })
      animation.translateY(500).step()
      _this.setData({
        myAnimationData: animation.export()
      });

      setTimeout(function () {
        _this.setData({
          myShow: false,
          isShow:false,
          myStop:true,
        })
      }, 500)
    },
    _itemOperate(e) {
      var _this = this
      var pIndex = e.currentTarget.dataset.replyPindex
      var cIndex = e.currentTarget.dataset.replyCindex
      var operate = e.currentTarget.dataset.replyOperate
      var productSku = _this.data.myCart.blocks[pIndex].productSkus[cIndex];

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
        shopMode : productSku.shopMode
      });

      function _operate() {

        apiCart.operate({
          storeId: ownRequest.getCurrentStoreId(),
          operate: operate,
          productSkus: operateProductSkus
        }).then(function (res) {
          if(res.result==1){
            console.log("storeage.getCart():"+JSON.stringify(storeage.getCart()))
            _this.setData({ myCart: storeage.getCart() })
          }
          else{
            toast.show({
              title: res.message
            })
          }
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
            }
          }
        })

      } else {
        _operate()
      }
    },
    _immeBuy: function (e) {
      var _this = this

      var blocks = _this.data.myCart.blocks

      var productSkus = []

      for (var i = 0; i < blocks.length; i++) {
        for (var j = 0; j < blocks[i].productSkus.length; j++) {
          if (blocks[i].productSkus[j].selected) {
            productSkus.push({
              cartId: blocks[i].productSkus[j].cartId,
              id: blocks[i].productSkus[j].id,
              quantity: blocks[i].productSkus[j].quantity,
              shopMode: blocks[i].productSkus[j].shopMode
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
    _itemTouchstart: function (e) {

      //开始触摸时 重置所有删除
      var _this = this

      for (var i = 0; i < _this.data.myCart.blocks.length; i++) {
        for (var j = 0; j < _this.data.myCart.blocks[i].productSkus.length; j++) {
          if (_this.data.myCart.blocks[i].productSkus[j].isTouchMove) {
            _this.data.myCart.blocks[i].productSkus[j].isTouchMove = false;
          }
        }
      }

      this.setData({
        startX: e.changedTouches[0].clientX,
        startY: e.changedTouches[0].clientY,
        myCart: _this.data.myCart
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


      for (var i = 0; i < _this.data.myCart.blocks.length; i++) {
        for (var j = 0; j < _this.data.myCart.blocks[i].productSkus.length; j++) {

          _this.data.myCart.blocks[i].productSkus[j].isTouchMove = false

          //滑动超过30度角 return

          if (Math.abs(angle) > 30) return;

          if (cartId == _this.data.myCart.blocks[i].productSkus[j].cartId) {

            if (touchMoveX > startX) //右滑

              _this.data.myCart.blocks[i].productSkus[j].isTouchMove = false

            else //左滑

              _this.data.myCart.blocks[i].productSkus[j].isTouchMove = true

          }
        }
      }

      //更新数据
      _this.setData({
        myCart: _this.data.myCart
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