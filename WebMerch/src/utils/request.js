import axios from 'axios'
import { MessageBox, Message } from 'element-ui'
import store from '@/store'
import { getToken } from '@/utils/auth'

axios.defaults.retry = 4
axios.defaults.retryDelay = 1000

// create an axios instance
const service = axios.create({
  baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
  // withCredentials: true, // send cookies when cross-domain requests
  timeout: 10000 // request timeout
})

// request interceptor
service.interceptors.request.use(
  config => {
    // do something before request is sent
    // console.log('responseType' + config.responseType)
    // console.log(config.url)
    if (store.getters.token) {
      // let each request carry token
      // ['X-Token'] is a custom headers key
      // please modify it according to the actual situation
      config.headers['X-Token'] = getToken()
    }
    return config
  },
  error => {
    // do something with request error
    console.log(error) // for debug
    return Promise.reject(error)
  }
)

// response interceptor
service.interceptors.response.use(
  /**
   * If you want to get http information such as headers or status
   * Please return  response => response
  */

  /**
   * Determine the request status by custom code
   * Here is just an example
   * You can also judge the status by HTTP Status Code
   */
  response => {
    var contentType = response.headers['content-type']

    const res = response.data
    if (contentType.indexOf('application/json') > -1) {
    // console.log(JSON.stringify(res))
    // if the custom code is not 20000, it is judged as an error.
      if (res.result === 1 || res.result === 2) {
        if (res.result === 2) {
        // Message({
        //   message: res.message || 'Error',
        //   type: 'error',
        //   duration: 5 * 1000
        // })

          if (res.code === 5000) {
          // to re-login
            MessageBox.confirm('您链接请求已经超时', '确定退出？', {
              confirmButtonText: '重新登录',
              cancelButtonText: '取消',
              type: 'warning'
            }).then(() => {
              store.dispatch('own/resetToken').then(() => {
                var path = encodeURIComponent(window.location.href)
                window.location.href = `${process.env.VUE_APP_LOGIN_URL}?appId=${process.env.VUE_APP_ID}&logout=2&redirect=${path}`
              })
            }).catch(() => {
            })
          }
        }

        return res
      } else {
        Message({
          message: res.message || 'Error',
          type: 'error',
          duration: 5 * 1000
        })

        return Promise.reject(new Error(res.message || 'Error'))
      }
    } else {
      return response
    }
  },
  error => {
    // console.log('err' + error) // for debug
    // Message({
    //   message: error.message,
    //   type: 'error',
    //   duration: 5 * 1000
    // })
    // return Promise.reject(error)

    var config = error.config

    // 如果config不存在或未设置重试选项，请拒绝
    if (!config || !config.retry) return Promise.reject('Error')
    // 设置变量跟踪重试次数
    config.__retryCount = config.__retryCount || 0
    // 检查是否已经达到最大重试总次数
    if (config.__retryCount >= config.retry) {
      // 抛出错误信息
      return Promise.reject(error)
    }
    // 增加请求重试次数
    config.__retryCount += 1
    // 创建新的异步请求
    var backoff = new Promise(function(resolve) {
      setTimeout(function() {
        resolve()
      }, config.retryDelay || 1)
    })
    // 返回axios信息，重新请求
    return backoff.then(function() {
      return axios(config)
    })
  }
)

export default service
