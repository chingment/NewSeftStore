<template>
  <span>
    <span :style="{'color': value.color}">{{ value.value }}</span>
    <span v-if="value.sign!='-'" :style="{'color': value.color}">{{ value.sign }}</span>
  </span>

</template>

<script>

export default {
  name: 'ClientUserList',
  props: {
    value: {
      type: Object,
      default: null
    },
    sign: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  }
}
</script>

