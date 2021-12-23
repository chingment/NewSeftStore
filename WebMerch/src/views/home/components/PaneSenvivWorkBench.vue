<template>
  <div style="padding:0px">
    <!-- <el-row :gutter="40" class="panel-group">
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
    </el-row> -->

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

        <el-card v-loading="loadingConsoleInfo" class="box-card box-card-1">
          <div slot="header" class="clearfix">
            <span>待办事项</span>
          </div>
          <div class="body">
            <el-row :gutter="20" style="margin-bottom:20px">
              <el-col :span="12"> <div class="num_box todotask_1" @click="onTodoTaskClick(0)">
                <div class="tl">待处理</div>
                <div class="num">{{ consoleInfo.todoTask.waitHandle }}</div>
              </div></el-col>
              <el-col :span="12">    <div class="num_box todotask_2" @click="onTodoTaskClick(1)">
                <div class="tl">已处理</div>
                <div class="num">{{ consoleInfo.todoTask.handled }}</div>
              </div></el-col>
            </el-row>
          </div>
        </el-card>

      </el-aside>
      <el-main style="padding:0px 0px 0px 20px">

        <el-card v-show="isShowByUsers" class="box-card box-card-1">
          <div slot="header" class="clearfix">
            <span>我的服务对象 {{ careLevelName }}</span>
          </div>
          <div class="body" style="min-height:340px">
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
                        <el-button type="text" @click="onOpenDialogByDetail(item)">查看</el-button>
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

              <pane-user-detail v-if="dialogIsShowByDetail" :visible.sync="dialogIsShowByDetail" :user-id="selectUserId" />

            </div>

          </div>
        </el-card>

        <el-card v-show="isShowByTodoTask" class="box-card box-card-1">
          <div slot="header" class="clearfix">
            <span>待办事项</span>
          </div>
          <div class="body" style="min-height:340px">

            <el-table
              :key="tasks.listKey"
              v-loading="tasks.loading"
              :data="tasks.listData"
              fit
              highlight-current-row
              style="width: 100%;"
            >
              <el-table-column label="序号" prop="id" align="left" width="80">
                <template slot-scope="scope">
                  <span>{{ scope.$index+1 }} </span>
                </template>
              </el-table-column>
              <el-table-column label="类型" align="left" min-width="10%">
                <template slot-scope="scope">
                  <span>{{ scope.row.taskType.text }}</span>
                </template>
              </el-table-column>
              <el-table-column label="标题" align="left" min-width="65%">
                <template slot-scope="scope">
                  <span>{{ scope.row.title }}</span>
                </template>
              </el-table-column>
              <el-table-column label="状态" align="left" min-width="10%">
                <template slot-scope="scope">
                  <span>{{ scope.row.status.text }}</span>
                </template>
              </el-table-column>
              <el-table-column label="时间" align="left" min-width="15%">
                <template slot-scope="scope">
                  <span>{{ scope.row.createTime }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
                <template slot-scope="scope">
                  <el-button v-if="scope.row.status.value===1||scope.row.status.value===2" type="text" size="mini" @click="onHandleTask(scope.row,'task_handle')">
                    处理
                  </el-button>
                  <el-button v-else-if="scope.row.status.value===3" type="text" size="mini" @click="onHandleTask(scope.row,'task_saw')">
                    查看
                  </el-button>
                </template>
              </el-table-column>
            </el-table>
            <pagination v-show="tasks.listTotal>0" :total="tasks.listTotal" :page.sync="tasks.listQuery.page" :limit.sync="tasks.listQuery.limit" @pagination="onGetTasks" />
          </div>
        </el-card>

      </el-main>
    </el-container>

    <pane-stage-report-detail v-if="dialogIsShowByStageReport" :visible.sync="dialogIsShowByStageReport" :report-id="selectStageReportId" :task-id="selectTaskId" :work-type="selectTaskWorkType" @aftersave="onAfterSaveStageReport" />
    <pane-day-report-detail v-if="dialogIsShowByDayReport" :visible.sync="dialogIsShowByDayReport" :report-id="selectDayReportId" :task-id="selectTaskId" :work-type="selectTaskWorkType" @aftersave="onAfterSaveDayReport" />
  </div>

</template>

<script>

import { getInitData } from '@/api/senvivworkbench'
import { getUsers, getTasks } from '@/api/senviv'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import PaneUserDetail from '@/views/senviv/components/PaneUserDetail.vue'
import PaneDayReportDetail from '@/views/senviv/components/PaneDayReportDetail.vue'
import PaneStageReportDetail from '@/views/senviv/components/PaneStageReportDetail.vue'
export default {
  name: 'ClientUserList',
  components: { Pagination, PaneUserDetail, PaneDayReportDetail, PaneStageReportDetail },
  data() {
    return {
      loadingConsoleInfo: false,
      isShowByUsers: true,
      isShowByTodoTask: false,
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
      tasks: {
        loading: false,
        listKey: 0,
        listData: null,
        listTotal: 0,
        listQuery: {
          page: 1,
          limit: 10,
          status: 0
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
        },
        todoTask: {
          waitHandle: 0,
          handled: 0
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
      dialogIsShowByStageReport: false,
      selectStageReportId: '',
      dialogIsShowByDayReport: false,
      selectTaskWorkType: '',
      selectDayReportId: '',
      selectTaskId: '',
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.init()
    this.onGetUsers()
  },
  methods: {
    init() {
      this.consoleInfo.loading = true
      getInitData({}).then(res => {
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
    onGetTasks() {
      this.tasks.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.tasks.listQuery })
      getTasks(this.tasks.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.tasks.listData = d.items
          this.tasks.listTotal = d.total
          this.consoleInfo.todoTask.waitHandle = d.count.waitHandle
          this.consoleInfo.todoTask.handled = d.count.handled
        }
        this.tasks.loading = false
      })
    },
    onCareLevelClick(level) {
      this.isShowByUsers = true
      this.isShowByTodoTask = false
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
    onTodoTaskClick(status) {
      this.isShowByUsers = false
      this.isShowByTodoTask = true
      this.tasks.listQuery.status = status
      this.tasks.listQuery.page = 1
      this.onGetTasks()
    },
    onOpenDialogByDetail(item) {
      this.selectUserId = item.id
      this.dialogIsShowByDetail = true
    },
    onHandleTask(item, workType) {
      this.selectTaskId = item.id
      this.selectTaskWorkType = workType
      if (item.taskType.value === 1) {
        this.dialogIsShowByDayReport = true
        this.selectDayReportId = item.reportId
      } else if (item.taskType.value === 2 || item.taskType.value === 3 || item.taskType.value === 4) {
        this.dialogIsShowByStageReport = true
        this.selectStageReportId = item.reportId
      }
    },
    onAfterSaveStageReport() {
      this.onGetTasks()
    },
    onAfterSaveDayReport() {
      this.onGetTasks()
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

.todotask_1{
background-color: #ff8080;
}

.todotask_2{
 background-color: #66cccc;
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

