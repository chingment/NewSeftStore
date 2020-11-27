import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/coupon/getlist',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/coupon/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/coupon/add',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  add: add,
  initAdd: initAdd
}
