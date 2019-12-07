<template>
  <div id="productsku_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" style="width: 100%" placeholder="商品名称" va class="filter-item" @keyup.enter.native="handleFilter" />
        </el-col>
        <el-col :span="8" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
          <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">
            新建
          </el-button>
        </el-col>
      </el-row>
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
      <el-table-column label="名称" prop="name" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="分类" prop="kindNames" align="left" min-width="30%">
        <template slot-scope="scope">
          <span>{{ scope.row.kindNames }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="默认销售价" prop="skus" align="left" min-width="30%">
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
import { getList } from '@/api/prdproduct'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ProductSkuList',
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
        name: undefined
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
    }
  }
}
</script>
