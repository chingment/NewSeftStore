const getters = {
  sidebar: state => state.app.sidebar,
  device: state => state.app.device,
  token: state => state.own.token,
  userInfo: state => state.own.userInfo
}
export default getters
