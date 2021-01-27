//index.js

Page({
  data: {
    tag:'myserviceprom',
    painting: {},
    shareImage: ''
  },
  onLoad () {
    this.eventDraw()
  },
  eventDraw () {
    wx.showLoading({
      title: '绘制分享图片中',
      mask: true
    })
    this.setData({
      painting: {
        width: 375,
        height: 555,
        clear: true,
        views: [
          {
            type: 'image',
            url: 'https://file.17fanju.com/upload/sharecode/1/bg_white.png',
            top: 0,
            left: 0,
            width: 375,
            height: 555
          },
          {
            type: 'image',
            url: 'https://file.17fanju.com/upload/Avatar/e032f079-5b22-4615-80b6-9841e5397fcb.png',
            top: 27.5,
            left: 29,
            width: 55,
            height: 55,
            borderRadius:100
          },
          {
            type: 'text',
            content: '您的好友【kuckboy】',
            fontSize: 16,
            color: '#402D16',
            textAlign: 'left',
            top: 33,
            left: 96,
            bolder: true,
          },
          {
            type: 'text',
            content: '发现一件好货，邀请你一起购买！',
            fontSize: 15,
            color: '#563D20',
            textAlign: 'left',
            top: 59.5,
            left: 96
          },
          {
            type: 'image',
            url: 'https://file.17fanju.com/upload/sharecode/1/product.png',
            top: 110,
            left: 375/2-186/2,
            width: 186,
            height: 186
          },
          {
            type: 'image',
            url: 'https://file.17fanju.com/upload/acode/714732c6-9a76-4e8d-a86f-8e87f71e85f0.png',
            top: 443,
            left: 85,
            width: 68,
            height: 68
          },
          {
            type: 'text',
            content: '正品MAC魅可口红礼盒生日唇膏小辣椒Chili西柚情人',
            fontSize: 16,
            lineHeight: 21,
            color: '#383549',
            textAlign: 'left',
            top: 336,
            left: 44,
            width: 287,
            MaxLineNumber: 2,
            breakWord: true,
            bolder: true
          },
          {
            type: 'text',
            content: '￥0.00',
            fontSize: 19,
            color: '#E62004',
            textAlign: 'left',
            top: 387,
            left: 44.5,
            bolder: true
          },
          {
            type: 'text',
            content: '原价:￥138.00',
            fontSize: 13,
            color: '#7E7E8B',
            textAlign: 'left',
            top: 391,
            left: 110,
            textDecoration: 'line-through'
          },
          {
            type: 'text',
            content: '长按识别图中二维码立即试一试~',
            fontSize: 14,
            color: '#383549',
            textAlign: 'left',
            top: 460,
            left: 165.5,
            lineHeight: 20,
            MaxLineNumber: 2,
            breakWord: true,
            width: 125
          }
        ]
      }
    })
  },
  eventSave () {
    wx.saveImageToPhotosAlbum({
      filePath: this.data.shareImage,
      success (res) {
        wx.showToast({
          title: '保存图片成功',
          icon: 'success',
          duration: 2000
        })
      }
  })
  },
  eventGetImage (event) {
    console.log("event:"+ JSON.stringify(event))
    //
    const { tempFilePath, errMsg } = event.detail
    if (errMsg === 'canvasdrawer:ok') {
      wx.hideLoading()
      this.setData({
        shareImage: tempFilePath
      })
    }
    else if(errMsg==='canvasdrawer:download fail'){
      wx.showToast({
        title: '生成失败',
        icon: '',
        duration: 2000
      })
    }
  },
  onShow: function () {},
  onUnload: function () {},
})
