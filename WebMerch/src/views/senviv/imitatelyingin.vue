<template>
  <div id="clientuser_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.title" clearable style="width: 100%" placeholder="标题" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onOpenDialogImitateLyingIn(null)">
            新建
          </el-button>
        </el-col>
      </el-row>

    </div>

    <div class="tip"><p>以下文章内容，客户在微信公众号点击-模拟坐月时，会在相应的时间段展示对应的内容，若在同一个时间段存在多篇内容，将随机抽取展示</p></div>

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
      <el-table-column label="时间段" prop="proExplain" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.startTimeQt }}</span>-<span>{{ scope.row.endTimeQt }}</span>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" prop="createTime" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="onOpenDialogImitateLyingIn(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <el-dialog
      title="编辑"
      :visible.sync="dialogVisibleImitateLyingIn"
      :width="isDesktop==true?'600px':'90%'"
    >
      <el-form
        ref="formImitateLyingIn"
        v-loading="dialogLoadingImitateLyingIn"
        :model="formImitateLyingIn"
        label-width="75px"
      >
        <el-form-item label="标题">
          <el-input v-model="formImitateLyingIn.title" placeholder="标题" clearable />
        </el-form-item>
        <el-form-item label="时间段">

          <el-time-select
            v-model="formImitateLyingIn.startTimeQt"
            :picker-options="{
              start: '00:00',
              step: '2:00',
              end: '24:00'
            }"
            placeholder="开始时间"
            style="width:120px"
          />
          <span>至</span>
          <el-time-select
            v-model="formImitateLyingIn.endTimeQt"
            :picker-options="{
              start: '00:00',
              step: '2:00',
              end: '24:00'
            }"
            style="width:120px"
            placeholder="结束时间"
          />

        </el-form-item>
        <el-form-item label="内容">
          <el-input v-model="formImitateLyingIn.content" rows="5" type="textarea" show-word-limit />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="onSaveImitateLyingIn">保存</el-button>
        <el-button size="small" @click="dialogVisibleImitateLyingIn=false">关闭</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getImitateLyingIns, saveImitateLyingIn } from '@/api/senviv'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
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
      formImitateLyingIn: {
        id: '',
        title: '',
        content: '',
        startTimeQt: '',
        endTimeQt: ''
      },
      dialogLoadingImitateLyingIn: false,
      dialogVisibleImitateLyingIn: false,
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
      getImitateLyingIns(this.listQuery).then(res => {
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
    onOpenDialogImitateLyingIn(item) {
      if (item == null) {
        this.formImitateLyingIn.id = ''
        this.formImitateLyingIn.title = ''
        this.formImitateLyingIn.content = ''
        this.formImitateLyingIn.startTimeQt = ''
        this.formImitateLyingIn.endTimeQt = ''
      } else {
        this.formImitateLyingIn.id = item.id
        this.formImitateLyingIn.title = item.title
        this.formImitateLyingIn.content = item.content
        this.formImitateLyingIn.startTimeQt = item.startTimeQt
        this.formImitateLyingIn.endTimeQt = item.endTimeQt
      }

      this.dialogVisibleImitateLyingIn = true
    },
    onSaveImitateLyingIn() {
      this.$refs['formImitateLyingIn'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          })
            .then(() => {
              saveImitateLyingIn(this.formImitateLyingIn).then(res => {
                if (res.result === 1) {
                  this.$message({
                    message: res.message,
                    type: 'success'
                  })
                  this.dialogVisibleImitateLyingIn = false
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
