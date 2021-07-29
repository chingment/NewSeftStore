const getters = {
  sidebar: state => state.app.sidebar,
  device: state => state.app.device,
  isMobile: state => state.app.isMobile,
  isDesktop: state => state.app.isDesktop,
  token: state => state.own.token,
  userInfo: state => state.own.userInfo,
  permission: state => state.own.permission,
  listPageQuery: state => state.app.listPageQuery
}
export default getters
