import request from '@/utils/request'

export function authUrl(data) {
  return request({
    url: '/own/authUrl',
    method: 'post',
    data
  })
}

export function authInfo(data) {
  return request({
    url: '/own/authInfo',
    method: 'post',
    data
  })
}

export default {
  authInfo: authInfo,
  authUrl: authUrl
}
