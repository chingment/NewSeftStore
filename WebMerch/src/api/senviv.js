import request from '@/utils/request'

export function getUsers(params) {
  return request({
    url: '/senviv/getUsers',
    method: 'get',
    params
  })
}

export function getUserDetail(params) {
  return request({
    url: '/senviv/getUserDetail',
    method: 'get',
    params
  })
}

export function getDayReports(params) {
  return request({
    url: '/senviv/getDayReports',
    method: 'get',
    params
  })
}

export function getDayReportDetail(params) {
  return request({
    url: '/senviv/getDayReportDetail',
    method: 'get',
    params
  })
}

export function getMonthReports(params) {
  return request({
    url: '/senviv/getMonthReports',
    method: 'get',
    params
  })
}

export default {
  getUsers: getUsers,
  getUserDetail: getUserDetail,
  getDayReports: getDayReports,
  getDayReportDetail: getDayReportDetail,
  getMonthReports: getMonthReports
}
