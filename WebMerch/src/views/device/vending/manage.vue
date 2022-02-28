<template>
  <div id="device_manage">
    <page-header />
    <div class="cur-device cur-tab">
      <div class="it-name">
        <span class="title">当前设备:</span><span class="name">{{ activeDropdown.name }}</span>
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
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <pane-base-info :device-id="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane label="库存信息" name="tabStock"><pane-stock :device-id="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane label="控制中心" name="tabControlCenter"><pane-control-center :device-id="activeDropdown.id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { initManage } from '@/api/devvending'
import PaneBaseInfo from './components/PaneBaseInfo'
import PaneStock from './components/PaneStock'
import PaneControlCenter from './components/PaneControlCenter'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'DeviceVendingManage',
  components: { PaneBaseInfo, PaneStock, PaneControlCenter, PageHeader },
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
          this.dropdownOptions = d.devices
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
