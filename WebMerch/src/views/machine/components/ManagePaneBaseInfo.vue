<template>
  <div id="machine_baseinfo" v-loading="loading" class="app-container">

    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="100px" :hide-required-asterisk="!isEdit">
      <el-form-item label="机器编号">
        {{ temp.id }}
      </el-form-item>
      <el-form-item label="机器Logo">
        <img v-show="!isEdit" :src="temp.logoImgUrl" class="singlepic-machine-banner">

        <el-upload
          v-show="isEdit"
          class="singlepic-uploader"
          :action="uploadImgServiceUrl"
          :show-file-list="false"
          :on-success="uploadSuccessHandle"
          :before-upload="uploadBeforeHandle"
        >
          <img v-if="form.logoImgUrl" :src="form.logoImgUrl" class="singlepic-machine-banner">
          <i v-else class="el-icon-plus singlepic-uploader-icon singlepic-machine-banner" />
        </el-upload>

      </el-form-item>
      <el-form-item label="所属店铺">
        {{ temp.storeName }}
      </el-form-item>
      <el-form-item label="控制程序号">
        {{ temp.ctrlSdkVersion }}
      </el-form-item>
      <el-form-item label="应用程序号">
        {{ temp.appVersion }}
      </el-form-item>
      <el-form-item label="机器状态">
        {{ temp.status.text }}
      </el-form-item>
      <el-form-item label="最后运行时间">
        {{ temp.lastRequestTime }}
      </el-form-item>
      <el-form-item>
        <el-button v-show="!isEdit" type="primary" @click="openEdit">编辑</el-button>
        <el-button v-show="isEdit" type="info" @click="cancleEdit">取消</el-button>
        <el-button v-show="isEdit" type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { edit, initManageBaseInfo } from '@/api/machine'
import { getUrlParam } from '@/utils/commonUtil'
import { all } from 'q'

export default {
  name: 'ManagePaneBaseInfo',
  data() {
    return {
      isEdit: false,
      loading: false,
      temp: {
        name: '',
        logoImgUrl: '',
        status: {
          text: '',
          value: ''
        }
      },
      form: {
        id: '',
        name: '',
        logoImgUrl: ''
      },
      rules: {
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }]
      },
      uploadImglist: [],
      uploadImgPreImgDialogUrl: '',
      uploadImgPreImgDialogVisible: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL
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
      this.loading = true
      var id = getUrlParam('id')
      initManageBaseInfo({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form.id = d.id
          this.form.name = d.name
          this.form.logoImgUrl = d.logoImgUrl

          this.temp.id = d.id
          this.temp.name = d.name
          this.temp.logoImgUrl = d.logoImgUrl
          this.temp.status = d.status
          this.temp.ctrlSdkVersion = d.ctrlSdkVersion
          this.temp.appVersion = d.appVersion
          this.temp.storeName = d.storeName
          this.temp.lastRequestTime = d.lastRequestTime
        }
        this.loading = false
      })
    },
    onSubmit() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            edit(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this.isEdit = false
                this.init()
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    openEdit() {
      this.isEdit = true
    },
    cancleEdit() {
      this.isEdit = false
    },
    uploadBeforeHandle(file) {
      var imgtype = file.name.toLowerCase().split('.')[1]

      if (imgtype !== 'png' && imgtype !== 'jpeg' && imgtype !== 'jpg') {
        this.$message.error('上传图片只能是 jpg,png 格式!')
        return false
      }
      const isLt4M = file.size / 1024 / 1024 < 4
      if (!isLt4M) {
        this.$message.error('上传图片大小不能超过 4MB!')
        return false
      }
      return true
    },
    uploadSuccessHandle(response, file, fileList) {
      this.form.logoImgUrl = response.data.url
    }

  }
}
</script>
<style lang="scss" scoped>

#machine_baseinfo{
.el-form .el-form-item{
  max-width: 600px;
}

.singlepic-machine-banner{
  width: 500px;
  height: 47px;
  line-height: 47px;
  font-size: 16px;
}
}

</style>
