<template>
  <div>
    <pane-month-report cache-query />
  </div>
</template>

<script>

import PaneMonthReport from './components/PaneMonthReport.vue'
export default {
  name: 'SenvivMonthReport',
  components: { PaneMonthReport },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 20,
        name: undefined,
        sas: '0',
        chronic: '0',
        perplexs: null
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  },
  methods: {

  }
}
</script>

