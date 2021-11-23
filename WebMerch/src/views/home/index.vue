<template>
  <div id="home_container" class="app-container">
    <div v-permission="['Merch_Home_Shop']">
      <pane-shop-console />
    </div>
    <div v-permission="['Merch_Home_Senviv']">
      <pane-senviv-console />
    </div>
  </div>
</template>
<script>

import permission from '@/directive/permission/index.js' // 权限判断指令
import PaneShopConsole from '@/views/home/components/PaneShopConsole.vue'
import PaneSenvivConsole from '@/views/home/components/PaneSenvivConsole.vue'

export default {
  name: 'HomeIndex',
  components: {
    PaneShopConsole,
    PaneSenvivConsole
  },
  directives: { permission },
  data() {
    return {
    }
  },
  created() {

  },
  methods: {

  }
}

</script>
