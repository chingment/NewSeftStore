<template>
  <div id="pane_receive_record">
    <div class="filter-container">

      <el-row :gutter="16">
        <el-col :span="8" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.nickName" clearable style="width: 100%" placeholder="昵称" class="filter-item" />
        </el-col>
        <el-col :span="8" :xs="24" style="margin-bottom:20px">
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
      <el-table-column label="客户头像" align="left" min-width="15%">
        <template slot-scope="scope">
          <img :src="scope.row.avatar" style="width:30px;height:30px;">
        </template>
      </el-table-column>
      <el-table-column label="昵称" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.nickName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="领取方式" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.sourceTypeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="发送者" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.sourceObjName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="领取时间" align="left" min-width="25%">
        <template slot-scope="scope">
          <span>{{ scope.row.sourceTime }}</span>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>

import { getReceiveRecords } from '@/api/coupon'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'OperationCenterCouponPaneReceiveRecord',
  components: { Pagination },
  props: {
    couponId: {
      type: String,
      default: ''
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
        couponId: '',
        nickName: undefined
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {

  },
  created() {
    this.listQuery.couponId = this.couponId
    this.getListData()
  },
  methods: {
    getListData() {
      this.loading = true

      getReceiveRecords(this.listQuery).then(res => {
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
    handleSelect(item) {
      if (this.selectMethod) {
        this.selectMethod(item)
      }
    }
  }
}
</script>
