import request from '@/utils/request'

export function getInitData(params) {
  return request({
    url: '/senvivworkbench/getInitData',
    method: 'get',
    params
  })
}

export default {
  getInitData: getInitData
}
