<template>
  <div :style="svgStyle">
    <svg :style="svgStyle">
      <!--    内边框 -->
      <path :d="pathString" :stroke="trailColor" :stroke-width="trailWidth" :fill-opacity="0" />
      <!--  外边框 -->
      <path :d="pathString" stroke-linecap="round" :stroke="strokeColor" :stroke-width="strokeWidth" fill-opacity="0" :style="pathStyle" />
    </svg>
    <div class="vux-circle-content"><slot /></div>
  </div>
</template>

<script>
/**
 1.进度条中间文字支持自定义

*/

export default {
  name: 'ProcessCircle',
  props: {
    strokeWidth: {// 线条宽度
      type: Number,
      default: 1
    },
    strokeColor: {// 线条颜色
      type: String,
      default: '#3FC7FA'
    },
    trailWidth: { // 背景线条宽度
      type: Number,
      default: 1
    },
    trailColor: { // 背景线条宽度
      type: String,
      default: '#D9D9D9'
    },
    percent: { // 进度百分比
      type: Number,
      default: 0
    },
    rd: { // 圆大小半径
      type: Number,
      default: 50
    }
  },
  computed: {
    svgStyle() {
      return {
        'width': `${2 * this.rd}px`,
        'height': `${2 * this.rd}px`,
        'position': 'relative'
      }
    },
    radius() { // 外圆圆半径
      return this.rd - this.strokeWidth / 2
    },
    pathString() {
      return `M ${this.rd},${this.rd} m 0,-${this.radius}
      a ${this.radius},${this.radius} 0 1 1 0,${2 * this.radius}
      a ${this.radius},${this.radius} 0 1 1 0,-${2 * this.radius}`
    },
    len() { // Math.PI 圆周率 3.141592653589793  len是圆的周长
      return Math.PI * 2 * this.radius
    },
    pathStyle() {
      return {
        'stroke-dasharray': `${this.len}px ${this.len}px`,
        'stroke-dashoffset': `${((100 - this.percent) / 100 * this.len)}px`, // 动画占圆周长百分比
        'transition': 'stroke-dashoffset 1s ease' // 2s代表动画时间
      }
    }
  }
}
</script>

<style>
.vux-circle-content {
  text-align: center;
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%,-50%);
}
</style>
