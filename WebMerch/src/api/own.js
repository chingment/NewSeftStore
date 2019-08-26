import request from '@/utils/request'

export function getInfo(token, website, path) {
  return request({
    url: '/own/getInfo',
    method: 'get',
    params: { token, website, path }
  })
}

export function checkPermission(tpye, content) {
  return request({
    url: '/own/checkPermission',
    method: 'get',
    params: { tpye, content }
  })
}

export default {
  getInfo: getInfo,
  checkPermission: checkPermission
}
