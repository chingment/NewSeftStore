<template>
  <div style="background:#fff;height:100%">
    <router-view />
  </div>
</template>
<script>

import { updateVisitCount } from '@/api/monthreport'

export default {
  name: 'MonthIndex',
  data() {
    return {
    }
  },
  mounted() {
    if (window.performance.navigation.type === 1) {
      console.log('页面被刷新！')
    } else {
      updateVisitCount({ rptId: this.$route.query.rptId }).then(res => {})
    }
  }
}
</script>

<style lang="scss" scoped>

</style>
