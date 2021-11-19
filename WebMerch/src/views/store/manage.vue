<template>
  <div id="store_manage">
    <page-header />
    <div class="cur-store cur-tab">
      <div class="it-name">
        <span class="title">当前店铺:</span><span class="name">{{ activeDropdown.name }}</span>
      </div>
      <el-dropdown class="it-switch" trigger="click" @command="onChangeDropdown">
        <span class="el-dropdown-link">
          切换<i class="el-icon-arrow-down el-icon--right" />
        </span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item v-for="option in dropdownOptions" :key="option.id" :command="option.id"> {{ option.name }}</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
    <el-tabs v-model="activeTab" v-loading="loading" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <pane-base-info :store-id="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane label="商品分类" name="tabKind"><pane-kind :store-id="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane label="关联门店" name="tabShop"><pane-shop :store-id="activeDropdown.id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { initManage } from '@/api/store'
import PaneBaseInfo from './components/PaneBaseInfo'
import PaneShop from './components/PaneShop'
import PaneKind from './components/PaneKind'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'StoreManage',
  components: { PaneBaseInfo, PaneKind, PaneShop, PageHeader },
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
    onChangeDropdown(id) {
      this.activeDropdown.id = id
      this.init()
    }
  }
}
</script>
<style lang="scss" scoped>

</style>
