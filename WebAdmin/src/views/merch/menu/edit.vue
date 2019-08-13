<template>
  <div id="useradd_container" class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="75px">
      <el-form-item label="上构名称">
        {{ form.pMenuName }}
      </el-form-item>
      <el-form-item label="上级标题">
        {{ form.pMenuTitle }}
      </el-form-item>
      <el-form-item label="名称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="标题" prop="title">
        <el-input v-model="form.title" />
      </el-form-item>
      <el-form-item label="图标" prop="icon">
        <el-input v-model="form.icon" />
      </el-form-item>
      <el-form-item label="路径" prop="path">
        <el-input v-model="form.path" />
      </el-form-item>
      <el-form-item label="是否路由" prop="isRouter">
        <el-switch v-model="form.isRouter" />
      </el-form-item>
      <el-form-item label="左边导航" prop="isSidebar">
        <el-switch v-model="form.isSidebar" />
      </el-form-item>
      <el-form-item label="头像导航" prop="isNavbar">
        <el-switch v-model="form.isNavbar" />
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
import { editMenu, initEditMenu } from '@/api/merchmenu'
import fromReg from '@/utils/formReg'
import { getUrlParam, goBack } from '@/utils/commonUtil'
export default {
  data() {
    return {
      isOpenEditPassword: false,
      form: {
        pMenuName: '',
        pMenuTitle: '',
        menuId: '',
        name: '',
        title: '',
        icon: '',
        path: '',
        isRouter: false,
        isSidebar: false,
        isNavbar: false,
        description: ''
      },
      rules: {
        name: [{ required: true, message: '必填,且由3到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.userName }],
        title: [{ required: true, min: 1, max: 20, message: '必填,且不能超过20个字符', trigger: 'change' }],
        description: [{ required: false, min: 0, max: 500, message: '不能超过500个字符', trigger: 'change' }]
      }
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var menuId = getUrlParam('menuId')
      initEditMenu({ menuId: menuId }).then(res => {
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
            editMenu(this.form).then(res => {
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

