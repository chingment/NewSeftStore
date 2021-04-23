import request from '@/utils/request'

export function getStores(params) {
  return request({
    url: '/common/getStores',
    method: 'get',
    params
  })
}

export default {
  getStores: getStores
}
