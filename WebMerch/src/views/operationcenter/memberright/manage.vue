<template>
  <div id="memberright_manage">
    <page-header />
    <div class="cur-device cur-tab">
      <div class="it-name">
        <span class="title">当前会员:</span><span class="name">{{ activeDropdown.name }}</span>
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
    <el-tabs v-model="activeTab" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo">
        <pane-base-info :levelst-id="activeDropdown.id" />
      </el-tab-pane>
      <el-tab-pane label="会费设置" name="tabFee">
        <pane-fee :levelst-id="activeDropdown.id" />
      </el-tab-pane>
      <el-tab-pane label="赠券设置" name="tabCoupon">
        <pane-coupon :levelst-id="activeDropdown.id" />
      </el-tab-pane>
      <el-tab-pane label="优惠商品" name="tabSku">
        <pane-sku :levelst-id="activeDropdown.id" />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import { initManage } from '@/api/memberright'
import PaneBaseInfo from './components/PaneBaseInfo'
import PaneFee from './components/PaneFee'
import PaneSku from './components/PaneSku'
import PaneCoupon from './components/PaneCoupon'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'OperationCenterMemberRightManage',
  components: { PaneBaseInfo, PaneFee, PaneSku, PaneCoupon, PageHeader },
  data() {
    return {
      loading: false,
      activeId: '',
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
      this.activeId = to.query.id
      this.init()
    }
  },
  created() {
    this.activeId = this.$route.query.id
    this.activeTab =
      typeof this.$route.query.tab === 'undefined'
        ? 'tabBaseInfo'
        : this.$route.query.tab
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initManage({ id: this.activeId }).then(res => {
        if (res.result === 1) {
          var d = res.data

          this.dropdownOptions = d.levelSts
          this.activeDropdown = this.getActiveDropdown(this.activeId)
        }
        this.loading = false
      })
    },
    onChangeDropdown(id) {
      this.$router.replace({
        query: {
          id: id,
          tab: this.activeTab
        }
      })
    },
    getActiveDropdown(id) {
      const result = this.dropdownOptions.filter((item) => { return item.id === id })[0]
      return result
    }
  }
}
</script>
<style lang="scss" scoped>

</style>
