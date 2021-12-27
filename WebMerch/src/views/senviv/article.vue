<template>
  <div id="clientuser_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.title" clearable style="width: 100%" placeholder="标题" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilter">
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
      <el-table-column label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="标题" prop="title" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.title }}</span>
        </template>
      </el-table-column>
      <el-table-column label="类项" prop="type" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.ctegory.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" prop="createTime" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="onOpenDialogEdit(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetListData" />

    <el-dialog
      title="标签编辑"
      :visible.sync="dialogEditIsVisible"
      :width="isDesktop==true?'600px':'90%'"
    >

      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="onSave">保存</el-button>
        <el-button size="small" @click="dialogEditIsVisible=false">关闭</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getArticles, saveTagExplain } from '@/api/senviv'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ArticleList',
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
        title: undefined
      },
      tagExplainForm: {
        tagName: '',
        proExplain: '',
        tcmExplain: '',
        suggest: ''
      },
      dialogEditIsLoading: false,
      dialogEditIsVisible: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.onGetListData()
  },
  methods: {
    onGetListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getArticles(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    onFilter() {
      this.listQuery.page = 1
      this.onGetListData()
    },
    onOpenDialogEdit(row) {
      this.dialogEditIsVisible = true
    },
    onSave() {

    }
  }
}
</script>
