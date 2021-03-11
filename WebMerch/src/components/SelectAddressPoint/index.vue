<template>

  <div class="selectAddress" style="width:100%;height:500px">

    <div class="filter-container">
      <el-row :gutter="12">
        <el-col :span="10" :xs="24">

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

        </el-col>
        <el-col :span="2" :xs="24">
          <el-button class="filter-item" type="primary" @click="handleSure">
            确定
          </el-button>
        </el-col>
      </el-row>
    </div>
    <div
      style=" height: 50px;
    line-height: 50px;
    font-size: 16px;"
    > <span>当前地址：</span> <span class="deviceAddressText">{{ curAddressDetails }}</span></div>
    <div id="container" style="height:400px;width:100%" />
  </div>

</template>

<script>

export default {
  props: {
    opCode: {
      type: String,
      default: 'list'
    },
    curAdddress: {
      type: Object,
      default: null
    },
    selectMethod: {
      type: Function,
      default: null
    }
  },
  data() {
    return {
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
      // 地图弹框的数据源
      curAddressDetails: '',
      mapSearchText: '',
      mapObject: '', // 地图实例
      mapMarker: '', // Marker实例
      mapGeoc: null,
      curSelectAddress: null
    }
  },
  created() {
    var that = this
    this.mapequipmentDialog = true
    this.$nextTick(function() {
      // 创建变量，用于存储地址
      var address
      var dizhi
      var marker
      if (marker === undefined) {
        //  初始化百度地图
        var map = new BMap.Map('container', { enableMapClick: false })
        // 创建地图实例
        var point = new BMap.Point(113.26688358298644, 23.135843274289744)
        if (that.curAdddress != null) {
          // console.log('that.curAdddress:' + JSON.stringify(that.curAdddress))
          point = new BMap.Point(that.curAdddress.point.lng, that.curAdddress.point.lat)
          that.curAddressDetails = that.curAdddress.address
        }
        // 创建点坐标
        map.centerAndZoom(point, 15)
        map.addControl(new BMap.NavigationControl())
        map.enableScrollWheelZoom(true)// 允许鼠标滚动缩放

        if (that.curAdddress != null) {
          var marker = new BMap.Marker(point)
          map.addOverlay(marker)
          map.enableScrollWheelZoom(true)
        }

        map.addEventListener('click', function(e) {
          // console.log(e);

          // 移除标注
          map.removeOverlay(marker)

          // 创建变量，用于存储经纬度
          var point = e.point
          // console.log(point)

          // 创建标注实例
          marker = new BMap.Marker(point)

          // 添加标注
          map.addOverlay(marker)

          // 创建地理编码实例
          var geoc = new BMap.Geocoder()

          geoc.getLocation(point, function(rs) {
            // console.log(rs)
            that.curSelectAddress = rs
            that.curAddressDetails = rs.address
          })
        })
        this.mapObject = map
        this.mapMarker = marker
      }
    })
  },
  methods: {
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
      this.mapSearchText = item.address // 记录详细地址，含建筑物名
      this.mapObject.clearOverlays() // 清除地图上所有覆盖物
      // this.mapMarker = new BMap.Marker(item.point) // 根据所选坐标重新创建Marker
      // this.mapObject.addOverlay(this.mapMarker) // 将覆盖物重新添加到地图中
      this.mapObject.panTo(item.point) // 将地图的中心点更改为选定坐标点
    },
    handleSure() {
      if (this.curSelectAddress == null) {
        this.$message('请点击地图选择地址')
        return
      }
      if (this.selectMethod) {
        this.selectMethod(this.curSelectAddress)
      }
    }
  }
}
</script>
