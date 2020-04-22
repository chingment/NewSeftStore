<template>
  <div class="app-container">
    <div class="filter-container">
      <el-input v-model="listQuery.id" placeholder="编号" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="handleFilter">
        查询
      </el-button>
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
          <span>{{ scope.$index+1 }}</span>
        </template>
      </el-table-column>
      <el-table-column label="编号" prop="id" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.id }}</span>
        </template>
      </el-table-column>
      <el-table-column label="名称" prop="name" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="App版本" prop="appVersion" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.appVersion }}</span>
        </template>
      </el-table-column>
      <el-table-column label="机器控制版本" prop="ctrlSdkVersion" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.ctrlSdkVersion }}</span>
        </template>
      </el-table-column>
      <el-table-column label="当前使用商户" prop="merchName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.merchName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="创建时间" prop="createTime" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="180" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button v-if="row.merchId==null" type="success" size="mini" width="100" @click="_dialogBindOnMerchOpen(row)">
            绑定
          </el-button>
          <el-button v-if="row.merchId!=null" type="warning" size="mini" width="100" @click="_bindOffMerch(row)">
            解绑
          </el-button>
          <el-button type="primary" size="mini" width="100" @click="_dialogEditOpen(row)">
            设置
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <el-dialog title="绑定商户" :visible.sync="dialogBindOnMerchIsVisible" width="800px">
      <el-form ref="formByBindOnMerch" :model="formByBindOnMerch" :rules="rulesByBindOnMerch" label-position="left" label-width="80px">
        <el-form-item label="机器编号">
          <span>{{ formByBindOnMerch.machineId }}</span>
        </el-form-item>
        <el-form-item label="商户名称" prop="merchId">
          <el-select v-model="formByBindOnMerch.merchId" class="filter-item" placeholder="选择" clearable style="width:500px">
            <el-option v-for="item in formSelectMerchs" :key="item.value" :label="item.label" :value="item.value" :disabled="item.disabled" />
          </el-select>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogBindOnMerchIsVisible = false">
          取消
        </el-button>
        <el-button type="primary" @click="_bindOnMerch">
          确定
        </el-button>
      </div>
    </el-dialog>

    <el-dialog title="机器信息" :visible.sync="dialogEditIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div v-loading="dialogEditLoading">

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>基本信息</h5>
          </div>
        </div>

        <el-form class="form-container" style="display:flex">
          <el-col :span="24">

            <div class="postInfo-container">
              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="80px" label="机器编码:" class="postInfo-container-item">
                    {{ details.id }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="80px" label="设备编码:" class="postInfo-container-item">
                    {{ details.deviceId }}
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="80px" label="ImeiId:" class="postInfo-container-item">
                    {{ details.imeiId }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="80px" label="物理地址:" class="postInfo-container-item">
                    {{ details.macAddress }}
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="80px" label="App版本:" class="postInfo-container-item">
                    {{ details.appVersionName }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="80px" label="控制版本:" class="postInfo-container-item">
                    {{ details.ctrlSdkVersionCode }}
                  </el-form-item>
                </el-col>

              </el-row>

            </div>
          </el-col>
        </el-form>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>购物界面</h5>
          </div>
        </div>

        <el-form class="form-container" style="display:flex">
          <el-col :span="24">
            <div class="postInfo-container">
              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="80px" label="显示分类:" class="postInfo-container-item">
                    <el-checkbox v-model="checked">是</el-checkbox>
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="80px" label="每行显示:" class="postInfo-container-item">
                    <el-input-number v-model="num" :min="1" :max="10" label="描述文字" />
                  </el-form-item>
                </el-col>

              </el-row>
            </div>
          </el-col>
        </el-form>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>摄像头</h5>
          </div>
        </div>

        <el-form class="form-container" style="display:flex">
          <el-col :span="24">
            <div class="postInfo-container">
              <el-row>
                <el-col :span="8">
                  <el-form-item label-width="80px" label="人脸:" class="postInfo-container-item">
                    <el-checkbox v-model="checked">打开</el-checkbox>
                  </el-form-item>
                </el-col>
                <el-col :span="8">
                  <el-form-item label-width="80px" label="取货口:" class="postInfo-container-item">
                    <el-checkbox v-model="checked">打开</el-checkbox>
                  </el-form-item>
                </el-col>
                <el-col :span="8">
                  <el-form-item label-width="80px" label="机柜:" class="postInfo-container-item">
                    <el-checkbox v-model="checked">打开</el-checkbox>
                  </el-form-item>
                </el-col>
              </el-row>

            </div>
          </el-col>
        </el-form>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>扩展设备</h5>
          </div>
        </div>

      </div>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="_edit(details)">
          保存
        </el-button>
        <el-button @click="dialogEditIsVisible = false">
          关闭
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initGetList, getList, bindOffMerch, bindOnMerch } from '@/api/merchmachine'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ComplexTable',
  components: { Pagination },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      dialogBindOnMerchIsVisible: false,
      dialogEditIsVisible: false,
      dialogEditLoading: false,
      formByBindOnMerch: {
        merchiId: '',
        machineId: ''
      },
      rulesByBindOnMerch: {
        merchiId: [
          { required: true, message: '请选择机器', trigger: 'change' }
        ]
      },
      formSelectMerchs: [
      ],
      details: {
        id: '',
        name: '',
        imeiId: '',
        macAddress: '',
        deviceId: '',
        appVersionCode: '',
        appVersionName: '',
        ctrlSdkVersionCode: '',
        kndIsHidden: false,
        kindRowCellSize: 0,
        isTestMode: false,
        cameraByChkIsUse: false,
        cameraByJgIsUse: false,
        cameraByRlIsUse: false,
        exIsHas: false,
        sannerIsUse: false,
        sannerComId: '',
        fingerVeinnerIsUse: false,
        mstVern: '',
        ostVern: ''
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this._init()
  },
  methods: {
    _init() {
      this.loading = true
      initGetList().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.formSelectMerchs = d.formSelectMerchs
        }
        this.loading = false
      })
      this._getList(this.listQuery)
    },
    _getList() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getList(this.listQuery).then(res => {
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
      this._getList()
    },
    _dialogBindOnMerchOpen(row) {
      this.dialogBindOnMerchIsVisible = true
      this.formByBindOnMerch.machineId = row.id
    },
    _bindOnMerch() {
      MessageBox.confirm('确定要将机器(' + this.formByBindOnMerch.machineId + ')绑定商户？', '提示（慎重操作）', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        bindOnMerch(this.formByBindOnMerch).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.dialogBindOnMerchIsVisible = false
            this._getList()
          }
        })
      })
    },
    _bindOffMerch(row) {
      MessageBox.confirm('确定要从商户(' + row.merchName + ')解绑机器(' + row.id + ')？', '提示（慎重操作）', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        bindOffMerch({ merchId: row.merchId, machineId: row.id }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this._getList()
          }
        })
      })
    },
    _dialogEditOpen() {
      this.dialogEditIsVisible = true
    }
  }
}
</script>
