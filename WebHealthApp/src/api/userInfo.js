import request from '@/utils/request'

export function changePassword(data) {
  return request({
    url: '/userInfo/changePassword',
    method: 'post',
    data
  })
}

export default {
  changePassword: changePassword
}
