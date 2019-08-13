<template>
  <div id="useradd_container" class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="角色名称" prop="userName">
        {{ form.name }}
      </el-form-item>
      <el-form-item label="描述">
        <el-input v-model="form.description" type="textarea" />
      </el-form-item>
      <el-form-item label="菜单">
        <el-tree
          ref="treemenu"
          :check-strictly="true"
          :data="tree_menu_options"
          :props="tree_menu_props"
          node-key="id"
          class="filter-tree"
          show-checkbox
          default-expand-all
          :default-checked-keys="form.menuIds"
        />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { editRole, initEditRole } from '@/api/agentrole'
import { getUrlParam, getCheckedKeys, goBack } from '@/utils/commonUtil'
export default {
  data() {
    return {
      form: {
        roleId: '',
        name: '',
        description: '',
        menuIds: []
      },
      rules: {
        description: [{ required: false, min: 0, max: 500, message: '不能超过500个字符', trigger: 'change' }]
      },
      tree_menu_options: [],
      tree_menu_props: {
        children: 'children',
        label: 'label'
      }
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var roleId = getUrlParam('roleId')
      initEditRole({ roleId: roleId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form.roleId = d.roleId
          this.form.name = d.name
          this.form.description = d.description
          this.form.menuIds = d.menuIds
          this.tree_menu_options = d.menus
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
            this.form.menuIds = getCheckedKeys(this.$refs.treemenu)
            editRole(this.form).then(res => {
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

