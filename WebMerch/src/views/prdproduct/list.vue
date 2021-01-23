<template>
  <div id="product_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">

          <el-autocomplete
            v-model="listQuery.key"
            :fetch-suggestions="handleProductSrh"
            placeholder="商品名称/编码/条形码/首拼音母"
            clearable
            style="width: 100%"
            @select="handleProductSel"
            @keyup.enter.native="handleFilter"
            @clear="handleFilter"
          />

          <!-- <el-input v-model="listQuery.key" clearable style="width: 100%" placeholder="商品名称/编码/条形码/首拼音" va class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" /> -->
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
          <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">
            新建
          </el-button>
        </el-col>
      </el-row>
      <el-button style="position: absolute;right: 10px;top: 20px;" icon="el-icon-refresh" circle @click="handleFilter" />

    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column v-if="isDesktop" label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="图片" prop="mainImgUrl" align="center" width="110">
        <template slot-scope="scope">
          <img :src="scope.row.mainImgUrl" style="width:80px;height:80px;">
        </template>
      </el-table-column>
      <el-table-column label="名称" align="left" min-width="30%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="货号" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.spuCode }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="条形码" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.skus[0].barCode }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="分类" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.kindNames }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="默认销售价" align="left" min-width="10%">
        <template slot-scope="{row}">
          <el-button type="text" @click="handleSalePrice(row)">{{ row.skus[0].salePrice }}</el-button>

        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="handleUpdate(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { getList, searchSku } from '@/api/prdproduct'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'PrdProductList',
  components: { Pagination },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        key: ''
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.getListData()
  },
  methods: {
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
    },
    handleCreate() {
      this.$router.push({
        path: '/prdproduct/add'
      })
    },
    handleUpdate(row) {
      this.$router.push({
        path: '/prdproduct/manage?id=' + row.id + '&tab=tabBaseInfo'
      })
    },
    handleSalePrice(row) {
      this.$router.push({
        path: '/prdproduct/manage?id=' + row.id + '&tab=tabStoreSale'
      })
    },
    handleProductSrh(queryString, cb) {
      console.log('queryString:' + queryString)
      searchSku({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({ 'value': d[j].name, 'mainImgUrl': d[j].mainImgUrl, 'name': d[j].name, 'productId': d[j].productId })
          }

          cb(restaurants)
        }
      })
    },
    handleProductSel(item) {
      this.listQuery.key = item.name
      getList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    }
  }
}
</script>
