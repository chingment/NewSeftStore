<template>
  <div id="user_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="优惠券名称" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
        </el-col>
      </el-row>

    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column v-if="isDesktop" label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="优惠券名称" prop="name" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="优惠券类型" prop="category" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.category }}</span>
        </template>
      </el-table-column>
      <el-table-column label="可使用范围" prop="useAreaType" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.useAreaType }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="使用门槛" prop="atLeastAmount" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.atLeastAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="券种" prop="faceType" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.faceType }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="券值" prop="faceValue" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.faceValue }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="有效期" prop="validDate" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.validDate }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="状态" prop="validDate" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.status }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="handleDetails(row)">
            查看
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { getList } from '@/api/coupon'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'AdminUserList',
  components: { Pagination },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
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
      getList(this.listQuery).then(res => {
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
        path: '/clientuser/details?id=' + row.id
      })
    }
  }
}
</script>
