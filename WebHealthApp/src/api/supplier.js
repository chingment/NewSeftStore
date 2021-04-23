import request from '@/utils/request'

export function search(params) {
  return request({
    url: '/supplier/search',
    method: 'get',
    params
  })
}

export default {
  search: search
}
