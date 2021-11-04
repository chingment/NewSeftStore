import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/paytrans/getList',
    method: 'get',
    params
  })
}

export function getSummary(params) {
  return request({
    url: '/paytrans/getSummary',
    method: 'get',
    params
  })
}

export default {
  getList: getList,
  getSummary: getSummary
}

