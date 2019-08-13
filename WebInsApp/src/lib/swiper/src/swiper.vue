<template>
    <div class="lumos-swiper pd">
        <div class="wrapper" ref="swiper">
            <div class="item" ref="ritem" v-for="(item, index) in newlist"
            @touchstart="touchStart($event)"
            @touchmove="touchMove($event)"
            @touchend="touchEnd($event)"
              >
             
             <img :src="item.src">

            </div>
        </div>
    </div>
</template>


<script>
export default {
  name: "lumos-swiper",
  data() {
    return {
      startX: "",
      moveX: "",
      list: [
        { src: "http://file.17fanju.com/Upload/Banner/1.png" },
        { src: "http://file.17fanju.com/Upload/Banner/2.png" }
      ],
      contenter: this.$refs.swiper,
      active: -1,
      off: true,
      autoplay: 2000,
      start: null
    };
  },
  mounted: function() {
    this.$nextTick(() => {
      this.backlate();
      this.backtime();
      this._autoplay();
    });
  },

  computed: {
    //生成新的图片组合
    newlist: function() {
      this.list.push(this.list[0]);
      this.list.unshift(this.list[this.list.length - 2]);
      return this.list;
    }
  },
  methods: {
    //图片移动
    backlate(offert) {
      let _that = this;
      if (!offert) offert = 0;
      this.list.forEach((item, index) => {
        this.$refs.ritem[index].style.transform =
          "translate3d(" +
          ((index + this.active) * _that.$refs.ritem[index].clientWidth +
            offert) +
          "px,0,0)";
      });
    },
    //图片移动时间
    backtime(duration) {
      if (!duration) duration = "0ms";
      this.$refs.ritem.forEach(item => {
        item.style.webkitTransition = duration;
        item.style.transition = duration;
      });
    },
    touchStart(e) {
      this.startX = e.touches[0].pageX;
    },
    touchMove(e) {
      e.preventDefault();
      e.stopPropagation();
      this.moveX = e.touches[0].pageX - this.startX;
      this.backlate(this.moveX);
    },
    touchEnd(e) {
      this.backtime("300ms");
      if (this.moveX > 100) {
        this.backlate(this.$refs.ritem[0].clientWidth);
        this.active++;
      } else if (this.moveX < -100) {
        this.backlate(-this.$refs.ritem[0].clientWidth);
        this.active--;
      }

      this.setactive(this.active);
      setTimeout(() => {
        this._autoplay();
      });
    },
    //循环滚动处理
    setactive(active) {

      if (active == 0) {
        this.active = -(this.list.length - 2);
      } else if (active == -(this.list.length - 1)) {
        this.active = -1;
      } else {
        return false;
      }
      this.backtime();
      setTimeout(() => {
        this.backlate();
      }, 300);
    },
    next() {
      this.backtime("300ms");
      this.active--;
      this.backlate();
      this.setactive(this.active);
    },
    _autoplay() {
      if (this.autoplay != "") {
        this.cleartime();
        this.start = setTimeout(() => {
          this.next();
          this._autoplay();
        }, this.autoplay);
      }
    },
    cleartime() {
      clearTimeout(this.start);
      this.start = null;
    }
  }
};
</script>

<style lang="less" scoped>
.lumos-swiper {
  overflow: hidden;
  width: 100%;
  position: relative;
  background-color: #fff;
  padding: 1rem 0;
  > .wrapper {
    width: 100%;
    height: 150px;
    position: relative;
    overflow: hidden;
    .item {
      width: 100%;
      position: absolute;
      flex: 1;
      height: 100%;
      background-size: 100%;
      text-align: center;
      font-size: 30px;
      color: #fff;

      img {
        border-radius: 10px;
      }
    }
  }
}
</style>