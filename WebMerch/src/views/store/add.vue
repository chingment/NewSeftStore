<template>
  <div id="store_add" class="app-container">

    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="地理位置">
        <el-autocomplete
          v-model="mapSearchText"
          style="width:100%;"
          popper-class="autoAddressClass"
          :fetch-suggestions="mapQuerySearchAsync"
          :trigger-on-focus="false"
          placeholder="搜索"
          clearable
          @select="mapHandleSelect"
        >
          <template slot-scope="{item}">
            <div style="address-ct overflow:hidden;">
              <span class="address ellipsis">{{ item.address }}</span>
            </div>
          </template>
        </el-autocomplete>
        <div id="mapContainer" class="bm-view" />
      </el-form-item>

      <el-form-item label="店铺名称" prop="name">
        <el-input v-model="form.name" clearable />
      </el-form-item>
      <el-form-item label="联系地址" prop="address">
        <el-input v-model="form.address" clearable />
      </el-form-item>
      <el-form-item label="图片" prop="displayImgUrls">
        <el-input :value="form.displayImgUrls.toString()" style="display:none" />
        <el-upload
          ref="uploadImg"
          v-model="form.displayImgUrls"
          :action="uploadImgServiceUrl"
          list-type="picture-card"
          :before-upload="uploadBeforeHandle"
          :on-success="uploadSuccessHandle"
          :on-remove="uploadRemoveHandle"
          :on-error="uploadErrorHandle"
          :on-preview="uploadPreviewHandle"
          :file-list="uploadImglist"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisible">
          <img width="100%" :src="uploadImgPreImgDialogUrl" alt="">
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：图片500*500，格式（jpg,png）不超过4M；第一张为主图，可拖动改变图片顺序</div>
      </el-form-item>
      <el-form-item label="简短描述" style="max-width:1000px">
        <el-input v-model="form.briefDes" type="text" maxlength="200" clearable show-word-limit />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { add, initAdd } from '@/api/store'
import { goBack } from '@/utils/commonUtil'
import Sortable from 'sortablejs'
import { all } from 'q'

export default {
  data() {
    return {
      loading: false,
      form: {
        name: '',
        address: '',
        briefDes: '',
        displayImgUrls: [],
        addressPoint: { // 详细地址经纬度
          lng: 0,
          lat: 0
        }
      },
      rules: {
        name: [{ required: true, min: 1, max: 30, message: '必填,且不能超过30个字符', trigger: 'change' }],
        address: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        briefDes: [{ required: false, min: 0, max: 200, message: '不能超过200个字符', trigger: 'change' }]
      },
      uploadImglist: [],
      uploadImgMaxSize: 4,
      uploadImgPreImgDialogUrl: '',
      uploadImgPreImgDialogVisible: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
      mapSearchText: '',
      mapObject: '', // 地图实例
      mapMarker: '', // Marker实例
      mapGeoc: null
    }
  },
  mounted() {
    this.setUploadImgSort()
    this.bdMap()
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
          this.uploadCardCheckShow()
        }
        this.loading = false
      })
    },
    bdMap() {
      var _this = this
      // 创建地图
      var mapObject = new BMap.Map('mapContainer', { enableMapClick: false })
      var mapGeoc = new BMap.Geocoder()
      var point = new BMap.Point(113.3309751406, 23.1123809784)
      mapObject.centerAndZoom(point, 16) // 创建点坐标
      mapObject.addControl(new BMap.NavigationControl())
      mapObject.enableScrollWheelZoom(true)// 允许鼠标滚动缩放

      // 初始化地图， 设置中心点坐标和地图级别
      var mapMarker = new BMap.Marker(point)
      mapObject.addOverlay(mapMarker) // 添加覆盖物
      mapMarker.setAnimation(BMAP_ANIMATION_BOUNCE) // 跳动的动画
      // 添加覆盖物文字
      const label = new BMap.Label('广州塔', {
        offset: new BMap.Size(20, -25)
      })
      mapMarker.setLabel(label)

      // 鼠标点击
      mapObject.addEventListener('click', function(e) {
        var pt = e.point
        var marker = new BMap.Marker(pt) // 创建标注
        mapObject.clearOverlays()
        mapObject.addOverlay(marker)
        mapGeoc.getLocation(pt, function(rs) {
          var addComp = rs.addressComponents
          _this.form.addressPoint = rs.point
          _this.form.address =
                addComp.province +
                addComp.city +
                addComp.district +
                addComp.street +
                addComp.streetNumber
        })
      })

      this.mapObject = mapObject
      this.mapMarker = mapMarker
    },
    resetForm() {

    },
    onSubmit() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            add(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                goBack(this)
              }
            })
          })
        }
      })
    },
    getdisplayImgUrls(fileList) {
      var _displayImgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _displayImgUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _displayImgUrls
    },
    uploadBeforeHandle(file) {
      if (this.form.displayImgUrls.length >= this.uploadImgMaxSize) {
        this.$message.error('上传图片不能超过4张!')
        return false
      }

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
    uploadCardCheckShow() {
      var uploadcard = this.$refs.uploadImg.$el.querySelectorAll('.el-upload--picture-card')
      if (this.form.displayImgUrls.length === this.uploadImgMaxSize) {
        uploadcard[0].style.display = 'none'
      } else {
        uploadcard[0].style.display = 'inline-block'
      }
    },
    uploadRemoveHandle(file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
      this.uploadCardCheckShow()
    },
    uploadSuccessHandle(response, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
      this.uploadCardCheckShow()
    },
    uploadErrorHandle(errs, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    uploadPreviewHandle(file) {
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

          _this.form.displayImgUrls = _this.getdisplayImgUrls(_this.uploadImglist)
        // 下一个tick就会走patch更新
        },
        animation: 150
      })
    },
    mapQuerySearchAsync(str, cb) {
      var options = {
        onSearchComplete: function(res) { // 检索完成后的回调函数
          var s = []
          if (local.getStatus() == BMAP_STATUS_SUCCESS) {
            for (var i = 0; i < res.getCurrentNumPois(); i++) {
              s.push(res.getPoi(i))
            }
            cb(s) // 获取到数据时，通过回调函数cb返回到<el-autocomplete>组件中进行显示
          } else {
            cb(s)
          }
        }
      }
      var local = new BMap.LocalSearch(this.mapObject, options) // 创建LocalSearch构造函数
      local.search(str) // 调用search方法，根据检索词str发起检索
    },
    mapHandleSelect(item) {
      this.mapSearchText = item.address + item.title // 记录详细地址，含建筑物名
      this.form.address = item.address + item.title
      this.form.addressPoint = item.point // 记录当前选中地址坐标
      this.mapObject.clearOverlays() // 清除地图上所有覆盖物
      this.mapMarker = new BMap.Marker(item.point) // 根据所选坐标重新创建Marker
      this.mapObject.addOverlay(this.mapMarker) // 将覆盖物重新添加到地图中
      this.mapObject.panTo(item.point) // 将地图的中心点更改为选定坐标点
    }
  }
}
</script>

<style lang="scss" scoped>

#store_add{
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

.bm-view {
  width: 100%;
  height: 200px;
  margin-top: 20px;
}

.autoAddressClass{
  li {
    display: flex;
    i.el-icon-search {margin-top:11px;}
    .mgr10 {margin-right: 10px;}
    .title {
      text-overflow: ellipsis;
      overflow: hidden;
    }

.address-ct{
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

