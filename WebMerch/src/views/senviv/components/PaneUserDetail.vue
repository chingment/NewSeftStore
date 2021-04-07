<template>
  <div id="senviv_user_detail" v-loading="loading" class="app-container my-container2">

    <el-container>
      <el-aside style="width:300px;margin-right:10px">

        <el-card class="box-card box-card-senviv-user" :body-style="{ padding: '0px' }">
          <div class="it-header clearfix">
            <div class="left">
              <div class="l1">
                <el-avatar :src="userDetail.headImgurl" size="medium" />
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
              <el-menu-item index="1">基本信息</el-menu-item>
              <el-menu-item index="DayReport">健康报告（日）</el-menu-item>
              <el-menu-item index="MonthReport">健康报告（月）</el-menu-item>
            </el-menu-item-group>
          </el-menu>
        </el-card>
      </el-aside>
      <el-container>
        <el-main class="">

          <pane-day-report v-show="activeMenu==='DayReport'" :user-id="userId" />
          <pane-month-report v-show="activeMenu==='MonthReport'" :user-id="userId" />
        </el-main>
      </el-container>
    </el-container>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import PaneDayReport from './PaneDayReport.vue'
import PaneMonthReport from './PaneMonthReport.vue'
import { getUserDetail } from '@/api/senviv'

export default {
  name: 'SenvivPaneUserDetail',
  components: { PaneDayReport, PaneMonthReport },
  props: {
    userId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
      activeMenu: 'DayReport',
      userDetail: {
        headImgurl: '',
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
