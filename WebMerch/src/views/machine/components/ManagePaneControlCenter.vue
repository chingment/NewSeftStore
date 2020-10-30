<template>
  <div id="machine_controlcenter" class="app-container">
    <div class="pane-ctl">
      <div class="title"><span>系统相关</span>
      </div>
      <div class="content">
        <el-button type="primary" @click="onOpenDialogSysSetStatus">设置状态</el-button>
        <el-button type="primary" @click="onSysReboot">重启系统</el-button>
        <el-button type="primary" @click="onSysShutDown">关闭系统</el-button>
      </div>
    </div>

    <div class="pane-ctl">
      <div class="title"><span>DS设备相关</span>
      </div>
      <div class="content">
        <el-button type="primary" @click="onDsx01OpenPickupDoor">打开设备取货门</el-button>
      </div>
    </div>

    <el-dialog title="设置状态" :visible.sync="dialogSysSetStatusIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div>

        <el-form ref="formBySysSetStatus" :model="formBySysSetStatus" :rules="formBySysSetStatusRules" label-width="80px">
          <el-form-item label="机器编码">
            <span>{{ machineId }}</span>
          </el-form-item>
          <el-form-item label="状态" prop="status">
            <el-radio v-model="formBySysSetStatus.status" label="1">正常</el-radio>
            <el-radio v-model="formBySysSetStatus.status" label="2">维护中</el-radio>
          </el-form-item>
          <el-form-item label="描述" prop="helpTip">
            <el-input v-model="formBySysSetStatus.helpTip" type="textarea" :rows="5" />
          </el-form-item>
        </el-form>

      </div>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="onSysSetStatus()">
          确定
        </el-button>
        <el-button @click="dialogSysSetStatusIsVisible = false">
          关闭
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { getUrlParam } from '@/utils/commonUtil'
import { sysReboot, sysShutdown, sysSetStatus, queryMsgPushResult, dsx01OpenPickupDoor } from '@/api/machine'

export default {
  name: 'ManagePaneBaseInfo',
  data() {
    return {
      machineId: '',
      dialogSysSetStatusIsVisible: false,
      isDesktop: this.$store.getters.isDesktop,
      formBySysSetStatus: {
        status: undefined,
        helpTip: '机器设备正在维护中'
      },
      formBySysSetStatusRules: {
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
    onSysReboot() {
      MessageBox.confirm('确定要重启系统？请确保机器在空闲状态中，否则会影响机器正常运行！', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        sysReboot({ id: this.machineId }).then(res => {
          if (res.result === 1) {
            this.onQueryMsgStatus(res.data.msg_id)
          } else {
            this.$message(res.message)
          }
        })
      }).catch(() => {
      })
    },
    onSysShutDown() {
      MessageBox.confirm('确定要关闭系统？关闭系统需要人工前往机器开启！', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        sysShutdown({ id: this.machineId }).then(res => {
          if (res.result === 1) {
            this.onQueryMsgStatus(res.data.msg_id)
          } else {
            this.$message(res.message)
          }
        })
      }).catch(() => {
      })
    },
    onOpenDialogSysSetStatus() {
      this.formBySysSetStatus.status = undefined
      this.formBySysSetStatus.helpTip = '机器设备正在维护中'
      this.dialogSysSetStatusIsVisible = true
    },
    onSysSetStatus() {
      this.$refs['formBySysSetStatus'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要设置状态？', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            sysSetStatus({ id: this.machineId, status: this.formBySysSetStatus.status, helpTip: this.formBySysSetStatus.helpTip }).then(res => {
              if (res.result === 1) {
                this.dialogSysSetStatusIsVisible = false
                this.onQueryMsgStatus(res.data.msg_id)
              } else {
                this.$message(res.message)
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    onDsx01OpenPickupDoor() {
      MessageBox.confirm('确定要打开设备型号DSX01的取货门', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        dsx01OpenPickupDoor({ id: this.machineId }).then(res => {
          if (res.result === 1) {
            this.onQueryMsgStatus(res.data.msg_id)
          } else {
            this.$message(res.message)
          }
        })
      }).catch(() => {
      })
    },
    onQueryMsgStatus(msgId) {
      var _this = this
      const loading = _this.$loading({
        lock: true,
        text: '正常处理中，请耐心等候，大概需要10秒',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      })

      var timeout = null
      var interval = window.setInterval(function() {
        queryMsgPushResult({ machineId: _this.machineId, msg_id: msgId }).then(res => {
          if (res.result === 1) {
            loading.close()
            _this.$message(res.message)
            if (timeout != null) {
              window.clearInterval(interval)
              window.clearTimeout(timeout)
            }
          }
        })
      }, 1000)

      timeout = window.setTimeout(() => {
        loading.close()
        window.clearInterval(interval)
        queryMsgPushResult({ machineId: _this.machineId, msg_id: msgId }).then(res => {
          _this.$message(res.message)
        })
      }, 10000)
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
