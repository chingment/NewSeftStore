<template>
  <div id="store_list" class="app-container">
    <div class="circle-status-bar">
      <div class="circle-item"> <span class="icon-status icon-status-1" /> <span class="name">关闭</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-2" /> <span class="name">正常</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-4" /> <span class="name">维护</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-3" /> <span class="name">异常</span></div>
    </div>
    <el-row v-loading="loading" :gutter="20">
      <el-col v-for="item in listData" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">
              <div class="circle-item"> <span :class="'icon-status icon-status-'+item.status.value" /> <span class="name">{{ item.name }}</span>          <span style="font-size:12px;"> ({{ item.status.text }})</span></div>
            </div>
            <div class="right">
              <el-button type="text" @click="handleRemoveMachine(item)">移除</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li><el-button type="text" @click="handleViewStock(item)">库存管理</el-button></li>
                <li><el-button type="text" style="color:#67c23a" @click="handleViewStock(item)">订单信息</el-button></li>
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left" />
            <el-button type="text" @click="dialogAddMachineOpen">添加机器</el-button>
          </div>
          <div class="it-component">

            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card" @click="dialogAddMachineOpen"><i class="el-icon-plus" /></div>

          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog title="添加机器" :visible.sync="dialogAddMachineIsVisible" width="800px">
      <el-form ref="formByAddMachine" :model="formByAddMachine" :rules="rulesByAddMachine" label-position="left" label-width="50px">
        <el-form-item label="店铺">
          <span>{{ storeName }}</span>
        </el-form-item>
        <el-form-item label="机器" prop="machineId">
          <el-select v-model="formByAddMachine.machineId" class="filter-item" placeholder="选择" clearable style="width:500px">
            <el-option v-for="item in formSelectMachines" :key="item.value" :label="item.label" :value="item.value" :disabled="item.disabled" />
          </el-select>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogAddMachineIsVisible = false">
          取消
        </el-button>
        <el-button type="primary" @click="handleAddMachine">
          确定
        </el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initManageMachine, manageMachineGetMachineList, removeMachine, addMachine } from '@/api/store'
import { getUrlParam, isEmpty } from '@/utils/commonUtil'
export default {
  name: 'ManagePaneMachine',
  props: {
    storeid: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: true,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      listData: [],
      dialogAddMachineIsVisible: false,
      formByAddMachine: {
        storeId: '',
        machineId: ''
      },
      rulesByAddMachine: {
        machineId: [
          { required: true, message: '请选择机器', trigger: 'change' }
        ]
      },
      storeId: '',
      storeName: '',
      formSelectMachines: [
      ]
    }
  },
  watch: {
    storeid: function(val, oldval) {
      console.log('storeid3 值改变:' + val)
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      if (!isEmpty(this.storeid)) {
        console.log('storeid3 1值改变:' + this.storeid)
        this.loading = true
        this.storeId = this.storeid
        this.listQuery.storeId = this.storeid
        initManageMachine({ id: this.storeid }).then(res => {
          if (res.result === 1) {
            var d = res.data
            this.storeName = d.storeName
            this.formSelectMachines = d.formSelectMachines
          }
          this.loading = false
        })
        this.getListData(this.listQuery)
      }
    },
    getListData(listQuery) {
      this.loading = true
      // this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: listQuery })
      manageMachineGetMachineList(listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
        }
        this.loading = false
      })
    },
    dialogAddMachineOpen() {
      this.dialogAddMachineIsVisible = true
    },
    handleAddMachine() {
      this.$refs['formByAddMachine'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要添加该机器，慎重操作', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            this.formByAddMachine.storeId = this.storeId
            addMachine(this.formByAddMachine).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this.dialogAddMachineIsVisible = false
                this.init()
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    handleRemoveMachine(item) {
      MessageBox.confirm('确定要移除该机器，慎重操作', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        removeMachine({ storeId: this.storeId, machineId: item.id }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.init()
          }
        })
      }).catch(() => {
      })
    },
    handleViewStock(row) {
      this.$router.push({
        path: '/machine/manage?id=' + row.id + '&tab=tabStock'
      })
    },
    handleViewOrder(row) {
      this.$router.push({
        path: '/machine/manage?id=' + row.id + '&tab=tabOrder'
      })
    }
  }
}
</script>

<style lang="scss" scoped>

#store_list{
  padding: 20px;

  .it-header{
    display: flex;
    justify-content: flex-start;
    align-items: center;
    position: relative;
    height:20px ;
    .left{
      flex: 1;
      justify-content: flex-start;
      align-items: center;
      display: block;
      height: 100%;
    overflow: hidden;
text-overflow:ellipsis;
white-space: nowrap;
    .name{
    padding: 0px 5px;
    }
    }
    .right{
      width: 100px;
      display: flex;
      justify-content: flex-end;
      align-items: center;
    }

  }
  .it-component{
    min-height: 100px;
    display: flex;
    .img{
      width: 120px;
      height: 120px;

      img{
        width: 100%;
        height: 100%;
      }
    }

    .describe{
      flex: 1;
      padding: 5px;
      font-size: 12px;

      ul{
        padding: 0px;
        margin: 0px;
        list-style: none;
         li{
           width: 100%;
             text-align: right;
        height: 26px;
        line-height: 26px;
      }
      }
    }
  }
}
</style>
