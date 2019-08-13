<template>
  <div id="productsku_container" class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="名称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="所属模块" prop="kindIds">
        <treeselect
          v-model="form.kindIds"
          :multiple="true"
          :options="treeselect_kind_options"
          :normalizer="treeselect_kind_normalizer"
          :flat="true"
          sort-value-by="INDEX"
          :default-expand-level="99"
          placeholder="选择"
          no-children-text=""
        />

      </el-form-item>
      <el-form-item label="所属栏目" prop="subjectIds">
        <treeselect
          v-model="form.subjectIds"
          :multiple="true"
          :options="treeselect_subject_options"
          :normalizer="treeselect_subject_normalizer"
          :flat="true"
          sort-value-by="INDEX"
          :default-expand-level="99"
          placeholder="选择"
          no-children-text=""
        />
      </el-form-item>
      <el-form-item label="销售价" prop="salePrice">
        <el-input v-model="form.salePrice" />
      </el-form-item>
      <el-form-item label="展示价" prop="showPrice">
        <el-input v-model="form.showPrice" />
      </el-form-item>
      <el-form-item label="图片" prop="dispalyImgUrls">
        <el-upload
          action="http://upload.17fanju.com/Api/ElementUploadImage"
          list-type="picture-card"
          :on-success="handleSuccess"
          :on-remove="handleRemove"
          :on-error="handleError"
          :file-list="fileList"
          :limit="4"
        >
          <i class="el-icon-plus" />
        </el-upload>

      </el-form-item>
      <el-form-item label="简短描述">
        <el-input v-model="form.briefInfo" />
      </el-form-item>
      <el-form-item label="商品详情" />
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { addProductSku, initAddProductSku } from '@/api/productsku'
import fromReg from '@/utils/formReg'
import { goBack, treeselectNormalizer } from '@/utils/commonUtil'
import Treeselect from '@riophae/vue-treeselect'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'

export default {
  components: { Treeselect },
  data() {
    return {
      headers: { 'Content-Type': 'multipart/form-data' },
      form: {
        name: '',
        kindIds: [],
        subjectIds: [],
        showPrice: '',
        salePrice: '',
        detailsDes: '',
        specDes: '',
        briefDes: '',
        dispalyImgUrls: []
      },
      fileList: [],
      rules: {
        name: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        kindIds: [{ type: 'array', required: true, message: '至少必选一个,且必须少于3个', max: 3 }],
        subjectIds: [{ type: 'array', required: true, message: '至少必选一个,且必须少于3个', max: 3 }],
        salePrice: [{ required: true, message: '金额格式,eg:88.88', pattern: fromReg.money }],
        showPrice: [{ required: true, message: '金额格式,eg:88.88', pattern: fromReg.money }],
        dispalyImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        briefDes: [{ required: false, min: 0, max: 200, message: '不能超过200个字符', trigger: 'change' }]
      },
      treeselect_kind_normalizer: treeselectNormalizer,
      treeselect_kind_options: [],
      treeselect_subject_normalizer: treeselectNormalizer,
      treeselect_subject_options: []
    }
  },
  mounted() {

  },
  created() {
    this.init()
  },
  methods: {
    init() {
      initAddProductSku().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.treeselect_subject_options = d.subjects
          this.treeselect_kind_options = d.kinds
        }
      })
    },
    resetForm() {

    },
    onSubmit() {
      console.log(JSON.stringify(this.form))
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            addProductSku(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                goBack(this)
              }
            })
          })
        }
      })
    },
    handleRemove(file, fileList) {
      this.fileList = fileList
    },
    handleSuccess(response, file, fileList) {
      this.fileList = fileList
    },
    handleError(errs, file, fileList) {
      this.fileList = fileList
    }
  }
}
</script>

<style scoped>
#productsku_container {
  max-width: 600px;
}

</style>

