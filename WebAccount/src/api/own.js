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

export function logout(token) {
  return request({
    url: '/own/logout',
    method: 'post',
    params: { token }
  })
}

export function checkPermission(tpye, content) {
  return request({
    url: '/own/checkPermission',
    method: 'get',
    params: { tpye, content }
  })
}
