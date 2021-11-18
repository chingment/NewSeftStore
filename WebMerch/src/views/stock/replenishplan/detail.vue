<template>
  <div>
    <device-replenish-plan :plan-id="planId" /></div>
</template>

<script>

import DeviceReplenishPlan from '@/views/report/devicereplenishplan.vue'
export default {
  name: 'StockRelenishPlanDetail',
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

