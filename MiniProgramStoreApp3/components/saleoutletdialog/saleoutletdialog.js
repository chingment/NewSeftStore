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
        console.log('isShow:' + newVal)
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
        console.log('dataS:' + JSON.stringify(newVal))
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
        lng: 0
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
    }
  }
})