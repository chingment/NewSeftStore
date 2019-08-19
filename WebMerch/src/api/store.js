import request from '@/utils/request'

export function getStoreList(params) {
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

export function initManageProductSkus(params) {
  return request({
    url: '/store/initManageProductSkus',
    method: 'get',
    params
  })
}

export function getStoreProductSkuList(params) {
  return request({
    url: '/store/GetProductSkuList',
    method: 'get',
    params
  })
}
