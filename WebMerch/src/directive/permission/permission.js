import store from '@/store'

export default {
  inserted(el, binding, vnode) {
    const { value } = binding
    console.log('value:' + value)
    // console.log(JSON.stringify(store.getters.userInfo.menus))
    const menus = store.getters.userInfo.menus

    if (menus && menus instanceof Array && menus.length > 0) {
      const permissionRoles = value

      const hasPermission = menus.some(menu => {
        console.log('menus:' + menu.name)
        return false
      })

      if (!hasPermission) {
        el.parentNode && el.parentNode.removeChild(el)
      }
    } else {
      throw new Error(`need roles! Like v-permission="['admin','editor']"`)
    }
  }
}
