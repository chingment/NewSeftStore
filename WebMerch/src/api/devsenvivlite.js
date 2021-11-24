import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/devsenvivlite/getlist',
    method: 'get',
    params
  })
}

export function initGetList() {
  return request({
    url: '/devsenvivlite/initGetList',
    method: 'get'
  })
}

export function bindMerch(data) {
  return request({
    url: '/devsenvivlite/bindMerch',
    method: 'post',
    data
  })
}

export function unBindMerch(data) {
  return request({
    url: '/devsenvivlite/unBindMerch',
    method: 'post',
    data
  })
}

export default {
  initGetList: initGetList,
  getList: getList,
  bindMerch: bindMerch,
  unBindMerch: unBindMerch
}
