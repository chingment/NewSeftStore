import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/merch/getList',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/merch/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/merch/add',
    method: 'post',
    data
  })
}

export function initEdit(params) {
  return request({
    url: '/merch/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/merch/edit',
    method: 'post',
    data
  })
}
