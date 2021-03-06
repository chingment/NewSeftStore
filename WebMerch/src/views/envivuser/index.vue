<template>
  <div class="app-container">
    <router-view />
  </div>
</template>
<script>
export default {
  name: 'EnvivUserIndex',
  data() {
    return {
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  },
  methods: {

  }
}
</script>
