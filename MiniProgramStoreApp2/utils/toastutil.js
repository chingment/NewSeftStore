//小程序交互反馈接口
/*
显示toast提示
title:    提示的内容 必填
icon:     图标，//请指定正确的路径，选填
duration: 提示的延迟时间，单位毫秒，默认：1500, 10000永远存在除非手动清除 选填
mask:     是否显示透明蒙层，防止触摸穿透，默认：true 选填
cb:       接口调用成功的回调函数 选填
 */
function show(obj) {
  if (typeof obj == 'object' && obj.title) {
    if (!obj.duration || typeof obj.duration != 'number') { obj.duration = 1500; }//默认1.5s后消失
    var that = getCurrentPages()[getCurrentPages().length - 1];//获取当前page实例

    if (obj.duration < 10000) {
      setTimeout(function () {
        obj.isShow = false;
        obj.cb && typeof obj.cb == 'function' && obj.cb();//如果有成功的回调则执行
        that.setData({
          'showToast.isShow': obj.isShow
        });
      }, obj.duration);
    }
    obj.isShow = true;//开启toast
    that.setData({
      showToast: obj
    });
  } else {
   
  }
}
/**
 *手动关闭toast提示
 */
function hide() {
  var that = getCurrentPages()[getCurrentPages().length - 1];//获取当前page实例
  if (that.data.showToast) {
    that.setData({
      'showToast.isShow': false
    });
  }
}
module.exports = {
  show: show,
  hide: hide
}