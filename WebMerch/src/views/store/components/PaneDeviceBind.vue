<template>
  <div id="shop_list">
    <div class="filter-container">

      <el-row :gutter="16">
        <el-col :span="8" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQueryByShop.id" clearable style="width: 100%" placeholder="设备编码" class="filter-item" />
        </el-col>
        <el-col :span="8" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilterByShop">
            查询
          </el-button>
          <el-button class="filter-item" type="primary" icon="el-icon-plus" @click="onDialogBySbShop()">
            添加
          </el-button>
        </el-col>
      </el-row>

    </div>
    <el-table
      :key="listKeyByShop"
      v-loading="loadingByShop"
      :data="listDataByShop"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="设备编码" align="left" min-width="30%">
        <template slot-scope="scope">
          <span>{{ scope.row.code }}</span>
        </template>
      </el-table-column>
      <el-table-column label="门店" align="left" min-width="70%">
        <template slot-scope="scope">
          <span>{{ scope.row.shopName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="right" width="200" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="onUnBindShop(row)">
            解绑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotalByShop>0" :total="listTotalByShop" :page.sync="listQueryByShop.page" :limit.sync="listQueryByShop.limit" @pagination="onGetListByShop" />

    <el-dialog v-if="dialogIsVisibleBySbShop" :title="'选择设备'" width="600px" :visible.sync="dialogIsVisibleBySbShop" append-to-body>
      <div style="width:100%;height:400px">
        <div class="filter-container">
          <el-row :gutter="16">
            <el-col :span="8" :xs="24" style="margin-bottom:20px">
              <el-input v-model="listQueryBySbShop.id" clearable style="width: 100%" placeholder="设备编码" class="filter-item" />
            </el-col>
            <el-col :span="8" :xs="24" style="margin-bottom:20px">
              <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilterBySbShop">
                查询
              </el-button>
            </el-col>
          </el-row>

        </div>
        <el-table
          :key="listKeyBySbShop"
          v-loading="loadingBySbShop"
          :data="listDataBySbShop"
          fit
          highlight-current-row
          style="width: 100%;"
        >
          <el-table-column label="序号" prop="id" align="left" width="80">
            <template slot-scope="scope">
              <span>{{ scope.$index+1 }} </span>
            </template>
          </el-table-column>
          <el-table-column label="编码" align="left" min-width="45%">
            <template slot-scope="scope">
              <span>{{ scope.row.code }}</span>
            </template>
          </el-table-column>
          <el-table-column label="门店" align="left" min-width="55%">
            <template slot-scope="scope">
              <span>{{ scope.row.shopName }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="right" width="100" class-name="small-padding fixed-width">
            <template slot-scope="{row}">
              <el-button v-if="row.isCanSelect" type="text" size="mini" @click="onBindShop(row)">
                选择
              </el-button>
              <el-button v-else type="text" size="mini" disabled>{{ row.opTips }}</el-button>
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="listTotalBySbShop>0" :total="listTotalBySbShop" :page.sync="listQueryBySbShop.page" :limit.sync="listQueryBySbShop.limit" @pagination="onGetListBySbShop" />
      </div>
    </el-dialog>
  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getListByShop, getListBySbShop, unBindShop, bindShop } from '@/api/devvending'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ShopSelect',
  components: { Pagination },
  props: {
    storeId: {
      type: String,
      default: ''
    },
    shopId: {
      type: String,
      default: ''
    },
    selectMethod: {
      type: Function,
      default: null
    }
  },
  data() {
    return {
      loadingByShop: false,
      listKeyByShop: 0,
      listDataByShop: null,
      listTotalByShop: 0,
      listQueryByShop: {
        page: 1,
        limit: 10,
        id: '',
        type: 'vending'
      },
      loadingBySbShop: false,
      listKeyBySbShop: 'listKeyBySelect',
      listKeyBySbSShop: 0,
      listDataBySbShop: null,
      listTotalBySbShop: 0,
      listQueryBySbShop: {
        page: 1,
        limit: 10,
        type: 'vending',
        shopId: '',
        storeId: '',
        id: ''
      },
      dialogIsVisibleBySbShop: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {

  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQueryByShop = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.listQueryByShop.storeId = this.storeId
    this.listQueryByShop.shopId = this.shopId
    this.onGetListByShop()
  },
  methods: {
    onGetListByShop() {
      this.loadingByShop = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQueryByShop })
      getListByShop(this.listQueryByShop).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listDataByShop = d.items
          this.listTotalByShop = d.total
        }
        this.loadingByShop = false
      })
    },
    onFilterByShop() {
      this.listQueryByShop.page = 1
      this.onGetListByShop()
    },
    onGetListBySbShop() {
      this.loadingBySbShop = true
      getListBySbShop(this.listQueryBySbShop).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listDataBySbShop = d.items
          this.listTotalBySbShop = d.total
        }
        this.loadingBySbShop = false
      })
    },
    onFilterBySbShop() {
      this.listQueryBySbShop.page = 1
      this.onGetListBySbShop()
    },
    onDialogBySbShop() {
      this.dialogIsVisibleBySbShop = true
      this.onGetListBySbShop()
    },
    onUnBindShop: function(item) {
      MessageBox.confirm('确定要解绑该设备，解绑后库存将被清空，请谨慎操作！', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        unBindShop({ deviceId: item.id, storeId: item.storeId, shopId: item.shopId }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.onGetListByShop()
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }
        })
      })
    },
    onBindShop: function(item) {
      MessageBox.confirm('确定绑定设备', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        bindShop({ deviceId: item.id, storeId: this.storeId, shopId: this.shopId }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.onGetListByShop()
            this.dialogIsVisibleBySbShop = false
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }
        })
      })
    }

  }
}
</script>
