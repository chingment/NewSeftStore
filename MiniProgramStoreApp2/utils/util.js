const formatTime = date => {
  const year = date.getFullYear()
  const month = date.getMonth() + 1
  const day = date.getDate()
  const hour = date.getHours()
  const minute = date.getMinutes()
  const second = date.getSeconds()

  return [year, month, day].map(formatNumber).join('/') + ' ' + [hour, minute, second].map(formatNumber).join(':')
}

const formatNumber = n => {
  n = n.toString()
  return n[1] ? n : '0' + n
}


function throttle(fn, gapTime) {
  if (gapTime == null || gapTime == undefined) {
    gapTime = 1500
  }

  let _lastTime = null

  // 返回新的函数
  return function () {
    let _nowTime = + new Date()
    if (_nowTime - _lastTime > gapTime || !_lastTime) {
      fn.apply(this, arguments)   //将this和参数传给原函数
      _lastTime = _nowTime
    }
  }
}

function rem2px(rem) {
  var size = 0;

  var wHidth = wx.getSystemInfoSync().windowWidth;
 

  if (wHidth >= 360 & wHidth < 414) {
    size = wHidth / 23 * rem
  }
  else if (wHidth >= 414) {
    size = wHidth / 26 * rem
  }
  else {
    size = wHidth / 26 * rem
  }


  return size
}

module.exports = {
  formatTime: formatTime,
  throttle: throttle,
  rem2px: rem2px
}
