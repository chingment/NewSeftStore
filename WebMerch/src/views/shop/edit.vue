<template>
  <div id="shop_add">
      <page-header/>
        <el-form ref="form" :model="form" :rules="rules" label-width="80px">
          <el-form-item label="门店名称" prop="name">
            <el-input v-model="form.name" clearable style="max-width:500px" />
          </el-form-item>
          <el-form-item label="门店地址" prop="address">
            <span>{{ form.address }}</span>
            <el-button type="text" @click="getGpsList">选择</el-button>
          </el-form-item>
          <el-form-item label="联系人" prop="contactName">
            <el-input v-model="form.contactName" clearable style="width:200px" />
          </el-form-item>
          <el-form-item label="联系电话" prop="contactPhone">
            <el-input v-model="form.contactPhone" clearable style="width:200px" />
          </el-form-item>
          <el-form-item label="联系地址" prop="contactAddress">
            <el-input v-model="form.contactAddress" clearable style="max-width:500px" />
          </el-form-item>
          <el-form-item label="门店图片" prop="displayImgUrls">
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
            <el-button type="primary" @click="handleSave">保存</el-button>
      </el-form-item>
        </el-form>
      
  
    <!-- 点击新增设备定位按钮弹框 -->
    <el-dialog title="地图定位" :visible.sync="mapequipmentDialog" width="800px">
      <div style="width:100%;height:500px">  
      <div class="selectAddress">
        <p class="deviceAddressText">{{ dizhiMap }}</p>
      </div>
      <div id="container" style="height:400px;width:100%" />
      </div>
      <span slot="footer" class="dialog-footer">
        <el-button @click="mapequipmentDialog = false">取 消</el-button>
        <el-button type="primary" @click="getClick">确 定</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { save,getDetails } from '@/api/shop'
import { getUrlParam } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'ShopAdd',
  components: { PageHeader },
  data() {
    return {
      loading: false,
      form: {
        id: '',
        name: '',
        areaCode: '',
        areaName: '',
        address: 'xx',
        contactName: '',
        contactPhone: '',
        contactAddress: '',
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
      // 默认新增设备弹框控制器
      addequipmentDialog: false,
      // 新增设备的数据源
      addobjequipment: {
        deviceNumber: '',
        // 地图弹框的数据源
        devicename: '',
        address: '',
        latitude: '',
        longitude: ''
      },
      // 地图弹框
      mapequipmentDialog: false,
      // 地图弹框的数据源
      dizhiMap: '',
      isDesktop: this.$store.getters.isDesktop,
    }
  },
  mounted() {

  },
  created() {
  this.loading = true
          var id = getUrlParam('id')
       getDetails({ id: id }).then(res => {
          if (res.result === 1) {
            var d = res.data
            this.form = d
            this.form.displayImgUrls = this.form.displayImgUrls == null ? [] : this.form.displayImgUrls
            this.uploadImglist = this.getUploadImglist(d.displayImgUrls)
          }
          this.loading = false
        })
  },
  methods: {
    handleSave() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            save(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {

              }
            })
          })
        }
      })
    },
    getUploadImglist(displayImgUrls) {
      var _uploadImglist = []
      if (displayImgUrls !== null) {
        for (var i = 0; i < displayImgUrls.length; i++) {
          _uploadImglist.push({ status: 'success', url: displayImgUrls[i].url, response: { data: { name: displayImgUrls[i].name, url: displayImgUrls[i].url }}})
        }
      }

      return _uploadImglist
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
    getGpsList() {
      var that = this
      this.mapequipmentDialog = true
      this.$nextTick(function() {
        // 创建变量，用于存储地址
        var address
        var dizhi
        var marker
        if (marker === undefined) {
          //  初始化百度地图
          var map = new BMap.Map('container')
          // 创建地图实例
          var point = new BMap.Point(116.404, 39.915)
          // 创建点坐标
          map.centerAndZoom(point, 15)
          // var marker = new BMap.Marker(point);
          // map.addOverlay(marker);
          map.enableScrollWheelZoom(true)

          map.addEventListener('click', function(e) {
            // console.log(e);

            // 移除标注
            map.removeOverlay(marker)

            // 创建变量，用于存储经纬度
            var point = e.point
            // console.log(point)

            // 设置lng的值
            window.localStorage.setItem('lng', point.lng)

            // 设置lat的值
            window.localStorage.setItem('lat', point.lat)

            // 创建标注实例
            marker = new BMap.Marker(point)

            // 添加标注
            map.addOverlay(marker)

            // 创建地理编码实例
            var geoc = new BMap.Geocoder()

            geoc.getLocation(point, function(rs) {
              // console.log(rs)

              dizhi = rs.address

              address = rs.addressComponents
              // console.log(address)

              // 设置currentProvince的值
              window.localStorage.setItem('currentProvince', address.province)

              // 设置currentCity的值
              window.localStorage.setItem('currentCity', address.city)
              // 设置地址的值
              window.localStorage.setItem('dizhi', dizhi)
              that.dizhiMap = window.localStorage.getItem('dizhi')
            })
          })
        }
      })
    },
    getClick() {
      this.mapequipmentDialog = false
      // console.log(that.addobjequipment.dizhiInput)
      this.addobjequipment.address = window.localStorage.getItem('dizhi')
      this.addobjequipment.latitude = window.localStorage.getItem('lat')
      this.addobjequipment.longitude = window.localStorage.getItem('lng')
      this.editobjequipment.address = window.localStorage.getItem('dizhi')
      this.editobjequipment.latitude = window.localStorage.getItem('lat')
      this.editobjequipment.longitude = window.localStorage.getItem('lng')
    },
    async addClick() {
      // this.addobjequipment = {}
      if (this.addobjequipment.deviceNumber === '') {
        this.$message.error('请输入设备号')
        return
      }
      if (this.addobjequipment.devicename === '') {
        this.$message.error('请输入设备名')
        return
      }
      if (this.addobjequipment.address === '') {
        this.$message.error('请选择地址')
        return
      }
      this.addequipmentDialog = false
      var res = await this.$http.post(url, {
        deviceNumber: this.addobjequipment.deviceNumber,
        devicename: this.addobjequipment.devicename,
        address: this.addobjequipment.address,
        latitude: this.addobjequipment.latitude,
        longitude: this.addobjequipment.longitude
      })
      console.log(res)
      var data = res.data
      if (res.status === 200) {
        if (data.success === true) {
          this.$message({
            message: data.results,
            type: 'success'
          })
          this.getStreetList()
          this.cancelAddobj()
        } else {
          this.$message.error(data.msg)
        }
      } else {
        this.$message.error(data.results)
      }
    }
  }
}
</script>
