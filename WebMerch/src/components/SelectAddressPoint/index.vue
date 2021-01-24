<template>
    <!-- 点击新增设备定位按钮弹框 -->
      <div style="width:100%;height:500px">  
      <div class="selectAddress">
        

     <div class="filter-container">
      <el-row :gutter="12">
        <el-col :span="10" :xs="24" style="margin-bottom:20px">
        

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
        <el-col :span="2" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            确定
          </el-button>
        </el-col>
      </el-row>
    </div>




        <p class="deviceAddressText">{{ dizhiMap }}</p>
      </div>
      <div id="container" style="height:400px;width:100%" />
      </div>
</template>

<script>

export default {
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
      dizhiMap: '',
      mapSearchText:'',
      mapObject: '', // 地图实例
      mapMarker: '', // Marker实例
      mapGeoc: null
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
          var point = new BMap.Point(116.404, 39.915)
          // 创建点坐标
          map.centerAndZoom(point, 15)
          map.addControl(new BMap.NavigationControl())
          map.enableScrollWheelZoom(true)// 允许鼠标滚动缩放
          var marker = new BMap.Marker(point);
          map.addOverlay(marker);
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
      this.mapSearchText = item.address + item.title // 记录详细地址，含建筑物名
      this.mapObject.clearOverlays() // 清除地图上所有覆盖物
      this.mapMarker = new BMap.Marker(item.point) // 根据所选坐标重新创建Marker
      this.mapObject.addOverlay(this.mapMarker) // 将覆盖物重新添加到地图中
      this.mapObject.panTo(item.point) // 将地图的中心点更改为选定坐标点
    },
    getClick() {
    //   this.mapequipmentDialog = false
    //   // console.log(that.addobjequipment.dizhiInput)
    //   this.addobjequipment.address = window.localStorage.getItem('dizhi')
    //   this.addobjequipment.latitude = window.localStorage.getItem('lat')
    //   this.addobjequipment.longitude = window.localStorage.getItem('lng')
    //   this.editobjequipment.address = window.localStorage.getItem('dizhi')
    //   this.editobjequipment.latitude = window.localStorage.getItem('lat')
    //   this.editobjequipment.longitude = window.localStorage.getItem('lng')
    },
  }
}
</script>