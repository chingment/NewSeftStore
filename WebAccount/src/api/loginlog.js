import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/loginlog/getlist',
    method: 'get',
    params
  })
}
