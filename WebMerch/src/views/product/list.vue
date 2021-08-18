<template>
  <div id="product_list">
    <div class="filter-container">
      <el-row :gutter="24">
        <el-col :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">

          <el-autocomplete
            v-model="listQuery.key"
            :fetch-suggestions="handleSpuSrh"
            placeholder="商品名称/编码/条形码/首拼音母"
            clearable
            style="width: 100%"
            @select="handleSpuSel"
            @keyup.enter.native="handleFilter"
            @clear="handleFilter"
          />

        </el-col>
        <el-col :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">
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
      <el-table-column label="序号" prop="id" fixed="left" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="图片" prop="mainImgUrl" fixed="left" align="center" width="110">
        <template slot-scope="scope">
          <img :src="scope.row.mainImgUrl" style="width:80px;height:80px;">
        </template>
      </el-table-column>
      <el-table-column label="名称" align="left" min-width="100%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="货号" width="180">
        <template slot-scope="scope">
          <span>{{ scope.row.spuCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="条形码" align="left" width="180">
        <template slot-scope="scope">
          <span>{{ scope.row.skus[0].barCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="分类" align="left" width="200">
        <template slot-scope="scope">
          <span>{{ scope.row.kindNames }}</span>
        </template>
      </el-table-column>
      <el-table-column label="默认销售价" align="left" width="110">
        <template slot-scope="{row}">
          {{ row.skus[0].salePrice }}

        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="180" fixed="right" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="handleUpdate(row)">
            编辑
          </el-button>
          <el-button type="text" size="mini" @click="handleDelete(row)">
            加入回收站
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" background :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getList, searchSku, del } from '@/api/product'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ProductList',
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
        isDelete: false,
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
        path: '/product/add'
      })
    },
    handleUpdate(row) {
      this.$router.push({
        path: '/product/edit?id=' + row.id
      })
    },
    handleSpuSrh(queryString, cb) {
      searchSku({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({ 'value': d[j].name, 'mainImgUrl': d[j].mainImgUrl, 'name': d[j].name, 'spuId': d[j].spuId })
          }

          cb(restaurants)
        }
      })
    },
    handleSpuSel(item) {
      this.listQuery.key = item.name
      getList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleDelete(row) {
      MessageBox.confirm('确定要删除', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        del({ id: row.id }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.getListData()
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }
        })
      }).catch(() => {
      })
    }
  }
}
</script>
