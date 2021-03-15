<template>
  <div id="senviv_user_list">
    <div class="filter-container">

      <el-form ref="form" label-width="120px">
        <el-form-item label="困扰">

          <el-checkbox-group v-model="checkboxGroup1">
            <el-checkbox-button v-for="item in perplexs" :key="item.value" :label="item.value">{{ item.label }}</el-checkbox-button>
          </el-checkbox-group>

        </el-form-item>
        <el-form-item label="呼吸综合征">
          <el-radio-group v-model="checkboxGroup2">
            <el-radio-button v-for="item in sass" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="姓名">
          <el-input v-model="listQuery.name" clearable style="max-width: 300px;" @keyup.enter.native="handleFilter" />
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
        <el-card class="box-card box-card-product" :body-style="{ padding: '0px' }">
          <div class="it-header clearfix">
            <div class="left">
              <img class="headImgurl" :src="item.headImgurl">
              <span class="name">{{ item.signName }}</span>
            </div>
            <div class="right">
              <el-button type="text" @click="dialogKindSpuOpen(true,item)">查看</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="t1"><span class="sex">性别：{{ item.sex }}</span><span class="height">身高：{{ item.height }}</span><span class="weight">体重：{{ item.weight }}</span></div>
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
    </el-row>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { getUsers } from '@/api/senviv'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ClientUserList',
  components: { Pagination },
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
        perplexs: null
      },
      checkboxGroup1: [],
      checkboxGroup2: [],
      perplexs: [
        { value: '0', label: '全部' },
        { value: '1', label: '没有困扰' },
        { value: '2', label: '睡眠呼吸暂停综合征' },
        { value: '3', label: '打鼾' },
        { value: '4', label: '糖尿病' },
        { value: '5', label: '高血压' },
        { value: '6', label: '冠心病' },
        { value: '7', label: '心脏病' },
        { value: '8', label: '心衰' },
        { value: '9', label: '慢性阻塞性肺疾病' },
        { value: '10', label: '脑梗塞/脑卒中' },
        { value: '11', label: '长期失眠' },
        { value: '12', label: '癫痫' },
        { value: '13', label: '不宁腿综合征' },
        { value: '14', label: '其它' }
      ],
      sass: [
        { value: '0', label: '全部' },
        { value: '1', label: '轻度' },
        { value: '2', label: '中度' },
        { value: '3', label: '重度' },
        { value: '4', label: '无' }
      ],
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
    handleDetails(row) {
      this.$router.push({
        path: '/client/shop/details?id=' + row.id
      })
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

.box-card-product {

  .it-header {
    display: flex;
    justify-content: flex-start;
    align-items: center;
    position: relative;
    padding: 5px 20px;
    border-bottom: 1px solid #EBEEF5;
    box-sizing: border-box;
    .left {
      flex: 1;
      justify-content: flex-start;
      align-items: center;
      display: flex;
      height: 100%;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
      .name {
        padding: 0px 5px;
      }

      .headImgurl{
          width: 32px;
          height: 32px;
          border-radius: 50%;
          display: block;
      }
    }
    .right {
      width: 100px;
      display: flex;
      justify-content: flex-end;
      align-items: center;
    }
  }
  .it-component {
    height: 160px;
    overflow: hidden;
    text-overflow: ellipsis;
    padding: 5px 20px;

  .t1{
    padding:  5px 0px;
    span{
    font-size: 14px;
    margin-left: 5px;
    }
  }
  }
}

}
</style>
