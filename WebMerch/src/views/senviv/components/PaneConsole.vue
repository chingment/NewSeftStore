<template>

  <el-card class="box-card">
    <div slot="header" class="clearfix">
      <span>我的服务对象</span>
    </div>
    <div class="item">
      <div class="app-container" style="padding:0px">
        <el-container>
          <el-aside width="400px">
            <el-card class="box-card box-card-1">
              <div slot="header" class="clearfix">
                <span>关注情况</span>
              </div>
              <div class="body">
                <el-row :gutter="20" style="margin-bottom:20px">
                  <el-col :span="12"> <div class="num_box gz_1">
                    <div class="tl">紧急关注</div>
                    <div class="num">1</div>
                  </div></el-col>
                  <el-col :span="12">    <div class="num_box gz_2">
                    <div class="tl">密切关注</div>
                    <div class="num">1</div>
                  </div></el-col>
                </el-row>
                <el-row :gutter="20">
                  <el-col :span="12"> <div class="num_box gz_3">
                    <div class="tl">中等关注</div>
                    <div class="num">1</div>
                  </div></el-col>
                  <el-col :span="12">    <div class="num_box gz_4">
                    <div class="tl">轻微关注</div>
                    <div class="num">1</div>
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

              <el-row v-loading="loading" :gutter="20">

                <el-col
                  v-for="item in listData"
                  :key="item.id"
                  :span="6"
                  style="margin-bottom:10px"
                >
                  <el-card class="box-card box-card-senviv-user" :body-style="{ padding: '0px' }">
                    <div class="it-header clearfix">
                      <div class="left">
                        <div class="l1">
                          <el-avatar :src="item.headImgurl" size="medium" />
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

                <el-col v-if="listData===null||listData.length===0" style="text-align: center;color: #909399">

                  <span>数据为空</span>

                </el-col>

              </el-row>
              <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

              <el-dialog v-if="dialogIsShowByDetail" title="详情" :visible.sync="dialogIsShowByDetail" width="80%" custom-class="user-detail" append-to-body>
                <pane-user-detail :user-id="selectUserId" />
              </el-dialog>

            </div>

          </el-main>
        </el-container>
      </div>

    </div>
  </el-card>

</template>

<script>
import { getUsers } from '@/api/senviv'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import PaneUserDetail from '@/views/senviv/components/PaneUserDetail.vue'
export default {
  name: 'ClientUserList',
  components: { Pagination, PaneUserDetail },
  data() {
    return {
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
        perplex: '0'
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
    this.getListData()
  },
  methods: {
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getUsers(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
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
.tl{
    height: 30px;
}

.num{
    height: 30px;
    font-weight: bold;
    font-size: 24px;
}
}

.gz_1{
background-color: #ff8080;
}
.gz_2{
    background-color: #6699ff;
}
.gz_3{
   background-color: #6666cc;
}
.gz_4{
  background-color: #ffd580;
}
</style>
