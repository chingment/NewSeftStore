<template>
  <div id="store_baseinfo_container" v-loading="loading" class="app-container">

    <el-form ref="form" label-width="80px">
      <el-form-item label="名称">
        {{ form.name }}
      </el-form-item>
      <el-form-item label="地址">
        {{ form.address }}
      </el-form-item>
      <el-form-item label="图片">
        <el-upload
          ref="uploadImg"
          v-model="form.dispalyImgUrls"
          :action="uploadImgServiceUrl"
          list-type="picture-card"
          :on-success="handleSuccess"
          :on-remove="handleRemove"
          :on-error="handleError"
          :on-preview="handlePreview"
          :file-list="uploadImglist"
          :limit="4"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisible">
          <img width="100%" :src="uploadImgPreImgDialogUrl" alt="">
        </el-dialog>
      </el-form-item>
      <el-form-item label="简短描述" style="max-width:1000px">
        <el-input v-model="form.briefDes" type="text" maxlength="200" show-word-limit />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">编辑</el-button>
      </el-form-item>
    </el-form>

    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="名称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="地址" prop="name">
        <el-input v-model="form.address" />
      </el-form-item>
      <el-form-item label="图片" prop="dispalyImgUrls">
        <el-input :value="form.dispalyImgUrls.toString()" style="display:none" />
        <el-upload
          ref="uploadImg"
          v-model="form.dispalyImgUrls"
          :action="uploadImgServiceUrl"
          list-type="picture-card"
          :on-success="handleSuccess"
          :on-remove="handleRemove"
          :on-error="handleError"
          :on-preview="handlePreview"
          :file-list="uploadImglist"
          :limit="4"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisible">
          <img width="100%" :src="uploadImgPreImgDialogUrl" alt="">
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：第一张默认为主图，可拖动改变图片顺便</div>
      </el-form-item>
      <el-form-item label="简短描述" style="max-width:1000px">
        <el-input v-model="form.briefDes" type="text" maxlength="200" show-word-limit />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>

  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { editStore, initEditStore } from '@/api/store'
import { goBack } from '@/utils/commonUtil'
import Sortable from 'sortablejs'

export default {
  name: 'ManagePaneBaseInfo',
  props: {
    storeId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
      form: {
        name: '',
        address: '',
        briefDes: '',
        dispalyImgUrls: []
      },
      rules: {
        name: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        address: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        dispalyImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        briefDes: [{ required: false, min: 0, max: 200, message: '不能超过200个字符', trigger: 'change' }]
      },
      uploadImglist: [],
      uploadImgPreImgDialogUrl: '',
      uploadImgPreImgDialogVisible: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initEditStore({ id: this.storeId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form.storeId = d.storeId
          this.form.name = d.name
          this.form.address = d.address
          this.form.briefDes = d.briefDes
        }
        this.loading = false
      })
    }
  }
}
</script>
