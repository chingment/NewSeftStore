<template>
  <div id="home_container" class="app-container">
    <pane-shop-work-bench v-if="workBench===1" />
    <pane-senviv-work-bench v-else-if="workBench===2" />
  </div>
</template>
<script>

import permission from '@/directive/permission/index.js' // 权限判断指令
import PaneShopWorkBench from '@/views/home/components/PaneShopWorkBench.vue'
import PaneSenvivWorkBench from '@/views/home/components/PaneSenvivWorkBench.vue'
import { getInitData } from '@/api/home'
export default {
  name: 'HomeIndex',
  components: {
    PaneShopWorkBench,
    PaneSenvivWorkBench
  },
  directives: { permission },
  data() {
    return {
      workBench: 0
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      getInitData().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.workBench = d.workBench
        }
      })
    }
  }
}

</script>
