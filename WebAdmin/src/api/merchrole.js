import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/merchrole/getlist',
    method: 'get',
    params
  })
}

export function initAddRole(params) {
  return request({
    url: '/merchrole/initAdd',
    method: 'get',
    params
  })
}

export function addRole(data) {
  return request({
    url: '/merchrole/add',
    method: 'post',
    data
  })
}

export function initEditRole(params) {
  return request({
    url: '/merchrole/initEdit',
    method: 'get',
    params
  })
}

export function editRole(data) {
  return request({
    url: '/merchrole/edit',
    method: 'post',
    data
  })
}
