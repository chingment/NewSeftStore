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
        list: {
          pageIndex: 0,
          pageSize: 10,
          pageCount: 0,
          total: 0,
          isEmpty: false,
          loading: false,
          allloaded: false,
          items: []
        }
      },
      {
        name: "待支付",
        selected: false,
        status: 2000,
        list: {
          pageIndex: 0,
          pageSize: 10,
          pageCount: 0,
          total: 0,
          isEmpty: false,
          loading: false,
          allloaded: false,
          items: []
        }
      }, {
        name: "待取货",
        selected: false,
        status: 3000,
        list: {
          pageIndex: 0,
          pageSize: 10,
          pageCount: 0,
          total: 0,
          isEmpty: false,
          loading: false,
          allloaded: false,
          items: []
        }
      }, {
        name: "已完成",
        selected: false,
        status: 4000,
        list: {
          pageIndex: 0,
          pageSize: 10,
          pageCount: 0,
          total: 0,
          isEmpty: false,
          loading: false,
          allloaded: false,
          items: []
        }
      }, {
        name: "已失效",
        selected: false,
        status: 5000,
        list: {
          pageIndex: 0,
          pageSize: 10,
          pageCount: 0,
          total: 0,
          isEmpty: false,
          loading: false,
          allloaded: false,
          items: []
        }
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
  loadmore: function (e) {
    var _this = this

    var index = _this.data.tabsSliderIndex
    console.log("index:" + index)
    _this.data.tabs[index].list.pageIndex += 1
    _this.setData({
      tabs: _this.data.tabs
    })

    _this.getList().then(function (res) {
      e.detail.success();
    })

  },
  //刷新处理
  refresh: function (e) {

    console.log("index:" + JSON.stringify(e))

    var _this = this

    var index = _this.data.tabsSliderIndex

    console.log("index:" + index)

    _this.data.tabs[index].list.pageIndex = 0
    _this.data.tabs[index].list.loading = true
    _this.data.tabs[index].list.allloaded = false
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

    var pageIndex = currentTab.list.pageIndex
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
        var allloaded = false
        var isEmpty = false
        var list = _this.data.tabs[currentTabIndex].list
        if (d.pageIndex == 0) {
          items = d.items
        } else {
          items = list.items.concat(d.items)
        }

        if (d.total == 0) {
          isEmpty = true
        }

        if ((d.pageIndex + 1) >= d.pageCount) {
          allloaded = true
        }


        list.allloaded = allloaded
        list.isEmpty = isEmpty
        list.loading = false
        list.total = d.total
        list.pageSize = d.pageSize
        list.pageCount = d.pageCount
        list.pageIndex = d.pageIndex
        list.items = items;
        _this.data.tabs[currentTabIndex].list = list
        _this.setData({
          tabs: _this.data.tabs
        })

      }
    })
  },
  stopTouchMove: function () {
    return false;
  }
})