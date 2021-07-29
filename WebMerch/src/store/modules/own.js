import { getInfo, checkPermission } from '@/api/own'
import { getToken, setToken, removeToken } from '@/utils/auth'
import router from '@/router'
import { generateRoutes } from '@/utils/ownResource'

const state = {
  token: getToken(),
  userInfo: null,
  permission: null,
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
  },
  SET_PERMISSION: (state, permission) => {
    state.permission = permission
  }
}

const actions = {
  setToken({ commit }, token) {
    commit('SET_TOKEN', token)
    setToken(token)
  },

  // get user info
  getInfo({ commit, state }) {
    return new Promise((resolve, reject) => {
      getInfo(state.token, 'merch').then(res => {
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
  checkPermission({ commit }, state) {
    return new Promise((resolve, reject) => {
      checkPermission('merch', state.type, state.content).then(res => {
        if (res.result === 1) {
          const d = res.data
          commit('SET_PERMISSION', d.permission)
        }
        resolve(res)
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

