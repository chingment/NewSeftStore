<template>
  <div id="user_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.userName" clearable style="width: 100%" placeholder="用户名" class="filter-item" />
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
      <el-table-column label="商品名称" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="销售渠道" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.sellChannelRefName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="操作时间" prop="createTime" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作类型" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.changeTypeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="变化数量" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.changeQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="变化后数量" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sellQuantity }}</span>
          <span>{{ scope.row.waitPayLockQuantity }}</span>
          <span>{{ scope.row.waitPickupLockQuantity }}</span>
          <span>{{ scope.row.sumQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="备注" align="left" min-width="50%">
        <template slot-scope="scope">
          <span>{{ scope.row.remark }}</span>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { getListByStock } from '@/api/log'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ListByStockLog',
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
        userName: undefined
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
      getListByStock(this.listQuery).then(res => {
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
    }
  }
}
</script>
