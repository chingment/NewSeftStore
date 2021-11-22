<template>
  <div id="device_controlcenter">
    <div class="pane-ctl">
      <div class="title"><span>系统相关</span>
      </div>
      <div class="content">

        <el-button type="primary" style="margin-bottom:20px;margin-right: 10px;" @click="onOpenDialogSetSysStatus">设置状态</el-button>
        <el-button type="primary" style="margin-bottom:20px;margin-right: 10px;margin-left: 0px;" @click="onOpenDialogSetSysParams">参数设置</el-button>
        <el-button type="primary" style="margin-bottom:20px;margin-right: 10px;margin-left: 0px;" @click="onUpdateApp">远程更新</el-button>
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

    <el-dialog title="设置状态" :visible.sync="dialogSetSysStatusIsVisible" :width="isDesktop==true?'600px':'90%'">
      <div>

        <el-form ref="formBySetSysStatus" :model="formBySetSysStatus" :rules="rulesBySetSysStatus" label-width="80px">
          <el-form-item label="设备编码">
            <span>{{ deviceId }}</span>
          </el-form-item>
          <el-form-item label="状态" prop="status">
            <el-radio v-model="formBySetSysStatus.status" label="1">正常</el-radio>
            <el-radio v-model="formBySetSysStatus.status" label="2">维护中</el-radio>
          </el-form-item>
        </el-form>

      </div>
      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogSetSysStatusIsVisible = false">
          取消
        </el-button>
        <el-button type="primary" size="small" @click="onSetSysStatus()">
          设置
        </el-button>
      </div>
    </el-dialog>

    <el-dialog title="参数设置" :visible.sync="dialogSetSysParamsIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div v-loading="dialogSetSysParamsIsLoading">

        <el-form ref="formBySetSysParams" :model="formBySetSysParams" :rules="rulesBySetSysParams" label-width="80px">
          <div class="row-title clearfix">
            <div class="pull-left"> <h5> 基本信息</h5>
            </div>
          </div>
          <el-form-item label="设备编码">
            <span>{{ deviceId }}</span>
          </el-form-item>

          <div class="row-title clearfix">
            <div class="pull-left"> <h5> 门边灯光设置</h5>
            </div>
          </div>

          <el-row v-for="(cbLight, index) in formBySetSysParams.cbLight" :key="index">
            <el-col :span="12">
              <el-form-item label-width="80px" :label="'时间段' + (index+1)" class="postInfo-container-item">

                <el-time-select
                  v-model="formBySetSysParams.cbLight[index].start"
                  placeholder="起始时间"
                  :picker-options="{
                    start: '00:00',
                    step: '01:00',
                    end: '24:00',
                  }"
                  style="width:120px"
                />
                <el-time-select
                  v-model="formBySetSysParams.cbLight[index].end"
                  placeholder="结束时间"
                  :picker-options="{
                    start: '00:00',
                    step: '01:00',
                    end: '24:00',
                    minTime: formBySetSysParams.cbLight[index].start
                  }"
                  style="width:120px"
                />

              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label-width="80px" label="光度" class="postInfo-container-item">
                <el-select v-model="formBySetSysParams.cbLight[index].value" placeholder="请选择">
                  <el-option
                    v-for="item in options_cbLightValues"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                  />
                </el-select>

              </el-form-item>
            </el-col>
          </el-row>

        </el-form>

      </div>
      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogSetSysParamsIsVisible = false">
          取消
        </el-button>
        <el-button type="primary" size="small" @click="onSetSysParams()">
          保存
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { rebootSys, shutdownSys, setSysStatus, getSysParams, setSysParams, openPickupDoor, updateApp } from '@/api/devvending'

export default {
  name: 'DeviceVendingPaneBaseInfo',
  props: {
    deviceId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      dialogSetSysParamsIsVisible: false,
      dialogSetSysStatusIsVisible: false,
      dialogSetSysParamsIsLoading: false,
      formBySetSysStatus: {
        status: undefined,
        helpTip: '设备正在维护中'
      },
      rulesBySetSysStatus: {
        status: [{ required: true, message: '请选择状态', trigger: 'change' }],
        helpTip: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }]
      },
      formBySetSysParams: {
        cbLight: [
          { start: '00:00', end: '06:00', value: 1 },
          { start: '06:00', end: '18:00', value: 2 },
          { start: '18:00', end: '24:00', value: 3 }
        ]
      },
      rulesBySetSysParams: {

      },
      options_cbLightValues: [{
        value: 1,
        label: '100%'
      }, {
        value: 2,
        label: '75%'
      }, {
        value: 3,
        label: '50%'
      }, {
        value: 4,
        label: '25%'
      }, {
        value: 5,
        label: '关'
      }],
      handleLoading: null,
      isDesktop: this.$store.getters.isDesktop
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
        this.showLoading()
        rebootSys({ id: this.deviceId }).then(res => {
          this.hideLoading()
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
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
    onUpdateApp() {
      MessageBox.confirm('确定要远程更新App？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.showLoading()
        updateApp({ id: this.deviceId }).then(res => {
          this.hideLoading()
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
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
        this.showLoading()
        shutdownSys({ id: this.deviceId }).then(res => {
          this.hideLoading()
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
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
    onOpenDialogSetSysParams() {
      this.dialogSetSysParamsIsVisible = true
      this.dialogSetSysParamsIsLoading = true
      getSysParams({ id: this.deviceId }).then(res => {
        this.dialogSetSysParamsIsLoading = false

        if (res.result === 1) {
          var d = res.data
          this.formBySetSysParams.id = d.id
          this.formBySetSysParams.cbLight = d.cbLight
        }
        this.loading = false
      })
    },
    onSetSysStatus() {
      this.$refs['formBySetSysStatus'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要设置状态？', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            this.showLoading()
            setSysStatus({ id: this.deviceId, status: this.formBySetSysStatus.status, helpTip: this.formBySetSysStatus.helpTip }).then(res => {
              this.hideLoading()
              if (res.result === 1) {
                this.dialogSetSysStatusIsVisible = false
                this.$message({
                  message: res.message,
                  type: 'success'
                })
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
    onSetSysParams() {
      this.$refs['formBySetSysParams'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存系统参数？', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            this.showLoading()
            setSysParams({ id: this.deviceId, cbLight: this.formBySetSysParams.cbLight }).then(res => {
              this.hideLoading()
              if (res.result === 1) {
                this.dialogSetSysParamsIsVisible = false
                this.$message({
                  message: res.message,
                  type: 'success'
                })
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
        this.showLoading()
        openPickupDoor({ id: this.deviceId }).then(res => {
          this.hideLoading()
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
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
    showLoading() {
      this.handleLoading = this.$loading({
        lock: true,
        text: '正常处理中，请耐心等候，大概需要10秒',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      })
    },
    hideLoading() {
      if (this.handleLoading != null) {
        this.handleLoading.close()
      }
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
