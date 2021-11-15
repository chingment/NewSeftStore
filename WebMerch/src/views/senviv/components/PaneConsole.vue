<template>

  <div class="app-container" style="padding:0px">
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
        <el-card class="box-card box-card-1">
          <div slot="header" class="clearfix">
            <span>今日完成情况</span>
          </div>
          <div v-for="o in 4" :key="o" class="text item">
            {{ '列表内容 ' + o }}
          </div>
        </el-card>
      </el-aside>
      <el-main style="padding:0px 20px 0px 20px">

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
      this.onGetUsers()
    },
    handleFilter() {
      this.users.listQuery.page = 1
      this.onGetUsers()
    },
    handleOpenDialogByDetail(item) {
      this.selectUserId = item.id
      this.dialogIsShowByDetail = true
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
</style>
