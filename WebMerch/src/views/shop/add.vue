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
      <el-form-item label="门店图片" prop="displayImgUrls" class="el-form-item-upload">
        <el-input :value="form.displayImgUrls.toString()" style="display:none" />
        <lm-upload
          v-model="form.displayImgUrls"
          list-type="picture-card"
          :file-list="form.displayImgUrls"
          :action="uploadFileServiceUrl"
          :headers="uploadFileHeaders"
          :data="{folder:'shop'}"
          ext=".jpg,.png,.jpeg"
          tip="图片500*500，格式（jpg,png）不超过4M；第一张为主图，可拖动改变图片顺序"
          :max-size="1024"
          :sortable="true"
          :limit="4"
        />

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
import LmUpload from '@/components/Upload/index.vue'
import { getToken } from '@/utils/auth'
export default {
  name: 'ShopAdd',
  components: { PageHeader, SelectAddressPoint, LmUpload },
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
      uploadFileHeaders: {},
      uploadFileServiceUrl: process.env.VUE_APP_UPLOAD_FILE_SERVICE_URL,
      dialogIsShowBySelectAddressPoint: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {

  },
  created() {
    this.uploadFileHeaders = { 'X-Token': getToken() }
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
    }
  }
}
</script>
