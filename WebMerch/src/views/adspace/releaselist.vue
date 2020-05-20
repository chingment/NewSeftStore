<template>
  <div id="user_list" class="app-container">
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
      <el-table-column label="图片" prop="imgUrl" align="left" width="100">
        <template slot-scope="scope">
          <img :src="scope.row.url" style="width:80px;height:80px;">
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="标题" prop="title" align="left" min-width="70%">
        <template slot-scope="scope">
          <span>{{ scope.row.title }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="status" align="left" min-width="15%">
        <template slot-scope="scope">
          <span :class="'enable-status enable-status-'+scope.row.status.value">{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="发布时间" prop="createTime" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="handleDelete(row)">
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getReleaseList, deleteAdContent } from '@/api/adspace'
import { getUrlParam } from '@/utils/commonUtil'
export default {
  name: 'AdminUserList',
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        adSpaceId: 0
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    this.listQuery.adSpaceId = getUrlParam('id')
    this.getListData()
  },
  methods: {
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getReleaseList(this.listQuery).then(res => {
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
    handleDelete(row) {
      MessageBox.confirm('确定要删除', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        deleteAdContent({ id: row.id }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.getListData()
          }
        })
      }).catch(() => {
      })
    }
  }
}
</script>
