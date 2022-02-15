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

export function authTokenCheck(params) {
  return request({
    url: '/own/authTokenCheck',
    method: 'get',
    params
  })
}

export function info(params) {
  return request({
    url: '/own/info',
    method: 'get',
    params
  })
}

export function deviceInfo(params) {
  return request({
    url: '/own/deviceInfo',
    method: 'get',
    params
  })
}

export default {
  authInfo: authInfo,
  authUrl: authUrl,
  authTokenCheck: authTokenCheck,
  info: info,
  deviceInfo: deviceInfo
}
