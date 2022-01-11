import request from '@/utils/request'

export function initFill(params) {
  return request({
    url: '/quest/initFill',
    method: 'get',
    params
  })
}

export function fill(data) {
  return request({
    url: '/quest/fill',
    method: 'post',
    data
  })
}

export default {
  initFill: initFill
}

