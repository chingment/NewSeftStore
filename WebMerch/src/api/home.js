import request from '@/utils/request'

export function getInitData(params) {
  return request({
    url: '/home/getInitData',
    method: 'get',
    params
  })
}

export default {
  getInitData: getInitData
}
