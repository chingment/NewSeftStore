import request from '@/utils/request'

export function getUserList(params) {
  return request({
    url: '/user/getlist',
    method: 'get',
    params
  })
}

export function initAddUser(params) {
  return request({
    url: '/user/initAdd',
    method: 'get',
    params
  })
}

export function addUser(data) {
  return request({
    url: '/user/add',
    method: 'post',
    data
  })
}

export function initEditUser(params) {
  return request({
    url: '/user/initEdit',
    method: 'get',
    params
  })
}

export function editUser(data) {
  return request({
    url: '/user/edit',
    method: 'post',
    data
  })
}
