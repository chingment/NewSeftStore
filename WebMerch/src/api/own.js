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

export default {
  getInfo: getInfo,
  checkPermission: checkPermission
}
