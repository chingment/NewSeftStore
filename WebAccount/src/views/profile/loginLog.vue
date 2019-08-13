<template>
  <div class="app-container">
    <div class="filter-container">
      <el-date-picker v-model="listQuery.startDate"  type="date" placeholder="开始日期" style="width: 200px;" />
      -
      <el-date-picker v-model="listQuery.endDate"  type="date" placeholder="结束日期" style="width: 200px;" />
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="handleFilter">
        查询
      </el-button>
    </div>
    <el-table
      :key="tableKey"
      v-loading="listLoading"
      :data="list"
      border
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column v-if="isMobileHidden" label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }}</span>
        </template>
      </el-table-column>
      <el-table-column label="方式" prop="loginWay" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.loginWay }}</span>
        </template>
      </el-table-column>
      <el-table-column label="时间" prop="loginTime" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.loginTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="IP" prop="ip" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.location }}</span>
        </template>
      </el-table-column>
      <el-table-column label="地点" prop="location" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.location }}</span>
        </template>
      </el-table-column>
      <el-table-column label="描述" prop="description" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.description }}</span>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="total>0" :total="total" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getList" />

  </div>
</template>

<script>
import { fetchList } from '@/api/loginlog'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ComplexTable',
  components: { Pagination },
  data() {
    return {
      tableKey: 0,
      list: null,
      total: 0,
      listLoading: true,
      listQuery: {
        page: 1,
        limit: 10,
        startDate: undefined,
        endDate: undefined
      },
      isMobileHidden: this.$store.state.app.device !== 'mobile'
    }
  },
  created() {
    this.getList()
  },
  methods: {
    getList() {
      this.listLoading = true
      fetchList(this.listQuery).then(response => {
        this.list = response.data.items
        this.total = response.data.total

        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1.5 * 1000)
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getList()
    }
  }
}
</script>
