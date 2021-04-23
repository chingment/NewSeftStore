
export function getUrlParam(name) {
  return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.href) || [, ''])[1].replace(/\+/g, '%20')) || null
}

export function isEmpty(str) {
  if (typeof str === 'undefined') { return true }
  if (str === null) { return true }
  if (str.length === 0) { return true }

  str = str.replace(/(^\s*)|(\s*$)/g, '')

  if (str.length === 0) { return true }

  return false
}

export function changeURLArg(url, arg, arg_val) {
  var pattern = arg + '=([^&]*)'
  var replaceText = arg + '=' + arg_val
  if (url.match(pattern)) {
    var tmp = '/(' + arg + '=)([^&]*)/gi'
    tmp = url.replace(eval(tmp), replaceText)
    return tmp
  } else {
    if (url.match('[\?]')) {
      return url + '&' + replaceText
    } else {
      return url + '?' + replaceText
    }
  }
  return url + '\n' + arg + '\n' + arg_val
}

export function getCheckedKeys(tree) {
  var rad = ''
  var ridsa = tree.getCheckedKeys().join(',')// 获取当前的选中的数据[数组] -id, 把数组转换成字符串
  var ridsb = tree.getCheckedNodes()// 获取当前的选中的数据{对象}
  ridsb.forEach(ids => { // 获取选中的所有的父级id
    rad += ',' + ids.pId
  })
  rad = rad.substr(1) // 删除字符串前面的','
  var rids = rad + ',' + ridsa
  var arr = rids.split(',')//  把字符串转换成数组
  arr = [...new Set(arr)] // 数组去重

  return arr
}

export function goBack(_this) {
  if (window.history.length <= 1) {
    _this.$router.push({ path: '/' })
    return false
  } else {
    _this.$router.go(-1)
  }
}

export function treeselectNormalizer(node) {
  if (node.children == null || node.children.length === 0) {
    delete node.children
  }
  return {
    id: node.id,
    label: node.label,
    value: node.id,
    children: node.children
  }
}

export function treeGetNodesByDepth(tree, depth) {
  const nodes = []
  treeGetNodeByDepth(tree, depth, nodes)
  return nodes
}

function treeGetNodeByDepth(tree, depth, nodes) {
  for (let i = 0; i < tree.length; i++) {
    const item = tree[i]

    if (item.depth <= depth) {
      nodes.push(item)
    }
    if (item.children && item.children.length > 0) {
      treeGetNodeByDepth(item.children, depth, nodes)
    }
  }
}

export function strLen(str) {
  var len = 0

  if (typeof str === 'undefined') return 0

  if (str === null) return 0
  if (str != null) {
    str = str.replace(/(^\s*)|(\s*$)/g, '')
  }

  for (var i = 0; i < str.length; i++) {
    var c = str.charCodeAt(i)
    // 单字节加1
    if ((c >= 0x0001 && c <= 0x007e) || (c >= 0xff60 && c <= 0xff9f)) {
      len++
    } else {
      len += 2
    }
  }
  return len
}

export function isMoney(str) {
  if (str == null) return false
  if (typeof str === 'undefined') return false

  // str = str.replace(/(^\s*)|(\s*$)/g, '')

  var reg = /^(-?\d+)(\.\d{1,2})?$/
  if (reg.test(str)) {
    return true
  } else {
    return false
  }
}
