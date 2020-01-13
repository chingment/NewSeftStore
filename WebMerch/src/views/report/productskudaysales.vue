<template>
  <div id="productsku_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-select v-model="listQuery.storeId" clearable placeholder="选择店铺" style="width: 100%">
            <el-option
              v-for="item in options_stores"
              :key="item.id"
              :label="item.name"
              :value="item.id"
            />
          </el-select>
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">

 <el-date-picker
      v-model="listQuery.tradeDateTimeArea"
      type="datetimerange"
      range-separator="至"
      value-format="yyyy-MM-dd HH:mm:ss"
      start-placeholder="开始日期"
      end-placeholder="结束日期">
    </el-date-picker>
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
          <span>{{ scope.row.sellChannelRefName }}</span>
        </template>
      </el-table-column>
      <el-table-column  label="订单号"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.orderSn }}</span>
        </template>
      </el-table-column>
      <el-table-column  label="交易时间"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.tradeTime }}</span>
        </template>
      </el-table-column>
      <el-table-column  label="商品名称" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品编码" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuBarCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品规格"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuSpecDes }}</span>
        </template>
      </el-table-column>
      <el-table-column label="单价"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.salePrice }}</span>
        </template>
      </el-table-column>
      <el-table-column label="数量"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.quantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="总金额"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.tradeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付方式"  align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.payWay }}</span>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { productSkuDaySalesInit,productSkuDaySalesGet } from '@/api/report'

export default {
  name: 'OrderList',
  props: {
  },
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '商品销售报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        machineId: undefined,
        tradeDateTimeArea:[],
      },
      options_stores: [],
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
     // var dateNow= new Date()

     // this.listQuery.tradeDateTimeArea=[dateNow,dateNow]         
      productSkuDaySalesInit().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.options_stores = d.stores
        }
        this.loading = false
      })

    },
    _productSkuDaySalesGet() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      productSkuDaySalesGet(this.listQuery).then(res => {

        this.listData = res.data
        if (res.result === 1) {
          //this.listData = res.data
        }
        else{
            this.$message(res.message)
        }
        this.loading = false
      })
    },
    handleFilter() {
    //   if(this.listQuery.machineId==null||this.listQuery.machineId==undefined||this.listQuery.machineId.length==0){
    //       this.$message('请选择机器')
    //       return 
    //   }
      this._productSkuDaySalesGet()
    },
    handleDownload() {
      this.downloadLoading = true
      import('@/vendor/Export2Excel').then(excel => {
        const tHeader = ['店铺', '机器', '订单号', '交易时间','商品名称', '商品编码','商品规格','单价','数量','总金额','支付方式']
        const filterVal = ['storeName', 'sellChannelRefName', 'orderSn','tradeTime', 'productSkuName', 'productSkuBarCode','productSkuSpecDes','salePrice','quantity','tradeAmount','payWay']
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
