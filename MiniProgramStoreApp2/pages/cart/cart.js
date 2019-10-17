const util = require('../../utils/util')
const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')

Component({
  options: {
    addGlobalClass: true,
    multipleSlots: true // 在组件定义时的选项中启用多slot支持
  },
  properties: {
    initdata: {
      type: Object,
      observer: function (newVal, oldVal, changedPath) {
        var _self = this
        _self.setData({
          isLogin: ownRequest.isLogin(),
          blocks: newVal.blocks,
          count: newVal.count,
          sumPrice: newVal.sumPrice,
          countBySelected: newVal.countBySelected,
          sumPriceBySelected: newVal.sumPriceBySelected
        })
      }
    },
    height: {
      type: Number
    }
  },
  data: {},
  methods: {
    itemOperate: util.throttle(function (e) {

      var _self = this
      var pIndex = e.currentTarget.dataset.replyPindex
      var cIndex = e.currentTarget.dataset.replyCindex
      var operate = e.currentTarget.dataset.replyOperate


      var productSku = _self.data.blocks[pIndex].productSkus[cIndex];

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

      function _operate() {

        apiCart.operate({
          storeId: ownRequest.getCurrentStoreId(),
          operate: operate,
          productSkus: operateProductSkus
        }, {
            success: function (res) {
              if (res.result == 1) {
                
              }
              else {
                toast.show({
                  title: res.message
                })
              }
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
            }
          }
        })

      } else {
        _operate()
      }
    },1000),
    immeBuy: function (e) {
      var _this = this

      var blocks = _this.data.blocks

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
    itemTouchstart: function (e) {

      //开始触摸时 重置所有删除
      var _this = this

      for (var i = 0; i < _this.data.blocks.length; i++) {
        for (var j = 0; j < _this.data.blocks[i].productSkus.length; j++) {
          if (_this.data.blocks[i].productSkus[j].isTouchMove) {
            _this.data.blocks[i].productSkus[j].isTouchMove = false;
          }
        }
      }

      this.setData({
        startX: e.changedTouches[0].clientX,
        startY: e.changedTouches[0].clientY,
        blocks: _this.data.blocks
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

      for (var i = 0; i < _this.data.blocks.length; i++) {
        for (var j = 0; j < _this.data.blocks[i].productSkus.length; j++) {

          _this.data.blocks[i].productSkus[j].isTouchMove = false

          //滑动超过30度角 return

          if (Math.abs(angle) > 30) return;

          if (cartId == _this.data.blocks[i].productSkus[j].cartId) {

            if (touchMoveX > startX) //右滑

              _this.data.blocks[i].productSkus[j].isTouchMove = false

            else //左滑

              _this.data.blocks[i].productSkus[j].isTouchMove = true

          }
        }
      }

      //更新数据
      // console.log(JSON.stringify(_this.data.cart))
      _this.setData({
        blocks: _this.data.blocks
      })

    },
    angle: function (start, end) {

      var _X = end.X - start.X,

        _Y = end.Y - start.Y

      //返回角度 /Math.atan()返回数字的反正切值

      return 360 * Math.atan(_Y / _X) / (2 * Math.PI);

    },
    getPageData:function(e){
      var self=this
      apiCart.pageData({
        success: function (res) { 
          self.setData({initdata:res.data})
        }
      })
    },
    goLogin: function (e) {
      ownRequest.goLogin()
    }
  }
})