import request from '@/utils/request'

export function getInfo(token, website) {
  return request({
    url: '/own/getInfo',
    method: 'get',
    params: { token, website }
  })
}

export function checkPermission(website, tpye, content) {
  return request({
    url: '/own/checkPermission',
    method: 'get',
    params: { website, tpye, content }
  })
}

export function changePassword(data) {
  return request({
    url: '/own/changePassword',
    method: 'post',
    data
  })
}

export default {
  getInfo: getInfo,
  checkPermission: checkPermission,
  changePassword: changePassword
}
