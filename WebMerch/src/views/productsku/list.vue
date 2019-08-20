<template>
  <div class="app-container">
    <div class="filter-container">
      <el-input v-model="listQuery.name" placeholder="名称" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="handleFilter">
        查询
      </el-button>
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">
        新建
      </el-button>
    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      border
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
      <el-table-column v-if="isDesktop" label="分类" prop="kindNames" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.kindNames }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="栏目" prop="subjectNames" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.subjectNames }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="销售价" prop="salePrice" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.salePrice }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="展示价" prop="showPrice" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.showPrice }}</span>
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
import { getProductSkuList } from '@/api/productsku'
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
      getProductSkuList(this.listQuery).then(res => {
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
        path: '/productsku/add'
      })
    },
    handleUpdate(row) {
      this.$router.push({
        path: '/productsku/edit?id=' + row.id
      })
    }
  }
}
</script>
