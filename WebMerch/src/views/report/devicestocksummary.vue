<template>
  <div id="report_list">

    <div class="filter-container">
      <el-form ref="form" label-width="120px" class="query-box">
        <el-form-item label="店铺" style="max-width: 600px;">
          <el-select v-model="listQuery.storeIds" multiple placeholder="选择" style="width: 100%">
            <el-option
              v-for="item in optionsStores"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="交易日期" style="max-width: 600px;">

          <el-date-picker
            v-model="listQuery.tradeDateArea"
            type="daterange"
            value-format="yyyy-MM-dd"
            :picker-options="pickerOptions"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            align="right"
            style="max-width: 400px;"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="onFilter">查询</el-button>
          <el-button :loading="downloadLoading" style="margin-left: 10px;" type="primary" icon="el-icon-document" @click="onDownload">
            导出
          </el-button>
        </el-form-item>
      </el-form>
    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      border
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="店铺" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品名称" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.skuName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品编码" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.skuCumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品规格" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.skuSpecDes }}</span>
        </template>
      </el-table-column>
      <el-table-column label="可售数量" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sellQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="锁定数量" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.lockQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="实际数量" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="最大数量" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.maxQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="需补数量" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.rshQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="已销售数量" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.saleQuantity }}</span>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />
  </div>
</template>

<script>
import fileDownload from 'js-file-download'
import { deviceStockSummaryInit, deviceStockSummaryGet, deviceStockSummaryExport, checkRightExport } from '@/api/report'
import Pagination from '@/components/Pagination'
export default {
  name: 'ReportOrderSalesDateHis',
  components: { Pagination },
  props: {
  },
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '设备实时总表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        storeIds: [],
        tradeDateArea: []
      },
      optionsStores: [],
      pickerOptions: {
        shortcuts: [{
          text: '最近一周',
          onClick(picker) {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 7)
            picker.$emit('pick', [start, end])
          }
        }, {
          text: '最近一个月',
          onClick(picker) {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 30)
            picker.$emit('pick', [start, end])
          }
        }, {
          text: '最近三个月',
          onClick(picker) {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 90)
            picker.$emit('pick', [start, end])
          }
        }]
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      deviceStockSummaryInit().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.optionsStores = d.optionsStores
        }
        this.loading = false
      })
    },
    onGetList() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      deviceStockSummaryGet(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        } else {
          this.$message({
            message: res.message,
            type: 'error'
          })
        }
        this.loading = false
      })
    },
    onFilter() {
      if (this.listQuery.tradeDateArea.length === 0) {
        this.$message('请选择日期范围')
        return
      }
      this.onGetList()
    },
    onDownload() {
      if (this.listQuery.tradeDateArea.length === 0) {
        this.$message('请选择日期范围')
        return
      }

      var filename = this.filename

      if (this.listQuery.tradeDateArea[0] === this.listQuery.tradeDateArea[1]) {
        filename = filename + '(' + this.listQuery.tradeDateArea[0] + ')'
      } else {
        filename = filename + '(' + this.listQuery.tradeDateArea[0] + '~' + this.listQuery.tradeDateArea[1] + ')'
      }

      this.downloadLoading = true
      checkRightExport({ fileName: filename }).then(res => {
        if (res.result === 1) {
          const data = this.listQuery
          deviceStockSummaryExport(data).then(res => {
            fileDownload(res.data, filename + '.xls')
            this.downloadLoading = false
          })
        } else {
          this.downloadLoading = false
          this.$message({
            message: res.message,
            type: 'error'
          })
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
