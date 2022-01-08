<template>
  <div id="pg_device_bind">
    首页
  </div>

</template>
<script>
export default {
  name: 'Index',
  components: {
  },
  data() {
    return {
      loading: false,
      appConfig: {
        appName: '全线通'
      }
    }
  },
  created() {
    // var code = this.$route.query.code
    // var merchId = this.$route.query.merchId
    // var redirectUrl = this.$route.query.redirectUrl

    // console.log('code：' + code)
    // console.log('merchId：' + merchId)
    // console.log('redirectUrl：' + redirectUrl)

    // var href = (window.location.href)
    // console.log('href:' + href)
    // var appId = 'wxc6e80f8c575cf3f5'
    // var redriect_uri = encodeURIComponent('http://health.17fanju.com/index?redirectUrl=/device/bind')
    // console.log('redriect_uri:' + redriect_uri)
    // if (typeof code === 'undefined' || code == null) {
    //   console.log('AAAAAAAAAAAAAAA')
    //   window.location.href = 'https://open.weixin.qq.com/connect/oauth2/authorize?appid=' + appId + '&redirect_uri=' + redriect_uri + '&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect'
    // } else {
    //   console.log('BBBBBBBB')
    //   this.$router.push(redirectUrl)
    // }
  },
  methods: {

  }
}
</script>

<style lang="scss" scope>

</style>
