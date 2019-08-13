import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/adminorg/getlist',
    method: 'get',
    params
  })
}

export function initAddOrg(params) {
  return request({
    url: '/adminorg/initAdd',
    method: 'get',
    params
  })
}

export function addOrg(data) {
  return request({
    url: '/adminorg/add',
    method: 'post',
    data
  })
}

export function initEditOrg(params) {
  return request({
    url: '/adminorg/initEdit',
    method: 'get',
    params
  })
}

export function editOrg(data) {
  return request({
    url: '/adminorg/edit',
    method: 'post',
    data
  })
}
