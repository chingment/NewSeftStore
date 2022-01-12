import request from '@/utils/request'

export function initBind(params) {
  return request({
    url: '/device/initBind',
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

export default {
  initBind: initBind,
  initInfo: initInfo,
  bindSerialNo: bindSerialNo,
  bindPhoneNumber: bindPhoneNumber,
  unBind: unBind
}

