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

export function egyContacts(params) {
  return request({
    url: '/own/egyContacts',
    method: 'get',
    params
  })
}

export function followers(params) {
  return request({
    url: '/own/followers',
    method: 'get',
    params
  })
}

export function idolers(params) {
  return request({
    url: '/own/idolers',
    method: 'get',
    params
  })
}

export function qrcodeUrlByRp(params) {
  return request({
    url: '/own/qrcodeUrlByRp',
    method: 'get',
    params
  })
}

export function removeFollower(data) {
  return request({
    url: '/own/removeFollower',
    method: 'post',
    data
  })
}

export function removeIdoler(data) {
  return request({
    url: '/own/removeIdoler',
    method: 'post',
    data
  })
}

export default {
  authInfo: authInfo,
  authUrl: authUrl,
  authTokenCheck: authTokenCheck,
  info: info,
  egyContacts: egyContacts,
  followers: followers,
  idolers: idolers,
  qrcodeUrlByRp: qrcodeUrlByRp,
  removeFollower: removeFollower,
  removeIdoler: removeIdoler
}
