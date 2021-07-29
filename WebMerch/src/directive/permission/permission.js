import store from '@/store'

export default {
  inserted(el, binding, vnode) {
    const { value } = binding
    // console.log('value:' + value)
    const permissions = store.getters.permission

    // console.log(JSON.stringify(permissions))

    if (permissions && permissions instanceof Array && permissions.length > 0) {
      const permission = value

      const hasPermission = permissions.some(item => {
        return permission.includes(item)
      })

      if (!hasPermission) {
        el.parentNode && el.parentNode.removeChild(el)
      }
    } else {
      throw new Error(`need roles! Like v-permission="['admin','editor']"`)
    }
  }
}
