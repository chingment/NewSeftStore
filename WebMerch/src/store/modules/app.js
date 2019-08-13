import Cookies from 'js-cookie'

const state = {
  sidebar: {
    opened: Cookies.get('sidebarStatus') ? !!+Cookies.get('sidebarStatus') : true,
    withoutAnimation: false
  },
  listPageQuery: new Map(),
  device: 'desktop',
  isMobile: false,
  isDesktop: true
}

const mutations = {
  TOGGLE_SIDEBAR: state => {
    state.sidebar.opened = !state.sidebar.opened
    state.sidebar.withoutAnimation = false
    if (state.sidebar.opened) {
      Cookies.set('sidebarStatus', 1)
    } else {
      Cookies.set('sidebarStatus', 0)
    }
  },
  CLOSE_SIDEBAR: (state, withoutAnimation) => {
    Cookies.set('sidebarStatus', 0)
    state.sidebar.opened = false
    state.sidebar.withoutAnimation = withoutAnimation
  },
  TOGGLE_DEVICE: (state, device) => {
    state.device = device
    if (device === 'mobile') {
      state.isMobile = true
      state.isDesktop = false
    } else {
      state.isMobile = false
      state.isDesktop = true
    }
  },
  SAVE_LIST_PAGE_QUERY: (state, { path, query }) => {
    state.listPageQuery.set(path, query)
  }
}

const actions = {
  toggleSideBar({ commit }) {
    commit('TOGGLE_SIDEBAR')
  },
  closeSideBar({ commit }, { withoutAnimation }) {
    commit('CLOSE_SIDEBAR', withoutAnimation)
  },
  toggleDevice({ commit }, device) {
    commit('TOGGLE_DEVICE', device)
  },
  saveListPageQuery({ commit }, { path, query }) {
    commit('SAVE_LIST_PAGE_QUERY', { path, query })
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
