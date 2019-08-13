import { loginByAccount, logout, getInfo, checkPermission } from '@/api/own'
import { getToken, setToken, removeToken } from '@/utils/auth'
import { resetRouter } from '@/router'
import { generateRoutes } from '@/utils/ownResource'

const state = {
  token: getToken(),
  userInfo: null
}

const mutations = {
  SET_TOKEN: (state, token) => {
    state.token = token
  },
  SET_USERINFO: (state, userInfo) => {
    state.userInfo = userInfo
  }
}

const actions = {
  setToken({ commit }, token) {
    commit('SET_TOKEN', token)
  },
  // user login
  loginByAccount({ commit }, userInfo) {
    const { username, password, loginWay } = userInfo
    return new Promise((resolve, reject) => {
      loginByAccount({ username: username.trim(), password: password, loginWay: loginWay }).then(response => {
        const { data } = response
        commit('SET_TOKEN', data.token)
        setToken(data.token)
        resolve()
      }).catch(error => {
        reject(error)
      })
    })
  },

  // get user info
  getInfo({ commit, state }, path) {
    return new Promise((resolve, reject) => {
      getInfo(state.token, 'account', path).then(res => {
        if (res.result === 1) {
          const d = res.data
          commit('SET_USERINFO', d)
          generateRoutes(d.menus)
        }
        resolve(res)
      }).catch(error => {
        reject(error)
      })
    })
  },

  // user logout
  logout({ commit, state }) {
    return new Promise((resolve, reject) => {
      logout(state.token).then(() => {
        commit('SET_TOKEN', '')
        removeToken()
        resetRouter()
        resolve()
      }).catch(error => {
        reject(error)
      })
    })
  },

  // remove token
  resetToken({ commit }) {
    return new Promise(resolve => {
      commit('SET_TOKEN', '')
      removeToken()
      resolve()
    })
  },

  // checkperminssion
  checkPermission({ commit }, type, content) {
    return new Promise((resolve, reject) => {
      checkPermission(type, content).then(response => {
        resolve(response)
      }).catch(error => {
        reject(error)
      })
    })
  }

}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}

