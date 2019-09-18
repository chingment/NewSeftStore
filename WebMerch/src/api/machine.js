import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/machine/getlist',
    method: 'get',
    params
  })
}

export default {
  getList: getList
}
