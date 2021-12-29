const getters = {
  device: state => state.app.device,
  isMobile: state => state.app.isMobile,
  isDesktop: state => state.app.isDesktop,
  token: state => state.own.token,
  userInfo: state => state.own.userInfo,
  listPageQuery: state => state.app.listPageQuery
}
export default getters
