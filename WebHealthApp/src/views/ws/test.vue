<template>
  <div>

    <mt-datetime-picker
      ref="pickerByDate"
      type="date"
      :start-date="new Date('1900-01-01')"
      @touchmove.native.stop.prevent
    />

    <mt-cell class="mint-field" style="width:300px" @click.native="openPickerByHeight(0)">
      <span style="margin-right:10px">  {{ item.value }} </span> <span>{{ item.append }}</span>
      <i class="mint-cell-allow-right" />
    </mt-cell>
    <mt-popup
      v-model="popupVisibleByHeight"
      position="bottom"
      style="width:100%;"
    >
      <div class="picker-toolbar" style="border-bottom: solid 1px #eaeaea;">
        <span class="mint-datetime-action mint-datetime-cancel" @click="popupVisibleByHeight=false">取消</span>
        <span class="mint-datetime-action mint-datetime-confirm" @click="onConfirmByHeight">确定</span>
      </div>

    </mt-popup>

    <button @click="send">发消息</button>
  </div>
</template>

<script>
var socket = []
export default {
  data() {
    return {
      idx_height: 0,
      item: {
        value: '60333',
        append: ''
      },
      popupVisibleByHeight: false,
      slotsByHeight: [{
        flex: 1,
        values: [],
        className: 'slot1',
        textAlign: 'center',
        defaultIndex: 0
      }],
      path: 'ws://47.106.255.69:3009/bar',
      socket: ''
    }
  },
  mounted() {
    // 初始化
    this.init()
  },
  destroyed() {
    // 销毁监听
    this.socket.onclose = this.close
  },
  created() {
    for (let index = 50; index < 200; index++) {
      this.slotsByHeight[0].values.push(index)
    }
  },
  methods: {
    openPickerByHeight(index) {
      this.idx_height = index

      // if (this.item.value !== '') {
      //   var defaultIndex = 0
      //   var j = 0
      //   for (let i = 50; i < 200; i++) {
      //     j++
      //     if (i.toString() === this.item.value.toString()) {
      //       defaultIndex = j - 1
      //       break
      //     }
      //   }

      //   this.slotsByHeight[0].defaultIndex = defaultIndex
      // }

      this.popupVisibleByHeight = true
    },
    onConfirmByHeight() {
      this.popupVisibleByHeight = false
    },
    onValuesChangeByHeight(picker, values) {

    },
    init: function() {
      this.connect('ws://47.106.255.69:3009/bar', '1004E747A302')

      // if (typeof (WebSocket) === 'undefined') {
      //   alert('您的浏览器不支持socket')
      // } else {
      //   // 实例化socket
      //   this.socket = new WebSocket(this.path)
      //   // 监听socket连接
      //   this.socket.onopen = this.open
      //   // 监听socket错误信息
      //   this.socket.onerror = this.error
      //   // 监听socket消息
      //   this.socket.onmessage = this.getMessage
      // }
    },
    open: function() {
      console.log('socket连接成功')
    },
    error: function() {
      console.log('连接错误')
    },
    getMessage: function(msg) {
      console.log(msg.data)
    },
    send: function() {
      this.$refs.pickerByDate.open()
    },
    close: function() {
      console.log('socket已经关闭')
    },
    connect: function(host, wsobj) {
      // 浏览器支持？
      if ('WebSocket' in window) {
        socket[wsobj] = new WebSocket(host)
        try {
          // 连接事件
          socket[wsobj].onopen = function(msg) {
            console.log('socket连接成功')
            socket[wsobj].send(JSON.stringify({ 'online': wsobj }))
            // alert(wsobj+":连接已建立！");
          }
          // 错误事件
          socket[wsobj].onerror = function(msg) {
            console.log('连接错误')
          }

          // 消息事件
          socket[wsobj].onmessage = function(msg) {
            var data = JSON.parse(msg.data)
            console.log(data.online)
          }
          // 关闭事件
          socket[wsobj].onclose = function(msg) {
            console.log('socket已经关闭')
          }
        } catch (ex) {
          console.log(ex)
        }
      }
    }
  }
}
</script>

<style lang="scss" scoped>

.picker{
  width: 100%;
    height: 100%;
        background-color: green;
}

/deep/ .picker-items{
width: 100%;
    height: 100%;
}

/deep/ .picker-items{
width: 100%;
    height: 100%;
    background-color: red;
}

.picker-slot{
  width: 100%;
    height: 100%;
    background-color: blue;
}
</style>
