<template>
  <div id="shop_add">
    <page-header />
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="门店名称" prop="name">
        <el-input v-model="form.name" clearable style="max-width:500px" />
      </el-form-item>
      <el-form-item label="门店地址" prop="address">
        <el-input v-model="form.address" clearable style="width:450px" />
        <el-button type="text" @click="dialogIsShowBySelectAddressPoint=true">选择</el-button>
      </el-form-item>
      <el-form-item label="联系人" prop="contactName">
        <el-input v-model="form.contactName" clearable style="width:200px" />
      </el-form-item>
      <el-form-item label="联系电话" prop="contactPhone">
        <el-input v-model="form.contactPhone" clearable style="width:200px" />
      </el-form-item>
      <el-form-item label="联系地址" prop="contactAddress">
        <el-input v-model="form.contactAddress" clearable style="max-width:500px" />
      </el-form-item>
      <el-form-item label="门店图片" prop="displayImgUrls">
        <el-input :value="form.displayImgUrls.toString()" style="display:none" />
        <el-upload
          ref="uploadImg"
          v-model="form.displayImgUrls"
          :action="uploadImgServiceUrl"
          list-type="picture-card"
          :before-upload="uploadBeforeHandle"
          :on-success="uploadSuccessHandle"
          :on-remove="uploadRemoveHandle"
          :on-error="uploadErrorHandle"
          :on-preview="uploadPreviewHandle"
          :file-list="uploadImglist"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisible">
          <img width="100%" :src="uploadImgPreImgDialogUrl" alt="">
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：图片500*500，格式（jpg,png）不超过4M；第一张为主图，可拖动改变图片顺序</div>
      </el-form-item>
      <el-form-item label="简短描述" style="max-width:1000px">
        <el-input v-model="form.briefDes" type="text" maxlength="200" clearable show-word-limit />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSave">保存</el-button>
      </el-form-item>
    </el-form>
    <el-dialog v-if="dialogIsShowBySelectAddressPoint" title="选择位置" :visible.sync="dialogIsShowBySelectAddressPoint" width="800px" append-to-body>
      <select-address-point :select-method="onSelectAddressPoint" :cur-adddress="form.addressDetails" />
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { save } from '@/api/shop'
import PageHeader from '@/components/PageHeader/index.vue'
import SelectAddressPoint from '@/components/SelectAddressPoint/index.vue'
export default {
  name: 'ShopAdd',
  components: { PageHeader, SelectAddressPoint },
  data() {
    return {
      loading: false,
      form: {
        id: '',
        name: '',
        areaCode: '',
        areaName: '',
        address: '',
        contactName: '',
        contactPhone: '',
        contactAddress: '',
        briefDes: '',
        displayImgUrls: [],
        addressDetails: null
      },
      rules: {
        name: [{ required: true, min: 1, max: 30, message: '必填,且不能超过30个字符', trigger: 'change' }],
        address: [{ required: true, min: 1, max: 512, message: '请选择' }],
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        briefDes: [{ required: false, min: 0, max: 200, message: '不能超过200个字符', trigger: 'change' }]
      },
      uploadImglist: [],
      uploadImgMaxSize: 4,
      uploadImgPreImgDialogUrl: '',
      uploadImgPreImgDialogVisible: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
      dialogIsShowBySelectAddressPoint: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {

  },
  created() {

  },
  methods: {
    onSave() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            this.loading = true
            save(this.form).then(res => {
              this.loading = false
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.$router.push({
                  path: '/shop/list'
                })
              } else {
                this.$message({
                  message: res.message,
                  type: 'error'
                })
              }
            })
          })
        }
      })
    },
    onSelectAddressPoint(rs) {
      this.form.addressDetails = rs
      this.form.address = rs.address
      this.dialogIsShowBySelectAddressPoint = false
    },
    getUploadImglist(displayImgUrls) {
      var _uploadImglist = []
      if (displayImgUrls !== null) {
        for (var i = 0; i < displayImgUrls.length; i++) {
          _uploadImglist.push({ status: 'success', url: displayImgUrls[i].url, response: { data: { name: displayImgUrls[i].name, url: displayImgUrls[i].url }}})
        }
      }

      return _uploadImglist
    },
    getdisplayImgUrls(fileList) {
      var _displayImgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _displayImgUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _displayImgUrls
    },
    uploadBeforeHandle(file) {
      if (this.form.displayImgUrls.length >= this.uploadImgMaxSize) {
        this.$message.error('上传图片不能超过4张!')
        return false
      }

      const imgType = file.type
      const isLt4M = file.size / 1024 / 1024 < 4
      //  var a = isLt4M === true ? 'true' : 'false'
      if (imgType !== 'image/jpeg' && imgType !== 'image/png' && imgType !== 'image/jpg') {
        this.$message('图片格式仅支持(jpg,png)')
        return false
      }

      if (!isLt4M) {
        this.$message('图片大小不能超过4M')
        return false
      }

      return true
    },
    uploadCardCheckShow() {
      var uploadcard = this.$refs.uploadImg.$el.querySelectorAll('.el-upload--picture-card')
      if (this.form.displayImgUrls.length === this.uploadImgMaxSize) {
        uploadcard[0].style.display = 'none'
      } else {
        uploadcard[0].style.display = 'inline-block'
      }
    },
    uploadRemoveHandle(file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
      this.uploadCardCheckShow()
    },
    uploadSuccessHandle(response, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
      this.uploadCardCheckShow()
    },
    uploadErrorHandle(errs, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    uploadPreviewHandle(file) {
      this.uploadImgPreImgDialogUrl = file.url
      this.uploadImgPreImgDialogVisible = true
    }
  }
}
</script>
