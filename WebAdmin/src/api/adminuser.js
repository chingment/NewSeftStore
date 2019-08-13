import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/adminuser/getlist',
    method: 'get',
    params
  })
}

export function initAddUser(params) {
  return request({
    url: '/adminuser/initAdd',
    method: 'get',
    params
  })
}

export function addUser(data) {
  return request({
    url: '/adminuser/add',
    method: 'post',
    data
  })
}

export function initEditUser(params) {
  return request({
    url: '/adminuser/initEdit',
    method: 'get',
    params
  })
}

export function editUser(data) {
  return request({
    url: '/adminuser/edit',
    method: 'post',
    data
  })
}
