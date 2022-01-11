<template>
  <div id="pg_device_bind">
    <div class="step-1">
      <div class="lm-header-big">
        <div class="bg-title">设备解绑</div>
        <div class="sm-title">以下是您已绑定的设备</div>
      </div>
      <div class="form">
        <mt-cell title="使用者" />
        <mt-field label="设备号" placeholder="请输入S/N号">
          <img src="@/assets/images/icon_scan_code.png" height="32px" width="32px">
        </mt-field>
      </div>
      <mt-button class="btn-scan" type="primary" @click="onSaveStep1">解绑</mt-button>
    </div>

  </div>

</template>
<script>
import { initInfo } from '@/api/device'
export default {
  name: 'App',
  components: {
  },
  data() {
    return {
      loading: false,
      appInfo: {}
    }
  },
  created() {
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      initInfo({ }).then(res => {
        if (res.result === 1) {
          var d = res.data
        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scope>

#pg_device_bind{
    padding: 20px;

.form{
    padding: 50px 0px;
}

    .btn-scan{
        width: 100%;
    }
}

.popup-toolbar{
    border-bottom-width: 1px;
    border-bottom-style: solid;
    border-bottom-color: rgb(234, 234, 234);
    height: 40px;
    .close{
    width: 2em;
    height: 2rem;
    right: 0;
    position: absolute;
    top: 2px;
    }
}

.pa-box{
  text-align: center;

  .img-pa{
    height: 300px;
    width: 300px;
  }

  .tip{
    padding: 30px;
  }
}
</style>
