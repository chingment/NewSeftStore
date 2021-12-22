<template>

  <el-dialog v-if="visible" :lock-scroll="false" title="详情" :visible.sync="visible" width="80%" custom-class="user-detail" append-to-body :before-close="onBeforeClose">
    <div id="senviv_user_detail" v-loading="loading" class="app-container my-container2">

      <el-container>
        <el-aside style="width:300px;margin-right:10px">

          <el-card class="box-card box-card-senviv-user" :body-style="{ padding: '0px' }">
            <div class="it-header clearfix">
              <div class="left">
                <div class="l1">
                  <el-avatar :src="userDetail.avatar" size="medium" />
                </div>
                <div class="l2">
                  <span class="name">{{ userDetail.signName }}</span>
                </div>
              </div>
            </div>
            <div class="it-component">
              <div class="t1"><span class="sex">{{ userDetail.sex }}</span> <span class="age">{{ userDetail.age }}岁</span> <span class="height">身高：{{ userDetail.height }}</span><span class="weight">体重：{{ userDetail.weihgt }}</span></div>
              <div>

                <el-tag
                  v-for="tag in userDetail.signTags"
                  :key="tag.name"
                  style="margin-right: 10px;margin-bottom: 10px"
                  :type="tag.type"
                >
                  {{ tag.name }}
                </el-tag>

              </div>
            </div>
          </el-card>
          <el-card class="box-card box-card-menu" :body-style="{ padding: '0px'}" style="margin-top:10px;">
            <el-menu :default-active="activeMenu" style="background:#fff" @select="leftMenuChange">
              <el-menu-item-group>
                <el-menu-item index="UserInfo">基本信息</el-menu-item>
                <el-menu-item index="VisitRecord">回访告知</el-menu-item>
                <el-menu-item index="DayReport">健康（日）报告</el-menu-item>
                <el-menu-item index="MonthReport">健康（月）报告</el-menu-item>
              </el-menu-item-group>
            </el-menu>
          </el-card>
        </el-aside>
        <el-container>
          <el-main class="">
            <pane-user-info v-if="activeMenu==='UserInfo'" :user-id="userId" />
            <pane-visit-record v-if="activeMenu==='VisitRecord'" :user-id="userId" />
            <pane-day-report v-if="activeMenu==='DayReport'" :user-id="userId" />
            <pane-stage-report v-if="activeMenu==='MonthReport'" :user-id="userId" rpt-type="per_month" />
          </el-main>
        </el-container>
      </el-container>

    </div>
  </el-dialog>

</template>

<script>

import { MessageBox } from 'element-ui'
import PaneDayReport from './PaneDayReport.vue'
import PaneStageReport from './PaneStageReport.vue'
import PaneUserInfo from './PaneUserInfo.vue'
import PaneVisitRecord from './PaneVisitRecord.vue'
import { getUserDetail } from '@/api/senviv'

export default {
  // name: 'SenvivPaneUserDetail',
  components: { PaneDayReport, PaneStageReport, PaneUserInfo, PaneVisitRecord },
  props: {
    userId: {
      type: String,
      default: ''
    },
    visible: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      loading: false,
      activeMenu: 'UserInfo',
      userDetail: {
        avatar: '',
        signName: '',
        sex: '',
        age: '',
        height: '',
        weight: '',
        signTags: []
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {

  },
  created() {
    this._getUserDetail()
  },
  methods: {
    _getUserDetail() {
      this.loading = true
      getUserDetail({ userId: this.userId }).then(res => {
        if (res.result === 1) {
          this.userDetail = res.data
        }
        this.loading = false
      })
    },
    leftMenuChange(index, indexPath) {
      this.activeMenu = index
    },
    onBeforeClose() {
      this.$emit('update:visible', false)
    }
  }
}
</script>

<style lang="scss" scoped>

.el-card__header{
     padding: 5px 5px !important;
 }

#senviv_user_detail
{

.el-col-5 {
  width: 20%;
}

}

.box-card-menu{
min-height: calc(100vh - 450px);
}
</style>
