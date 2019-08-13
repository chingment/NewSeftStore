import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/adminrole/getlist',
    method: 'get',
    params
  })
}

export function initAddRole(params) {
  return request({
    url: '/adminrole/initAdd',
    method: 'get',
    params
  })
}

export function addRole(data) {
  return request({
    url: '/adminrole/add',
    method: 'post',
    data
  })
}

export function initEditRole(params) {
  return request({
    url: '/adminrole/initEdit',
    method: 'get',
    params
  })
}

export function editRole(data) {
  return request({
    url: '/adminrole/edit',
    method: 'post',
    data
  })
}
