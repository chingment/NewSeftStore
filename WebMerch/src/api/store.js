import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/store/getlist',
    method: 'get',
    params
  })
}

export function initAddStore(params) {
  return request({
    url: '/store/initAdd',
    method: 'get',
    params
  })
}

export function addStore(data) {
  return request({
    url: '/store/add',
    method: 'post',
    data
  })
}

export function initStore(params) {
  return request({
    url: '/store/initEdit',
    method: 'get',
    params
  })
}

export function editStore(data) {
  return request({
    url: '/store/edit',
    method: 'post',
    data
  })
}

export function initGetProductSkuList(params) {
  return request({
    url: '/store/initGetProductSkuList',
    method: 'get',
    params
  })
}
