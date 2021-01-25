<template>
  <div id="adspace_release_list">
    <page-header />
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
      <el-table-column label="图片" prop="imgUrl" align="left" width="100">
        <template slot-scope="scope">
          <img :src="scope.row.url" style="width:80px;height:80px;">
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="标题" prop="title" align="left" min-width="40%">
        <template slot-scope="scope">
          <span>{{ scope.row.title }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="版位" prop="title" align="left" min-width="30%">
        <template slot-scope="scope">
          <span>{{ scope.row.adSpaceName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="status" align="left" min-width="15%">
        <template slot-scope="scope">
          <span :class="'enable-status enable-status-'+scope.row.status.value">{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="发布时间" prop="createTime" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="200" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="handleOpenDialogByBelong(row)">
            涉及对象
          </el-button>
          <el-button type="primary" size="mini" @click="handleDelete(row)">
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <el-dialog v-if="dialogByBelongsIsVisible" :title="'涉及对象'" width="800px" :visible.sync="dialogByBelongsIsVisible">
      <div style="width:100%;height:600px">

        <el-table
          :key="listKey"
          v-loading="loadingByBelongs"
          :data="listDataByBelongs"
          fit
          highlight-current-row
          style="width: 100%;"
        >
          <el-table-column v-if="isDesktop" label="序号" prop="id" align="left" width="80">
            <template slot-scope="scope">
              <span>{{ scope.$index+1 }} </span>
            </template>
          </el-table-column>
          <el-table-column v-if="isDesktop" label="对象" prop="title" align="left" min-width="70%">
            <template slot-scope="scope">
              <span>{{ scope.row.belongName }}</span>
            </template>
          </el-table-column>
          <el-table-column v-if="isDesktop" label="版位" prop="title" align="left" min-width="30%">
            <template slot-scope="scope">
              <span>{{ scope.row.adSpaceName }}</span>
            </template>
          </el-table-column>
          <el-table-column label="状态" prop="status" align="left" min-width="15%">
            <template slot-scope="scope">
              <span :class="'enable-status enable-status-'+scope.row.status.value">{{ scope.row.status.text }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" width="200" class-name="small-padding fixed-width">
            <template slot-scope="{row}">
              <el-button v-if="row.status.value==1" type="primary" size="mini" @click="handleSetBelongStatus(row)">
                停用
              </el-button>
              <el-button v-if="row.status.value==2" type="primary" size="mini" @click="handleSetBelongStatus(row)">
                恢复
              </el-button>
            </template>
          </el-table-column>
        </el-table>

      </div>
    </el-dialog>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getReleaseList, deleteAdContent, getAdContentBelongs, setAdContentBelongStatus } from '@/api/adspace'
import { getUrlParam } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
import Pagination from '@/components/Pagination'
export default {
  name: 'OperationCenterAdspaceReleaseList',
  components: {
    PageHeader, Pagination
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
        adSpaceId: 0
      },
      loadingByBelongs: false,
      listDataByBelongs: null,
      listTotalByBelongs: 0,
      listQueryByBelongs: {
        page: 1,
        limit: 10,
        adSpaceId: 0,
        adContentId: ''
      },
      dialogByBelongsIsVisible: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    this.listQuery.adSpaceId = getUrlParam('id')
    this.getListData()
  },
  methods: {
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getReleaseList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    getListDataByBelong(listQuery) {
      this.loadingByBelongs = true
      getAdContentBelongs(listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listDataByBelongs = d.items
          this.listTotalByBelongs = d.total
        }
        this.loadingByBelongs = false
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
    },
    handleDelete(row) {
      MessageBox.confirm('确定要删除', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        deleteAdContent({ id: row.id }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.getListData()
          }
        })
      }).catch(() => {
      })
    },
    handleOpenDialogByBelong(item) {
      this.dialogByBelongsIsVisible = true

      this.listQueryByBelongs.adContentId = item.id
      this.getListDataByBelong(this.listQueryByBelongs)
    },
    handleSetBelongStatus(item) {
      var status = 0
      var tip = ''
      if (item.status.value === 1) {
        tip = '确定要停用？'
        status = 2
      } else if (item.status.value === 2) {
        tip = '确定要恢复？'
        status = 1
      }
      MessageBox.confirm(tip, '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        setAdContentBelongStatus({ id: item.id, status: status }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.getListDataByBelong(this.listQueryByBelongs)
          }
        })
      }).catch(() => {
      })
    }
  }
}
</script>
