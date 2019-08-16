import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/store/getlist',
    method: 'get',
    params
  })
}
