import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/merchmaster/getlist',
    method: 'get',
    params
  })
}

export function initAddUser(params) {
  return request({
    url: '/merchmaster/initAdd',
    method: 'get',
    params
  })
}

export function addUser(data) {
  return request({
    url: '/merchmaster/add',
    method: 'post',
    data
  })
}

export function initEditUser(params) {
  return request({
    url: '/merchmaster/initEdit',
    method: 'get',
    params
  })
}

export function editUser(data) {
  return request({
    url: '/merchmaster/edit',
    method: 'post',
    data
  })
}
