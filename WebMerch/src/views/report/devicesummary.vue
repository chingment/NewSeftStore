<template>
  <div id="report_list">

    <div class="filter-container">
      <el-form ref="form" label-width="120px" class="query-box">
        <el-form-item label="交易日期" style="max-width: 400px;">
          <el-date-picker
            v-model="listQuery.tradeDateTimeArea"
            type="daterange"
            range-separator="-"
            value-format="yyyy-MM-dd"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            style="width: 100%"
          />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <el-button :loading="downloadLoading" style="margin-left: 10px;" type="primary" icon="el-icon-document" @click="handleDownload">
            导出
          </el-button>
        </el-form-item>
      </el-form>
    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      show-summary
      border
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="店铺" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="门店" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.shopName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="设备" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.deviceCumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="销售总额" align="left" min-width="10%" prop="sumChargeAmount">
        <template slot-scope="scope">
          <span>{{ scope.row.sumChargeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="订单量" align="left" min-width="10%" prop="sumCount">
        <template slot-scope="scope">
          <span>{{ scope.row.sumCount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="客单价" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.perCumPrice }}</span>
        </template>
      </el-table-column>
      <el-table-column label="交易日期" align="客单价" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.salesDate }}</span>
        </template>
      </el-table-column>
    </el-table>
    <el-alert
      title="提示：以设备单位维度来统计销售报表， 不统计测试模式"
      type="remark-gray"
      :closable="false"
    />

  </div>
</template>

<script>

import { deviceSummaryInit, deviceSummaryGet, checkRightExport } from '@/api/report'
import { parseTime } from '@/utils'
export default {
  name: 'ReportDeviceSummary',
  props: {
  },
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '设备概况报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        tradeDateTimeArea: []
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this._initData()
  },
  methods: {
    _initData() {
      deviceSummaryInit().then(res => {
        if (res.result === 1) {
          var d = res.data
        }
        this.loading = false
      })
    },
    formatJson(filterVal, jsonData) {
      return jsonData.map(v => filterVal.map(j => {
        if (j === 'timestamp') {
          return parseTime(v[j])
        } else {
          return v[j]
        }
      }))
    },
    _getData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      deviceSummaryGet(this.listQuery).then(res => {
        this.listData = res.data == null ? [] : res.data
        if (res.result === 1) {
          // this.listData = res.data
          if (this.listData === null || this.listData.length === 0) {
            this.$message('查询不到对应条件的数据')
          }
        } else {
          this.$message({
            message: res.message,
            type: 'error'
          })
        }
        this.loading = false
      })
    },
    handleFilter() {
      if (this.listQuery.tradeDateTimeArea.length === 0) {
        this.$message('请选择日期范围')
        return
      }
      this._getData()
    },
    handleDownload() {
      if (this.listData === null || this.listData.length === 0) {
        this.$message('没有可导出的数据')
        return
      }
      var filename = this.filename

      if (this.listQuery.tradeDateTimeArea[0] === this.listQuery.tradeDateTimeArea[1]) {
        filename = filename + '(' + this.listQuery.tradeDateTimeArea[0] + ')'
      } else {
        filename = filename + '(' + this.listQuery.tradeDateTimeArea[0] + '~' + this.listQuery.tradeDateTimeArea[1] + ')'
      }

      checkRightExport({ fileName: filename }).then(res => {
        if (res.result === 1) {
          this.downloadLoading = true
      import('@/vendor/Export2Excel').then(excel => {
        const tHeader = ['店铺', '门第', '设备', '销售总额', '订单量', '客单价', '交易日期']
        const filterVal = ['storeName', 'shopName', 'deviceCumCode', 'sumChargeAmount', 'sumCount', 'perCumPrice', 'salesDate']
        const list = this.listData
        const data = this.formatJson(filterVal, list)

        excel.export_json_to_excel({
          header: tHeader,
          data,
          filename: filename,
          autoWidth: this.autoWidth,
          bookType: this.bookType
        })
        this.downloadLoading = false
      })
        } else {
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
