<template>
  <div id="store_list" class="app-container">
    <el-row v-loading="loading" :gutter="20">
      <el-col v-for="item in listData" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">
              <span :class="'circle-status circle-status-'+item.status.value" /> <span class="name">{{ item.name }}</span>
            </div>
            <div class="right">
              <el-button type="text" @click="handleUpdate(item)">管理</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe" />
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
      <el-form ref="formByAddMachine" :rules="rulesByAddMachine" label-position="left" label-width="50px">
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
import { initManageMachine, manageMachineGetMachineList } from '@/api/store'
import { getUrlParam } from '@/utils/commonUtil'
export default {
  name: 'ManagePaneMachine',
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

      },
      storeName: '',
      formSelectMachines: [
      ]
    }
  },
  watch: {
    '$route'(to, from) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var id = getUrlParam('id')
      this.loading = true
      this.listQuery.storeId = id

      initManageMachine({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.storeName = d.storeName
          this.formSelectMachines = d.formSelectMachines
        }
        this.loading = false
      })

      this.getListData(this.listQuery)
    },
    getListData(listQuery) {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      manageMachineGetMachineList(this.listQuery).then(res => {
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
      this.dialogAddMachineIsVisible = false
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
    }
  }
}
</style>
