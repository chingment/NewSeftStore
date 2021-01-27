const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiGlobal = require('../../api/global.js')
const storeage = require('../../utils/storeageutil.js')

Component({
  options: {
    addGlobalClass: true
  },
  properties: {
    isShow: {
      type: Boolean,
      value: false,
      observer: function (newVal, oldVal) {
        var _this = this

        if (newVal) {
          this._dialogOpen()
        } else {
          this._dialogClose()
        }
      }
    },
    dataS: {
      type: Object,
      value: {},
      observer: function (newVal, oldVal) {
        if (newVal == null)
          return
        this.setData({
          myDataS: newVal
        })
      }
    }
  },
  data: {
    myAnimationData: {},
    myShow: false,
    myStop: true,
    myDataS: {}
  },
  methods: {
    _dialogOpen: function (e) {
      var _this = this;
      if (!_this.data.myStop)
        return

      var animation = wx.createAnimation({
        duration: 300,
        timingFunction: 'linear'
      })
      animation.translateY(500).step()
      _this.setData({
        myAnimationData: animation.export(),
        myShow: true,
        isShow: true,
        myStop: false
      })
      setTimeout(function () {
        animation.translateY(0).step()
        _this.setData({
          myAnimationData: animation.export()
        })
      }, 100)

      // _this.setData({
      //   myCart: storeage.getCart()
      // })
    },
    _dialogClose: function (e) {
      var _this = this;

      var animation = wx.createAnimation({
        duration: 300,
        timingFunction: 'linear'
      })
      animation.translateY(500).step()
      _this.setData({
        myAnimationData: animation.export()
      });

      setTimeout(function () {
        _this.setData({
          myShow: false,
          isShow: false,
          myStop: true,
        })
        _this.triggerEvent('cartdialogClose')
      }, 300)
    },
    myClose: function (e) {
      console.log(e)
      var self = e.target.dataset.ref
      if (self == 'self') {
        this._dialogClose()
      }
    },
    _myClose: function (e) {
      this._dialogClose()
    },
    _buildPoster:function(e){
      var _this=this
      _this.triggerEvent('buildPoster')
    }
  }
})