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

export function getMonthReportDetail(params) {
  return request({
    url: '/senviv/getMonthReportDetail',
    method: 'get',
    params
  })
}

export function saveMonthReportSug(data) {
  return request({
    url: '/senviv/saveMonthReportSug',
    method: 'post',
    data
  })
}

export function getMonthReportSug(params) {
  return request({
    url: '/senviv/getMonthReportSug',
    method: 'get',
    params
  })
}

export function getTagExplains(params) {
  return request({
    url: '/senviv/getTagExplains',
    method: 'get',
    params
  })
}

export function saveTagExplain(data) {
  return request({
    url: '/senviv/saveTagExplain',
    method: 'post',
    data
  })
}

export function saveVisitRecordByTelePhone(data) {
  return request({
    url: '/senviv/saveVisitRecordByTelePhone',
    method: 'post',
    data
  })
}

export function SaveVisitRecordByPapush(data) {
  return request({
    url: '/senviv/SaveVisitRecordByPapush',
    method: 'post',
    data
  })
}

export function getVisitRecords(params) {
  return request({
    url: '/senviv/getVisitRecords',
    method: 'get',
    params
  })
}

export default {
  getVisitRecords: getVisitRecords,
  getUsers: getUsers,
  getUserDetail: getUserDetail,
  getDayReports: getDayReports,
  getDayReportDetail: getDayReportDetail,
  getMonthReports: getMonthReports,
  getMonthReportDetail: getMonthReportDetail,
  saveMonthReportSug: saveMonthReportSug,
  getMonthReportSug: getMonthReportSug,
  getTagExplains: getTagExplains,
  saveTagExplain: saveTagExplain,
  saveVisitRecordByTelePhone: saveVisitRecordByTelePhone,
  SaveVisitRecordByPapush: SaveVisitRecordByPapush
}
