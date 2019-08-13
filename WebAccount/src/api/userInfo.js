import request from '@/utils/request'

export function save(data) {
  return request({
    url: '/userInfo/save',
    method: 'post',
    data
  })
}
