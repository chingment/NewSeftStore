import request from '@/utils/request'

export function reLoadSpuCache(data) {
  return request({
    url: '/merchrepairfun/reLoadSpuCache',
    method: 'post',
    data
  })
}
