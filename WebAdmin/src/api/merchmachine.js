import request from '@/utils/request'

export function initGetList(params) {
  return request({
    url: '/merchmachine/initGetlist',
    method: 'get',
    params
  })
}

export function getList(params) {
  return request({
    url: '/merchmachine/getlist',
    method: 'get',
    params
  })
}

export function bindOffMerch(data) {
  return request({
    url: '/merchmachine/bindOffMerch',
    method: 'post',
    data
  })
}

export function bindOnMerch(data) {
  return request({
    url: '/merchmachine/bindOnMerch',
    method: 'post',
    data
  })
}
