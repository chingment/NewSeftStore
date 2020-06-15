const config = require('../config')
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')

var class2type = {}
var toString = class2type.toString
var hasOwn = class2type.hasOwnProperty
var support = {}

var requestHandler = {
  success: function (res) {
    // success  
  },
  fail: function () {
    // fail  
  }
}

function extend() {
  var src, copyIsArray, copy, name, options, clone,
    target = arguments[0] || {},
    i = 1,
    length = arguments.length,
    deep = false;

  // Handle a deep copy situation
  if (typeof target === "boolean") {
    deep = target;

    // skip the boolean and the target
    target = arguments[i] || {};
    i++;
  }

  // Handle case when target is a string or something (possible in deep copy)
  if (typeof target !== "object" && !isFunction(target)) {
    target = {};
  }

  // extend jQuery itself if only one argument is passed
  if (i === length) {
    target = this;
    i--;
  }

  for (; i < length; i++) {
    // Only deal with non-null/undefined values
    if ((options = arguments[i]) != null) {
      // Extend the base object
      for (name in options) {
        src = target[name];
        copy = options[name];

        // Prevent never-ending loop
        if (target === copy) {
          continue;
        }

        // Recurse if we're merging plain objects or arrays
        if (deep && copy && (isPlainObject(copy) || (copyIsArray = isArray(copy)))) {
          if (copyIsArray) {
            copyIsArray = false;
            clone = src && isArray(src) ? src : [];

          } else {
            clone = src && isPlainObject(src) ? src : {};
          }

          // Never move original objects, clone them
          target[name] = extend(deep, clone, copy);

          // Don't bring in undefined values
        } else if (copy !== undefined) {
          target[name] = copy;
        }
      }
    }
  }

  // Return the modified object
  return target;
}

function isPlainObject(obj) {
  var key;

  // Must be an Object.
  // Because of IE, we also have to check the presence of the constructor property.
  // Make sure that DOM nodes and window objects don't pass through, as well
  if (!obj || type(obj) !== "object" || obj.nodeType || isWindow(obj)) {
    return false;
  }

  try {
    // Not own constructor property must be Object
    if (obj.constructor &&
      !hasOwn.call(obj, "constructor") &&
      !hasOwn.call(obj.constructor.prototype, "isPrototypeOf")) {
      return false;
    }
  } catch (e) {
    // IE8,9 Will throw exceptions on certain host objects #9897
    return false;
  }

  // Support: IE<9
  // Handle iteration over inherited properties before own properties.
  if (support.ownLast) {
    for (key in obj) {
      return hasOwn.call(obj, key);
    }
  }

  // Own properties are enumerated firstly, so to speed up,
  // if last one is own, then all properties are own.
  for (key in obj) { }

  return key === undefined || hasOwn.call(obj, key);
}

function isFunction(obj) {
  return type(obj) === "function";
}

function isArray(obj) {
  return type(obj) === "array";
}

function isWindow(obj) {
  /* jshint eqeqeq: false */
  return obj != null && obj == obj.window;
}

function type(obj) {
  if (obj == null) {
    return obj + "";
  }
  return typeof obj === "object" || typeof obj === "function" ?
    class2type[toString.call(obj)] || "object" :
    typeof obj;
}

function isNullOrEmpty(obj) {
  if (typeof obj == "undefined")
    return true


  if (obj == null)
    return true

  if (obj.toString().length == 0)
    return true

  return false
}

const postJson = (opts) => {
  opts = extend({
    isShowLoading: true,
    url: '',
    method: 'POST',
    urlParams: null,
    dataParams: null,
    async: true,
    timeout: 0
  }, opts)


  return myWxRequest(opts)
}

const getJson = (opts) => {

  opts = extend({
    isShowLoading: true,
    url: '',
    method: 'GET',
    urlParams: null,
    async: true,
    timeout: 0
  }, opts)

  return myWxRequest(opts)

}

const myWxRequest = (opts) => {

  var promise = new Promise((resolve, reject) => {

    opts = extend({
      isShowLoading: false,
      url: '',
      method: 'GET',
      urlParams: null,
      dataParams: null,
      async: true,
      timeout: 0
    }, opts)

    var _url = opts.url
    var _urlParams = opts.urlParams
    var _dataParams = opts.dataParams
    var _method = opts.method
    var _isShowLoading = opts.isShowLoading

    if (_isShowLoading) {
      wx.showLoading({
        title: ''
      })
    }

    console.log("wxRequest.url->>>" + _url)
    console.log("wxRequest.urlParams->>>" + JSON.stringify(opts.urlParams))

    if (isNullOrEmpty(_url)) {
      return
    }

    if (_url.indexOf("?") < 0) {
      _url += "?"
    } else {
      _url += "&"
    }

    _url += "appId=" + config.appId + "&merchId=" + config.merchId + "&token=" + storeage.getAccessToken()

    if (!isNullOrEmpty(_urlParams)) {
      _url += "&"
      for (var p in _urlParams) {
        _url += p + '=' + encodeURIComponent(_urlParams[p]).toUpperCase() + '&';
      }

      _url = _url.substring(0, _url.length - 1)
    }

    console.log("wxRequest.url and urlParams ->>>" + _url)
    console.log("wxRequest.method->>>" + _method)

    if (_method == "POST") {
      console.log("wxRequest.dataParams->>>" + JSON.stringify(_dataParams))
    }

    wx.request({
      url: _url,
      data: _dataParams,
      method: _method,
      dataType: "json",
      success: function (res) {
        console.log("wxRequest.success->>>" + JSON.stringify(res));
        if (res.statusCode == 200) {
          resolve(res.data);
        } else {//返回错误提示信息
          reject(res.errMsg);
        }
        if (_isShowLoading) {
          wx.hideLoading()
        }
      },
      error: function (e) {
        if (_isShowLoading) {
          wx.hideLoading()
        }
        reject('网络出错');
      }
    })
  })

  return promise
}

// function wxRequest(opts) {
//   opts = extend({
//     isShowLoading: false,
//     url: '',
//     method: 'GET',
//     urlParams: null,
//     dataParams: null,
//     async: true,
//     timeout: 0,
//     beforeSend: function (res) {

//     },
//     complete: function (res, status) {

//     },
//     suceess: function () {

//     }
//   }, opts)

//   var _url = opts.url
//   var _urlParams = opts.urlParams
//   var _dataParams = opts.dataParams
//   var _method = opts.method
//   var _isShowLoading = opts.isShowLoading

//   if (_isShowLoading) {
//     wx.showLoading({
//       title: ''
//     })
//   }

//   console.log("wxRequest.url->>>" + _url)
//   console.log("wxRequest.urlParams->>>" + JSON.stringify(opts.urlParams))

//   if (isNullOrEmpty(_url)) {
//     return
//   }

//   if (_url.indexOf("?") < 0) {
//     _url += "?"
//   } else {
//     _url += "&"
//   }

//   _url += "appId=" + config.appId + "&merchId=" + config.merchId + "&token=" + storeage.getAccessToken()

//   if (!isNullOrEmpty(_urlParams)) {
//     _url += "&"
//     for (var p in _urlParams) {
//       _url += p + '=' + encodeURIComponent(_urlParams[p]).toUpperCase() + '&';
//     }

//     _url = _url.substring(0, _url.length - 1)
//   }

//   console.log("wxRequest.url and urlParams ->>>" + _url)
//   console.log("wxRequest.method->>>" + _method)

//   if (_method == "POST") {
//     console.log("wxRequest.dataParams->>>" + JSON.stringify(_dataParams))
//   }

//   wx.request({
//     url: _url,
//     data: _dataParams,
//     method: _method,
//     dataType: "json",
//     success: function (res) {
//       if (res.statusCode != 200) {
//         console.log("wxRequest.success->>>" + JSON.stringify(res));
//       }

//       if (typeof res.data == "undefined" || res.data == null) {
//         console.log("wxRequest.success->>>ata is undefined or null");
//       } else if (typeof res.data.result == "undefined" || res.data.result == null) {
//         console.log("wxRequest.success->>>data.result is undefined or null");
//       } else {
//         console.log("wxRequest.success->>>" + JSON.stringify(res.data));
//         if (res.data.result == 3) {
//           console.log("wxRequest.success->>>data.result is exception");
//           wx.showModal({
//             showCancel: false,
//             title: '提示',
//             content: res.data.message
//           })
//         } else {
//           if (res.data.code == "2501") {
//             storeage.setAccessToken("")
//             if (_method == "POST") {
//               ownRequest.goLogin()
//             }
//           } else {
//             opts.success(res.data)
//           }
//         }
//       }
//     },
//     fail: function (res) {
//       console.log("fail:" + JSON.stringify(res))
//       wx.showModal({
//         showCancel: false,
//         title: '提示',
//         content: '网络请求错误'
//       })
//     },
//     complete: function () {

//       if (_isShowLoading) {
//         wx.hideLoading()
//       }

//       opts.complete()

//     }
//   })
// }

module.exports = {
  resultType: {
    unknown: 0,
    success: 1,
    failure: 2,
    exception: 3
  },
  postJson: postJson,
  getJson: getJson
}