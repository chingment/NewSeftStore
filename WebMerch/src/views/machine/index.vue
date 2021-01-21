<template>
  <div  class="app-container my-container1">
      <router-view  />
  </div>
</template>
<script>
export default {
  name: 'MachineIndex',
  data() {
    return {
      isDesktop: this.$store.getters.isDesktop,
    }
  },
  created() {

  },
  methods: {

  }
}
</script>