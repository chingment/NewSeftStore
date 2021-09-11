<template>
  <div id="report_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-date-picker
            v-model="listQuery.tradeDateTimeArea"
            type="daterange"
            range-separator="-"
            value-format="yyyy-MM-dd"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            style="width: 100%"
          />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
          <el-button :loading="downloadLoading" style="margin-left: 10px;" type="primary" icon="el-icon-document" @click="handleDownload">
            导出
          </el-button>
        </el-col>
      </el-row>
    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      border
      show-summary
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="店铺" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="总订单数" align="left" prop="sumCount">
        <template slot-scope="scope">
          <span>{{ scope.row.sumCount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="设备自提" align="left" prop="sumReceiveMode3">
        <template slot-scope="scope">
          <span>{{ scope.row.sumReceiveMode3 }}</span>
        </template>
      </el-table-column>
      <el-table-column label="店铺自取" align="left" prop="sumReceiveMode2">
        <template slot-scope="scope">
          <span>{{ scope.row.sumReceiveMode2 }}</span>
        </template>
      </el-table-column>
      <el-table-column label="配送商品" align="left" prop="sumReceiveMode1">
        <template slot-scope="scope">
          <span>{{ scope.row.sumReceiveMode1 }}</span>
        </template>
      </el-table-column>
      <el-table-column label="已完成" align="left" prop="sumComplete">
        <template slot-scope="scope">
          <span>{{ scope.row.sumComplete }}</span>
        </template>
      </el-table-column>
      <el-table-column label="未完成" align="left" prop="sumNoComplete">
        <template slot-scope="scope">
          <span>{{ scope.row.sumNoComplete }}</span>
        </template>
      </el-table-column>
      <el-table-column label="异常" align="left" prop="sumEx">
        <template slot-scope="scope">
          <span>{{ scope.row.sumEx }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品数量" align="left" prop="sumQuantity">
        <template slot-scope="scope">
          <span>{{ scope.row.sumQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付金额" align="left" prop="sumChargeAmount">
        <template slot-scope="scope">
          <span>{{ scope.row.sumChargeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="退款数量" align="left" prop="sumRefundedQuantity">
        <template slot-scope="scope">
          <span>{{ scope.row.sumRefundedQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="退款金额" align="left" prop="sumRefundedAmount">
        <template slot-scope="scope">
          <span>{{ scope.row.sumRefundedAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="结算数量" align="left" prop="sumTradeQuantity">
        <template slot-scope="scope">
          <span>{{ scope.row.sumTradeQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="结算金额" align="left" prop="sumTradeAmount">
        <template slot-scope="scope">
          <span>{{ scope.row.sumTradeAmount }}</span>
        </template>
      </el-table-column>
    </el-table>
    <el-alert
      title="提示：以店铺单位维度来统计销售报表， 不统计测试模式"
      type="remark-gray"
      :closable="false"
    />
  </div>
</template>

<script>

import { storeSummaryInit, storeSummaryGet, checkRightExport } from '@/api/report'
import { parseTime } from '@/utils'
export default {
  name: 'ReportStoreSummary',
  props: {
  },
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '店铺概况报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: [],
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
      storeSummaryInit().then(res => {
        if (res.result === 1) {
          var d = res.data
        }
        this.loading = false
      })
    },
    _getData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      storeSummaryGet(this.listQuery).then(res => {
        this.listData = res.data

        if (res.result === 1) {
          if (this.listData === null || this.listData.length === 0) {
            this.$message('查询不到对应条件的数据')
          }

          // this.listData = res.data
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
    formatJson(filterVal, jsonData) {
      return jsonData.map(v => filterVal.map(j => {
        if (j === 'timestamp') {
          return parseTime(v[j])
        } else {
          return v[j]
        }
      }))
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
        const tHeader = ['店铺', '总订单数', '设备自提', '店铺自取', '配送商品', '已完成', '未完成', '异常', '商品数量', '支付金额', '退款金额', '合计金额']
        const filterVal = ['storeName', 'sumCount', 'sumReceiveMode3', 'sumReceiveMode2', 'sumReceiveMode1', 'sumNoComplete', 'sumNoComplete', 'sumEx', 'sumQuantity', 'sumChargeAmount', 'sumRefundAmount', 'sumAmount']
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
