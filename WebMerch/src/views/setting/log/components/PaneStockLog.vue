<template>
  <div id="log_stock">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.skuName" clearable style="width: 100%" placeholder="商品名称" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
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
      <el-table-column label="商品名称" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.skuName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="销售渠道" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.sellChannelName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="时间" prop="createTime" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="事件" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.eventName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="变化数量" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.changeQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="备注" align="left" min-width="35%">
        <template slot-scope="scope">
          <span>{{ scope.row.remark }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" align="center" width="100" class-name="small-padding fixed-width">
        <template slot-scope="{row}">

          <el-button type="text" size="mini" @click="dialogRelStockOpen(row)">
            相关记录
          </el-button>

        </template>
      </el-table-column>>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <el-dialog title="相关记录" :visible.sync="dialogRelStockIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div v-loading="dialogRelStockLoading">

        <el-table
          :key=" dialogRelStockListKey"
          v-loading="dialogRelStockLoading"
          :data="dialogRelStockListData"
          fit
          highlight-current-row
          style="width: 100%;"
        >
          <el-table-column label="序号" prop="id" align="left" width="80">
            <template slot-scope="scope">
              <span>{{ scope.$index+1 }} </span>
            </template>
          </el-table-column>
          <el-table-column label="时间" prop="createTime" align="left" min-width="15%">
            <template slot-scope="scope">
              <span>{{ scope.row.createTime }}</span>
            </template>
          </el-table-column>
          <el-table-column label="备注" align="left" min-width="35%">
            <template slot-scope="scope">
              <span>{{ scope.row.remark }}</span>
            </template>
          </el-table-column>
        </el-table>

        <pagination v-show="dialogRelStockListTotal>0" :total="dialogRelStockListTotal" :page.sync="dialogRelStockListQuery.page" :limit.sync="dialogRelStockListQuery.limit" @pagination="getListDataByRelStock" />

      </div>
      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogRelStockIsVisible = false">
          关闭
        </el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>
import { getListByStock, getListByRelStock } from '@/api/log'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'LogPaneStockLog',
  components: { Pagination },
  data() {
    return {
      dialogRelStockIsVisible: false,
      dialogRelStockLoading: false,
      dialogRelStockListKey: 0,
      dialogRelStockListData: null,
      dialogRelStockListTotal: 0,
      dialogRelStockListQuery: {
        page: 1,
        limit: 10
      },
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        skuName: undefined
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
      getListByStock(this.listQuery).then(res => {
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
    getListDataByRelStock() {
      getListByRelStock(this.dialogRelStockListQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.dialogRelStockListData = d.items
          this.dialogRelStockListTotal = d.total
        }
        this.dialogRelStockLoading = false
      })
    },
    dialogRelStockOpen(row) {
      this.dialogRelStockIsVisible = true
      this.dialogRelStockLoading = true

      this.dialogRelStockListQuery.skuId = row.skuId
      this.dialogRelStockListQuery.storeId = row.storeId
      this.dialogRelStockListQuery.shopId = row.shopId
      this.dialogRelStockListQuery.deviceId = row.deviceId
      this.dialogRelStockListQuery.cabinetId = row.cabinetId
      this.dialogRelStockListQuery.slotId = row.slotId

      this.getListDataByRelStock()
    }
  }
}
</script>
