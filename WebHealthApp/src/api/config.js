import request from '@/utils/request'

export function jsSdk(data) {
  return request({
    url: '/config/jsSdk',
    method: 'post',
    data
  })
}

export default {
  jsSdk: jsSdk
}
