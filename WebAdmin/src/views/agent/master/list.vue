<template>
  <div class="app-container">
    <div class="filter-container">
      <el-input v-model="listQuery.userName" placeholder="用户名" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="handleFilter">
        查询
      </el-button>
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">
        新建
      </el-button>
    </div>
    <el-table
      :key="listKey"
      v-loading="listLoading"
      :data="listData"
      border
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column v-if="isDesktop" label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="用户名" prop="userName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.userName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="姓名" prop="fullName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.fullName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="手机号码" prop="phoneNumber" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.phoneNumber }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="邮箱" prop="email" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.email }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="status" align="left" min-width="15%">
        <template slot-scope="scope">
          <span :class="'enable-status enable-status-'+scope.row.status.value">{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="创建时间" prop="createTime" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="handleUpdate(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { fetchList } from '@/api/agentmaster'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'AdminUserList',
  components: { Pagination },
  data() {
    return {
      listKey: 0,
      listData: null,
      listTotal: 0,
      listLoading: true,
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
      this.listLoading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      fetchList(this.listQuery).then(res => {
        var d = res.data
        this.listData = d.items
        this.listTotal = d.total
        this.listLoading = false
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
    },
    handleCreate() {
      this.$router.push({
        path: '/agent/master/add'
      })
    },
    handleUpdate(row) {
      this.$router.push({
        path: '/agent/master/edit?userId=' + row.id
      })
    }
  }
}
</script>
