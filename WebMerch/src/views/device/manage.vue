<template>
  <div id="device_manage">
    <page-header />
    <div class="cur-device cur-tab">
      <div class="it-name">
        <span class="title">当前设备:</span><span class="name">{{ activeDropdown.name }}</span>
      </div>
      <el-dropdown class="it-switch" trigger="click" @command="handleChangeDropdown">
        <span class="el-dropdown-link">
          切换<i class="el-icon-arrow-down el-icon--right" />
        </span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item v-for="option in dropdownOptions" :key="option.id" :command="option.id"> {{ option.name }}</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
    <el-tabs v-model="activeTab" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <manage-pane-base-info :device-id="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane label="库存信息" name="tabStock"><manage-pane-stock :device-id="activeDropdown.id" /></el-tab-pane>
      <el-tab-pane label="控制中心" name="tabControlCenter"><manage-pane-control-center :devcie-id="activeDropdown.id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { initManage } from '@/api/device'
import { getUrlParam } from '@/utils/commonUtil'
import managePaneBaseInfo from './components/ManagePaneBaseInfo'
import managePaneStock from './components/ManagePaneStock'
import managePaneControlCenter from './components/ManagePaneControlCenter'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'DeviceManage',
  components: { managePaneBaseInfo, managePaneStock, managePaneControlCenter, PageHeader },
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
          this.activeDropdown = d.curDevice
          this.dropdownOptions = d.devices
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

</style>
