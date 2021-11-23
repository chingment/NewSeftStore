<template>
  <div style="padding:0px">
    <el-row :gutter="40" class="panel-group">
      <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class="card-panel" @click="handleSenvivUsers">
          <div class="card-panel-icon-wrapper icon-select">
            <svg-icon icon-class="t_users" class-name="card-panel-icon" />
          </div>
          <div class="card-panel-description">
            <div class="card-panel-text">
              客户信息
            </div>
          </div>
        </div>
      </el-col>
      <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class="card-panel" @click="handleSenvivDayReport">
          <div class="card-panel-icon-wrapper icon-select">
            <svg-icon icon-class="t_dayreport" class-name="card-panel-icon" />
          </div>
          <div class="card-panel-description">
            <div class="card-panel-text">
              健康日报
            </div>
          </div>
        </div>
      </el-col>
      <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class="card-panel" @click="handleSenvivMonthReport">
          <div class="card-panel-icon-wrapper icon-select">
            <svg-icon icon-class="t_monthreport" class-name="card-panel-icon" />
          </div>
          <div class="card-panel-description">
            <div class="card-panel-text">
              健康月报
            </div>
          </div>
        </div>
      </el-col>
    </el-row>

    <el-container>
      <el-aside width="400px">
        <el-card v-loading="loadingConsoleInfo" class="box-card box-card-1">
          <div slot="header" class="clearfix">
            <span>关注情况</span>
          </div>
          <div class="body">
            <el-row :gutter="20" style="margin-bottom:20px">
              <el-col :span="24"> <div class="num_box gz_0" @click="onCareLevelClick(0)">
                <div class="tl">我的服务对象</div>
                <div class="num">{{ consoleInfo.userCount }}</div>
              </div></el-col>
            </el-row>
            <el-row :gutter="20" style="margin-bottom:20px">
              <el-col :span="12"> <div class="num_box gz_4" @click="onCareLevelClick(4)">
                <div class="tl">紧急关注</div>
                <div class="num">{{ consoleInfo.careLevel.level4 }}</div>
              </div></el-col>
              <el-col :span="12">    <div class="num_box gz_3" @click="onCareLevelClick(3)">
                <div class="tl">密切关注</div>
                <div class="num">{{ consoleInfo.careLevel.level3 }}</div>
              </div></el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12"> <div class="num_box gz_2" @click="onCareLevelClick(2)">
                <div class="tl">中等关注</div>
                <div class="num">{{ consoleInfo.careLevel.level2 }}</div>
              </div></el-col>
              <el-col :span="12">    <div class="num_box gz_1" @click="onCareLevelClick(1)">
                <div class="tl">轻微关注</div>
                <div class="num">{{ consoleInfo.careLevel.level1 }}</div>
              </div></el-col>
            </el-row>
          </div>
        </el-card>

      </el-aside>
      <el-main style="padding:0px 0px 0px 20px">

        <el-card class="box-card box-card-1">
          <div slot="header" class="clearfix">
            <span>我的服务对象 {{ careLevelName }}</span>
          </div>
          <div class="body">
            <div id="senviv_user_list">

              <el-row v-loading="users.loading" :gutter="20">

                <el-col
                  v-for="item in users.listData"
                  :key="item.id"
                  :span="6"
                  style="margin-bottom:10px"
                >
                  <el-card class="box-card box-card-senviv-user" :body-style="{ padding: '0px' }">
                    <div class="it-header clearfix">
                      <div class="left">
                        <div class="l1">
                          <el-avatar :src="item.avatar" size="medium" />
                        </div>
                        <div class="l2">
                          <span class="name">{{ item.signName }}</span>
                        </div>
                      </div>
                      <div class="right">
                        <el-button type="text" @click="handleOpenDialogByDetail(item)">查看</el-button>
                      </div>
                    </div>
                    <div class="it-component">
                      <div class="t1"><span class="sex">{{ item.sex }}</span> <span class="age">{{ item.age }}岁</span> <span class="height">身高：{{ item.height }}</span><span class="weight">体重：{{ item.weight }}</span></div>
                      <div>

                        <el-tag
                          v-for="tag in item.signTags"
                          :key="tag.name"
                          style="margin-right: 10px;margin-bottom: 10px"
                          :type="tag.type"
                        >
                          {{ tag.name }}
                        </el-tag>

                      </div>
                    </div>
                  </el-card>
                </el-col>

                <el-col v-if="users.listData===null||users.listData.length===0" style="text-align: center;color: #909399">

                  <span>数据为空</span>

                </el-col>

              </el-row>
              <pagination v-show="users.listTotal>0" :total="users.listTotal" :page.sync="users.listQuery.page" :limit.sync="users.listQuery.limit" @pagination="onGetUsers" />

              <el-dialog v-if="dialogIsShowByDetail" title="详情" :visible.sync="dialogIsShowByDetail" width="80%" custom-class="user-detail" append-to-body>
                <pane-user-detail :user-id="selectUserId" />
              </el-dialog>

            </div>

          </div>
        </el-card>

      </el-main>
    </el-container>
  </div>

</template>

<script>
import { getUsers, getConsoleInfo } from '@/api/senviv'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import PaneUserDetail from '@/views/senviv/components/PaneUserDetail.vue'
export default {
  name: 'ClientUserList',
  components: { Pagination, PaneUserDetail },
  data() {
    return {
      loadingConsoleInfo: false,
      users: {
        loading: false,
        listKey: 0,
        listData: null,
        listTotal: 0,
        listQuery: {
          page: 1,
          limit: 20,
          name: undefined,
          sas: '0',
          chronic: '0',
          perplex: '0',
          careLevel: 0
        }
      },
      careLevelName: '',
      consoleInfo: {
        loading: false,
        userCount: 0,
        careLevel: {
          level0: 0,
          level1: 1,
          level2: 2,
          level3: 3,
          level4: 4
        }
      },
      perplexs: [
        { value: '0', label: '全部' },
        { value: '1', label: '没有困扰' },
        { value: '2', label: '睡眠呼吸暂停综合症' },
        { value: '3', label: '打鼾' },
        { value: '11', label: '长期失眠' },
        { value: '13', label: '不宁腿综合症' },
        { value: '14', label: '其它' }
      ],
      chronics: [
        { value: '0', label: '全部' },
        { value: '4', label: '糖尿病' },
        { value: '5', label: '高血压' },
        { value: '6', label: '冠心病' },
        { value: '7', label: '心脏病' },
        { value: '8', label: '心衰' },
        { value: '9', label: '慢性阻塞性肺疾病' },
        { value: '10', label: '脑梗塞/脑卒中' },
        { value: '12', label: '癫痫' }
      ],
      sass: [
        { value: '0', label: '全部' },
        { value: '4', label: '无' },
        { value: '1', label: '轻度' },
        { value: '2', label: '中度' },
        { value: '3', label: '重度' }
      ],
      selectUserId: '',
      dialogIsShowByDetail: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.onGetConsoleInfo()
    this.onGetUsers()
  },
  methods: {
    onGetConsoleInfo() {
      this.consoleInfo.loading = true
      getConsoleInfo({}).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.consoleInfo = d
        }
        this.consoleInfo.loading = false
      })
    },
    onGetUsers() {
      this.users.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.users.listQuery })
      getUsers(this.users.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.users.listData = d.items
          this.users.listTotal = d.total
        }
        this.users.loading = false
      })
    },
    onCareLevelClick(level) {
      this.users.listQuery.careLevel = level

      switch (level) {
        case 0:
          this.careLevelName = ''
          break
        case 1:
          this.careLevelName = '紧紧关注'
          break
        case 2:
          this.careLevelName = '密切关注'
          break
        case 3:
          this.careLevelName = '中等关注'
          break
        case 4:
          this.careLevelName = '轻微关注'
          break
      }

      this.onGetUsers()
    },
    handleFilter() {
      this.users.listQuery.page = 1
      this.onGetUsers()
    },
    handleOpenDialogByDetail(item) {
      this.selectUserId = item.id
      this.dialogIsShowByDetail = true
    },
    handleSenvivUsers() {
      this.$router.push({
        path: '/senviv/users'
      })
    },
    handleSenvivDayReport() {
      this.$router.push({
        path: '/senviv/dayreport'
      })
    },
    handleSenvivMonthReport() {
      this.$router.push({
        path: '/senviv/monthreport'
      })
    }
  }
}
</script>

<style lang="scss" scoped>

.box-card-1{
    margin-bottom: 15px;
}

.num_box{
border-radius: 4px!important;
color:#fff;
text-align: center;
padding: 20px;
cursor: pointer;
.tl{
    height: 30px;
}

.num{
    height: 30px;
    font-weight: bold;
    font-size: 24px;
}
}
.gz_0{
  background-color: #3fbdf3;
}
.gz_4{
background-color: #ff8080;
}
.gz_3{
    background-color: #6699ff;
}
.gz_2{
   background-color: #6666cc;
}
.gz_1{
  background-color: #ffd580;
}

.today-sum{
  display: flex;
  .it{
    flex: 1;
    justify-content: center;
    align-items: center;
    align-content: center;
    display: flex;

    .t1{
      text-align: center;
      cursor: pointer;
      .m1{
       font-size: 42px;
       line-height: 60px;
      }

       .m2{
       font-size: 42px;
       color: #cf9236;
         line-height: 60px;
      }

           .m3{
       font-size: 42px;
       color: #ff4949;
         line-height: 60px;
      }
    }
  }
}

.rl{
    list-style: none;
    padding: 0px;
    margin: 0px;

  .it{
  display: flex;
line-height: 30px;
height:30px;
overflow:hidden;
  .name{
    flex: 2;
    text-align: left
  }

  .sumQuantity{
    flex: 1;
    text-align:center;
  }
  .sumTradeAmount{
    flex: 1;
    text-align: right;
  }
  }

  .rli-0{
    color: #2096d4;
  }
  .rli-1{
    color: #24ad8c;
  }
   .rli-2{
    color: #d747a6;
  }

}

.panel-group {

  .card-panel-col {
    margin-bottom: 30px;
  }

  .card-panel {
    height: 108px;
    cursor: pointer;
    font-size: 12px;
    position: relative;
    overflow: hidden;
    color: #666;
    background: #fff;
    -webkit-box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
    box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
    border-color: rgba(0, 0, 0, .05);
    border-radius: 4px;
    &:hover {
      .card-panel-icon-wrapper {
        color: #fff;
      }

      .icon-select {
        background: #40c9c6;
      }
    }

    .icon-people {
      color: #40c9c6;
    }

    .icon-message {
      color: #36a3f7;
    }

    .icon-money {
      color: #f4516c;
    }

    .icon-shopping {
      color: #34bfa3
    }

    .card-panel-icon-wrapper {
      float: left;
      margin: 14px 0 0 14px;
      padding: 16px;
      transition: all 0.38s ease-out;
      border-radius: 6px;
    }

    .card-panel-icon {
      float: left;
      font-size: 48px;
    }

    .card-panel-description {
      float: right;
      font-weight: bold;
      margin: 26px;
      margin-left: 0px;

      .card-panel-text {
        line-height: 18px;
        color: rgba(0, 0, 0, 0.45);
        font-size: 16px;
        margin-bottom: 12px;
      }

      .card-panel-num {
        font-size: 20px;
      }
    }
  }
}

@media (max-width:550px) {
  .card-panel-description {
    display: none;
  }

  .card-panel-icon-wrapper {
    float: none !important;
    width: 100%;
    height: 100%;
    margin: 0 !important;

    .svg-icon {
      display: block;
      margin: 14px auto !important;
      float: none !important;
    }
  }
}

</style>

