<template>
  <div id="useradd_container" class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="75px">
      <el-form-item label="上级名称">
        {{ form.pOrgName }}
      </el-form-item>
      <el-form-item label="名称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="描述" prop="description">
        <el-input v-model="form.description" type="textarea" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { editOrg, initEditOrg } from '@/api/adminorg'
import fromReg from '@/utils/formReg'
import { getUrlParam, goBack } from '@/utils/commonUtil'
export default {
  data() {
    return {
      isOpenEditPassword: false,
      form: {
        pOrgName: '',
        orgId: '',
        name: '',
        description: ''
      },
      rules: {
        name: [{ required: true, min: 1, max: 20, message: '必填,且不能超过20个字符', trigger: 'change' }],
        description: [{ required: false, min: 0, max: 500, message: '不能超过500个字符', trigger: 'change' }]
      }
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var orgId = getUrlParam('orgId')
      initEditOrg({ orgId: orgId }).then(res => {
        if (res.result === 1) {
          this.form = res.data
        }
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
            editOrg(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                goBack(this)
              }
            })
          })
        }
      })
    }
  }
}
</script>

<style scoped>
.line {
  text-align: center;
}
#useradd_container {
  max-width: 600px;
}
</style>

