<template>
  <div id="store_manage" class="app-container">
    <div class="cur-store">
      <span class="title">当前会员:</span><span class="name">{{ curLevelSt.name }}</span>
      <el-dropdown trigger="click" @command="handleChangeLevelSt">
        <span class="el-dropdown-link">切换<i class="el-icon-arrow-down el-icon--right" /></span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item v-for="item in levelSts" :key="item.id" :command="item.id"> {{ item.name }}</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
    <el-tabs v-model="activeTab" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <manage-pane-base-info :levelstid="id" /></el-tab-pane>
      <el-tab-pane label="会费设置" name="tabFee"><manage-pane-fee :levelstid="id" /></el-tab-pane>
      <el-tab-pane label="权益设置" name="tabRight"><manage-pane-right :levelstid="id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { initManage } from '@/api/memberright'
import managePaneBaseInfo from './components/ManagePaneBaseInfo'
import managePaneFee from './components/ManagePaneFee'
import managePaneRight from './components/ManagePaneRight'

export default {
  components: { managePaneBaseInfo, managePaneFee, managePaneRight },
  data() {
    return {
      activeTab: 'tabBaseInfo',
      curLevelSt: {
        id: '',
        name: ''
      },
      levelSts: []
    }
  },
  created() {
    this.id = this.$route.params.id
    this.activeTab = typeof this.$route.params.tab === 'undefined' ? 'tabBaseInfo' : this.$route.params.tab
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initManage({ id: this.id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.curLevelSt = d.curLevelSt
          this.id = d.curLevelSt.id
          this.levelSts = d.levelSts
        }
        this.loading = false
      })
    },
    handleChangeLevelSt(id) {
      this.id = id
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
