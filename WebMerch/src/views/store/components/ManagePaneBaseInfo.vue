<template>
  <div id="store_baseinfo" v-loading="loading" class="app-container">

    <el-form v-show="!isEdit" class="noeditform" label-width="80px">
      <el-form-item label="名称">
        {{ temp.name }}
      </el-form-item>
      <el-form-item label="地址">
        {{ temp.address }}
      </el-form-item>
      <el-form-item label="图片">
        <el-upload
          action=""
          list-type="picture-card"
          disabled
          :file-list="temp.uploadImglist"
        />

      </el-form-item>
      <el-form-item label="简短描述" style="max-width:1000px">
        {{ temp.briefDes }}
      </el-form-item>
      <el-form-item label="营业状态">
        {{ temp.status.text }}
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="openEdit">编辑</el-button>
      </el-form-item>
    </el-form>

    <el-form v-show="isEdit" ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
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
      <el-form-item label="营业状态">
        <el-switch v-model="form.isClose" />
      </el-form-item>
      <el-form-item>
        <el-button type="info" @click="cancleEdit">取消</el-button>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>

  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { editStore, initEditStore } from '@/api/store'
import { getUrlParam } from '@/utils/commonUtil'
import Sortable from 'sortablejs'

export default {
  name: 'ManagePaneBaseInfo',
  data() {
    return {
      isEdit: false,
      loading: false,
      temp: {
        name: '',
        address: '',
        briefDes: '',
        uploadImglist: [],
        status: {
          text: '',
          value: ''
        }
      },
      form: {
        id: '',
        name: '',
        address: '',
        briefDes: '',
        dispalyImgUrls: [],
        isClose: ''
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
  watch: {
    '$route'(to, from) {
      this.init()
    }
  },
  mounted() {
    this.setUploadImgSort()
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      var id = getUrlParam('id')
      initEditStore({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form.id = d.id
          this.form.name = d.name
          this.form.address = d.address
          this.form.briefDes = d.briefDes
          this.form.dispalyImgUrls = d.dispalyImgUrls
          this.form.isClose = d.isClose
          this.uploadImglist = this.getUploadImglist(d.dispalyImgUrls)

          this.temp.name = d.name
          this.temp.address = d.address
          this.temp.briefDes = d.briefDes
          this.temp.uploadImglist = this.getUploadImglist(d.dispalyImgUrls)
          this.temp.status = d.status
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
            editStore(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this.isEdit = false
                this.init()
              }
            })
          })
        }
      })
    },
    openEdit() {
      this.isEdit = true
    },
    cancleEdit() {
      this.isEdit = false
    },
    getUploadImglist(dispalyImgUrls) {
      var _uploadImglist = []
      if (dispalyImgUrls !== null) {
        for (var i = 0; i < dispalyImgUrls.length; i++) {
          _uploadImglist.push({ status: 'success', url: dispalyImgUrls[i].url, response: { data: { name: dispalyImgUrls[i].name, url: dispalyImgUrls[i].url }}})
        }
      }

      return _uploadImglist
    },
    getDispalyImgUrls(fileList) {
      var _dispalyImgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _dispalyImgUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _dispalyImgUrls
    },
    handleRemove(file, fileList) {
      this.uploadImglist = fileList
      this.form.dispalyImgUrls = this.getDispalyImgUrls(fileList)
    },
    handleSuccess(response, file, fileList) {
      this.uploadImglist = fileList
      this.form.dispalyImgUrls = this.getDispalyImgUrls(fileList)
    },
    handleError(errs, file, fileList) {
      this.uploadImglist = fileList
      this.form.dispalyImgUrls = this.getDispalyImgUrls(fileList)
    },
    handlePreview(file) {
      this.uploadImgPreImgDialogUrl = file.url
      this.uploadImgPreImgDialogVisible = true
    },
    setUploadImgSort() {
      var _this = this
      const $ul = _this.$refs.uploadImg.$el.querySelectorAll('.el-upload-list')[0]
      new Sortable($ul, {
        onUpdate: function(event) {
        // 修改items数据顺序
          var newIndex = event.newIndex
          var oldIndex = event.oldIndex
          var $li = $ul.children[newIndex]
          var $oldLi = $ul.children[oldIndex]
          // 先删除移动的节点
          $ul.removeChild($li)
          // 再插入移动的节点到原有节点，还原了移动的操作
          if (newIndex > oldIndex) {
            $ul.insertBefore($li, $oldLi)
          } else {
            $ul.insertBefore($li, $oldLi.nextSibling)
          }
          // 更新items数组
          var item = _this.uploadImglist.splice(oldIndex, 1)
          _this.uploadImglist.splice(newIndex, 0, item[0])

          _this.form.dispalyImgUrls = _this.getDispalyImgUrls(_this.uploadImglist)
        // 下一个tick就会走patch更新
        },
        animation: 150
      })
    }
  }
}
</script>
<style lang="scss" scoped>

#store_baseinfo{
.el-form .el-form-item{
  max-width: 600px;
}

.el-upload-list >>> .sortable-ghost {
  opacity: .8;
  color: #fff !important;
  background: #42b983 !important;
}

.el-upload-list >>> .el-tag {
  cursor: pointer;
}
}

</style>
