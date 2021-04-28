<template>
  <div id="app">
    <loading v-show="isLoading" />
    <router-view />
  </div>
</template>

<script>
import { mapState } from 'vuex'
import Loading from '@/components/Loading'
export default {
  name: 'App',
  components: {
    Loading
  },
  computed: {
    ...mapState(['isLoading'])
  }
}
</script>
