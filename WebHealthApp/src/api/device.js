import request from '@/utils/request'

export function initBind(params) {
  return request({
    url: '/device/initBind',
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

export default {
  initBind: initBind,
  bindSerialNo: bindSerialNo,
  bindPhoneNumber: bindPhoneNumber
}

