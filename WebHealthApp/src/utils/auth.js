import Cookies from 'js-cookie'

const TokenKey = 'vue_token'

export function getToken() {
  var token = Cookies.get(TokenKey)
  if (typeof token === 'undefined') { return null }
  return token
}

export function setToken(token) {
  return Cookies.set(TokenKey, token)
}

export function removeToken() {
  return Cookies.remove(TokenKey)
}
