<template>
  <div id="machine_controlcenter" class="app-container">
    <div class="pane-ctl">
      <div class="title"><span>系统相关</span>
      </div>
      <div class="content">
        <el-button type="primary" @click="onOpenDalogSeutStatus">设置状态</el-button>
        <el-button type="primary" @click="onRebootSys">重启系统</el-button>
        <el-button type="primary" @click="onShutDownSys">关闭系统</el-button>
      </div>
    </div>

    <el-dialog title="设置状态" :visible.sync="dialogSetStatusIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div>

        <el-form ref="formBySetSysStatus" :model="formBySetSysStatus" :rules="formBySetSysStatusRules" label-width="80px">
          <el-form-item label="机器编码">
            <span>{{ machineId }}</span>
          </el-form-item>
          <el-form-item label="状态" prop="status">
            <el-radio v-model="formBySetSysStatus.status" label="1">正常</el-radio>
            <el-radio v-model="formBySetSysStatus.status" label="2">维护中</el-radio>
          </el-form-item>
          <el-form-item label="描述" prop="helpTip">
            <el-input v-model="formBySetSysStatus.helpTip" type="textarea" :rows="5" />
          </el-form-item>
        </el-form>

      </div>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="onSetSysStatus()">
          确定
        </el-button>
        <el-button @click="dialogSetStatusIsVisible = false">
          关闭
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { getUrlParam } from '@/utils/commonUtil'
import { rebootSys, shutdownSys, setSysStatus } from '@/api/machine'
export default {
  name: 'ManagePaneBaseInfo',
  data() {
    return {
      machineId: '',
      dialogSetStatusIsVisible: false,
      isDesktop: this.$store.getters.isDesktop,
      formBySetSysStatus: {
        status: undefined,
        helpTip: '机器设备正在维护中'
      },
      formBySetSysStatusRules: {
        status: [{ required: true, message: '请选择状态', trigger: 'change' }],
        helpTip: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }]
      }
    }
  },
  watch: {
    '$route'(to, from) {
      this.init()
    }
  },
  mounted() {

  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var id = getUrlParam('id')
      this.machineId = id
    },
    onRebootSys() {
      MessageBox.confirm('确定要重启系统', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        rebootSys({ id: this.machineId }).then(res => {
          this.$message(res.message)
        })
      })
    },
    onShutDownSys() {
      MessageBox.confirm('确定要关闭系统', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        shutdownSys({ id: this.machineId }).then(res => {
          this.$message(res.message)
        })
      })
    },
    onOpenDalogSeutStatus() {
      this.formBySetSysStatus.status = undefined
      this.formBySetSysStatus.helpTip = '机器设备正在维护中'
      this.dialogSetStatusIsVisible = true
    },
    onSetSysStatus() {
      this.$refs['formBySetSysStatus'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要设置状态', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            setSysStatus({ id: this.machineId, status: this.formBySetSysStatus.status, helpTip: this.formBySetSysStatus.helpTip }).then(res => {
              this.$message(res.message)
              this.dialogSetStatusIsVisible = false
            })
          })
        }
      })
    }
  }
}
</script>
<style lang="scss" scoped>

#machine_controlcenter{

  .pane-ctl {

    .title{
    padding: 10px 8px;
    background-color: #ecf8ff;
    border-radius: 4px;
    border-left: 5px solid #50bfff;
   }

   .content{
    padding: 20px 8px;
   }

  }

}
</style>
