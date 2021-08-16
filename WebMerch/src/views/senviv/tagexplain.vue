<template>
  <div id="clientuser_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.userName" clearable style="width: 100%" placeholder="标签" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
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
      <el-table-column label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="标签" prop="tagName" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.tagName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="专业解读" prop="proExplain" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.proExplain }}</span>
        </template>
      </el-table-column>
      <el-table-column label="中医解读" prop="tcmExplain" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.tcmExplain }}</span>
        </template>
      </el-table-column>
      <el-table-column label="建议" prop="suggest" align="left" min-width="20%">
        <template slot-scope="scope">
          <pre style="white-space: pre-line;line-height: 23px;">{{ scope.row.suggest }}</pre>

        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="handleOpenByTagExplain(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <el-dialog
      title="标签编辑"
      :visible.sync="dialogTagExplainIsVisible"
      :width="isDesktop==true?'600px':'90%'"
    >
      <el-form
        ref="tagExplainForm"
        v-loading="dialogTagExplainIsLoading"
        :model="tagExplainForm"
        label-width="75px"
      >
        <el-form-item label="名称">
          {{ tagExplainForm.tagName }}
        </el-form-item>
        <el-form-item label="专业解读" prop="name">
          <el-input v-model="tagExplainForm.proExplain" rows="5" type="textarea" show-word-limit />
        </el-form-item>
        <el-form-item label="中医解读" prop="name">
          <el-input v-model="tagExplainForm.tcmExplain" rows="5" type="textarea" show-word-limit />
        </el-form-item>
        <el-form-item label="建议" prop="name">
          <el-input v-model="tagExplainForm.suggest" rows="5" type="textarea" show-word-limit />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="handleSaveTagExplain">保存</el-button>
        <el-button size="small" @click="dialogTagExplainIsVisible=false">关闭</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getTagExplains, saveTagExplain } from '@/api/senviv'
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
        limit: 10,
        userName: undefined
      },
      tagExplainForm: {
        tagName: '',
        proExplain: '',
        tcmExplain: '',
        suggest: ''
      },
      dialogTagExplainIsLoading: false,
      dialogTagExplainIsVisible: false,
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
      getTagExplains(this.listQuery).then(res => {
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
    handleOpenByTagExplain(row) {
      this.dialogTagExplainIsVisible = true
      this.tagExplainForm.id = row.id
      this.tagExplainForm.tagName = row.tagName
      this.tagExplainForm.proExplain = row.proExplain
      this.tagExplainForm.tcmExplain = row.tcmExplain
      this.tagExplainForm.suggest = row.suggest
    },
    handleSaveTagExplain() {
      this.$refs['tagExplainForm'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          })
            .then(() => {
              saveTagExplain(this.tagExplainForm).then(res => {
                if (res.result === 1) {
                  this.$message({
                    message: res.message,
                    type: 'success'
                  })
                  this.dialogTagExplainIsVisible = false
                  this.getListData()
                } else {
                  this.$message({
                    message: res.message,
                    type: 'error'
                  })
                }
              })
            })
            .catch(() => {})
        }
      })
    }
  }
}
</script>
