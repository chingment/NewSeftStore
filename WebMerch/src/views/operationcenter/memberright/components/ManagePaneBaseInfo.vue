<template>
  <div id="memberright_baseinfo">
    <el-form
      ref="form"
      v-loading="loading"
      :model="form"
      :rules="rules"
      label-width="80px"
      :hide-required-asterisk="!isEdit"
    >
      <el-form-item label="名称" prop="name" :show-message="isEdit">
        <span>{{ temp.name }}</span>
      </el-form-item>
      <el-form-item label="折扣" prop="discount" :show-message="isEdit">
        <span v-show="!isEdit">{{ temp.discount }}</span>
        <el-input v-show="isEdit" v-model="form.discount" placeholder="请输入内容" style="width:100px" />
      </el-form-item>
      <el-form-item>
        <el-button v-show="!isEdit" type="primary" @click="openEdit">编辑</el-button>
        <el-button v-show="isEdit" type="info" @click="cancleEdit">取消</el-button>
        <el-button v-show="isEdit" type="primary" @click="handleSetLevelSt">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>
<script>
import { MessageBox } from 'element-ui'
import { edit, getLevelSt, setLevelSt } from '@/api/memberright'
import { getUrlParam, isEmpty } from '@/utils/commonUtil'
import Sortable from 'sortablejs'
import { all } from 'q'

export default {
  name: 'ManagePaneBaseInfo',
  props: {
    levelstId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      isEdit: false,
      loading: false,
      temp: {
        id: '',
        name: '',
        tag: '',
        discount: ''
      },
      form: {
        id: '',
        discount: ''
      },
      rules: {}
    }
  },
  watch: {
    levelstId: function(val, oldval) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      if (!isEmpty(this.levelstId)) {
        this.loading = true
        var id = this.levelstId
        getLevelSt({ id: id }).then(res => {
          if (res.result === 1) {
            var d = res.data
            this.temp.name = d.name
            this.temp.tag = d.tag
            this.temp.discount = d.discount
            this.form.levelStId = this.levelstId
            this.form.discount = d.discount
          }
          this.loading = false
        })
      }
    },
    openEdit() {
      this.isEdit = true
    },
    cancleEdit() {
      this.isEdit = false
    },
    handleSetLevelSt() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            setLevelSt(this.form).then(res => {
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
    }
  }
}
</script>
<style lang="scss" scoped>
#dsss .el-form-item__label:before {
  content: "";
  margin-left: 1000px;
}

#memberright_baseinfo {
  .el-form .el-form-item {
    max-width: 600px;
  }

  .el-upload-list >>> .sortable-ghost {
    opacity: 0.8;
    color: #fff !important;
    background: #42b983 !important;
  }

  .el-upload-list >>> .el-tag {
    cursor: pointer;
  }

  .bm-view {
    width: 100%;
    height: 200px;
    margin-top: 20px;
  }

  .autoAddressClass {
    li {
      display: flex;
      i.el-icon-search {
        margin-top: 11px;
      }
      .mgr10 {
        margin-right: 10px;
      }
      .title {
        text-overflow: ellipsis;
        overflow: hidden;
      }

      .address-ct {
        flex: 1;
      }

      .address {
        line-height: 1;
        font-size: 12px;
        color: #b4b4b4;
        margin-bottom: 5px;
      }
    }
  }
}
</style>
