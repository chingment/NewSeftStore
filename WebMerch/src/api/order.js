import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/order/getlist',
    method: 'get',
    params
  })
}

export function getDetails(params) {
  return request({
    url: '/order/getDetails',
    method: 'get',
    params
  })
}

export function pickupExceptionHandle(data) {
  return request({
    url: '/order/pickupExceptionHandle',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  getDetails: getDetails
}
