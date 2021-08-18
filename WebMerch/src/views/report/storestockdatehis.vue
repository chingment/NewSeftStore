<template>
  <div id="report_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">

          <el-select v-model="listQuery.storeIds" multiple placeholder="选择店铺" style="width: 100%">
            <el-option
              v-for="item in optionsStores"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-col>

        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-select v-model="listQuery.shopMode" clearable placeholder="销售模式" style="width: 100%">
            <el-option
              v-for="item in optionsShopModes"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-date-picker
            v-model="listQuery.stockDate"
            type="date"
            value-format="yyyy-MM-dd"
            placeholder="选择日期"
            style="width: 100%"
            :picker-options="datePickerOptionsStockDate"
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
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="店铺" align="left" width="220">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="销售模式" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sellChannelRefName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="模式备注" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sellChannelRemark }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品名称" align="left" min-width="10%">
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
    </el-table>
    <el-alert
      title="提示：统计店铺历史库存报表"
      type="remark-gray"
      :closable="false"
    />
  </div>
</template>

<script>

import { storeStockDateHisInit, storeStockDateHisGet, checkRightExport } from '@/api/report'
import { parseTime } from '@/utils'
export default {
  name: 'ReportStoreStockDateHis',
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '店铺历史库存报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: [],
      listTotal: 0,
      listQuery: {
        storeIds: [],
        stockDate: undefined
      },
      optionsStores: [],
      optionsShopModes: [{
        value: '1',
        label: '线上商城'
      }, {
        value: '3',
        label: '线下设备'
      }],
      datePickerOptionsStockDate: {
        disabledDate(time) {
          return time.getTime() > Date.now()
        }
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
      storeStockDateHisInit().then(res => {
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
      storeStockDateHisGet(this.listQuery).then(res => {
        if (res.result === 1) {
          this.listData = res.data

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
      if (this.listQuery.storeIds.length === 0) {
        this.$message('请选择店铺')
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
      var filename = this.filename + '(' + this.listQuery.stockDate + ')'

      checkRightExport({ fileName: filename }).then(res => {
        if (res.result === 1) {
          this.downloadLoading = true
      import('@/vendor/Export2Excel').then(excel => {
        const tHeader = ['店铺', '销售模式', '模式备注', '商品名称', '商品编码', '商品规格', '可售数量', '锁定数量', '实际数量', '最大数量', '需补数量']
        const filterVal = ['storeName', 'sellChannelRefName', 'sellChannelRemark', 'skuName', 'skuCumCode', 'skuSpecDes', 'sellQuantity', 'lockQuantity', 'sumQuantity', 'maxQuantity', 'rshQuantity']
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
