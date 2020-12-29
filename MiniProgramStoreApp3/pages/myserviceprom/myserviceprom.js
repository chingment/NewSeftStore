Page({
  data: {
    canvas: '',
    canvas_width: 700,
    canvas_height: 800,
    main: "https://file.17fanju.com/upload/Avatar_default.png",
    logo: "https://file.17fanju.com/upload/Avatar_default.png",
    explain: "种一棵树最好的时间是十年前，其次是现在",
    code: "https://file.17fanju.com/upload/Avatar_default.png",
    avatar: "https://file.17fanju.com/upload/Avatar_default.png"
  },

  onLoad: function (options) {
    var logo = this.data.logo
    var main = this.data.main
    var explain = this.data.explain
    var code = this.data.code
    var avatar = this.data.avatar
    this.getcanvas(logo, main, explain, code, avatar)
  },
  // 获取海报 画布设置宽700 高800 
  // 以此传入图标，主体图片，标题文字，小程序二维码
  getcanvas(logo, main, explain, code, avatar) {
    wx.showLoading({
      title: '加载中',
    })
    // 主图所需
    let primary = ""
    let primary_width = ""
    let primary_height = ""
    // logo所需
    let mark = ""
    let mark_width = ""
    let mark_height = ""
    // 小程序码所需
    let yard = ""
    let yard_width = ""
    let yard_height = ""
    // 用户头像
    let portrait = ""
    let portrait_width = ""
    let portrait_height = ""

    return new Promise(resolve => {
        wx.getImageInfo({
          src: main,
          success: (res) => {
            primary = res.path
            primary_width = res.width
            primary_height = res.height
            resolve(res)
          }
        })
      })

      .then(res => {
        if (res.errMsg == "getImageInfo:ok") {
          return new Promise(resolve => {
            wx.getImageInfo({
              src: logo,
              success: (res) => {
                mark = res.path
                mark_width = res.width
                mark_height = res.height
                resolve(res)
              }
            })
          })
        }
      })

      .then(res => {
        if (res.errMsg == "getImageInfo:ok") {
          return new Promise(resolve => {
            wx.getImageInfo({
              src: code,
              success: (res) => {
                yard = res.path
                yard_width = res.width
                yard_height = res.height
                resolve(res)
              }
            })
          })
        }
      })

      .then(res => {
        if (res.errMsg == "getImageInfo:ok") {
          return new Promise(resolve => {
            wx.getImageInfo({
              src: avatar,
              success: (res) => {
                portrait = res.path
                portrait_width = res.width
                portrait_height = res.height
                resolve(res)
              }
            })
          })
        }
      })

      .then(res => {
        let ctx = wx.createCanvasContext('mycanvas')
        // 填充背景颜色
        ctx.rect(0, 0, this.data.canvas_width, this.data.canvas_height)
        ctx.setFillStyle('#fff')
        ctx.fill()
        // logo图片
        ctx.drawImage(mark, 30, 30, mark_width, mark_height)
        // 竖线
        ctx.moveTo(mark_width + 40, 30)
        ctx.lineTo(mark_width + 40, mark_height + 30)
        ctx.stroke()
        // 标题文字
        ctx.setFontSize(28)
        ctx.setFillStyle('#000')
        ctx.fillText('标题文字', mark_width + 50, mark_height + 15)
        // 用户头像
        ctx.save()
        ctx.beginPath()
        ctx.arc(this.data.canvas_width - 90, 60, 50, 0, Math.PI * 2, false);
        ctx.stroke()
        ctx.clip();
        ctx.drawImage(portrait, this.data.canvas_width - 145, 5, 110, 110) //145为arc的90加110除以2
        ctx.restore()
        // 主体图片
        ctx.drawImage(primary, this.data.canvas_width / 2 - primary_width / 2.4, 140, primary_width / 1.2, primary_height / 1.2)
        // 详情
        ctx.setFontSize(24)
        ctx.setTextAlign('center')
        ctx.setFillStyle('#666')
        ctx.fillText(explain, this.data.canvas_width / 2, 500)
        ctx.fillText('你的好友分享给你的小程序', this.data.canvas_width / 2, 550)
        // 小程序二维码
        ctx.drawImage(yard, this.data.canvas_width / 2 - yard_width / 2.4, 600, yard_width / 1.2, yard_height / 1.2)
        ctx.draw(true, setTimeout(() => {
          wx.canvasToTempFilePath({
            canvasId: 'mycanvas',
            success: (res) => {
              wx.hideLoading()
              console.log(res.tempFilePath)
              this.setData({
                canvas: res.tempFilePath
              })
            },
          }, this)
        }, 500))
      })
  },
  // 保存为图片
  getsave() {
    wx.getSetting({ //询问用户是否保存相册到本地
      success: (set) => {
        wx.saveImageToPhotosAlbum({
          filePath: this.data.canvas,
          success: (res) => {
            if (res.errMsg == "saveImageToPhotosAlbum:ok") {
              wx.showToast({
                title: '保存成功',
              });
              this.setData({
                show: 0
              })
            }
          }
        })
        //拒绝保存到本地的处理机制
        if (set.authSetting['scope.writePhotosAlbum'] == false) {
          wx.openSetting()
        }
      }
    })
  },
})