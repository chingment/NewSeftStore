const storeage = require('../../utils/storeageutil.js')
const util = require('../../utils/util.js')
const qrcode = require('../../utils/qrcode.js')
const ownRequest = require('../../own/ownRequest.js')
const apiOrder = require('../../api/order.js')
const app = getApp()

Page({
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
        pageSize: 10,
        pageCount: 0,
        total: 0,
        loading: false,
        allloaded: false,
        list: []
      },
      {
        name: "待支付",
        selected: false,
        status: 2000,
        pageIndex: 0,
        pageSize: 10,
        pageCount: 0,
        total: 0,
        loading: false,
        allloaded: false,
        list: []
      }, {
        name: "待取货",
        selected: false,
        status: 3000,
        pageIndex: 0,
        pageSize: 10,
        pageCount: 0,
        total: 0,
        loading: false,
        allloaded: false,
        list: []
      }, {
        name: "已完成",
        selected: false,
        status: 4000,
        pageIndex: 0,
        pageSize: 10,
        pageCount: 0,
        total: 0,
        loading: false,
        allloaded: false,
        list: []
      }, {
        name: "已失效",
        selected: false,
        status: 5000,
        pageIndex: 0,
        pageSize: 10,
        pageCount: 0,
        total: 0,
        loading: false,
        allloaded: false,
        list: []
      }]
  },
  onLoad: function (options) {
    var _this = this

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
      tabs: _this.data.tabs
    })

    wx.createSelectorQuery().selectAll('.tabbar-items').boundingClientRect(function (rect) {
      var wHeight = wx.getSystemInfoSync().windowHeight;
      _this.setData({
        scrollHeight: wHeight - rect[0].height
      });
    }).exec()

    this.getList()

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

    _this.getList()
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
                      _this.getList()
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
  //加载更多
  loadmore:function(e) {
    var _this = this

    var index = _this.data.tabsSliderIndex 
    console.log("index:" + index)
    _this.data.tabs[index].pageIndex += 1
    _this.setData({
      tabs: _this.data.tabs
    })

    _this.getList().then(function (res) {
      e.detail.success();
    })
   
  },
  //刷新处理
  refresh:function(e) {

    console.log("index:" + JSON.stringify(e))
  
    var _this = this

    var index =_this.data.tabsSliderIndex 

    console.log("index:" + index)

    _this.data.tabs[index].pageIndex = 0
    _this.data.tabs[index].loading = true
    _this.data.tabs[index].allloaded = false
    _this.setData({
      tabs: _this.data.tabs
    })
    _this.getList().then(function (res) {
      e.detail.success();
    })
  },
  getList() {
    var _this = this

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

    console.log("pageIndex:" + pageIndex)
    return apiOrder.list({
      storeId: ownRequest.getCurrentStoreId(),
      pageIndex: pageIndex,
      status: status,
      caller: 1
    }).then(function (res) {
      if (res.result == 1) {
        var d = res.data
        var items = []
        if (currentTab.pageIndex == 0) {
          items = d.items
        } else {
          items = _this.data.tabs[currentTabIndex].list.concat(d.items)
        }

        if (d.items.length == 0) {
          _this.data.tabs[currentTabIndex].allloaded = true
        }
        else {
          _this.data.tabs[currentTabIndex].allloaded = false
        }

        _this.data.tabs[currentTabIndex].loading = false
        _this.data.tabs[currentTabIndex].total = d.total
        _this.data.tabs[currentTabIndex].pageSize = d.pageSize
        _this.data.tabs[currentTabIndex].pageCount = d.pageCount
        _this.data.tabs[currentTabIndex].pageIndex = d.pageIndex
        _this.data.tabs[currentTabIndex].list = items;

        _this.setData({
          tabs: _this.data.tabs
        })

      }
    })
  },
  stopTouchMove: function() {
    return false;
  }
})