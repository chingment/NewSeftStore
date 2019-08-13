import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/merchmenu/getlist',
    method: 'get',
    params
  })
}

export function initAddMenu(params) {
  return request({
    url: '/merchmenu/initAdd',
    method: 'get',
    params
  })
}

export function addMenu(data) {
  return request({
    url: '/merchmenu/add',
    method: 'post',
    data
  })
}

export function initEditMenu(params) {
  return request({
    url: '/merchmenu/initEdit',
    method: 'get',
    params
  })
}

export function editMenu(data) {
  return request({
    url: '/merchmenu/edit',
    method: 'post',
    data
  })
}
