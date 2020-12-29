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
    let _nowTime = +new Date()
    if (_nowTime - _lastTime > gapTime || !_lastTime) {
      fn.apply(this, arguments) //将this和参数传给原函数
      _lastTime = _nowTime
    }
  }
}

function isEmptyOrNull(str) {

  if (typeof str === 'undefined')
    return true

  if (str == null)
    return true

  console.log('str.constructor：' + str.constructor.toString())

  if (str.constructor === String) {
    console.log('str.constructor：String')
    if (str.replace(/(^s*)|(s*$)/g, "").length == 0) {
      return true
    } else {
      return false
    }
  } else if (str.constructor === Array) {
    console.log('str.constructor：Array')
    if (str.length == 0) {
      return true
    } else {
      return false
    }
  } else {
    return false
  }
}

module.exports = {
  formatTime: formatTime,
  throttle: throttle,
  isEmptyOrNull: isEmptyOrNull
}