import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/agentrole/getlist',
    method: 'get',
    params
  })
}

export function initAddRole(params) {
  return request({
    url: '/agentrole/initAdd',
    method: 'get',
    params
  })
}

export function addRole(data) {
  return request({
    url: '/agentrole/add',
    method: 'post',
    data
  })
}

export function initEditRole(params) {
  return request({
    url: '/agentrole/initEdit',
    method: 'get',
    params
  })
}

export function editRole(data) {
  return request({
    url: '/agentrole/edit',
    method: 'post',
    data
  })
}
