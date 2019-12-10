import request from '@/utils/request'

export function reLoadProductSkuCache(data) {
  return request({
    url: '/merchrepairfun/reLoadProductSkuCache',
    method: 'post',
    data
  })
}