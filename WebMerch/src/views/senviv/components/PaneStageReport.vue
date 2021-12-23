<template>
  <div id="senviv_report_list">

    <div class="filter-container">
      <el-form ref="form" label-width="120px" class="query-box">

        <el-form-item label="月份">

          <el-date-picker
            v-model="listQuery.healthDate"
            type="monthrange"
            range-separator="-"
            value-format="yyyy-MM"
            start-placeholder="开始月份"
            end-placeholder="结束月份"
            style="width: 300px"
          />

        </el-form-item>
        <el-form-item v-if="userId===''" label="搜索">
          <el-input v-model="listQuery.name" clearable style="max-width: 300px;" placeholder="昵称/姓名" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="onFilter">查询</el-button>
        </el-form-item>
      </el-form>
    </div>

    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >

      <el-table-column
        label="序号"
        fixed
        width="50"
        align="center"
      >
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>

      <el-table-column
        prop="healthDate"
        label="月份"
        width="120"
        align="center"
        fixed
      />
      <el-table-column
        v-if="userId===''"
        prop="signName"
        label="对象"
        width="120"
        align="center"
        fixed
      />

      <el-table-column
        v-if="userId===''"
        prop="sex"
        label="性别"
        width="60"
        align="center"
      />

      <el-table-column
        v-if="userId===''"
        prop="age"
        label="年龄"
        width="60"
        align="center"
      />

      <el-table-column
        prop="totalScore"
        align="center"
        label="总分"
        width="60"
      />
      <el-table-column label="免疫力" align="center">
        <el-table-column
          label="免疫力指数"
          width="120"
          align="center"
        >

          <template slot-scope="scope">
            <dv-item :value="scope.row.mylMylzs" sign />
          </template>

        </el-table-column>
        <el-table-column
          label="感染风险"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.mylGrfx" sign />
          </template>
        </el-table-column>
      </el-table-column>
      <el-table-column label="慢病管控" align="center">
        <el-table-column
          label="高血压"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.mbGxygk" sign />
          </template>

        </el-table-column>
        <el-table-column
          label="冠心病"
          width="120"
          align="center"
        >

          <template slot-scope="scope">

            <dv-item :value="scope.row.mbGxbgk" sign />

          </template>

        </el-table-column>
        <el-table-column
          label="糖尿病"
          width="120"
          align="center"
        >
          <template slot-scope="scope">

            <dv-item :value="scope.row.mbTlbgk" sign />

          </template>

        </el-table-column>
      </el-table-column>
      <el-table-column label="情绪心理" align="center">
        <el-table-column
          prop="qxxlJlqx"
          label="焦虑情绪"
          width="120"
          align="center"
        />
        <el-table-column
          label="抗压能力"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.qxxlKynl" sign />
          </template>
        </el-table-column>
        <el-table-column
          prop="qxxlQxyj"
          label="情绪应激"
          width="120"
          align="center"
        />
      </el-table-column>
      <el-table-column label="疾病风险" align="center">
        <el-table-column
          label="心律失常风险指数"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.jbfxXlscfx" sign />

          </template>
        </el-table-column>
        <el-table-column
          label="心率减速力"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.jbfxXljsl" sign />
          </template>

        </el-table-column>
      </el-table-column>
      <el-table-column label="心率变异性" align="center">
        <el-table-column
          prop="hrvXzznl"
          label="心脏总能量"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.hrvXzznl" sign />

          </template>
        </el-table-column>
        <el-table-column
          prop="hrvJgsjzlzs"
          label="交感神经张力指数"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.hrvJgsjzlzs" sign />
          </template>

        </el-table-column>
        <el-table-column
          prop="hrvMzsjzlzs"
          label="迷走神经张力指数"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.hrvMzsjzlzs" sign />
          </template>

        </el-table-column>
        <el-table-column
          prop="hrvZzsjzlzs"
          label="自主神经平衡"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.hrvZzsjzlzs" sign />

          </template>

        </el-table-column>
        <el-table-column
          prop="hrvHermzs"
          label="荷尔蒙指数"
          width="120"
          align="center"
        >

          <template slot-scope="scope">
            <dv-item :value="scope.row.hrvHermzs" sign />

          </template>
        </el-table-column>
        <el-table-column
          prop="hrvTwjxgsszh"
          label="体温及血管舒缩指数"
          width="120"
          align="center"
        >

          <template slot-scope="scope">
            <dv-item :value="scope.row.hrvTwjxgsszs" sign />

          </template>

        </el-table-column>
      </el-table-column>
      <el-table-column label="心率" align="center">
        <el-table-column
          prop="xlDcjzxl"
          label="当次基准心率"
          width="120"
          align="center"
        >

          <template slot-scope="scope">
            <dv-item :value="scope.row.xlDcjzxl" sign />

          </template>

        </el-table-column>
        <el-table-column
          prop="xlCqjzxl"
          label="长期基准心率"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.xlCqjzxl" sign />

          </template>

        </el-table-column>
        <el-table-column
          prop="xlDcpjxl"
          label="当次平均心率"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.xlDcpjxl" sign />

          </template>

        </el-table-column>
      </el-table-column>
      <el-table-column label="呼吸暂停" align="center">
        <el-table-column
          prop="hxZtahizs"
          label="AHI指数"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.hxZtahizs" sign />

          </template>
        </el-table-column>
        <el-table-column
          prop="hxZtcs"
          label="呼吸暂停次数"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.hxZtcs" sign />

          </template>
        </el-table-column>
      </el-table-column>
      <el-table-column label="呼吸" align="center">
        <el-table-column
          prop="hxDcjzhx"
          label="基准呼吸"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.hxDcjzhx" sign />

          </template>
        </el-table-column>
        <el-table-column
          prop="hxCqjzhx"
          label="长期基准呼吸"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.hxCqjzhx" sign />

          </template>

        </el-table-column>
        <el-table-column
          prop="hxDcpjhx"
          label="平均呼吸"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.hxDcpjhx" sign />

          </template>
        </el-table-column>
      </el-table-column>
      <el-table-column label="睡眠结构" align="center">
        <el-table-column
          prop="smSdsmsc"
          label="深睡期时长"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.smSdsmsc" sign />

          </template>
        </el-table-column>
        <el-table-column
          prop="smQdsmsc"
          label="浅睡期时长"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.smQdsmsc" sign />
          </template>
        </el-table-column>
        <el-table-column
          prop="smRemsmsc"
          label="REM期时长"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.smRemsmsc" sign />

          </template>
        </el-table-column>
        <el-table-column
          prop="smTdcs"
          label="体动次数"
          width="120"
          align="center"
        >
          <template slot-scope="scope">
            <dv-item :value="scope.row.smTdcs" sign />

          </template>
        </el-table-column>
      </el-table-column>

      <el-table-column
        fixed="right"
        prop="status"
        label="状态"
        width="120"
        align="center"
      >
        <template slot-scope="scope">
          {{ scope.row.status.text }}
        </template>
      </el-table-column>

      <el-table-column label="操作" align="center" width="120" fixed="right" class-name="small-padding fixed-width">
        <template slot-scope="scope">

          <el-link v-if="scope.row.isSend" type="primary" size="mini" @click="onOpenDialogByDetial(scope.row)">查看</el-link>
          <el-link v-else type="warning" size="mini" @click="onOpenDialogByDetial(scope.row)">评价 </el-link>

        </template>
      </el-table-column>

    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />

    <pane-stage-report-detail v-if="dialogIsShowByReportDetail" :visible.sync="dialogIsShowByReportDetail" work-type="health_sug" :report-id="selectReportId" @aftersave="onAfterSaveMonthReportSug" />

  </div>
</template>

<script>
import { getStageReports } from '@/api/senviv'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import PaneStageReportDetail from './PaneStageReportDetail.vue'
import DvItem from './DvItem.vue'
export default {
  name: 'ClientUserList',
  components: { Pagination, PaneStageReportDetail, DvItem },
  props: {
    userId: {
      type: String,
      default: ''
    },
    rptType: {
      type: String,
      default: ''
    },
    cacheQuery: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        healthDate: undefined,
        name: undefined,
        userId: undefined
      },
      datePickerHealthDate: {
        disabledDate(time) {
          return time.getTime() > Date.now()
        }
      },
      selectReportId: '',
      selectUserId: '',
      dialogIsShowByReportDetail: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.cacheQuery) {
      if (this.$store.getters.listPageQuery.has(this.$route.path)) {
        this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
      }
    }
    this.listQuery.userId = this.userId
    this.listQuery.rptType = this.rptType
    this.onGetList()
  },
  methods: {
    onGetList() {
      this.loading = true
      if (this.cacheQuery) {
        this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      }
      getStageReports(this.listQuery).then(res => {
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
    onOpenDialogByDetial(row) {
      this.selectReportId = row.id
      this.dialogIsShowByReportDetail = true
    },
    onOpenDialogByClient(row) {
      this.selectUserId = row.svUserId
      this.dialogIsShowByClientDetail = true
    },
    onAfterSaveMonthReportSug() {
      this.onGetList()
    }
  }
}
</script>

