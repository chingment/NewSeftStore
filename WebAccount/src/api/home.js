import request from '@/utils/request'

export function getIndexPageData(params) {
  return request({
    url: '/home/getIndexPageData',
    method: 'get',
    params
  })
}
