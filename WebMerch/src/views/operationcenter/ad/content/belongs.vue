<template>
  <div id="adspace_belongs">
    <page-header />
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
      <div class="pull-left"> <h5>相关对象</h5>
      </div>
      <div class="pull-right">
        <el-button v-if="adContent.status.value==1" type="primary" size="mini" style="margin-top:-20px;margin-right:10px" @click="onOpenDialogByEdit(false,null)">
          添加
        </el-button>
      </div>
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
      <el-table-column label="对象" prop="title" align="left" min-width="40%">
        <template slot-scope="scope">
          <span>{{ scope.row.belongName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="有效期" prop="title" align="left" min-width="45%">
        <template slot-scope="scope">
          <span>{{ scope.row.validStartTime }}</span>~<span>{{ scope.row.validEndTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="status" align="left" min-width="15%">
        <template slot-scope="scope">
          <el-tag :type="getStatusColor(scope.row.status.value)">{{ scope.row.status.text }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="right" width="200" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button v-if="row.status.value==1" type="text" size="mini" @click="onSetBelongStatus(row)">
            停止
          </el-button>
          <el-button v-if="row.status.value==2" type="text" size="mini" @click="onSetBelongStatus(row)">
            恢复
          </el-button>

          <el-button v-if="row.status.value==1||row.status.value==2" type="text" size="mini" @click="onOpenDialogByEdit(true,row)">
            编辑
          </el-button>

          <span v-if="row.status.value==3">
            已停用
          </span>

        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />

    <el-dialog v-if="dialogByEditIsVisible" :title="'编辑'" width="600px" :visible.sync="dialogByEditIsVisible" append-to-body>
      <div style="width:100%;height:400px">
        <el-form ref="form" v-loading="loadingByEdit" :model="form" label-width="80px">
          <el-form-item label="有效期" prop="validDate">
            <el-date-picker
              v-model="formByEdit.validDate"
              type="daterange"
              range-separator="至"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
              value-format="yyyy-MM-dd"
              style="width: 380px"
            />
          </el-form-item>
          <el-form-item label="对象">
            <div v-show="formIsEdit">
              {{ temp.belongName }}
            </div>

            <div v-show="!formIsEdit">
              <el-checkbox v-model="temp.belongsCheckAll" :indeterminate="temp.belongsIsIndeterminate" @change="onBelongsCheckAllChange">全选</el-checkbox>
              <div style="margin: 15px 0;" />
              <el-checkbox-group v-model="formByEdit.belongIds" @change="onBelongsCheckedChange">
                <el-checkbox v-for="(belong,index) in temp.belongs" :key="index" :label="belong.id">{{ belong.name }}</el-checkbox>
              </el-checkbox-group>
            </div>

          </el-form-item>
        </el-form>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="onBelongEdit">
          确定
        </el-button>
        <el-button size="small" @click="dialogByEditIsVisible = false">
          关闭
        </el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initBelongs, getContentBelongs, setContentBelongStatus, getSelBelongs, editContentBelong, addContentBelong } from '@/api/ad'
import { getUrlParam } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
import Pagination from '@/components/Pagination'
export default {
  name: 'OperationCenterAdContentBelongs',
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
      adContent: {
        adSpaceName: '',
        url: '',
        title: '',
        status: {
          value: 2
        }
      },
      temp: {
        belongs: [],
        belongName: '',
        belongsCheckAll: false,
        belongsIsIndeterminate: true
      },
      loadingByEdit: false,
      formIsEdit: false,
      formByEdit: {
        adContentId: '',
        belongIds: [],
        validDate: []
      },
      dialogByEditIsVisible: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    this.listQuery.adContentId = getUrlParam('id')
    this.formByEdit.adContentId = this.listQuery.adContentId
    initBelongs({ adContentId: this.listQuery.adContentId }).then(res => {
      if (res.result === 1) {
        var d = res.data
        this.adContent.adSpaceName = d.adSpaceName
        this.adContent.url = d.url
        this.adContent.title = d.title
        this.adContent.status = d.status
      }
    })
    this.onGetList()
  },
  methods: {
    onGetList() {
      this.loading = true
      getContentBelongs(this.listQuery).then(res => {
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
    onSetBelongStatus(item) {
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
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.getListData(this.listQuery)
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
    onOpenDialogByEdit(isEdit, item) {
      this.dialogByEditIsVisible = true
      this.formIsEdit = isEdit
      if (isEdit) {
        this.temp.belongName = item.belongName
        this.formByEdit.belongIds = [item.belongId]
        this.formByEdit.validDate = [item.validStartTime, item.validEndTime]
      } else {
        this.formByEdit.belongIds = []
        this.temp.belongs = []
        getSelBelongs({ adContentId: this.listQuery.adContentId }).then(res => {
          if (res.result === 1) {
            this.temp.belongs = res.data
          }
        })

        this.formByEdit.validDate = [this.listData[0].validStartTime, this.listData[0].validEndTime]
      }
    },
    onBelongsCheckAllChange(val) {
      var belongsChecked = []
      for (var i = 0; i < this.temp.belongs.length; i++) {
        belongsChecked.push(this.temp.belongs[i].id)
      }
      this.formByEdit.belongIds = val ? belongsChecked : []
      this.temp.belongsIsIndeterminate = false
    },
    onBelongsCheckedChange(value) {
      const checkedCount = value.length
      this.temp.belongsCheckAll = checkedCount === this.temp.belongs.length
      this.temp.belongsIsIndeterminate = checkedCount > 0 && checkedCount < this.temp.belongs.length
    },
    onBelongEdit() {
      MessageBox.confirm('确定保存', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        if (this.formIsEdit) {
          editContentBelong(this.formByEdit).then(res => {
            if (res.result === 1) {
              this.$message({
                message: res.message,
                type: 'success'
              })
              this.dialogByEditIsVisible = false
              this.onGetList(this.listQuery)
            } else {
              this.$message({
                message: res.message,
                type: 'error'
              })
            }
          })
        } else {
          addContentBelong(this.formByEdit).then(res => {
            if (res.result === 1) {
              this.$message({
                message: res.message,
                type: 'success'
              })
              this.dialogByEditIsVisible = false
              this.onGetList(this.listQuery)
            } else {
              this.$message({
                message: res.message,
                type: 'error'
              })
            }
          })
        }
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
