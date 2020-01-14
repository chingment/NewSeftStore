<template>
  <div id="productsku_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-select v-model="listQuery.machineId" clearable placeholder="选择机器" style="width: 100%">
            <el-option
              v-for="item in options_machines"
              :key="item.id"
              :label="'['+item.storeName+']'+item.name"
              :value="item.id"
            />
          </el-select>
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
         <el-button :loading="downloadLoading" style="margin:0 0 20px 20px;" type="primary" icon="el-icon-document" @click="handleDownload">
         导出
        </el-button>
        </el-col>
      </el-row>
      <el-button style="position: absolute;right: 10px;top: 20px;" icon="el-icon-refresh" circle @click="getListData(listQuery)" />
    </div>
     <FilenameOption v-model="filename" />
      <AutoWidthOption v-model="autoWidth" />
      <BookTypeOption v-model="bookType" />
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="店铺"  align="left" :width="isDesktop==true?220:80">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="机器" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.machineName }}</span>
        </template>
      </el-table-column>
      <el-table-column  label="货道"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.slotId }}</span>
        </template>
      </el-table-column>
      <el-table-column  label="商品名称" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品编码" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuCumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品规格"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuSpecDes }}</span>
        </template>
      </el-table-column>
      <el-table-column label="可售数量"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sellQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="锁定数量"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.lockQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="实际数量"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sumQuantity }}</span>
        </template>
      </el-table-column>
            <el-table-column label="最大数量"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.maxQuantity }}</span>
        </template>
      </el-table-column>
            <el-table-column label="需补数量"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.rshQuantity }}</span>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { machineStockInit,machineStockGet } from '@/api/report'

export default {
  name: 'OrderList',
  props: {
  },
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '机器实时库存报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        machineId: undefined
      },
      options_machines: [],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.init()
  },
  methods: {
    init() {
      machineStockInit().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.options_machines = d.machines
        }
        this.loading = false
      })

    },
    _machineStockGet() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      machineStockGet(this.listQuery).then(res => {
        if (res.result === 1) {
          this.listData = res.data
        }
        else{
            this.$message(res.message)
        }
        this.loading = false
      })
    },
    handleFilter() {
      if(this.listQuery.machineId==null||this.listQuery.machineId==undefined||this.listQuery.machineId.length==0){
          this.$message('请选择机器')
          return 
      }
      this._machineStockGet()
    },
    handleDownload() {
      this.downloadLoading = true
      import('@/vendor/Export2Excel').then(excel => {
        const tHeader = ['店铺', '机器', '货道', '商品名称', '商品编码','商品规格','可售数量','锁定数量','实际数量','最大数量','需补数量']
        const filterVal = ['storeName', 'machineName', 'slotId', 'productSkuName', 'productSkuCumCode','productSkuSpecDes','sellQuantity','lockQuantity','sumQuantity','maxQuantity','rshQuantity']
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
    },
    formatJson(filterVal, jsonData) {
      return jsonData.map(v => filterVal.map(j => {
        if (j === 'timestamp') {
          return parseTime(v[j])
        } else {
          return v[j]
        }
      }))
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
