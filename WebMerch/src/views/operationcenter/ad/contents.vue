<template>
  <div id="adspace_release_list">
    <page-header />
    <div>
      <el-button type="primary" size="mini" @click="onRelease">
        发布
      </el-button>
    </div>
    <div class="row-title clearfix">
      <div class="pull-left"> <h5>版位信息</h5>
      </div>
      <div class="pull-right" />
    </div>
    <el-form class="form-container" style="display:flex">
      <el-col :span="24">
        <el-row>
          <el-col :span="12">
            <el-form-item label-width="80px" label="名称">

              <span>{{ adSpace.name }}</span>

            </el-form-item>
          </el-col>
          <el-col :span="12" />
        </el-row>
      </el-col>
    </el-form>
    <div class="row-title clearfix">
      <div class="pull-left"> <h5>发布记录</h5>
      </div>
      <div class="pull-right" />
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
      <el-table-column label="文件" prop="imgUrl" align="left" width="120">
        <template slot-scope="scope">

          <img v-if="scope.row.url.indexOf('png')>-1||scope.row.url.indexOf('jpg')>-1" :src="scope.row.url" style="width:80px;height:80px;">

          <video
            v-else-if="scope.row.url.indexOf('mp4')>-1"
            :src="scope.row.url"
            width="100"
            height="100"
            class="el-upload-list__item-thumbnail"
            controls="controls"
          />

        </template>
      </el-table-column>
      <el-table-column label="标题" prop="title" align="left" min-width="40%">
        <template slot-scope="scope">
          <span>{{ scope.row.title }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="status" align="left" min-width="15%">
        <template slot-scope="scope">
          <el-tag :type="getStatusColor(scope.row.status.value)">{{ scope.row.status.text }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="发布时间" prop="createTime" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="right" width="300" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button v-if="row.status.value==1" type="text" size="mini" @click="onSetContentStatus(row)">
            停止
          </el-button>
          <el-button v-if="row.status.value==2" type="text" size="mini" @click="onSetContentStatus(row)">
            恢复
          </el-button>
          <el-button type="text" size="mini" @click="onBelong(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initContents, getContents, setContentStatus } from '@/api/ad'
import { getUrlParam } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
import Pagination from '@/components/Pagination'
export default {
  name: 'OperationCenterAdContents',
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
      adSpace: {
        name: ''
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    this.listQuery.adSpaceId = getUrlParam('id')

    initContents({ adSpaceId: this.listQuery.adSpaceId }).then(res => {
      if (res.result === 1) {
        var d = res.data
        this.adSpace.name = d.adSpaceName
      }
    })
    this.onGetList()
  },
  methods: {
    onGetList() {
      this.loading = true
      getContents(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    onRelease() {
      this.$router.push({
        path: '/operationcenter/ad/release?id=' + this.listQuery.adSpaceId
      })
    },
    onSetContentStatus(item) {
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
        setContentStatus({ id: item.id, status: status }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.onGetList()
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }
        })
      }).catch(() => {
      })
    },
    onBelong(item) {
      this.$router.push({
        path: '/operationcenter/ad/content/belongs?id=' + item.id
      })
    },
    getStatusColor(status) {
      switch (status) {
        case 1:
          return 'success'
        case 2:
          return 'danger'
        case 3:
          return ''
        case 4:
        case 5:
          return ''
      }
      return ''
    }
  }
}
</script>
