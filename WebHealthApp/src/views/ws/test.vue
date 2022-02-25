<template>
  <div>
    <button @click="send">发消息</button>
  </div>
</template>

<script>
var socket = []
export default {
  data() {
    return {
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
  methods: {
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

<style>

</style>
