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
        console.log('newVal:' + newVal)
        this.setData({
          myCart: newVal
        })
      }
    }
  },
  data: {
    myAnimationData: {},
    myShow: false,
    myStop: true,
    calendar: [],
    width: 0,
    curDateAreaIndex: 0,
    curTimeAreaIndex: 0,
    dateArea: [{
        week: '今天',
        date: '2020-11-22',
        status: 1,
        tip: "xxx",
        timeArea: [{
          time: "1:00-2:00",
          type: 1,
          status: "1",
          tip: "约满"
        }, {
          time: "2:00-3:00",
          type: 1,
          status: "1",
          tip: "约满"
        }, {
          time: "4:00-4:00",
          type: 1,
          status: "1",
          tip: "约满"
        },
        {
          time: "1:00-2:00",
          type: 1,
          status: "1",
          tip: "约满"
        }, {
          time: "2:00-3:00",
          type: 1,
          status: "1",
          tip: "约满"
        }, {
          time: "4:00-4:00",
          type: 1,
          status: "1",
          tip: "约满"
        }
      ]
      },
      {
        week: '明天',
        date: '2020-11-23',
        status: 1,
        tip: "xxxx",
        timeArea: [{
          time: "4:00-5:00",
          type: 1,
          status: "1",
          tip: "约满2"
        }, {
          time: "6:00-7:00",
          type: 1,
          status: "1",
          tip: "约满"
        }, {
          time: "8:00-9:00",
          type: 1,
          status: "1",
          tip: "约满"
        }]
      },
      {
        week: '明天',
        date: '2020-11-24',
        status: 1,
        tip: "xxxx",
        timeArea: [{
          time: "4:00-5:00",
          type: 1,
          status: "1",
          tip: "约满3"
        }, {
          time: "6:00-7:00",
          type: 1,
          status: "1",
          tip: "约满"
        }, {
          time: "8:00-9:00",
          type: 1,
          status: "1",
          tip: "约满"
        }]
      },
      {
        week: '明天',
        date: '2020-11-25',
        status: 1,
        tip: "xxxx",
        timeArea: [{
          time: "4:00-5:00",
          type: 1,
          status: "1",
          tip: "约满4"
        }, {
          time: "6:00-7:00",
          type: 1,
          status: "1",
          tip: "约满"
        }, {
          time: "8:00-9:00",
          type: 1,
          status: "1",
          tip: "约满"
        }]
      },
      {
        week: '明天',
        date: '2020-11-23',
        status: 1,
        tip: "xxxx",
        timeArea: [{
          time: "4:00-5:00",
          type: 1,
          status: "1",
          tip: "约满5"
        }, {
          time: "6:00-7:00",
          type: 1,
          status: "1",
          tip: "约满"
        }, {
          time: "8:00-9:00",
          type: 1,
          status: "1",
          tip: "约满"
        }]
      }
    ],
    timeArea: null
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

      _this.setData({
        timeArea: _this.data.dateArea[_this.data.curDateAreaIndex].timeArea,
        width: 186 * parseInt(_this.data.dateArea.length - _this.data.curDateAreaIndex <= 7 ? _this.data.dateArea.length : 7)
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
      });
      setTimeout(function () {
        _this.setData({
          myShow: false,
          isShow: false,
          myStop: true,
        })
      }, 500)
    },
    _selectDate: function (e) {
      var _this = this
      var curDateAreaIndex = e.currentTarget.dataset.index
      console.log('curDateAreaIndex>>'+curDateAreaIndex)
      var timeArea = _this.data.dateArea[curDateAreaIndex].timeArea
      _this.setData({
        curDateAreaIndex: curDateAreaIndex,
        timeArea: timeArea
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
      this.triggerEvent('getselectbooktime', {
        params: {
          week: curDate.week,
          date: curDate.date,
          time: curTime.time,
          type: curTime.type
        }
      }, {})
    },
    _confirm: function (e) {
      var _this = this
      _this._getSelectBookTime()
      _this._dialogClose()
    }
  }
})