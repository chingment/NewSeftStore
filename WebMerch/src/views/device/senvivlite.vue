<template>
  <div id="shop_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.id" clearable style="width: 100%" placeholder="设备编码" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilter">
            查询
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
      <el-table-column label="设备编码" prop="id" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.id }}</span>
        </template>
      </el-table-column>
      <el-table-column label="设备名称" prop="name" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="设备型号" prop="model" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.model }}</span>
        </template>
      </el-table-column>
      <el-table-column label="分配状态" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.distributeStatus.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="主体商户" prop="merchName" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.merchName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button v-if="row.distributeStatus.value===1" type="text" size="mini" @click="onDialogOpenBindMerch(row)">
            分配
          </el-button>
          <el-button v-else type="text" size="mini" @click="onUnBindMerch(row)">
            回收
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />

    <el-dialog title="分配...." :visible.sync="dialogIsVisibleByBindMerch" width="600px">
      <el-form ref="formByBindMerch" :model="formByBindMerch" :rules="rulesByBindMerch" label-position="left" label-width="80px">
        <el-form-item label="设备编码">
          <span>{{ formByBindMerch.deviceCumCode }}</span>
        </el-form-item>
        <el-form-item label="商户名称" prop="merchId">
          <el-select v-model="formByBindMerch.merchId" class="filter-item" placeholder="选择" clearable style="width:80%">
            <el-option v-for="item in optionsByMerch" :key="item.id" :label="item.name" :value="item.id" />
          </el-select>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogIsVisibleByBindMerch = false">
          取消
        </el-button>
        <el-button type="primary" @click="onBindMerch">
          确定
        </el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getList, initGetList, bindMerch, unBindMerch } from '@/api/devsenvivlite'
import Pagination from '@/components/Pagination'
export default {
  name: 'DeviceSenvivLite',
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
        id: '',
        type: 'senvivlite'
      },
      dialogIsVisibleByBindMerch: false,
      formByBindMerch: {
        deviceCumCode: '',
        merchId: '',
        deviceId: ''
      },
      optionsByMerch: [],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initGetList().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.optionsByMerch = d.optionsByMerch
        }
        this.loading = false
      })
      this.onGetList()
    },
    onGetList() {
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
    onFilter() {
      this.listQuery.page = 1
      this.onGetList()
    },
    onDialogOpenBindMerch(item) {
      this.dialogIsVisibleByBindMerch = true
      this.formByBindMerch.deviceCumCode = item.code
      this.formByBindMerch.deviceId = item.id
      this.formByBindMerch.merchId = ''
    },
    onBindMerch() {
      MessageBox.confirm('确定要将设备(' + this.formByBindMerch.deviceId + ')绑定商户？', '提示（慎重操作）', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        bindMerch(this.formByBindMerch).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.dialogIsVisibleByBindMerch = false
            this.onGetList()
          }
        })
      })
    },
    onUnBindMerch(row) {
      MessageBox.confirm('确定要从商户(' + row.merchName + ')回收设备(' + row.id + ')？', '提示（慎重操作）', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        unBindMerch({ merchId: row.merchId, deviceId: row.id }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.onGetList()
          }
        })
      })
    }
  }
}
</script>
