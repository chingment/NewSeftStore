<template>
  <div id="store_manage" class="app-container">
    <div class="cur-store">
      <span class="title">当前店铺:</span><span class="name">{{ curStore.name }}</span>

      <el-dropdown trigger="click" @command="handleChangeStore">
        <span class="el-dropdown-link">
          切换<i class="el-icon-arrow-down el-icon--right" />
        </span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item v-for="store in stores" :key="store.id" :command="store.id"> {{ store.name }}</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
    <el-tabs v-model="activeName" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <manage-pane-base-info :storeid="id" /></el-tab-pane>
      <el-tab-pane label="机器信息" name="tabMachine"><manage-pane-machine :storeid="id" /></el-tab-pane>
      <el-tab-pane label="订单信息" name="tabOrder"><manage-pane-order :storeid="id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { initManage } from '@/api/store'
import { getUrlParam } from '@/utils/commonUtil'
import managePaneBaseInfo from './components/ManagePaneBaseInfo'
import managePaneMachine from './components/ManagePaneMachine'
import managePaneOrder from '@/views/order/list'
export default {
  components: { managePaneBaseInfo, managePaneMachine, managePaneOrder },
  data() {
    return {
      activeName: 'tabBaseInfo',
      curStore: {
        id: '',
        name: ''
      },
      stores: []
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
      this.loading = true
      initManage({ id: this.id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.curStore = d.curStore
          this.stores = d.stores
        }
        this.loading = false
      })
    },
    handleChangeStore(command) {
      this.$router.push({
        path: '/store/manage?id=' + command
      })
    },
    handleClick(tab, event) {
      console.log(tab, event)
    }
  }
}
</script>
<style lang="scss" scoped>

#store_manage{
  padding-top: 0px;
  .cur-store{
  font-size: 14px;
  line-height: 60px;
   .title{
    color: #5e6d82;
   }
   .name{
    padding: 0 10px;
   }
  }
}
</style>
