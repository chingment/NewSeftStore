<template>
    <!-- 点击新增设备定位按钮弹框 -->
      <div style="width:100%;height:500px">  
      <div class="selectAddress">
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
          var map = new BMap.Map('container')
          // 创建地图实例
          var point = new BMap.Point(116.404, 39.915)
          // 创建点坐标
          map.centerAndZoom(point, 15)
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
        }
      })

  },
  methods: {

  }
}
</script>