<template>
  <div id="shop_list">
    <div class="filter-container">

      <el-row :gutter="16">
        <el-col :span="8" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="门店" class="filter-item" />
        </el-col>
        <el-col :span="8" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilter">
            查询
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
      <el-table-column label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="门店" align="left" min-width="30%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="地址" align="left" min-width="70%">
        <template slot-scope="scope">
          <span>{{ scope.row.address }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="right" width="200" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button v-if="row.isCanSelect" type="text" size="mini" @click="onSelect(row)">
            选择
          </el-button>
          <el-button v-else type="text" disabled>{{ row.opTips }}</el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />
  </div>
</template>

<script>

import { getListBySbStore } from '@/api/shop'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ShopSelect',
  components: { Pagination },
  props: {
    storeId: {
      type: String,
      default: ''
    },
    selectMethod: {
      type: Function,
      default: null
    }
  },
  data() {
    return {
      loading: false,
      loadingByDetails: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        opCode: '',
        storeId: '',
        name: undefined
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {

  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.listQuery.opCode = this.opCode
    this.listQuery.storeId = this.storeId
    this.onGetList()
  },
  methods: {
    onGetList() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getListBySbStore(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    onFilter() {
      this.listQuery.page = 1
      this.onGetList()
    },
    onSelect(item) {
      if (this.selectMethod) {
        this.selectMethod(item)
      }
    }
  }
}
</script>
