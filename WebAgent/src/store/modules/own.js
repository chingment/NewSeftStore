import { getInfo, checkPermission } from '@/api/own'
import { getToken, setToken, removeToken } from '@/utils/auth'
import router from '@/router'
import { generateRoutes } from '@/utils/ownResource'

const state = {
  token: getToken(),
  userInfo: null,
  navBar: []
}

const mutations = {
  SET_TOKEN: (state, token) => {
    state.token = token
  },
  SET_USERINFO: (state, userInfo) => {
    state.userInfo = userInfo
  },
  SET_NAVBAR: (state, navBar) => {
    state.navBar = navBar
  }
}

const actions = {
  setToken({ commit }, token) {
    commit('SET_TOKEN', token)
    setToken(token)
  },

  // get user info
  getInfo({ commit, state }, path) {
    return new Promise((resolve, reject) => {
      getInfo(state.token, 'agent', path).then(res => {
        console.log(JSON.stringify(res))
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
      commit('SET_TOKEN', '')
      removeToken()
      router.resetRouter()
      resolve()
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

