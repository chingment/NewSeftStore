const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const apiOrder = require('../../api/order.js')
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
    width: 0,
    curDateAreaIndex: 0,
    curTimeAreaIndex: 0,
    dateArea: [],
    timeArea: [],
    myDataS: {}
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
        isShow: true,
        myStop: false
      })

      setTimeout(function () {
        animation.translateY(0).step()
        _this.setData({
          myAnimationData: animation.export()
        })
      }, 200)

      apiOrder.buildBookTimeArea({
        appCaller: 1
      }).then(function (res) {
        if (res.result == 1) {

          var d = res.data


          var value = _this.data.myDataS.bookTime.value
          // console.log(JSON.stringify(_this.data.myDataS))

          var dateArea = d.dateArea


          var curDateAreaIndex = _this.data.curDateAreaIndex
          for (var i = 0; i < dateArea.length; i++) {
            if (typeof value != 'undefined' && value != null) {
              if (value.indexOf(dateArea[i].value) > -1) {
                curDateAreaIndex = i
                break
              }
            }
          }

          var curTimeAreaIndex = _this.data.curTimeAreaIndex
          var curDateTimeArea = dateArea[curDateAreaIndex].timeArea

          for (var i = 0; i < curDateTimeArea.length; i++) {
            if (typeof value != 'undefined' && value != null) {
              if (value.indexOf(curDateTimeArea[i].value) > -1) {
                curTimeAreaIndex = i
                break
              }
            }
          }


          var timeArea = dateArea[curDateAreaIndex].timeArea
          _this.setData({
            curDateAreaIndex: curDateAreaIndex,
            curTimeAreaIndex: curTimeAreaIndex,
            dateArea: dateArea,
            timeArea: timeArea,
            width: 186 * parseInt(dateArea.length - _this.data.curDateAreaIndex <= 7 ? dateArea.length : 7)
          })

        }
      })


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
      })
      setTimeout(function () {
        _this.setData({
          myShow: false,
          isShow: false,
          myStop: true
        })
      }, 200)
    },
    _selectDate: function (e) {
      var _this = this
      var curDateAreaIndex = e.currentTarget.dataset.index

      var lastDateArea = _this.data.dateArea[_this.data.curDateAreaIndex]
      var lastTimeArea = lastDateArea.timeArea[_this.data.curTimeAreaIndex]


      var curTimeArea = _this.data.dateArea[curDateAreaIndex].timeArea

      var curTimeAreaIndex = _this.data.curTimeAreaIndex

      for (var i = 0; i < curTimeArea.length; i++) {
        if (typeof lastTimeArea.value != 'undefined') {
          if (lastTimeArea.value.indexOf(curTimeArea[i].value) > -1) {
            curTimeAreaIndex = i
            break
          }
        }
      }

      _this.setData({
        curDateAreaIndex: curDateAreaIndex,
        curTimeAreaIndex: curTimeAreaIndex,
        timeArea: curTimeArea
      })
      _this._getSelectBookTime()
    },
    _selectTime: function (e) {
      var _this = this
      var curTimeAreaIndex = e.currentTarget.dataset.tindex
      _this.setData({
        curTimeAreaIndex: curTimeAreaIndex
      })
      _this._getSelectBookTime()
    },
    _getSelectBookTime: function (e) {
      var _this = this
      var curDate = _this.data.dateArea[_this.data.curDateAreaIndex]
      var curTime = _this.data.timeArea[_this.data.curTimeAreaIndex]
      this.triggerEvent('getSelectBookTime', {
        params: {
          text: '（' + curDate.week + '）' + curDate.date + ' ' + curTime.time,
          value: curDate.value + ' ' + curTime.value,
          type: curTime.type
        }
      }, {})
    },
    _confirm: function (e) {
      var _this = this
      _this._getSelectBookTime()
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