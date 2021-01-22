<template>
  <div id="manage_level">
    <div class="cur-dropdown">
      <span class="title">当前会员:</span>
      <span class="name">{{ activeDropdown.name }}</span>
      <el-dropdown trigger="click" @command="handleChangeDropdown">
        <span class="el-dropdown-link">
          切换
          <i class="el-icon-arrow-down el-icon--right" />
        </span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item
            v-for="option in dropdownOptions"
            :key="option.id"
            :command="option.id"
          >{{ option.name }}</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
    <el-tabs v-model="activeTab" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo">
        <manage-pane-base-info :levelstid="activeDropdown.id" />
      </el-tab-pane>
      <el-tab-pane label="会费设置" name="tabFee">
        <manage-pane-fee :levelstid="activeDropdown.id" />
      </el-tab-pane>
      <el-tab-pane label="权益设置" name="tabRight">
        <manage-pane-right :levelstid="activeDropdown.id" />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import { initManage } from '@/api/memberright'
import managePaneBaseInfo from './components/ManagePaneBaseInfo'
import managePaneFee from './components/ManagePaneFee'
import managePaneRight from './components/ManagePaneRight'

export default {
  name: 'OperationCenterMemberRightManage',
  components: { managePaneBaseInfo, managePaneFee, managePaneRight },
  data() {
    return {
      activeTab: 'tabBaseInfo',
      activeDropdown: {
        id: '',
        name: ''
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
          this.activeDropdown = d.curLevelSt
          this.dropdownOptions = d.levelSts
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
#manage_level {
  padding-top: 0px;
  .cur-dropdown {
    font-size: 14px;
    line-height: 60px;
    .title {
      color: #5e6d82;
    }
    .name {
      padding: 0 10px;
      color: #2ac06d;
    }
  }
}
</style>
