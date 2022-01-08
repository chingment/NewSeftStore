<template>
  <div>
    系统异常
  </div>

</template>
<script>
import { removeToken } from '@/utils/auth'
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
    removeToken()
  },
  methods: {

  }
}
</script>

<style lang="scss" scope>

</style>
