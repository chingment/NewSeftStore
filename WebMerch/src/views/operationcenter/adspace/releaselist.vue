<template>
  <div id="adspace_release_list">
    <page-header />

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
      <el-table-column v-if="isDesktop" label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="文件" prop="imgUrl" align="left" width="120">
        <template slot-scope="scope">
          <img :src="scope.row.url" style="width:80px;height:80px;">
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="标题" prop="title" align="left" min-width="40%">
        <template slot-scope="scope">
          <span>{{ scope.row.title }}</span>
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
      <el-table-column label="操作" align="center" width="300" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="handleOpenDialogByBelong(row)">
            涉及对象
          </el-button>
          <el-button type="primary" size="mini" @click="handleOpenDialogByCopy(row)">
            同步至...
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
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>内容信息</h5>
          </div>
          <div class="pull-right" />
        </div>
        <el-form class="form-container">
          <el-form-item label-width="80px" label="所在版位">
            <span>{{ adContent.adSpaceName }}</span>
          </el-form-item>
          <el-form-item label-width="80px" label="标题">
            <span>{{ adContent.title }}</span>
          </el-form-item>
          <el-form-item label-width="80px" label="文件">
            <img :src="adContent.url" style="width:80px;height:80px;">
          </el-form-item>
        </el-form>
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>涉及对象</h5>
          </div>
          <div class="pull-right" />
        </div>
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
        <pagination v-show="listTotalByBelongs>0" :total="listTotalByBelongs" :page.sync="listQueryByBelongs.page" :limit.sync="listQueryByBelongs.limit" @pagination="getListDataByBelong" />
      </div>
    </el-dialog>

    <el-dialog v-if="dialogByCopyIsVisible" :title="'同步至...'" width="800px" :visible.sync="dialogByCopyIsVisible">
      <div style="width:100%;height:600px">
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>内容信息</h5>
          </div>
          <div class="pull-right" />
        </div>
        <el-form class="form-container">
          <el-form-item label-width="80px" label="所在版位">
            <span>{{ adContent.adSpaceName }}</span>
          </el-form-item>
          <el-form-item label-width="80px" label="标题">
            <span>{{ adContent.title }}</span>
          </el-form-item>
          <el-form-item label-width="80px" label="文件">
            <img :src="adContent.url" style="width:80px;height:80px;">
          </el-form-item>
        </el-form>
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>选择对象</h5>
          </div>
          <div class="pull-right" />
        </div>
        <el-form ref="form" v-loading="loading" :model="formByCopy" label-width="80px">

          <el-form-item label="对象">
            <el-checkbox v-model="belongsCheckAll" :indeterminate="belongsIsIndeterminate" @change="handleBelongsCheckAllChange">全选</el-checkbox>
            <div style="margin: 15px 0;" />
            <el-checkbox-group v-model="formByCopy.belongIds" @change="handleBelongsCheckedChange">
              <el-checkbox v-for="(belong,index) in belongs" :key="index" :label="belong.id">{{ belong.name }}</el-checkbox>
            </el-checkbox-group>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="handleCopyAdContent2Belongs">
          同步
        </el-button>
        <el-button @click="dialogByCopyIsVisible = false">
          关闭
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initRelease, getAdContents, deleteAdContent, getAdContentBelongs, setAdContentBelongStatus, copyAdContent2Belongs } from '@/api/adspace'
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
      adSpace: {
        name: ''
      },
      adContent: {
        adSpaceName: '',
        url: ''
      },
      formByCopy: {
        adContentId: '',
        belongIds: []
      },
      belongs: [],
      belongsCheckAll: false,
      belongsIsIndeterminate: true,
      dialogByBelongsIsVisible: false,
      dialogByCopyIsVisible: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.listQuery.adSpaceId = getUrlParam('id')

    initRelease({ id: this.listQuery.adSpaceId }).then(res => {
      if (res.result === 1) {
        var d = res.data
        this.adSpace.name = d.adSpaceName
        this.belongs = d.belongs
      }
    })

    this.getListData()
  },
  methods: {
    getListData() {
      this.loading = true
      getAdContents(this.listQuery).then(res => {
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
      this.adContent.url = item.url
      this.adContent.title = item.title
      this.adContent.adSpaceName = item.adSpaceName
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
    },
    handleOpenDialogByCopy(item) {
      this.formByCopy.adContentId = item.id
      this.belongsCheckAll = false
      this.belongsIsIndeterminate = false
      this.formByCopy.belongIds = []
      this.adContent.url = item.url
      this.adContent.title = item.title
      this.adContent.adSpaceName = item.adSpaceName
      this.dialogByCopyIsVisible = true
    },
    handleBelongsCheckAllChange(val) {
      var belongsChecked = []
      for (var i = 0; i < this.belongs.length; i++) {
        belongsChecked.push(this.belongs[i].id)
      }
      this.formByCopy.belongIds = val ? belongsChecked : []
      this.belongsIsIndeterminate = false
    },
    handleBelongsCheckedChange(value) {
      const checkedCount = value.length
      this.belongsCheckAll = checkedCount === this.belongs.length
      this.belongsIsIndeterminate = checkedCount > 0 && checkedCount < this.belongs.length
    },
    handleCopyAdContent2Belongs() {
      MessageBox.confirm('确定同步', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        copyAdContent2Belongs(this.formByCopy).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.dialogByCopyIsVisible = false
          }
        })
      })
    }
  }
}
</script>
