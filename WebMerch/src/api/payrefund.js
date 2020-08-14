import request from '@/utils/request'

export function searchOrder(params) {
  return request({
    url: '/payrefund/searchOrder',
    method: 'get',
    params
  })
}

export function getOrderDetails(params) {
  return request({
    url: '/payrefund/GetOrderDetails',
    method: 'get',
    params
  })
}

export default {
  searchOrder: searchOrder,
  getOrderDetails: getOrderDetails
}

