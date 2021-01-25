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
      <el-table-column label="操作" align="right" width="300" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
              <el-button v-if="row.status.value==1" type="primary" size="mini" @click="handleSetContenttatus(row)">
                停止
              </el-button>
              <el-button v-if="row.status.value==2" type="primary" size="mini" @click="handleSetContenttatus(row)">
                恢复
              </el-button>
          <el-button type="primary" size="mini" @click="handleOpenDialogByBelong(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <el-dialog v-if="dialogByBelongsIsVisible" :title="'编辑'" width="800px" :visible.sync="dialogByBelongsIsVisible">
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
          <div class="pull-right" >

          <el-button v-if="adContent.status.value==1" type="primary" size="mini" @click="handleOpenDialogByCopy(false,null)" style="margin-top:-20px;margin-right:10px">
            添加
          </el-button>

          </div>
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
          <el-table-column v-if="isDesktop" label="对象" prop="title" align="left" min-width="40%">
            <template slot-scope="scope">
              <span>{{ scope.row.belongName }}</span>
            </template>
          </el-table-column>
          <el-table-column v-if="isDesktop" label="有效期" prop="title" align="left" min-width="45%">
            <template slot-scope="scope">
              <span>{{ scope.row.validStartTime }}</span>~<span>{{ scope.row.validEndTime }}</span>
            </template>
          </el-table-column>
          <el-table-column label="状态" prop="status" align="left" min-width="15%">
            <template slot-scope="scope">
              <span :class="'enable-status enable-status-'+scope.row.status.value">{{ scope.row.status.text }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="right" width="200" class-name="small-padding fixed-width">
            <template slot-scope="{row}">
              <el-button v-if="row.status.value==1" type="primary" size="mini" @click="handleSetBelongStatus(row)">
                停止
              </el-button>
              <el-button v-if="row.status.value==2" type="primary" size="mini" @click="handleSetBelongStatus(row)">
                恢复
              </el-button>          
              
          <el-button v-if="row.status.value==1||row.status.value==2" type="primary" size="mini" @click="handleOpenDialogByCopy(true,row)" >
            编辑
          </el-button>

         <span v-if="row.status.value==3">
            已停用
          </span>


            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="listTotalByBelongs>0" :total="listTotalByBelongs" :page.sync="listQueryByBelongs.page" :limit.sync="listQueryByBelongs.limit" @pagination="getListDataByBelong" />
      </div>
    </el-dialog>

    <el-dialog v-if="dialogByCopyIsVisible" :title="'编辑'" width="600px" :visible.sync="dialogByCopyIsVisible" append-to-body>
      <div style="width:100%;height:400px">
        <el-form ref="form" v-loading="loading" :model="formByCopy" label-width="80px">
                <el-form-item label="有效期" prop="validDate">
        <el-date-picker
          v-model="formByCopy.validDate"
          type="daterange"
          range-separator="至"
          start-placeholder="开始日期"
          end-placeholder="结束日期"
          value-format="yyyy-MM-dd"
          style="width: 380px"
        />
      </el-form-item>
      <el-form-item label="对象">
<div  v-show="formByCopy.isEdit">
   {{formByCopy.belongName}}
</div>

          <div v-show="!formByCopy.isEdit">
            <el-checkbox v-model="belongsCheckAll" :indeterminate="belongsIsIndeterminate" @change="handleBelongsCheckAllChange">全选</el-checkbox>
            <div style="margin: 15px 0;" />
            <el-checkbox-group v-model="formByCopy.belongIds" @change="handleBelongsCheckedChange">
              <el-checkbox v-for="(belong,index) in belongs" :key="index" :label="belong.id">{{ belong.name }}</el-checkbox>
            </el-checkbox-group>
         </div>

          </el-form-item>
        </el-form>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="handleCopyAdContent2Belongs">
          确定
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
import { initRelease, getContents, setContentStatus, getContentBelongs, setContentBelongStatus, editContentBelong,addContentBelong } from '@/api/ad'
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
        url: '',
        status:{value:2}
      },
      formByCopy: {
        isEdit:false,
        adContentId: '',
        belongName:'',
        belongIds: [],
        validDate:[]
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
    this.formByCopy.adSpaceId=this.listQuery.adSpaceId
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
      getContents(this.listQuery).then(res => {
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
      getContentBelongs(listQuery).then(res => {
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
    handleSetContenttatus(item) {

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
        setContentStatus({ id: item.id ,status:status}).then(res => {
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
      this.formByCopy.adContentId=item.id
      this.adContent.url = item.url
      this.adContent.title = item.title
      this.adContent.adSpaceName = item.adSpaceName
         this.adContent.status=item.status
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
        setContentBelongStatus({ id: item.id, status: status }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.getListDataByBelong(this.listQueryByBelongs)
          }
        })
      }).catch(() => {
      })
    },
    handleOpenDialogByCopy(isEdit,item) {
      this.belongsCheckAll = false
      this.belongsIsIndeterminate = false
      this.dialogByCopyIsVisible = true
      if(isEdit){
        this.formByCopy.isEdit=true
        this.formByCopy.belongIds=[item.belongId]
        this.formByCopy.belongName=item.belongName
        this.formByCopy.validDate=[item.validStartTime,item.validEndTime]
      }
      else{
        this.formByCopy.isEdit=false
        this.formByCopy.validDate=[]
      }

      console.log(this.formByCopy.isEdit)
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
      MessageBox.confirm('确定保存', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {

       if(this.formByCopy.isEdit){
        editContentBelong(this.formByCopy).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.dialogByCopyIsVisible = false
            this.getListDataByBelong(this.listQueryByBelongs)
          }
        })
       }
       else{
           addContentBelong(this.formByCopy).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.dialogByCopyIsVisible = false
            this.getListDataByBelong(this.listQueryByBelongs)
          }
        })
       }


      })
    }
  }
}
</script>
