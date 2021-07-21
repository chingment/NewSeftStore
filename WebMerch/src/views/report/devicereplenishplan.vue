<template>
  <div id="report_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-select v-model="listQuery.planId" placeholder="计划单号" clearable style="width: 100%">
            <el-option
              v-for="item in optionsByPlan"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.shopName" clearable placeholder="门店" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.skuCumCode" clearable placeholder="商品编码" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.makerName" clearable placeholder="制单人" />
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
      <el-button style="position: absolute;right: 10px;top: 20px;" icon="el-icon-refresh" circle @click="handleFilter" />

    </div>

    <!-- <table id="abc" class="list-tb" cellspacing="0" cellpadding="0">
      <tr><td colspan="4">单号：test01 </td><td colspan="4">制单时间：2021/7/20 18:43:32 </td>
        <td colspan="4">制单人：东莞东华医院 </td></tr>
      <tr><td>店铺</td><td>商品编码</td><td>商品名称</td><td>商品规格</td><td>计划补货数</td><td>门店</td><td>总量</td><td>设备</td><td>数量</td><td>补货人</td><td>补数时间</td><td>补货量</td></tr>
      <tr><td rowspan="18">泰安医药</td><td rowspan="1">922031702</td><td rowspan="1">犬(动物)抓伤皮肤抗菌喷剂 液体,30ml</td><td rowspan="1">液体,30ml</td><td rowspan="1">0</td><td rowspan="1">东华医院总院一楼急诊室外</td><td>总量</td><td>D-DH-JZ-01-01</td><td>数量</td><td>补货人</td><td>补数时间</td><td>补货量</td></tr>
      <tr><td rowspan="1">22039679</td><td rowspan="1">安捷手消毒凝胶 500g</td><td rowspan="1">	 500g</td><td rowspan="1">0</td><td rowspan="1">东华医院总院一楼急诊室外</td><td>总量</td><td>D-DH-JZ-01-01</td><td>数量</td><td>补货人</td><td>补数时间</td><td>补货量</td></tr>
      <tr><td rowspan="2">22035610</td><td rowspan="2">创口贴（组合装） 无</td><td rowspan="2">无</td><td rowspan="2">0</td><td rowspan="2">东华医院总院一楼急诊室外</td><td>总量</td><td>D-DH-JZ-01-02</td><td>数量</td><td>补货人</td><td>补数时间</td><td>补货量</td></tr>
      <tr><td>总量</td><td>D-DH-JZ-01-01</td><td>数量</td><td>补货人</td><td>补数时间</td><td>补货量</td></tr>
      <tr>

        <td rowspan="1">922031038</td><td rowspan="1">赛乐洁皮肤黏膜消毒剂 60ml/瓶</td><td rowspan="1">60ml/瓶</td><td rowspan="1">0</td><td rowspan="1">东华医院总院一楼急诊室外</td><td>总量</td><td>D-DH-JZ-01-01</td><td>数量</td><td>补货人</td><td>补数时间</td><td>补货量</td>
      </tr>

    </table> -->

    <div v-html="report.html" />

    <!-- tr(v-for="item in ccxx")
    td(:rowspan="item.cclxspan" :style="{ display: item.cclxdis }") {{ item.cclx }}
    td(:rowspan="item.lsspan" :style="{ display: item.lsdis }") {{ item.ls }}
    td(:rowspan="item.ysspan" :style="{ display: item.ysdis }") {{ item.ys }}
    td {{ item.lhjgd }}
    td {{ item.lhjkd }}
    td {{ item.lhjpf }}
    td {{ item.ps }}
    td {{ item.bz }}
    td {{ item.blgd }}
    td {{ item.blkd }}
    td {{ item.blpf }} -->

    <!-- <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      border
      fit
      highlight-current-row
      style="width: 100%;"
      :span-method="spanMethod"
    >
      <el-table-column label="单据号" align="left" :width="isDesktop==true?220:80">
        <template slot-scope="scope">
          <span>{{ scope.row.planCumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="生成时间" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.buildTime }}</span>
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
      <el-table-column label="补货数" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.planQuantity }}</span>
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
      <el-table-column label="补货数量" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.rshQuantity }}</span>
        </template>
      </el-table-column>
    </el-table> -->
  </div>
</template>

<script>

import { deviceReplenishPlanInit, deviceReplenishPlanGet, checkRightExport } from '@/api/report'
import { parseTime } from '@/utils'
export default {
  name: 'ReportStoreStockRealData',
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '设备实时库存报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        planId: '',
        shopName: '',
        makerName: '',
        skuCumCode: ''
      },
      report: '',
      optionsByPlan: [],
      ccxx: [],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this._initData()
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
    _getData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      deviceReplenishPlanGet(this.listQuery).then(res => {
        if (res.result === 1) {
          this.report = res.data
          // this.combineCell(this.listData)
          // this.ccxx = list
          // if (this.listData === null || this.listData.length === 0) {
          // this.$message('查询不到对应条件的数据')
          // }
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
      if (this.listQuery.planId === null || this.listQuery.planId.length === 0) {
        this.$message('请选择计划单号')
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
      // if (this.listData === null || this.listData.length === 0) {
      //   this.$message('没有可导出的数据')
      //   return
      // }
      var filename = this.filename

      checkRightExport({ fileName: filename }).then(res => {
        if (res.result === 1) {
          this.downloadLoading = true
      import('@/vendor/Export2Excel').then(excel => {
        excel.export_table_to_excel('abc')
        this.downloadLoading = false
      })
        } else {
          this.$message({
            message: res.message,
            type: 'error'
          })
        }
      })
    },
    combineCell(list) {
      for (var field in list[0]) { // 获取数据中的字段，也就是table中的column，只需要取其中一条记录的就可以了
        // 定义数据list的index
        var k = 0
        while (k < list.length) {
          // 增加字段-用于统计有多少重复值
          list[k][field + 'span'] = 1
          // 增加字段-用于控制显示与隐藏
          list[k][field + 'dis'] = ''
          for (var i = k + 1; i <= list.length - 1; i++) {
            // 判断第k条数据的field字段，与下一条是否重复
            if (list[k][field] === list[i][field] && list[k][field] !== '') {
              // 如果重复，第k条数据的字段统计+1
              list[k][field + 'span']++
              // 设置为显示
              list[k][field + 'dis'] = ''
              // 重复的记录，则设置为1，表示不跨行
              list[i][field + 'span'] = 1
              // 并且该字段设置为隐藏
              list[i][field + 'dis'] = 'none'
            } else {
              break
            }
          }
          // 跳转到第i条数据的索引
          k = i
        }
      }
      this.ccxx = list
      console.log(list)
    },
    spanMethod({ row, column, rowIndex, columnIndex }) {
      if (columnIndex <= 2) {
        if (row[column.property] === '') {
          return [1, 1]
        }
        if (this[column.property] !== row[column.property]) {
          this[column.property] = row[column.property]
          let num = 0
          for (let i = rowIndex; i < this.list.length; i++) {
            if (this.list[i][column.property] === row[column.property]) {
              num++
              if (i === this.list.length - 1) {
                return [num, 1]
              }
            } else {
              return [num, 1]
            }
          }
        } else {
          return [0, 0]
        }
      }
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
