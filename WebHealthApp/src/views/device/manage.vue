<template>
  <div id="pg_device_manage">
    <div class="step-1">
      <div class="lm-header-big">
        <div class="bg-title">设备管理</div>
        <div class="sm-title">以下是您已绑定的设备</div>
      </div>

      <div v-for="(item, index) in devices" :key="index" class="device">
        <div class="form">
          <mt-cell title="设备号" is-link @click.native="onInfo(item)">
            <span>{{ item.id }}</span>
          </mt-cell>
          <mt-cell title="使用者">
            <span>{{ item.signName }}</span>
          </mt-cell>
          <mt-cell title="绑定状态">
            <span>{{ item.bindStatus.text }}</span>
          </mt-cell>
          <mt-cell v-if="item.bindStatus.value==2" title="绑定时间">
            <span>{{ item.bindTime }}</span>
          </mt-cell>
          <mt-cell v-if="item.bindStatus.value==2" title="在线状态">
            <span>{{ item.onLineStatus.text }}</span>
          </mt-cell>
          <mt-cell v-if="item.bindStatus.value==3" title="解绑时间">
            <span>{{ item.unBindTime }}</span>
          </mt-cell>
        </div>
        <mt-button v-if="item.bindStatus.value==2" class="btn-unbind" type="primary" @click="onUnBind(item)">解绑</mt-button>
      </div>
    </div>

  </div>

</template>
<script>
import { initManage, unBind } from '@/api/device'
import { isEmpty } from '@/utils/commonUtil'
var socket = []
export default {
  name: 'DeviceManage',
  components: {
  },
  data() {
    return {
      loading: false,
      appInfo: {},
      devices: [],
      socket: ''
    }
  },
  created() {
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      initManage({ }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.appInfo = d.appInfo
          this.devices = d.devices
          if (d.devices != null && d.devices.length > 0) {
            for (var i = 0; i < d.devices.length; i++) {
              if (!isEmpty(d.devices[i].webUrl)) {
                this.connectDevice(d.devices[i].webUrl, d.devices[i].id)
              }
            }
          }
        }
        this.loading = false
      })
    },
    onUnBind(item) {
      this.$messagebox.confirm('确定要解绑设备?').then(action => {
        this.loading = true
        unBind({ deviceId: item.id }).then(res => {
          this.$toast(res.message)
          if (res.result === 1) {
            this.onInit()
          }
          this.loading = false
        })
      })
    },
    onInfo(item) {
      this.$router.push({ path: '/device/info', query: {
        deviceId: item.id
      }})
    },
    connectDevice: function(host, wsobj) {
      var _this = this
      // 浏览器支持？
      if ('WebSocket' in window) {
        socket[wsobj] = new WebSocket(host)
        try {
          // 连接事件
          socket[wsobj].onopen = function(msg) {
            console.log('socket连接成功')
            socket[wsobj].send(JSON.stringify({ 'online': wsobj }))
            // alert(wsobj+":连接已建立！");
          }
          // 错误事件
          socket[wsobj].onerror = function(msg) {
            console.log('连接错误')
          }

          // 消息事件
          socket[wsobj].onmessage = function(msg) {
            var data = JSON.parse(msg.data)
            var devices = _this.devices
            for (var i = 0; i < devices.length; i++) {
              if (devices[i].id === wsobj) {
                if (data.online === '1') {
                  _this.devices[i].onLineStatus = { text: '在线', value: 1 }
                } else {
                  _this.devices[i].onLineStatus = { text: '离线', value: 2 }
                }
              }
            }
          }
          // 关闭事件
          socket[wsobj].onclose = function(msg) {
            console.log('socket已经关闭')
          }
        } catch (ex) {
          console.log(ex)
        }
      }
    }
  }
}
</script>

<style lang="scss" scope>

#pg_device_manage {
  padding: 20px;

  .form {
    padding: 50px 0;
  }

  .btn-unbind {
    width: 100%;
  }
}

</style>
