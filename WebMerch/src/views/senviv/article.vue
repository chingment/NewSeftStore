<template>
  <div id="article_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.title" clearable style="width: 100%" placeholder="标题" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilter">
            查询
          </el-button>
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onOpenDialogEdit()">
            新建
          </el-button>
        </el-col>
      </el-row>

    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="标题" prop="title" align="left" min-width="50%">
        <template slot-scope="scope">
          <span>{{ scope.row.title }}</span>
        </template>
      </el-table-column>
      <el-table-column label="标签" prop="tags" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.tags }}</span>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" prop="createTime" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="onOpenDialogEdit(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetListData" />

    <el-dialog
      title="编辑"
      :visible.sync="dialogEditIsVisible"
      width="800px"
    >
      <div class="form-article">
        <div style="margin-bottom:10px">
          <el-input v-model="form.title" placeholder="标题" clearable />
        </div>
        <div style="margin-bottom:10px">
          <el-tag
            v-for="tag in form.tags"
            :key="tag"
            closable
            :disable-transitions="false"
            @close="onDeleteTag(tag)"
          >
            {{ tag }}
          </el-tag>
          <el-input
            v-if="inputVisibleByTag"
            ref="saveTagInput"
            v-model="inputValueByTag"
            class="input-new-tag"
            size="small"
            @keyup.enter.native="onInputConfirmByTag"
            @blur="onInputConfirmByTag"
          />
          <el-button v-else class="button-new-tag" size="small" @click="onShowInputByTag">+ 新建标签</el-button>

        </div>

        <Editor id="tinymce" v-model="form.content" :init="tinymce_init" />
      </div>
      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="onSave">保存</el-button>
        <el-button size="small" @click="dialogEditIsVisible=false">关闭</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import axios from 'axios'
import { getArticles, getArticle, saveArticle } from '@/api/senviv'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import tinymce from 'tinymce/tinymce'
import Editor from '@tinymce/tinymce-vue'
import 'tinymce/themes/silver/theme'
import 'tinymce/themes/silver/theme'
import 'tinymce/plugins/image'// 插入上传图片插件
import 'tinymce/plugins/media'// 插入视频插件
import 'tinymce/plugins/table'// 插入表格插件
import 'tinymce/plugins/link' // 超链接插件
import 'tinymce/plugins/code' // 代码块插件
import 'tinymce/plugins/lists'// 列表插件
import 'tinymce/plugins/contextmenu' // 右键菜单插件
import 'tinymce/plugins/wordcount' // 字数统计插件
import 'tinymce/plugins/colorpicker' // 选择颜色插件
import 'tinymce/plugins/textcolor' // 文本颜色插件
import 'tinymce/plugins/fullscreen' // 全屏
import 'tinymce/plugins/help'
import 'tinymce/plugins/charmap'
import 'tinymce/plugins/paste'
import 'tinymce/plugins/print'
import 'tinymce/plugins/preview'
import 'tinymce/plugins/hr'
import 'tinymce/plugins/anchor'
import 'tinymce/plugins/pagebreak'
import 'tinymce/plugins/spellchecker'
import 'tinymce/plugins/searchreplace'
import 'tinymce/plugins/visualblocks'
import 'tinymce/plugins/visualchars'
import 'tinymce/plugins/insertdatetime'
import 'tinymce/plugins/nonbreaking'
import 'tinymce/plugins/autosave'
import 'tinymce/plugins/fullpage'
import 'tinymce/plugins/toc'

export default {
  name: 'ArticleList',
  components: { Pagination, Editor },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        title: undefined
      },
      form: {
        id: '',
        tags: [],
        content: ''
      },
      inputVisibleByTag: false,
      inputValueByTag: '',
      dialogEditIsLoading: false,
      dialogEditIsVisible: false,
      tinymce_init: {
        language_url: '/static/tinymce/langs/zh_CN.js', // 语言包的路径
        language: 'zh_CN', // 语言
        height: 430,
        skin_url: '/static/tinymce/skins/ui/oxide',
        // images_upload_url: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
        menubar: false, // 隐藏最上方menu菜单
        browser_spellcheck: true, // 拼写检查
        branding: false, // 去水印
        statusbar: false, // 隐藏编辑器底部的状态栏
        elementpath: false, // 禁用下角的当前标签路径
        paste_data_images: true, // 允许粘贴图像
        plugins: 'lists image media table wordcount code fullscreen help  toc fullpage autosave nonbreaking insertdatetime visualchars visualblocks searchreplace spellchecker pagebreak link charmap paste print preview hr anchor',
        // toolbar: [
        //   'newdocument undo redo | formatselect visualaid|cut copy paste selectall| bold italic underline strikethrough |codeformat blockformats| superscript subscript  | forecolor backcolor | alignleft aligncenter alignright alignjustify | outdent indent |  removeformat ',
        //   'code  bullist numlist | lists image media table link |fullscreen help toc fullpage restoredraft nonbreaking insertdatetime visualchars visualblocks searchreplace spellchecker pagebreak anchor charmap  pastetext print preview hr'
        // ],
        toolbar: ['undo redo | bold italic underline strikethrough | link unlink | fontsizeselect | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | outdent indent blockquote | code | removeforma t| lists image media'],
        images_upload_handler: (blobInfo, success, failure) => {
          // console.log(blobInfo)

          // if (blobInfo.blob().size > self.maxSize) {
          // failure('文件体积过大')
          // }

          uploadPic()

          function uploadPic() {
            const formData = new FormData()
            // 服务端接收文件的参数名，文件数据，文件名
            formData.append('upfile', blobInfo.blob(), blobInfo.filename())
            formData.append('folder', 'article')
            axios({
              method: 'POST',
              // 这里是你的上传地址
              url: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
              data: formData
            })
              .then((res) => {
                if (res.status === 200) {
                  var d = res.data
                  // console.log(d)
                  if (d.result === 1) {
                    var dd = d.data
                    // 这里返回的是你图片的地址
                    success(dd.url)
                  } else {
                    failure('服务上传失败..')
                  }
                } else {
                  failure('服务上传失败.')
                }
              })
              .catch(() => {
                failure('上传失败')
              })
          }

          // const img = 'data:image/jpeg;base64,' + blobInfo.base64()
          // success(img)
        }
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {
    tinymce.init({})
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.onGetListData()
  },
  methods: {
    onGetListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getArticles(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    onFilter() {
      this.listQuery.page = 1
      this.onGetListData()
    },
    onOpenDialogEdit(item) {
      if (item) {
        this.loading = true
        getArticle({ id: item.id }).then(res => {
          if (res.result === 1) {
            this.form = res.data
          }
          this.loading = false
        })
      } else {
        this.form.id = ''
        this.form.title = ''
        this.form.tags = []
        this.form.content = ''
      }
      this.dialogEditIsVisible = true
    },
    onSave() {
      MessageBox.confirm('确定要保存', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      })
        .then(() => {
          saveArticle(this.form).then(res => {
            if (res.result === 1) {
              this.$message({
                message: res.message,
                type: 'success'
              })
              this.dialogEditIsVisible = false
              this.onGetListData()
            } else {
              this.$message({
                message: res.message,
                type: 'error'
              })
            }
          })
        })
        .catch(() => {})
    },
    onDeleteTag(tag) {
      if (this.form.tags != null) {
        this.form.tags.splice(this.form.tags.indexOf(tag), 1)
      }
    },
    onShowInputByTag() {
      this.inputVisibleByTag = true
      this.$nextTick(_ => {
        this.$refs.saveTagInput.$refs.input.focus()
      })
    },
    onInputConfirmByTag() {
      const inputValue = this.inputValueByTag
      if (inputValue) {
        this.form.tags = this.form.tags == null ? [] : this.form.tags
        this.form.tags.push(inputValue)
      }
      this.inputVisibleByTag = false
      this.inputValueByTag = ''
    }
  }
}
</script>
<style lang="scss" scoped>

.form-article{
  .el-tag  {
    margin-right: 10px;
  }
  .button-new-tag {
    height: 32px;
    line-height: 30px;
    padding-top: 0;
    padding-bottom: 0;
  }
  .input-new-tag {
    width: 90px;
    vertical-align: bottom;
  }

}
</style>
