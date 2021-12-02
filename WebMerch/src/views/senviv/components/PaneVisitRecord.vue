<template>
  <div id="shop_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" @click="onOpenDialogVisitByTelePhone">
            电话回访
          </el-button>
          <el-button class="filter-item" type="primary">
            公众号告知
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
      <el-table-column label="方式" prop="visitType" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.visitType }}</span>
        </template>
      </el-table-column>
      <el-table-column label="时间" prop="visitTime" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.visitTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="内容" prop="content" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.content }}</span>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" prop="createTime" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="onDetails(row)">
            查看
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />
    <dialog-visit-by-telephone v-if="isVisibleDialogVisitByTelephone" :user-id="userId" :visible.sync="isVisibleDialogVisitByTelephone" @aftersave="onAfterSaveDialogVisitByTelephone" />
  </div>
</template>

<script>
import { getVisitRecords } from '@/api/senviv'
import Pagination from '@/components/Pagination'
import DialogVisitByTelephone from './DialogVisitByTelephone'

export default {
  name: 'PaneVisitRecord',
  components: { Pagination, DialogVisitByTelephone },
  props: {
    userId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      isVisibleDialogVisitByTelephone: false,
      listQuery: {
        page: 1,
        limit: 10,
        userId: undefined
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.onGetList()
  },
  methods: {
    onGetList() {
      this.loading = true
      this.listQuery.userId = this.userId
      getVisitRecords(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    onOpenDialogVisitByTelePhone() {
      this.isVisibleDialogVisitByTelephone = true
    },
    onAfterSaveDialogVisitByTelephone() {
      this.onGetList()
    },
    onDetails(item) {
      this.$router.push({
        path: '/client/shop/details?id=' + item.id
      })
    }
  }
}
</script>
