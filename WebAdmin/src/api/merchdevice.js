import request from '@/utils/request'

export function initGetList(params) {
  return request({
    url: '/merchdevice/initGetlist',
    method: 'get',
    params
  })
}

export function initEdit(params) {
  return request({
    url: '/merchdevice/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/merchdevice/edit',
    method: 'post',
    data
  })
}

export function getList(params) {
  return request({
    url: '/merchdevice/getlist',
    method: 'get',
    params
  })
}

export function bindOffMerch(data) {
  return request({
    url: '/merchdevice/bindOffMerch',
    method: 'post',
    data
  })
}

export function bindOnMerch(data) {
  return request({
    url: '/merchdevice/bindOnMerch',
    method: 'post',
    data
  })
}

export function copyBuild(data) {
  return request({
    url: '/merchdevice/copyBuild',
    method: 'post',
    data
  })
}
