<template>
  <div>
    无效页面
  </div>

</template>
<script>
export default {
  name: 'Index',
  components: {
  },
  data() {
    return {
      loading: false
    }
  },
  created() {
    var c = this.funcUrlDel('http://health.17fanju.com/own/info', ['code', 'state'])
    console.log('ccc' + c)
  },
  methods: {
    funcUrlDel(url, names) {
      if (typeof (names) === 'string') {
        names = [names]
      }
      //  http:/dsfsfsfs/?dfdsf=32321&code=33
      if (url.indexOf('?') > -1) {
        var href_cp = url.split('?')
        var loc_url = href_cp[0]
        var loc_qry = href_cp[1]
        var obj = {}
        var arr = loc_qry.split('&')
        // 获取参数转换为object
        for (var i = 0; i < arr.length; i++) {
          arr[i] = arr[i].split('=')
          obj[arr[i][0]] = arr[i][1]
        }
        // 删除指定参数
        for (var j = 0; j < names.length; j++) {
          delete obj[names[j]]
        }
        // 重新拼接url
        url = loc_url + '?' + JSON.stringify(obj).replace(/[\"\{\}]/g, '').replace(/\:/g, '=').replace(/\,/g, '&')
      }
      return url
    }
  }
}
</script>

<style lang="scss" scope>

</style>
