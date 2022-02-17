import request from '@/utils/request'

export function initBind(params) {
  return request({
    url: '/device/initBind',
    method: 'get',
    params
  })
}

export function initManage(params) {
  return request({
    url: '/device/initManage',
    method: 'get',
    params
  })
}

export function initInfo(params) {
  return request({
    url: '/device/initInfo',
    method: 'get',
    params
  })
}

export function bindSerialNo(data) {
  return request({
    url: '/device/bindSerialNo',
    method: 'post',
    data
  })
}

export function bindPhoneNumber(data) {
  return request({
    url: '/device/bindPhoneNumber',
    method: 'post',
    data
  })
}

export function unBind(data) {
  return request({
    url: '/device/unBind',
    method: 'post',
    data
  })
}

export function getPhoneValidCode(data) {
  return request({
    url: '/device/getPhoneValidCode',
    method: 'post',
    data
  })
}

export function infoEdit(data) {
  return request({
    url: '/device/infoEdit',
    method: 'post',
    data
  })
}

export function initFill(params) {
  return request({
    url: '/device/initFill',
    method: 'get',
    params
  })
}

export function fill(data) {
  return request({
    url: '/device/fill',
    method: 'post',
    data
  })
}

export default {
  initBind: initBind,
  initManage: initManage,
  initInfo: initInfo,
  bindSerialNo: bindSerialNo,
  bindPhoneNumber: bindPhoneNumber,
  unBind: unBind,
  getPhoneValidCode: getPhoneValidCode,
  infoEdit: infoEdit,
  initFill: initFill,
  fill: fill
}

