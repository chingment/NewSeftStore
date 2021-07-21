<template>
  <div id="report_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col v-show="planIdIsHidden" :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
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
import FileSaver from 'file-saver'
import XLSX from 'xlsx'
export default {
  name: 'ReportStoreStockRealData',
  components: {
    FileSaver,
    XLSX
  },
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
        shopName: '',
        makerName: '',
        skuCumCode: ''
      },
      planIdIsHidden: true,
      report: '',
      optionsByPlan: [],
      ccxx: [],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  watch: {
    planId: function(val, oldval) {
      // this._getData()
    }
  },
  created() {
    console.log('this.planId2:' + this.planId)
    if (this.planId !== undefined) {
      this.planIdIsHidden = false
    }
    this.listQuery.planId = this.planId
    this._initData()
    this._getData()
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
      var _this = this
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      deviceReplenishPlanGet(this.listQuery).then(res => {
        if (res.result === 1) {
          this.report = res.data

          // setTimeout(function() { _this.Controlwidth() }, 200)
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

          // 从表生成工作簿对象
          var wb = XLSX.utils.table_to_book(document.getElementById('abc'))
          // 得到二进制字符串作为输出
          var wbout = XLSX.write(wb, {
            bookType: 'xlsx',
            bookSST: false,
            type: 'binary',
            cellStyles: true
          })
          FileSaver.saveAs(new Blob([this.s2ab(wbout)], {
            type: 'application/octet-stream'
          }), filename + '.xlsx')

          this.downloadLoading = false
        } else {
          this.$message({
            message: res.message,
            type: 'error'
          })
        }
      })
    },
    s2ab: function(s) {
      var cuf
      var i
      if (typeof ArrayBuffer !== 'undefined') {
        cuf = new ArrayBuffer(s.length)
        var view = new Uint8Array(cuf)
        for (i = 0; i !== s.length; i++) {
          view[i] = s.charCodeAt(i) & 0xFF
        }
        return cuf
      } else {
        cuf = new Array(s.length)
        for (i = 0; i !== s.length; ++i) {
          cuf[i] = s.charCodeAt(i) & 0xFF
        }
        return cuf
      }
    },
    Controlwidth() {
      var tTD // 用来存储当前更改宽度的Table Cell,避免快速移动鼠标的问题

      var table = document.getElementById('abc')

      // console.log(table.outerHTML)

      var row = table.rows[1]
      console.log('cell:' + row.cells.length)
      for (let j = 0; j < row.cells.length; j++) {
        table.rows[1].cells[j].onmousedown = function() {
          console.log('width:' + table.rows[1].cells[j].width)

          // 记录单元格

          tTD = this

          if (event.offsetX > tTD.offsetWidth - 5) {
            tTD.mouseDown = true

            tTD.oldX = event.x

            tTD.oldWidth = tTD.offsetWidth
          }

          // 记录Table宽度

          // table = tTD; while (table.tagName != ‘TABLE') table = table.parentElement;

          // tTD.tableWidth = table.offsetWidth;
        }

        table.rows[1].cells[j].onmouseup = function() {
          // 结束宽度调整

          tTD.mouseDown = false
          tTD.style.cursor = 'default'
        }

        table.rows[1].cells[j].onmousemove = function() {
          // 更改鼠标样式

          if (event.offsetX > this.offsetWidth - 5) { this.style.cursor = 'col-resize' } else { this.style.cursor = 'default' }

          // 取出暂存的Table Cell

          if (tTD === undefined) tTD = this

          if (!tTD.mouseDown) return
          // 调整宽度

          if (tTD.mouseDown != null && tTD.mouseDown === true) {
            tTD.style.cursor = 'default'

            if (tTD.oldWidth + (event.x - tTD.oldX) > 0) { tTD.width = tTD.oldWidth + (event.x - tTD.oldX) }

            // 调整列宽

            tTD.style.width = tTD.width

            tTD.style.cursor = 'col-resize'

            // 调整该列中的每个Cell

            table = tTD

            // console.log(tTD.width)

            while (table.tagName !== 'TABLE') table = table.parentElement

            // console.log()

            table.rows[1].cells[j].style.width = tTD.width + 'px'

            // border = "1px solid #000"

            // console.log(table.rows[0].cells[j].style.width)

            // console.log()

            // for (let j = 0; j < table.rows.length; j++) {
            // // console.log(tTD.width)

            // }

            // 调整整个表

            // table.width = tTD.tableWidth + (tTD.offsetWidth – tTD.oldWidth);

            // table.style.width = table.width;
          }
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
