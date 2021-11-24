<template>
  <div id="home_container" class="app-container">
    <el-dropdown style="height:30px" @command="onChangeWorkBench">
      <span class="el-dropdown-link">
        切换工作台<i class="el-icon-arrow-down el-icon--right" />
      </span>
      <el-dropdown-menu slot="dropdown">
        <el-dropdown-item command="1">商城</el-dropdown-item>
        <el-dropdown-item command="2">心晓</el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
    <pane-shop-work-bench v-if="workBench===1" />
    <pane-senviv-work-bench v-else-if="workBench===2" />
  </div>
</template>
<script>

import permission from '@/directive/permission/index.js' // 权限判断指令
import PaneShopWorkBench from '@/views/home/components/PaneShopWorkBench.vue'
import PaneSenvivWorkBench from '@/views/home/components/PaneSenvivWorkBench.vue'
import { getInitData, saveWorkBench } from '@/api/home'
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
    },
    onChangeWorkBench(workBench) {
      saveWorkBench({ workBench: workBench }).then(res => {
        if (res.result === 1) {
          this.workBench = parseInt(workBench)
        }
      })
    }
  }
}

</script>

<style>
  .el-dropdown-link {
    cursor: pointer;
    color: #409EFF;
  }
  .el-icon-arrow-down {
    font-size: 12px;
  }
</style>
