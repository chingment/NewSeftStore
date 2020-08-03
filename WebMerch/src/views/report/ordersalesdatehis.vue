<template>
  <div id="productsku_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-select v-model="listQuery.storeIds" multiple placeholder="选择店铺" style="width: 100%">
            <el-option
              v-for="item in optionsStores"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
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
        <el-col :span="3" :xs="24" style="margin-bottom:20px">
          <el-select v-model="listQuery.receiveMode" clearable placeholder="全部提货方式">
            <el-option
              v-for="item in optionsReceiveModes"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
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
      <el-table-column v-if="isDesktop" label="提货方式" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.receiveModeName }}</span>
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
      title="提示：以订单单位维度来统计销售报表"
      type="remark-gray"
      :closable="false"
    />
  </div>
</template>

<script>

import { orderSalesDateHisInit, orderSalesDateHisGet } from '@/api/report'
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
        storeIds: [],
        tradeDateTimeArea: [],
        receiveMode: undefined
      },
      optionsStores: [],
      optionsReceiveModes: [{
        value: '3',
        label: '机器自提'
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
      orderSalesDateHisInit().then(res => {
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
      orderSalesDateHisGet(this.listQuery).then(res => {
        this.listData = res.data == null ? [] : res.data
        if (res.result === 1) {
          if (this.listData === null || this.listData.length === 0) {
            this.$message('查询不到对应条件的数据')
          }
        } else {
          this.$message(res.message)
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
      this.downloadLoading = true
      import('@/vendor/Export2Excel').then(excel => {
        const tHeader = ['店铺', '提货方式', '订单号', '交易时间', '数量', '总金额', '支付方式']
        const filterVal = ['storeName', 'receiveModeName', 'orderId', 'tradeTime', 'quantity', 'tradeAmount', 'payWay']
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
