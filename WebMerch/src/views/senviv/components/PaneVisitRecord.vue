<template>
  <div id="shop_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" @click="onOpenDialogVisitByTelePhone">
            电话回访
          </el-button>
          <el-button class="filter-item" type="primary" @click="onOpenDialogVisitByPapush">
            公众号告知
          </el-button>
        </el-col>
      </el-row>

    </div>

    <el-timeline>

      <el-timeline-item
        v-for="(record, index) in listData"
        :key="index"
        :timestamp="record.visitTime"
        placement="top"
      >

        <el-card class="box-card" style="width:600px">
          <div slot="header" class="clearfix">
            <span>{{ record.visitType }}</span>
          </div>
          <div v-for="item in record.visitContent" :key="item" class="text item" style=" margin-bottom: 18px;font-size: 14px;">
            {{ item.key +' ' + item.value }}
          </div>
          <p>{{ record.operater }} 提交于 {{ record.visitTime }}</p>
        </el-card>

      </el-timeline-item>

    </el-timeline>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />
    <dialog-visit-by-telephone v-if="isVisibleDialogVisitByTelephone" :user-id="userId" :visible.sync="isVisibleDialogVisitByTelephone" @aftersave="onAfterSaveDialogVisitByTelephone" />
    <dialog-visit-by-papush v-if="isVisibleDialogVisitByPapush" :user-id="userId" :visible.sync="isVisibleDialogVisitByPapush" @aftersave="onAfterSaveDialogVisitByPapush" />

  </div>
</template>

<script>
import { getVisitRecords } from '@/api/senviv'
import Pagination from '@/components/Pagination'
import DialogVisitByTelephone from './DialogVisitByTelephone'
import DialogVisitByPapush from './DialogVisitByPapush'
export default {
  name: 'PaneVisitRecord',
  components: { Pagination, DialogVisitByTelephone, DialogVisitByPapush },
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
      isVisibleDialogVisitByPapush: false,
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
    onOpenDialogVisitByPapush() {
      this.isVisibleDialogVisitByPapush = true
    },
    onAfterSaveDialogVisitByPapush() {
      this.onGetList()
    }
  }
}
</script>
