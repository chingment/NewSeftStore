<template>
  <div id="store_manage" class="app-container">
    <div class="cur-store">
      <span class="title">当前店铺:</span><span class="name">{{ activeDropdown.name }}</span>
      <el-dropdown trigger="click" @command="handleChangeDropdown">
        <span class="el-dropdown-link">
          切换<i class="el-icon-arrow-down el-icon--right" />
        </span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item v-for="option in dropdownOptions" :key="option.id" :command="option.id"> {{ option.name }}</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
    <el-tabs v-model="activeTab" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <manage-pane-base-info :storeid="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane label="商品分类" name="tabKind"><manage-pane-kind :storeid="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane label="门店信息" name="tabFront"><manage-pane-front :storeid="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane v-if="activeDropdown.sctMode.indexOf('K')>-1" label="机器信息" name="tabMachine"><manage-pane-machine :storeid="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane label="订单信息" name="tabOrder"><manage-pane-order ref="order" :storeid="activeDropdown.id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { initManage } from '@/api/store'
import { getUrlParam } from '@/utils/commonUtil'
import managePaneBaseInfo from './components/ManagePaneBaseInfo'
import managePaneFront from './components/ManagePaneFront'
import managePaneMachine from './components/ManagePaneMachine'
import managePaneKind from './components/ManagePaneKind'
import managePaneOrder from '@/views/order/list'
export default {
  components: { managePaneBaseInfo, managePaneKind, managePaneFront, managePaneMachine, managePaneOrder },
  data() {
    return {
      loading: false,
      activeTab: 'tabBaseInfo',
      activeDropdown: {
        id: '',
        name: '',
        sctMode: ''
      },
      dropdownOptions: []
    }
  },
  watch: {
    '$route'(to, from) {
      this.id = to.query.id
      this.$refs.order.listQuery.storeId = this.id
      this.init()
    }
  },
  created() {
    this.activeDropdown.id = this.$route.params.id
    this.activeTab =
      typeof this.$route.params.tab === 'undefined'
        ? 'tabBaseInfo'
        : this.$route.params.tab
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initManage({ id: this.activeDropdown.id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.activeDropdown = d.curStore
          this.dropdownOptions = d.stores
        }
        this.loading = false
      })
    },
    handleChangeDropdown(id) {
      this.activeDropdown.id = id
      this.init()
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
    color:#2ac06d;
   }
  }
}
</style>
