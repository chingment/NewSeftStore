const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const apiSaleOutlet = require('../../api/saleoutlet.js')
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
    myDataS: {
      curSaleOutletId: '',
      list: []
    }
  },
  methods: {
    _dialogOpen: function (e) {
      var _this = this;
      if (!_this.data.myStop)
        return

      apiSaleOutlet.list({
        lat: 0,
        lng: 0,
        storeId:storeage.getStoreId(),
      }).then(function (res) {
        if (res.result == 1) {
          var d = res.data
          var myDataS = _this.data.myDataS
          myDataS.list = d.saleOutlets
          _this.setData({
            myDataS: myDataS
          })
        }
      })

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
      }, 300)
    },
    _selectItem: function (e) {
      var _this = this
      var item = e.currentTarget.dataset.replyItem

      _this.triggerEvent('_selectSaleoutletItem', {
        saleOutlet: item
      })

      _this._dialogClose()

    },
    myClose: function (e) {
      var self = e.target.dataset.ref
      if (self == 'self') {
        this._dialogClose()
      }
    }
  }
})