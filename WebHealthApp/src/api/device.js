import request from '@/utils/request'

export function initBind(params) {
  return request({
    url: '/device/initBind',
    method: 'get',
    params
  })
}

export default {
  initBind: initBind
}

