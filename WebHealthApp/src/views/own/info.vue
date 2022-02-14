<template>
  <div id="own_info">
    <div class="info-card">
      <div class="avatar" />
      <div class="info" />
      <div class="qrcode" />
    </div>

    <div class="info-nav" />
  </div>

</template>

<script>
import { info } from '@/api/own'
export default {
  name: 'OwnInfo',
  data() {
    return {
      loading: false
    }
  },
  created() {
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      info({ }).then(res => {
        if (res.result === 1) {

        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scope>

</style>
