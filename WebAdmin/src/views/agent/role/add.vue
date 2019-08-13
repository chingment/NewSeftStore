<template>
  <div id="useradd_container" class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="角色名称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="描述" prop="description">
        <el-input v-model="form.description" type="textarea" />
      </el-form-item>
      <el-form-item label="菜单">
        <el-tree
          ref="treemenu"
          :data="tree_menu_options"
          :props="tree_menu_props"
          node-key="id"
          class="filter-tree"
          show-checkbox
          default-expand-all
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
import { addRole, initAddRole } from '@/api/agentrole'
import { getCheckedKeys, goBack } from '@/utils/commonUtil'
export default {
  data() {
    return {
      form: {
        name: '',
        description: '',
        menuIds: []
      },
      rules: {
        name: [{ required: true, min: 1, max: 20, message: '必填,且不能超过20个字符', trigger: 'change' }],
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
      initAddRole().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.tree_menu_options = d.menus
        }
      })
    },
    resetForm() {
      this.form = {
        name: '',
        description: '',
        menuIds: []
      }
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
            addRole(this.form).then(res => {
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
.el-tree-node__expand-icon.is-leaf{
  display: none;
}
</style>

