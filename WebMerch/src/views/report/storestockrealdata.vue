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
          <el-select v-model="listQuery.sellChannelRefType" clearable placeholder="销售模式" style="width: 100%">
            <el-option
              v-for="item in optionsSellChannelRefTypes"
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
      <el-button style="position: absolute;right: 10px;top: 20px;" icon="el-icon-refresh" circle @click="_ge(listQuery)" />
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
      <el-table-column label="销售模式" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sellChannelRefName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="模式备注" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sellChannelRemark }}</span>
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
    <el-alert
      title="提示：实时统计店铺库存报表"
      type="remark-gray"
      :closable="false"
    />
  </div>
</template>

<script>

import { storeStockRealDataInit, storeStockRealDataGet } from '@/api/report'
import { parseTime } from '@/utils'
export default {
  name: 'MachineStock',
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '店铺实时库存报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        storeIds: [],
        sellChannelRefType: undefined
      },
      optionsStores: [],
      optionsSellChannelRefTypes: [{
        value: '1',
        label: '线上商城'
      }, {
        value: '3',
        label: '线下机器'
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
      storeStockRealDataInit().then(res => {
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
      storeStockRealDataGet(this.listQuery).then(res => {
        if (res.result === 1) {
          this.listData = res.data

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
      this.downloadLoading = true
      import('@/vendor/Export2Excel').then(excel => {
        const tHeader = ['店铺', '销售模式', '模式备注', '商品名称', '商品编码', '商品规格', '可售数量', '锁定数量', '实际数量', '最大数量', '需补数量']
        const filterVal = ['storeName', 'sellChannelRefName', 'sellChannelRefRemark', 'productSkuName', 'productSkuCumCode', 'productSkuSpecDes', 'sellQuantity', 'lockQuantity', 'sumQuantity', 'maxQuantity', 'rshQuantity']
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
