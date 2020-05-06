<template>
  <div id="productsku_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-cascader
            v-model="listQuery.sellChannels"
            :props="optionsSellChannelsProps"
            :options="optionsSellChannels"
            placeholder="选择销售渠道"
            clearable
            style="width: 100%"
          />
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
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
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
          <el-button :loading="downloadLoading" style="margin-left: 10px;" type="primary" icon="el-icon-document" @click="handleDownload">
            导出
          </el-button>
        </el-col>
      </el-row>
      <el-button style="position: absolute;right: 10px;top: 20px;" icon="el-icon-refresh" circle @click="getListData(listQuery)" />
    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
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
      <el-table-column v-if="isDesktop" label="总订单数" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumCount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="已完成" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumComplete }}</span>
        </template>
      </el-table-column>
      <el-table-column label="未完成" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumNoComplete }}</span>
        </template>
      </el-table-column>
      <el-table-column label="异常" align="left" prop="quantity" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumEx }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品数量" align="left" prop="tradeAmount" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付金额" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumChargeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="退款金额" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumRefundAmount }}</span>
        </template>
      </el-table-column>
     <el-table-column label="合计金额" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumAmount }}</span>
        </template>
      </el-table-column>
    </el-table>

    <div class="remark-tip" style="line-height: 42px;font-size:14px;"><span class="sign">*注</span>：以订单单位维度来统计销售报表</div>
  </div>
</template>

<script>

import { storeSalesDateHisInit, storeSalesDateHisGet } from '@/api/report'
import { parseTime } from '@/utils'
export default {
  name: 'Order',
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
        sellChannels: [],
        tradeDateTimeArea: []
      },
      optionsSellChannels: [],
      optionsSellChannelsProps: { multiple: true, checkStrictly: false },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this._init()
  },
  methods: {
    _init() {
      storeSalesDateHisInit().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.optionsSellChannels = d.optionsSellChannels
        }
        this.loading = false
      })
    },
    _get() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      storeSalesDateHisGet(this.listQuery).then(res => {
        this.listData = res.data
        if (res.result === 1) {
          // this.listData = res.data
        } else {
          this.$message(res.message)
        }
        this.loading = false
      })
    },
    handleFilter() {
      this._get()
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
      this.downloadLoading = true
      import('@/vendor/Export2Excel').then(excel => {
        const tHeader = ['店铺', '销售渠道', '订单号', '交易时间', '数量', '总金额', '支付方式']
        const filterVal = ['storeName', 'sellChannelRefNames', 'orderId', 'tradeTime', 'quantity', 'tradeAmount', 'payWay']
        const list = this.listData
        const data = this.formatJson(filterVal, list)
        excel.export_json_to_excel({
          header: tHeader,
          data,
          filename: this.filename,
          autoWidth: this.autoWidth,
          bookType: this.bookType
        })
        this.downloadLoading = false
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
