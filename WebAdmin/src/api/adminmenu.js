import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/adminmenu/getlist',
    method: 'get',
    params
  })
}

export function initAddMenu(params) {
  return request({
    url: '/adminmenu/initAdd',
    method: 'get',
    params
  })
}

export function addMenu(data) {
  return request({
    url: '/adminmenu/add',
    method: 'post',
    data
  })
}

export function initEditMenu(params) {
  return request({
    url: '/adminmenu/initEdit',
    method: 'get',
    params
  })
}

export function editMenu(data) {
  return request({
    url: '/adminmenu/edit',
    method: 'post',
    data
  })
}
