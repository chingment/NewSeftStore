import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/agentmaster/getlist',
    method: 'get',
    params
  })
}

export function initAddUser(params) {
  return request({
    url: '/agentmaster/initAdd',
    method: 'get',
    params
  })
}

export function addUser(data) {
  return request({
    url: '/agentmaster/add',
    method: 'post',
    data
  })
}

export function initEditUser(params) {
  return request({
    url: '/agentmaster/initEdit',
    method: 'get',
    params
  })
}

export function editUser(data) {
  return request({
    url: '/agentmaster/edit',
    method: 'post',
    data
  })
}
