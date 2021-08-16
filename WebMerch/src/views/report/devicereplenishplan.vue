<template>
  <div id="report_list">
    <div class="filter-container">
      <el-form ref="form" label-width="120px" class="query-box">
        <el-form-item v-show="planIdIsHidden" label="计划单号">
          <el-input v-model="listQuery.planCumCode" clearable style="max-width: 300px;" />
        </el-form-item>
        <el-form-item label="门店">
          <el-input v-model="listQuery.shopName" clearable style="max-width: 300px;" />
        </el-form-item>
        <el-form-item label="商品编码">
          <el-input v-model="listQuery.skuCumCode" clearable style="max-width: 300px;" />
        </el-form-item>
        <el-form-item v-show="planIdIsHidden" label="制单人">
          <el-input v-model="listQuery.makerName" clearable style="max-width: 300px;" />
        </el-form-item>
        <el-form-item v-show="planIdIsHidden" label="制单日期">
          <el-date-picker
            v-model="listQuery.makeDateArea"
            type="daterange"
            range-separator="-"
            value-format="yyyy-MM-dd"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            style="max-width: 300px;"
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
      border
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="计划单号" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.planCumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="制单时间" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.makeTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="制单人" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.makerName }}</span>
        </template>
      </el-table-column>
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
          <span>{{ scope.row.deviceCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="机柜" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.cabinetName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="货道" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.slotName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品编码" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.skuCumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品名称" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.skuName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品规格" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.skuSpecDes }}</span>
        </template>
      </el-table-column>
      <el-table-column label="计划补货数" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.planRshQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="实际补货数" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.realRshQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="补货人" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.rsherName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="补货时间" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.rshTime }}</span>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>

import { deviceReplenishPlanInit, deviceReplenishPlanGet, checkRightExport } from '@/api/report'
import { parseTime } from '@/utils'

export default {
  name: 'ReportSkuSalesDateHis',
  props: {
    planId: {
      type: String,
      default: undefined
    }
  },
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '设备补货计划报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        planId: '',
        planCumCode: '',
        shopName: '',
        makerName: '',
        skuCumCode: '',
        makeDateArea: ['', '']
      },
      planIdIsHidden: true,
      optionsByPlan: [],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  watch: {
    planId: function(val, oldval) {
      console.log('this.planId3:' + val)
      // this._getData()
    }
  },
  created() {
    console.log('this.planId2:' + this.planId)
    if (this.planId !== undefined) {
      this.planIdIsHidden = false
      this.listQuery.planId = this.planId
      this._initData()
      this._getData()
    }
  },
  methods: {
    _initData() {
      deviceReplenishPlanInit().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.optionsByPlan = d.optionsByPlan
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
      deviceReplenishPlanGet(this.listQuery).then(res => {
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
      // if (this.planId === undefined) {
      //   if (this.listQuery.planCumCode === '') {
      //     this.$message('至少输入计划单号')
      //     return
      //   }
      // }

      this._getData()
    },
    handleDownload() {
      if (this.listData === null || this.listData.length === 0) {
        this.$message('没有可导出的数据')
        return
      }
      var filename = this.filename

      checkRightExport({ fileName: filename }).then(res => {
        if (res.result === 1) {
          this.downloadLoading = true
      import('@/vendor/Export2Excel').then(excel => {
        const tHeader = ['计划单号', '制单人', '制单时间', '店铺', '门店', '设备', '机柜', '货道', '商品编码', '商品名称', '商品规格', '计划补货数', '实际补货数', '补货人', '补货时间']
        const filterVal = ['planCumCode', 'makeTime', 'makerName', 'storeName', 'shopName', 'deviceCode', 'cabinetName', 'slotName', 'skuCumCode', 'skuName', 'skuSpecDes', 'planRshQuantity', 'realRshQuantity', 'rsherName', 'rshTime']
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

 .transition-box {

    height: 300px;

  }
</style>
