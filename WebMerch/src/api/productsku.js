import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/productsku/getlist',
    method: 'get',
    params
  })
}

export function initAddProductSku(params) {
  return request({
    url: '/productsku/initAdd',
    method: 'get',
    params
  })
}

export function addProductSku(data) {
  return request({
    url: '/productsku/add',
    method: 'post',
    data
  })
}

export function initEditProductSku(params) {
  return request({
    url: '/productsku/initEdit',
    method: 'get',
    params
  })
}

export function editProductSku(data) {
  return request({
    url: '/productsku/edit',
    method: 'post',
    data
  })
}
