const util = require('../../utils/util.js')
const storeage = require('../../utils/storeageutil.js')
const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const apiOrder = require('../../api/order.js')
const app = getApp()

Page({
  data: {
    tag: "myreffskus",
    scrollHeight:0,
    dataList: {
      isEmpty: false,
      allloaded: false,
      loading: false,
      total: 0,
      pageIndex: 0,
      pageCount: 0,
      items: []
    }
  },
  onLoad: function (options) {
    var _this = this

    var wHeight = wx.getSystemInfoSync().windowHeight;
      _this.setData({
        scrollHeight: wHeight
    });
  
    _this.search()
  },
  onShow: function () {},
  onUnload: function () {},
  //加载更多
  loadmore: function (e) {
    var _this = this

    if (!_this.data.dataList.loading) {
      _this.data.dataList.pageIndex += 1
      _this.setData({
        dataList: _this.data.dataList
      })
      _this.search().then(res => {
        e.detail.success();
      });
    }
  },
  //刷新处理
  refresh: function (e) {
    var _this = this
    _this.data.dataList.pageIndex = 0
    _this.data.dataList.loading = false
    _this.data.dataList.allloaded = false
    _this.data.dataList.isEmpty = false
    _this.search().then(res => {
      e.detail.success();
    });
  },
  onShow() {
    var _this = this
    app.globalData.skeletonPage = _this;
  },
  search: function () {
    var _this = this

    _this.setData({
      loading: true
    })

    var pageIndex = _this.data.dataList.pageIndex

    return apiOrder.getMyRent({
      pageIndex: pageIndex,
      pageSize: 10,
    }, false).then(function (res) {
      if (res.result == 1) {
        var d = res.data
        var items = []
        var allloaded = false
        var isEmpty = false
        var list = _this.data.dataList
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

        list.loading = false
        list.allloaded = allloaded
        list.isEmpty = isEmpty
        list.total = d.total
        list.pageSize = d.pageSize
        list.pageCount = d.pageCount
        list.pageIndex = d.pageIndex
        list.items = items;

        _this.setData({
          dataList: list
        })
      }
    })
  },
  operate: function (e) {
    var _this = this

    var opType = e.currentTarget.dataset.replyOptype
    var opVal = e.currentTarget.dataset.replyOpval
    var orderId = e.currentTarget.dataset.replyOrderId
    var skuId = e.currentTarget.dataset.replySkuId

    switch (opType) {
      case "FUN":

        switch (opVal) {
          case "payRent":
           
            var productSkus = []
            productSkus.push({
              cartId: 0,
              id: skuId,
              quantity: 1,
              shopMode: 1,
              shopMethod:5,
              shopId:'0'
            })
            wx.navigateTo({
              url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus) + "&shopMethod=5&action=rentfee&pOrderId="+orderId,
              success: function (res) {
                // success
              },
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
})