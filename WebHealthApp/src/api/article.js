import request from '@/utils/request'

export function details(params) {
  return request({
    url: '/article/details',
    method: 'get',
    params
  })
}

export default {
  details: details
}
