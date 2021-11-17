import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/org/getList',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/org/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/org/add',
    method: 'post',
    data
  })
}

export function initEdit(params) {
  return request({
    url: '/org/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/org/edit',
    method: 'post',
    data
  })
}

export function sort(data) {
  return request({
    url: '/org/sort',
    method: 'post',
    data
  })
}

