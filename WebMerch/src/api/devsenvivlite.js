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

export default {
  initGetList: initGetList,
  getList: getList
}
