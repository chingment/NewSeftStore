import request from '@/utils/request'

export function initRpFollow(params) {
  return request({
    url: '/invite/initRpFollow',
    method: 'get',
    params
  })
}

export function agreeRpFollow(data) {
  return request({
    url: '/invite/agreeRpFollow',
    method: 'post',
    data
  })
}

export default {
  initRpFollow: initRpFollow,
  agreeRpFollow: agreeRpFollow
}
