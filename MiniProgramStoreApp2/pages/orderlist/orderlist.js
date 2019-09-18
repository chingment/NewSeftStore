const storeage = require('../../utils/storeageutil.js')
const util = require('../../utils/util.js')
const ownRequest = require('../../own/ownRequest.js')
const apiOrder = require('../../api/order.js')
const app = getApp()

var getList = function (_this) {
  var currentTab;
  var currentTabIndex = -1;
  for (var i = 0; i < _this.data.tabs.length; i++) {
    if (_this.data.tabs[i].selected == true) {
      currentTab = _this.data.tabs[i];
      currentTabIndex = i;
    }
  }

  if (currentTabIndex == -1) {
    currentTabIndex = 0;
    currentTab = _this.data.tabs[currentTabIndex];
  }

  var pageIndex = currentTab.pageIndex
  var status = currentTab.status == undefined ? "" : currentTab.status


  apiOrder.list({
    storeId: ownRequest.getCurrentStoreId(),
    pageIndex: pageIndex,
    status: status,
    caller: 1
  }, {
      success: function (res) {
        if (res.result == 1) {
          var list
          if (currentTab.pageIndex == 0) {
            list = res.data
          } else {
            list = _this.data.tabs[currentTabIndex].list.concat(res.data)
          }

          _this.data.tabs[currentTabIndex].list = list;

          _this.setData({
            tabs: _this.data.tabs
          })
        }
      },
      fail: function () { }
    })
}

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "orderlist",
    scrollHeight: 0,
    tabsSliderIndex: -1,
    tabs: [
      {
        name: "全部",
        selected: false,
        status: "0000",
        pageIndex: 0,
        list: null
      },
      {
        name: "待支付",
        selected: false,
        status: 2000,
        pageIndex: 0,
        list: null
      }, {
        name: "待取货",
        selected: false,
        status: 3000,
        pageIndex: 0,
        list: null
      }, {
        name: "已完成",
        selected: false,
        status: 4000,
        pageIndex: 0,
        list: null
      }, {
        name: "已失效",
        selected: false,
        status: 5000,
        pageIndex: 0,
        list: null
      }]
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var wHeight = wx.getSystemInfoSync().windowHeight;
    var status = options.status == undefined ? "" : options.status


    var _tabsSliderIndex = -1
    for (var i = 0; i < _this.data.tabs.length; i++) {
      if (_this.data.tabs[i].status == status) {
        _this.data.tabs[i].selected = true;
        _tabsSliderIndex = i;
      } else {
        _this.data.tabs[i].selected = false;
      }
    }

    _this.setData({
      tabsSliderIndex: _tabsSliderIndex,
      tabs: _this.data.tabs,
      scrollHeight: wHeight - util.rem2px(2)
    })

    getList(_this)

  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },
  //tab点击
  tabBarClick: function (e) {

    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var _this = this

    for (var i = 0; i < _this.data.tabs.length; i++) {
      if (i == index) {
        _this.data.tabs[i].selected = true;
      } else {
        _this.data.tabs[i].selected = false;
      }
    }
    _this.setData({
      tabs: _this.data.tabs,
      tabsSliderIndex: index,
    })

    getList(_this)
  },
  operate: function (e) {
    var _this = this

    var opType = e.currentTarget.dataset.replyOptype
    var opVal = e.currentTarget.dataset.replyOpval
    var id = e.currentTarget.dataset.replyId

    console.log("opType:" + opType + ",opVal:" + opVal + ",id:" + id)

    switch (opType) {
      case "FUN":

        switch (opVal) {
          case "cancleOrder":
            wx.showModal({
              title: '提示',
              content: '确定要取消吗？',
              success: function (sm) {
                if (sm.confirm) {
                  apiOrder.cancle({
                    id: id
                  }, {
                      success: function (res) {
                        getList(_this)
                      },
                      fail: function () { }
                    })
                }
              }
            })
            break;
        }

        break;
      case "URL":
        wx.navigateTo({
          url: opVal
        })
        break;
    }
  },
  stopTouchMove: function () {
    return false;
  }
})