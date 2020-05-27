<template>
  <div id="productsku_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">

          <el-select v-model="listQuery.storeIds" multiple placeholder="选择店ss铺" style="width: 100%">
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
            v-model="listQuery.stockDate"
            type="date"
            value-format="yyyy-MM-dd"
            placeholder="选择日期"
            style="width: 100%"
            :picker-options="datePickerOptionsStockDate"
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
      <el-button style="position: absolute;right: 10px;top: 20px;" icon="el-icon-refresh" circle @click="_getData(listQuery)" />
    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column v-if="isDesktop" label="店铺" align="left" :width="isDesktop==true?220:80">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="销售渠道" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.machineName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="货道" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.slotId }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品名称" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品编码" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuCumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="商品规格" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuSpecDes }}</span>
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
    <div class="remark-tip" style="line-height: 42px;font-size:14px;"><span class="sign">*注</span>：实时统计机器库存报表</div>
  </div>
</template>

<script>

import { machineStockDateHisInit, machineStockDateHisGet } from '@/api/report'
import { parseTime } from '@/utils'
export default {
  name: 'MachineStock',
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '机器历史库存报表',
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
      machineStockDateHisInit().then(res => {
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
      machineStockDateHisGet(this.listQuery).then(res => {
        if (res.result === 1) {
          this.listData = res.data
        } else {
          this.$message(res.message)
        }
        this.loading = false
      })
    },
    handleFilter() {
      if (this.listQuery.storeIds.length === 0) {
        this.$message('请选择机器')
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
        const tHeader = ['店铺', '销售渠道', '货道', '商品名称', '商品编码', '商品规格', '可售数量', '锁定数量', '实际数量', '最大数量', '需补数量']
        const filterVal = ['storeName', 'machineName', 'slotId', 'productSkuName', 'productSkuCumCode', 'productSkuSpecDes', 'sellQuantity', 'lockQuantity', 'sumQuantity', 'maxQuantity', 'rshQuantity']
        const list = this.listData
        const data = this.formatJson(filterVal, list)
        excel.export_json_to_excel({
          header: tHeader,
          data,
          filename: this.filename + '(' + this.listQuery.stockDate + ')',
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
