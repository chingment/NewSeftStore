<template>
  <div id="senviv_user_list">
    <div class="filter-container">

      <el-form ref="form" label-width="120px" class="query-box">
        <el-form-item label="目前困扰">

          <el-radio-group v-model="listQuery.perplex" @change="handleFilter">
            <el-radio-button v-for="item in perplexs" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>

          <!-- <el-checkbox-group v-model="checkboxGroup1">
            <el-checkbox-button v-for="item in perplexs" :key="item.value" :label="item.value">{{ item.label }}</el-checkbox-button>
          </el-checkbox-group> -->

        </el-form-item>
        <el-form-item label="慢性病">
          <el-radio-group v-model="listQuery.chronic" @change="handleFilter">
            <el-radio-button v-for="item in chronics" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="呼吸综合症">
          <el-radio-group v-model="listQuery.sas" @change="handleFilter">
            <el-radio-button v-for="item in sass" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="搜索">
          <el-input v-model="listQuery.name" clearable style="max-width: 300px;" placeholder="昵称/姓名" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
        </el-form-item>
      </el-form>

    </div>
    <el-row v-loading="loading" :gutter="20">

      <el-col
        v-for="item in listData"
        :key="item.id"
        :span="5"
        class="my-col"
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

      <el-col v-if="listData===null||listData.length===0" style="text-align: center;color: #909399">

        <span>数据为空</span>

      </el-col>

    </el-row>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <pane-user-detail v-if="dialogIsShowByDetail" :visible.sync="dialogIsShowByDetail" :user-id="selectUserId" />

  </div>
</template>

<script>
import { getUsers } from '@/api/senviv'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import PaneUserDetail from './components/PaneUserDetail.vue'
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

.el-card__header{
     padding: 5px 5px !important;
 }

#senviv_user_list
{

.el-col-5 {
  width: 20%;
}

}
</style>
