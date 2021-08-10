<template>
  <div id="report_list">

    <div class="filter-container">
      <el-form ref="form" label-width="120px">
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
        <el-form-item label="提货方式">
          <el-radio-group v-model="listQuery.receiveMode">
            <el-radio-button v-for="item in options_ReceiveModes" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
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
      :data="listData"
      border
      show-summary
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column v-if="isDesktop" label="店铺" align="left" :width="isDesktop==true?220:80">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="提货方式" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.receiveModeName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="方式备注" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.receiveRemark }}</span>
        </template>
      </el-table-column>
      <el-table-column label="订单号" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.orderId }}</span>
        </template>
      </el-table-column>
      <el-table-column label="交易时间" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.tradeTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="数量" align="left" prop="quantity" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.quantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="总金额" align="left" prop="tradeAmount" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.tradeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付方式" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.payWay }}</span>
        </template>
      </el-table-column>
    </el-table>
    <el-alert
      title="提示：以订单单位维度来统计销售报表， 不统计测试模式"
      type="remark-gray"
      :closable="false"
    />
  </div>
</template>

<script>

import { orderSalesHisInit, orderSalesHisGet, checkRightExport } from '@/api/report'
import { parseTime } from '@/utils'
export default {
  name: 'ReportOrderSalesDateHis',
  props: {
  },
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '订单销售报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        storeIds: [],
        tradeDateTimeArea: [],
        receiveMode: '0'
      },
      optionsStores: [],
      options_ReceiveModes: [{
        value: '0',
        label: '全部'
      }, {
        value: '4',
        label: '设备自提'
      }, {
        value: '2',
        label: '店铺自取'
      }, {
        value: '1',
        label: '配送商品'
      }],
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
      orderSalesHisInit().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.optionsStores = d.optionsStores
        }
        this.loading = false
      })
    },
    _getData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      orderSalesHisGet(this.listQuery).then(res => {
        this.listData = res.data == null ? [] : res.data
        if (res.result === 1) {
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
        const tHeader = ['店铺', '提货方式', '方式备注', '订单号', '交易时间', '数量', '总金额', '支付方式']
        const filterVal = ['storeName', 'receiveModeName', 'receiveRemark', 'orderId', 'tradeTime', 'quantity', 'tradeAmount', 'payWay']
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
