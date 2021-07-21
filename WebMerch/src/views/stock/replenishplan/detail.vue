<template>
  <div>
    <device-replenish-plan :plan-id="planId" /></device-replenish-plan></div>
</template>

<script>

import DeviceReplenishPlan from '@/views/report/devicereplenishplan.vue'
export default {
  name: 'SenvivDayReport',
  components: { DeviceReplenishPlan },
  data() {
    return {
      planId: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this.planId = this.$route.params.id
    console.log('this.planId:' + this.planId)
  },
  methods: {

  }
}
</script>

