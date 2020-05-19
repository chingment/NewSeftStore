<template>
  <div id="productsku_add" class="app-container">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="100px">
      <el-form-item label="名称" prop="name">
        <el-input v-model="form.name" clearable />
      </el-form-item>
      <el-form-item v-show="!isOpenAddMultiSpecs" label="编码" prop="singleSkuCumCode">
        <el-input v-model="form.singleSkuCumCode" clearable />
      </el-form-item>
      <el-form-item v-show="!isOpenAddMultiSpecs" label="条形码" prop="singleSkuBarCode">
        <el-input v-model="form.singleSkuBarCode" clearable />
      </el-form-item>
      <el-form-item label="图片" prop="displayImgUrls">
        <el-input :value="form.displayImgUrls.toString()" style="display:none" />
        <el-upload
          ref="uploadImgByDisplayImgUrls"
          v-model="form.displayImgUrls"
          :action="uploadImgServiceUrl"
          :data="uploadImgPmsByByDisplayImgUrls"
          list-type="picture-card"
          :on-success="handleSuccessByDisplayImgUrls"
          :on-remove="handleRemoveByDisplayImgUrls"
          :on-error="handleErrorByDisplayImgUrls"
          :on-preview="handlePreviewByDisplayImgUrls"
          :before-upload="handleBeforeUploadByDisplayImgUrls"
          :file-list="uploadImglistByDisplayImgUrls"
          :limit="4"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisibleByDisplayImgUrls">
          <img width="100%" :src="uploadImgPreImgDialogUrlByDisplayImgUrls" alt="">
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：图片500*500，格式（jpg,png）不超过4M；第一张为主图，可拖动改变图片顺序</div>
      </el-form-item>
      <el-form-item label="所属分类" prop="kindIds">
        <el-input :value="form.kindIds.toString()" style="display:none" />
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

      <el-form-item label="开启多规格">
        <el-checkbox v-model="isOpenAddMultiSpecs" />
      </el-form-item>

      <el-form-item v-show="!isOpenAddMultiSpecs" label="默认销售价" prop="singleSkuSalePrice">
        <el-input v-model="form.singleSkuSalePrice" style="width:160px">
          <template slot="prepend">￥</template>
        </el-input>

        <div class="remark-tip"><span class="sign">*注</span>：该价格作为默认销售价，若更改可在编辑-》在售店铺里修改</div>

      </el-form-item>
      <el-form-item v-show="!isOpenAddMultiSpecs" label="规格" prop="singleSkuSpecDes">
        <el-input v-model="form.singleSkuSpecDes" clearable />
      </el-form-item>

      <el-form-item v-show="isOpenAddMultiSpecs" style="max-width:1000px">

        <div style="display:flex">
          <div style="min-width:50px;">规格：</div>
          <div style="flex:1;">
            <el-tag
              v-for="item in multiSpecsItems"
              :key="item.name"
              closable
              :disable-transitions="false"
              @close="multiSpecsHandleClose(item)"
            >
              {{ item.name }}
            </el-tag>
            <el-input
              v-if="multiSpecsInputVisible"
              ref="saveTagInput"
              v-model="multiSpecsInputValue"
              class="input-new-tag"
              size="small"
              @keyup.enter.native="multiSpecsHandleInputConfirm"
              @blur="multiSpecsHandleInputConfirm"
            />
            <el-button v-else class="button-new-tag" size="small" @click="multiSpecsShowInput">+ 添加新规格</el-button>
          </div>
        </div>
        <div
          v-for="(item,i) in multiSpecsItems"
          :key="item.name"
          style="display:flex"
        >
          <div style="min-width:50px;"> {{ item.name }}：</div>

          <div style="flex:1;">
            <el-tag
              v-for="value in item.values"
              :key="value"
              type="success"
              closable
              :disable-transitions="false"
              @close="multiSpecsValueHandleClose(item,value)"
            >
              {{ value }}
            </el-tag>
            <el-input
              v-if="item.inputVisible"
              :id="'saveTagInput'+i"
              v-model="item.inputValue"
              class="input-new-tag"
              size="small"
              @keyup.enter.native="multiSpecsValueHandleInputConfirm(item)"
              @blur="multiSpecsValueHandleInputConfirm(item)"
            />
            <el-button v-else class="button-new-tag" size="small" @click="multiSpecsValueShowInput(item,'saveTagInput'+i)">+ 添加新项</el-button>
          </div>

        </div>

        <table class="list-tb" cellpadding="0" cellspacing="0">
          <thead>
            <tr>
              <th
                v-for="item in multiSpecsItems"
                v-show="item.values.length>0"

                :key="item.name"
              >
                {{ item.name }}
              </th>
              <th style="width:150px">
                编码
              </th>
              <th style="width:150px">
                条形码
              </th>
              <th style="width:150px">
                价格
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(item,x) in multiSpecsSkuResult"
              :key="x"
            >
              <td
                v-for="(spec,y) in item.specDes"
                :key="y"
              >
                {{ spec.value }}
              </td>
              <td>
                <el-tooltip :content="item.cumCode" placement="top">
                  <el-input v-model="item.cumCode" clearable style="width:90%" />
                </el-tooltip>
              </td>
              <td>
                <el-tooltip :content="item.barCode" placement="top">
                  <el-input v-model="item.barCode" clearable style="width:90%" />
                </el-tooltip>
              </td>
              <td>
                <el-tooltip :content="item.salePrice" placement="top">
                  <el-input v-model="item.salePrice" clearable style="width:90%" />
                </el-tooltip>
              </td>
            </tr>
          </tbody>

        </table>

      </el-form-item>
      <el-form-item label="简短描述" style="max-width:1000px">
        <el-input v-model="form.briefDes" type="text" maxlength="200" show-word-limit />
      </el-form-item>
      <el-form-item label="详情图片" prop="detailsDes">

        <el-input :value="form.detailsDes.toString()" style="display:none" />
        <el-upload
          ref="uploadImgByDetailsDes"
          v-model="form.detailsDes"
          :action="uploadImgServiceUrl"
          list-type="picture-card"
          :on-success="handleSuccessByDetailsDes"
          :on-remove="handleRemoveByDetailsDes"
          :on-error="handleErrorByDetailsDes"
          :on-preview="handlePreviewByDetailsDes"
          :before-upload="handleBeforeUploadByDetailsDes"
          :file-list="uploadImglistByDetailsDes"
          :data="uploadImgPmsByByDetailsDes"
          :limit="4"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisibleByDetailsDes">
          <img width="100%" :src="uploadImgPreImgDialogUrlByDetailsDes" alt="">
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：图片不超过4M；可拖动改变图片顺序</div>

      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { add, initAdd } from '@/api/prdproduct'
import fromReg from '@/utils/formReg'
import { goBack, treeselectNormalizer } from '@/utils/commonUtil'
import Treeselect from '@riophae/vue-treeselect'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'

import Sortable from 'sortablejs'

export default {
  components: { Treeselect },
  data() {
    return {
      loading: false,
      isOpenAddMultiSpecs: false,
      form: {
        name: '',
        kindIds: [],
        subjectIds: [],
        detailsDes: [],
        briefDes: '',
        displayImgUrls: [],
        singleSkuCumCode: '',
        singleSkuBarCode: '',
        singleSkuSalePrice: 0,
        singleSkuSpecDes: ''
      },
      rules: {
        name: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        singleSkuCumCode: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        singleSkuBarCode: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        // kindIds: [{ type: 'array', required: true, message: '至少必选一个,且必须少于3个', max: 3 }],
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        singleSkuSalePrice: [{ required: true, message: '金额格式,eg:88.88', pattern: fromReg.money }],
        singleSkuSpecDes: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        briefDes: [{ required: false, min: 0, max: 200, message: '不能超过200个字符', trigger: 'change' }],
        detailsDes: [{ type: 'array', required: false, message: '不能超过3张', max: 3 }]
      },
      uploadImglistByDisplayImgUrls: [],
      uploadImgPmsByByDisplayImgUrls: { folder: 'product', isBuildms: 'true' },
      uploadImgPreImgDialogUrlByDisplayImgUrls: '',
      uploadImgPreImgDialogVisibleByDisplayImgUrls: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
      uploadImglistByDetailsDes: [],
      uploadImgPreImgDialogUrlByDetailsDes: '',
      uploadImgPreImgDialogVisibleByDetailsDes: false,
      uploadImgPmsByByDetailsDes: { folder: 'product', isBuildms: 'false' },
      treeselect_kind_normalizer: treeselectNormalizer,
      treeselect_kind_options: [],
      treeselect_subject_normalizer: treeselectNormalizer,
      treeselect_subject_options: [],
      multiSpecsItems: [],
      multiSpecsInputVisible: false,
      multiSpecsInputValue: '',
      multiSpecsSkuArray: [],
      multiSpecsSkuList: [],
      multiSpecsSkuResult: []
    }
  },
  watch: {
    isOpenAddMultiSpecs(val, oldVal) {
      if (val) {
        this.rules.singleSkuCumCode[0].required = false
        this.rules.singleSkuBarCode[0].required = false
        this.form.singleSkuSalePrice = 0.01
        this.rules.singleSkuSalePrice[0].required = false
        this.rules.singleSkuSpecDes[0].required = false
      } else {
        this.rules.singleSkuCumCode[0].required = true
        this.rules.singleSkuBarCode[0].required = true
        this.form.singleSkuSalePrice = 0
        this.rules.singleSkuSalePrice[0].required = true
        this.rules.singleSkuSpecDes[0].required = true
      }

      console.log('inputVal = ' + val + ' , oldValue = ' + oldVal)
    }
  },
  mounted() {
    this.setUploadImgSortByDisplayImgUrls()
    this.setUploadImgSortByDetailsDes()
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initAdd().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.treeselect_subject_options = d.subjects
          this.treeselect_kind_options = d.kinds
        }
        this.loading = false
      })
    },
    resetForm() {

    },
    onSubmit() {
      console.log(JSON.stringify(this.form))

      this.$refs['form'].validate((valid) => {
        if (valid) {
          var skus = []

          if (this.isOpenAddMultiSpecs) {
            this.multiSpecsSkuResult.forEach(item => {
              skus.push({ specDes: item.specDes, salePrice: item.salePrice, barCode: item.barCode, cumCode: item.cumCode })
            })
          } else {
            skus.push({ specDes: [{ name: '单规格', value: this.form.singleSkuSpecDes }], salePrice: this.form.singleSkuSalePrice, barCode: this.form.singleSkuBarCode, cumCode: this.form.singleSkuCumCode })
          }

          if (skus.length <= 0) {
            this.$message('至少录入一个规格                                                                                                                                                                                                                                                                                                                                                   商品')
            return false
          }

          var _form = {}
          _form.name = this.form.name
          _form.kindIds = this.form.kindIds
          _form.detailsDes = this.form.detailsDes
          _form.briefDes = this.form.briefDes
          _form.displayImgUrls = this.form.displayImgUrls
          _form.specItems = this.multiSpecsItems
          _form.skus = skus
          console.log(JSON.stringify(_form))
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            add(_form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                goBack(this)
              }
            })
          })
        }
      })
    },
    handleGetDisplayImgUrls(fileList) {
      var _imgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _imgUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _imgUrls
    },
    handleRemoveByDisplayImgUrls(file, fileList) {
      this.uploadImglistByDisplayImgUrls = fileList
      this.form.displayImgUrls = this.handleGetDisplayImgUrls(fileList)
    },
    handleSuccessByDisplayImgUrls(response, file, fileList) {
      this.uploadImglistByDisplayImgUrls = fileList
      this.form.displayImgUrls = this.handleGetDisplayImgUrls(fileList)
    },
    handleErrorByDisplayImgUrls(errs, file, fileList) {
      this.uploadImglistByDisplayImgUrls = fileList
      this.form.displayImgUrls = this.handleGetDisplayImgUrls(fileList)
    },
    handlePreviewByDisplayImgUrls(file) {
      this.uploadImgPreImgDialogUrl = file.url
      this.uploadImgPreImgDialogVisible = true
    },
    handleBeforeUploadByDisplayImgUrls(file) {
      const imgType = file.type
      const isLt4M = file.size / 1024 / 1024 < 4
      //  var a = isLt4M === true ? 'true' : 'false'
      if (imgType !== 'image/jpeg' && imgType !== 'image/png' && imgType !== 'image/jpg') {
        this.$message('图片格式仅支持(jpg,png)')
        return false
      }

      if (!isLt4M) {
        this.$message('图片大小不能超过4M')
        return false
      }

      return true
    },
    setUploadImgSortByDisplayImgUrls() {
      var _this = this
      const $ul = _this.$refs.uploadImgByDisplayImgUrls.$el.querySelectorAll('.el-upload-list')[0]
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
          var item = _this.uploadImglistByDisplayImgUrls.splice(oldIndex, 1)
          _this.uploadImglistByDisplayImgUrls.splice(newIndex, 0, item[0])

          _this.form.displayImgUrls = _this.handleGetDisplayImgUrls(_this.uploadImglistByDisplayImgUrls)
        // 下一个tick就会走patch更新
        },
        animation: 150
      })
    },
    handleGetDetailsDes(fileList) {
      var _imgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _imgUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _imgUrls
    },
    handleRemoveByDetailsDes(file, fileList) {
      this.uploadImglistByDetailsDes = fileList
      this.form.detailsDes = this.handleGetDetailsDes(fileList)
    },
    handleSuccessByDetailsDes(response, file, fileList) {
      this.uploadImglistByDetailsDes = fileList
      this.form.detailsDes = this.handleGetDetailsDes(fileList)
    },
    handleErrorByDetailsDes(errs, file, fileList) {
      this.uploadImglistByDetailsDes = fileList
      this.form.detailsDes = this.handleGetDetailsDes(fileList)
    },
    handlePreviewByDetailsDes(file) {
      this.uploadImgPreImgDialogUrlByDetailsDes = file.url
      this.uploadImgPreImgDialogVisibleByDetailsDes = true
    },
    handleBeforeUploadByDetailsDes(file) {
      const imgType = file.type
      const isLt4M = file.size / 1024 / 1024 < 4
      //  var a = isLt4M === true ? 'true' : 'false'
      if (imgType !== 'image/jpeg' && imgType !== 'image/png' && imgType !== 'image/jpg') {
        this.$message('图片格式仅支持(jpg,png)')
        return false
      }

      if (!isLt4M) {
        this.$message('图片大小不能超过4M')
        return false
      }

      return true
    },
    setUploadImgSortByDetailsDes() {
      var _this = this
      const $ul = _this.$refs.uploadImgByDetailsDes.$el.querySelectorAll('.el-upload-list')[0]
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
          var item = _this.uploadImglistByDetailsDes.splice(oldIndex, 1)
          _this.uploadImglistByDetailsDes.splice(newIndex, 0, item[0])

          _this.form.detailsDes = _this.handleGetDisplayImgUrls(_this.uploadImglistByDetailsDes)
        // 下一个tick就会走patch更新
        },
        animation: 150
      })
    },
    multiSpecsHandleClose(item) {
      var index = this.multiSpecsItems.indexOf(item)
      this.multiSpecsItems.splice(index, 1)
      this.buildCombination()
    },
    multiSpecsShowInput() {
      this.multiSpecsInputVisible = true
      this.$nextTick(_ => {
        this.$refs.saveTagInput.$refs.input.focus()
      })
    },
    multiSpecsHandleInputConfirm() {
      var _this = this
      var newItemName = _this.multiSpecsInputValue

      var isHasSame = false
      if (_this.multiSpecsItems != null) {
        _this.multiSpecsItems.forEach(item => {
          if (item.name === newItemName) {
            isHasSame = true
          }
        })
      }

      if (isHasSame) {
        _this.$message('已存在相同规格')
        return false
      }

      if (newItemName) {
        this.multiSpecsItems.push({ name: newItemName, values: [], inputVisible: false, inputValue: '' })
      }
      this.multiSpecsInputVisible = false
      this.multiSpecsInputValue = ''
      this.buildCombination()
    },
    multiSpecsValueHandleClose(item, val) {
      var item_index = this.multiSpecsItems.indexOf(item)
      var val_index = this.multiSpecsItems[item_index].values.indexOf(val)
      this.multiSpecsItems[item_index].values.splice(val_index, 1)
      this.buildCombination()
    },
    multiSpecsValueShowInput(item, id) {
      var item_index = this.multiSpecsItems.indexOf(item)
      this.multiSpecsItems[item_index].inputVisible = true
      this.$nextTick(_ => {
        document.querySelector('#' + id).focus()
      })
    },
    multiSpecsValueHandleInputConfirm(item) {
      var _this = this
      var index = this.multiSpecsItems.indexOf(item)
      var values = this.multiSpecsItems[index].values
      var newItemValue = item.inputValue

      var isHasSame = false
      if (values != null) {
        values.forEach(val => {
          if (val === newItemValue) {
            isHasSame = true
          }
        })
      }

      if (isHasSame) {
        _this.$message('已存在相同规格值')
        return false
      }

      if (newItemValue) {
        this.multiSpecsItems[index].values.push(newItemValue)
      }
      this.multiSpecsItems[index].inputVisible = false
      this.multiSpecsItems[index].inputValue = ''

      this.buildCombination()
    },
    getSkuData(skuArr = [], i, list) {
      for (let j = 0; j < list[i].length; j++) {
        if (i < list.length - 1) {
          skuArr[i] = list[i][j]
          this.getSkuData(skuArr, i + 1, list) // 递归循环
        } else {
          this.multiSpecsSkuList.push([...skuArr, list[i][j]]) // 扩展运算符，连接两个数组
        }
      }
    },
    buildCombination() {
      // var checkList = [
      //   { name: '尺码', list: ['X', 'L'] },
      //   { name: '颜色', list: ['红色'] },
      //   { name: '图案', list: ['圆'] }
      // ]

      this.multiSpecsSkuArray = []
      this.multiSpecsSkuList = []
      // 将选中的规格组合成一个大数组 [[1, 2], [a, b]...]
      this.multiSpecsItems.forEach(element => {
        element.values.length > 0 ? this.multiSpecsSkuArray.push(element.values) : ''
      })
      // 勾选了规格，才调用方法
      if (this.multiSpecsSkuArray.length > 0) {
        this.getSkuData([], 0, this.multiSpecsSkuArray)
      }

      var multiSpecs = []
      for (var j = 0; j < this.multiSpecsSkuList.length; j++) {
        var arr_spec_desc = this.multiSpecsSkuList[j]
        var ll_spec_desc = []
        for (var k = 0; k < arr_spec_desc.length; k++) {
          ll_spec_desc.push({ name: this.multiSpecsItems[k].name, value: arr_spec_desc[k] })
        }

        multiSpecs.push({ specDes: ll_spec_desc, salesPrice: 0, cumCode: '', barCode: '' })
      }

      // var multiSpecs = []
      // for (let x = 0; x < this.multiSpecsSkuList.length; x++) {
      //   multiSpecs.push({ specDes: this.multiSpecsSkuList[x], salesPrice: 0, cumCode: '', barCode: '' })
      // }

      this.multiSpecsSkuResult = multiSpecs
      console.log(this.multiSpecsSkuResult)
    }
  }
}
</script>

<style lang="scss" scoped>

#productsku_add {

.el-form .el-form-item{
  max-width: 600px;
}
.el-upload-list >>> .sortable-ghost {
  opacity: .8;
  color: #fff!important;
  background: #42b983!important;
}

.el-upload-list >>> .el-tag {
  cursor: pointer;
}

   .el-tag {
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
    margin-right: 10px;
    vertical-align: bottom;
  }

}
</style>

