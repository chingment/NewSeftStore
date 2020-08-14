import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/payrefund/getList',
    method: 'get',
    params
  })
}

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

export function apply(data) {
  return request({
    url: '/payrefund/apply',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  searchOrder: searchOrder,
  getOrderDetails: getOrderDetails,
  apply: apply
}

