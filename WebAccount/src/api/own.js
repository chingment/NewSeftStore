import request from '@/utils/request'

export function loginByAccount(data) {
  return request({
    url: '/own/loginByAccount',
    method: 'post',
    data
  })
}

export function getInfo(token, website, path) {
  return request({
    url: '/own/getInfo',
    method: 'get',
    params: { token, website, path }
  })
}

export function logout(token, appId) {
  return request({
    url: '/own/logout',
    method: 'post',
    params: { token: token, appId: appId },
    data: { token: token, appId: appId }
  })
}

export function checkPermission(tpye, content) {
  return request({
    url: '/own/checkPermission',
    method: 'get',
    params: { tpye, content }
  })
}
