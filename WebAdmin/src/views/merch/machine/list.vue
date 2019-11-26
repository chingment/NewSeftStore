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
      <el-table-column label="操作" align="center" width="100" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button v-if="row.merchId==null" type="primary" size="mini" width="100" @click="_dialogBindOnMerchOpen(row)">
            绑定
          </el-button>
          <el-button v-if="row.merchId!=null" type="warning" size="mini" width="100" @click="_bindOffMerch(row)">
            解绑
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
    }
  }
}
</script>
