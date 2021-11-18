<template>
  <div id="replenishplan_list">

    <div class="filter-container">
      <el-form ref="form" label-width="120px" class="query-box">
        <el-form-item label="单据号">
          <el-input v-model="listQuery.cumCode" clearable placeholder="单据号" style="max-width: 300px;" class="filter-item" />
        </el-form-item>
        <el-form-item>
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilter">
            查询
          </el-button>
          <el-button style="margin-left: 10px;" type="primary" icon="el-icon-document" @click="onDialogNewPlanOpen">
            新建补货计划单
          </el-button>
        </el-form-item>
      </el-form>

    </div>

    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="单号" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.cumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="制单人" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.makerName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="制单日期" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.makeDate }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="生成时间" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.buildTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="描述" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.remark }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">

          <el-button v-if="row.status.value===3" type="text" size="mini" @click="onSawDetail(row)">
            查看
          </el-button>

        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />

    <el-dialog title="新建补货计划" :visible.sync="dialogNewPlanIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div>

        <el-form
          ref="formByNewPlan"
          v-loading="dialogNewPlanLoading"
          :model="formByNewPlan"
          :rules="rulesByNewPlan"
          label-width="80px"
        >
          <el-form-item label="制单人">
            <span>{{ formByNewPlan.makerName }}</span>
          </el-form-item>
          <el-form-item label="制单日期">
            <span>{{ formByNewPlan.makeDate }}</span>
          </el-form-item>
          <el-form-item label="备注">
            <el-input v-model="formByNewPlan.remark" clearable style="max-width:500px" />
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="onNewPlan">
          新建
        </el-button>
        <el-button size="small" @click="dialogNewPlanIsVisible = false">
          关闭
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import Pagination from '@/components/Pagination'
import { getList, initNewPlan, newPlan } from '@/api/erpreplenishplan'
export default {
  name: 'StockReplenishPlanList',
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
        cumCode: ''
      },
      rulesByNewPlan: {

      },
      formByNewPlan: {
        cumCode: '',
        makerId: '',
        makerName: '',
        makeDate: '',
        remark: ''
      },
      dialogNewPlanIsVisible: false,
      dialogNewPlanLoading: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    this.onGetList()
  },
  methods: {
    onGetList() {
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
    onFilter() {
      this.onGetList()
    },
    onDialogNewPlanOpen() {
      this.formByNewPlan.cumCode = ''
      this.formByNewPlan.remark = ''
      this.dialogNewPlanIsVisible = true
      this.dialogNewPlanLoading = true
      initNewPlan({}).then(res => {
        this.dialogNewPlanLoading = false
        if (res.result === 1) {
          var d = res.data
          this.formByNewPlan.makerId = d.makerId
          this.formByNewPlan.makerName = d.makerName
          this.formByNewPlan.makeDate = d.makeDate
        }
      })
    },
    onNewPlan() {
      this.$refs['formByNewPlan'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要新建计划？', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            newPlan(this.formByNewPlan).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.dialogNewPlanIsVisible = false
                this.getListData(this.listQuery)
              } else {
                this.$message({
                  message: res.message,
                  type: 'error'
                })
              }
            })
          })
        }
      })
    },
    onSawDetail(row) {
      this.$router.push({
        name: 'MerchReplenishPlanDetail',
        path: '/stock/replenishplan/detail',
        params: {
          id: row.id
        }
      })
    }
  }
}
</script>
<style lang="scss" scoped>

.table-skus{
  margin-left: 20px;
  td{
    border: 0px  !important;
  }
}
</style>
