<template>
  <div id="useradd_container" class="app-container">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="用户名" prop="userName">
        {{ form.userName }}
      </el-form-item>
      <el-form-item v-show="!isOpenEditPassword" label="密码">
        <span>********</span>
        <span @click="openEditPassword()">修改</span>
      </el-form-item>
      <el-form-item v-show="isOpenEditPassword" label="密码" prop="password">
        <div style="display:flex">
          <div style="flex:1">
            <el-input v-model="form.password" type="password" />
          </div>
          <div style="width:50px;text-align: center;">
            <span @click="openEditPassword()">取消</span>
          </div>
        </div>
      </el-form-item>
      <el-form-item label="姓名" prop="fullName">
        <el-input v-model="form.fullName" />
      </el-form-item>
      <el-form-item label="所属机构" prop="orgIds">
        <treeselect
          v-model="form.orgIds"
          :multiple="true"
          :options="treeselect_org_options"
          :normalizer="treeselect_org_normalizer"
          :flat="true"
          sort-value-by="INDEX"
          :default-expand-level="99"
          placeholder="选择"
          no-children-text=""
        />
      </el-form-item>
      <el-form-item label="手机号码" prop="phoneNumber">
        <el-input v-model="form.phoneNumber" />
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="form.email" />
      </el-form-item>
      <el-form-item label="禁用">
        <el-switch v-model="form.isDisable" />
      </el-form-item>
      <el-form-item label="角色">
        <el-checkbox-group v-model="form.roleIds">
          <el-checkbox v-for="option in checkbox_group_role_options" :key="option.id" style="display:block" :label="option.id">{{ option.label }}</el-checkbox>
        </el-checkbox-group>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { editUser, initEditUser } from '@/api/adminuser'
import fromReg from '@/utils/formReg'
import { getUrlParam, goBack, treeselectNormalizer } from '@/utils/commonUtil'
import Treeselect from '@riophae/vue-treeselect'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'

export default {
  components: { Treeselect },
  data() {
    return {
      loading: false,
      isOpenEditPassword: false,
      form: {
        id: '',
        userName: '',
        password: '',
        fullName: '',
        phoneNumber: '',
        email: '',
        orgIds: [],
        roleIds: []
      },
      rules: {
        password: [{ required: false, message: '必填,且由6到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.password }],
        fullName: [{ required: true, message: '必填', trigger: 'change' }],
        orgIds: [{ required: true, message: '必选' }],
        phoneNumber: [{ required: false, message: '格式错误,eg:13800138000', trigger: 'change', pattern: fromReg.phoneNumber }],
        email: [{ required: false, message: '格式错误,eg:xxxx@xxx.xxx', trigger: 'change', pattern: fromReg.email }]
      },
      treeselect_org_normalizer: treeselectNormalizer,
      treeselect_org_options: [],
      checkbox_group_role_options: []
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      var id = getUrlParam('id')
      initEditUser({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form.id = d.id
          this.form.userName = d.userName
          this.form.fullName = d.fullName
          this.form.phoneNumber = d.phoneNumber
          this.form.email = d.email
          this.form.orgIds = d.orgIds
          this.form.roleIds = d.roleIds
          this.treeselect_org_options = d.orgs
          this.checkbox_group_role_options = d.roles
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
            editUser(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                goBack(this)
              }
            })
          })
        }
      })
    },
    openEditPassword() {
      if (this.isOpenEditPassword) {
        this.isOpenEditPassword = false
        this.form.password = ''
        this.rules.password[0].required = false
      } else {
        this.isOpenEditPassword = true
        this.rules.password[0].required = true
      }
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

.el-tree-node__expand-icon.is-leaf{
  display: none;
}
</style>

