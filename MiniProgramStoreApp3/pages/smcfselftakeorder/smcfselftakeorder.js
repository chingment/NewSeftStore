const config = require('../../config')
const toast = require('../../utils/toastutil')
const util = require('../../utils/util')
const storeage = require('../../utils/storeageutil.js')
const QRCode = require('../../utils/qrcode.js')
const ownRequest = require('../../own/ownRequest.js')
const apiSmCfSelfTakeOrdeOrder = require('../../api/smcfselftakeorder.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "smcfselftakeorder"
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var orderId = options.id == undefined ? "" : options.id

    apiSmCfSelfTakeOrdeOrder.details({
      id: orderId
    }).then(function (res) {
      if (res.result == 1) {
        _this.setData(res.data)
      }
    })
  },
  onShow: function () {},
  onUnload: function () {},
  clickToScanCode: function (e) {
    var _this = this
    var _data = _this.data
    var blockindex = e.currentTarget.dataset.replyBlockindex
    var dataindex = e.currentTarget.dataset.replyDataindex
    var deviceidindex = e.currentTarget.dataset.replyDeviceidindex

    //console.log("blockindex:" + blockindex)
    //console.log("dataindex:" + dataindex)
    //console.log("deviceidindex:" + deviceidindex)



    wx.scanCode({
      success: (res) => {
        var code = res.result

        console.log("code:" + code)


        for (var i = 0; i < _data.blocks.length; i++) {
          var data = _data.blocks[i].data

          for (var j = 0; j < data.length; j++) {
            var isNeedWrDeviceId = _data.blocks[i].data[j].value.isNeedWrDeviceId
            if (isNeedWrDeviceId) {
              var deviceIds =_data.blocks[i].data[j].value.deviceIds

              for (var z = 0; z < deviceIds.length; z++) {

                if(!util.isEmptyOrNull(deviceIds[z])){

                  if(deviceIds[z]==code){
                    
                    toast.show({
                      title: '已存在'
                    })
                    
                    return
                  }
                }
              }
            }
          }

        }

        _data.blocks[blockindex].data[dataindex].value.deviceIds[deviceidindex] = code

        _this.setData({
          blocks: _data.blocks
        })

      }
    })


  }
})