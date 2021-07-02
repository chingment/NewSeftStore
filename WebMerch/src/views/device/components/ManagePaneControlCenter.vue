<template>
  <div id="device_controlcenter">
    <div class="pane-ctl">
      <div class="title"><span>系统相关</span>
      </div>
      <div class="content">

        <el-button type="primary" style="margin-bottom:20px;margin-right: 10px;" @click="onOpenDialogSetSysStatus">设置状态</el-button>
        <el-button type="primary" style="margin-bottom:20px;margin-right: 10px;margin-left: 0px;" @click="onRebootSys">重启系统</el-button>
        <el-button type="primary" style="margin-bottom:20px;margin-right: 10px;margin-left: 0px;" @click="onShutDownSys">关闭系统</el-button>

      </div>
    </div>

    <div class="pane-ctl">
      <div class="title"><span>DS设备相关</span>
      </div>
      <div class="content">
        <el-button type="primary" @click="onOpenPickupDoor">打开取货口</el-button>
      </div>
    </div>

    <el-dialog title="设置状态" :visible.sync="dialogSetSysStatusIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div>

        <el-form ref="formBySetSysStatus" :model="formBySetSysStatus" :rules="formBySetSysStatusRules" label-width="80px">
          <el-form-item label="设备编码">
            <span>{{ deviceId }}</span>
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
        <el-button size="small" @click="dialogSetSysStatusIsVisible = false">
          取消
        </el-button>
        <el-button type="primary" size="small" @click="onSetSysStatus()">
          确定
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { rebootSys, shutdownSys, setSysStatus, openPickupDoor, queryMsgPushResult } from '@/api/device'

export default {
  name: 'ManagePaneBaseInfo',
  props: {
    deviceId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      dialogSetSysStatusIsVisible: false,
      isDesktop: this.$store.getters.isDesktop,
      formBySetSysStatus: {
        status: undefined,
        helpTip: '设备正在维护中'
      },
      formBySetSysStatusRules: {
        status: [{ required: true, message: '请选择状态', trigger: 'change' }],
        helpTip: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }]
      }
    }
  },
  watch: {
    deviceId: function(val, oldval) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {

    },
    onRebootSys() {
      MessageBox.confirm('确定要重启系统？请确保设备在空闲状态中，否则会影响设备正常运行！', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        rebootSys({ id: this.deviceId }).then(res => {
          if (res.result === 1) {
            this.onQueryMsgStatus(res.data.msg_id)
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
    onShutDownSys() {
      MessageBox.confirm('确定要关闭系统？关闭系统需要人工前往设备开启！', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        shutdownSys({ id: this.deviceId }).then(res => {
          if (res.result === 1) {
            this.onQueryMsgStatus(res.data.msg_id)
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
    onOpenDialogSetSysStatus() {
      this.formBySetSysStatus.status = undefined
      this.formBySetSysStatus.helpTip = '设备正在维护中'
      this.dialogSetSysStatusIsVisible = true
    },
    onSetSysStatus() {
      this.$refs['formBySetSysStatus'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要设置状态？', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            setSysStatus({ id: this.deviceId, status: this.formBySetSysStatus.status, helpTip: this.formBySetSysStatus.helpTip }).then(res => {
              if (res.result === 1) {
                this.dialogSetSysStatusIsVisible = false
                this.onQueryMsgStatus(res.data.msg_id)
              } else {
                this.$message({
                  message: res.message,
                  type: 'error'
                })
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    onOpenPickupDoor() {
      MessageBox.confirm('确定要打开设备型号DSX01的取货门', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        openPickupDoor({ id: this.deviceId }).then(res => {
          if (res.result === 1) {
            this.onQueryMsgStatus(res.data.msg_id)
          } else {
            this.$message({
              message: res.message,
              type: 'success'
            })
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
        queryMsgPushResult({ deviceId: _this.deviceId, msg_id: msgId }).then(res => {
          if (res.result === 1) {
            loading.close()

            _this.$message({
              message: res.message,
              type: 'success'
            })

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
        queryMsgPushResult({ deviceId: _this.deviceId, msg_id: msgId }).then(res => {
          _this.$message(res.message)
        })
      }, 10000)
    }
  }
}
</script>
<style lang="scss" scoped>

#device_controlcenter{

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
