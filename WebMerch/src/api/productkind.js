import request from '@/utils/request'

export function getProductKindList(params) {
  return request({
    url: '/productkind/getlist',
    method: 'get',
    params
  })
}

export function initAddProductKind(params) {
  return request({
    url: '/productkind/initAdd',
    method: 'get',
    params
  })
}

export function addProductKind(data) {
  return request({
    url: '/productkind/add',
    method: 'post',
    data
  })
}

export function initEditProductKind(params) {
  return request({
    url: '/productkind/initEdit',
    method: 'get',
    params
  })
}

export function editProductKind(data) {
  return request({
    url: '/productkind/edit',
    method: 'post',
    data
  })
}

export function sortProductKind(data) {
  return request({
    url: '/productkind/sort',
    method: 'post',
    data
  })
}
