<template>
  <div id="device_manage">
    <page-header />
    <span>test</span>
  </div>
</template>
<script>

import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'DeviceVendingTest',
  components: { PageHeader },
  data() {
    return {
      loading: false,
      activeTab: 'tabBaseInfo',
      activeDropdown: {
        id: '',
        name: ''
      },
      dropdownOptions: []
    }
  },
  created() {

  },
  methods: {

  }
}
</script>
<style lang="scss" scoped>

</style>
