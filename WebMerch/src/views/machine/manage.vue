<template>
  <div id="machine_manage" class="app-container">
    <div class="cur-machine">
      <span class="title">当前机器:</span><span class="name">{{ curMachine.name }}</span>

      <el-dropdown trigger="click" @command="handleChangeMachine">
        <span class="el-dropdown-link">
          切换<i class="el-icon-arrow-down el-icon--right" />
        </span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item v-for="machine in machines" :key="machine.id" :command="machine.id"> {{ machine.name }}</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
    <el-tabs v-model="activeName" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <manage-pane-base-info :machineid="id" /></el-tab-pane>
      <el-tab-pane label="库存信息" name="tabStock"><manage-pane-stock :machineid="id" /></el-tab-pane>
      <el-tab-pane label="订单信息" name="tabOrder"><manage-pane-order :machineid="id" /></el-tab-pane>
      <el-tab-pane label="控制中心" name="tabControlCenter"><manage-pane-control-center :machineid="id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { initManage } from '@/api/machine'
import { getUrlParam } from '@/utils/commonUtil'
import managePaneBaseInfo from './components/ManagePaneBaseInfo'
import managePaneStock from './components/ManagePaneStock'
import managePaneControlCenter from './components/ManagePaneControlCenter'
import managePaneOrder from '@/views/order/list'
export default {
  components: { managePaneBaseInfo, managePaneStock, managePaneOrder, managePaneControlCenter },
  data() {
    return {
      activeName: 'tabBaseInfo',
      curMachine: {
        id: '',
        name: ''
      },
      machines: []
    }
  },
  watch: {
    '$route'(to, from) {
      this.id = to.query.id
      this.init()
    }
  },
  created() {
    this.id = getUrlParam('id')
    this.init()
  },
  methods: {
    init() {
      this.activeName = getUrlParam('tab')
      this.loading = true
      initManage({ id: this.id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.curMachine = d.curMachine
          this.machines = d.machines
        }
        this.loading = false
      })
    },
    handleChangeMachine(command) {
      this.$router.push({
        path: '/machine/manage?id=' + command + '&tab=' + this.activeName
      })
    },
    handleClick(tab, event) {
      console.log(tab, event)
    }
  }
}
</script>
<style lang="scss" scoped>

#machine_manage{
  padding-top: 0px;
  .cur-machine{
  font-size: 14px;
  line-height: 60px;
   .title{
    color: #5e6d82;
   }
   .name{
    padding: 0 10px;
    color:#2ac06d;
   }
  }
}
</style>
