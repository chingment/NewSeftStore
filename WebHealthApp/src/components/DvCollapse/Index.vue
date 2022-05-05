<template>
  <div class="dv-collapse">
    <div class="collapse-item" @click="collapse = !collapse">
      <div class="t1">
        <span>{{ item.name }}</span>
      </div>
      <div class="t2">
        <span :style="{'color': item.color}">{{ item.value }}</span>
      </div>
      <div class="t3">
        <span :style="{'color': item.color}">{{ item.tips }}</span>
      </div>
      <div class="t4">
        <span style="float: right">
          <i v-if="collapse" class="el-icon-arrow-down" />
          <i v-else class="el-icon-arrow-right" />
        </span>
      </div>
    </div>
    <div v-if="collapse" class="collapse-more-item">
      <score-level :tag-dv="item" />
    </div>
  </div>
</template>

<script>
import ScoreLevel from '@/components/ScoreLevel.vue'
export default {
  name: 'LjCollapse',
  components: { ScoreLevel },
  filters: {},
  props: {
    item: {
      type: Object,
      default: null
    },
    headColor: {
      type: String,
      default: 'orange'
    },
    bgColor: {
      type: String,
      default: '#fff' // 浅黑
      // default: "#253266b7",//浅蓝
    },
    width: {
      type: String,
      default: '100%'
    },
    isCollapse: {
      type: Boolean,
      default: true
    }
  },
  data() {
    return {
      collapse: this.isCollapse
    }
  },
  computed: {},
  watch: {},
  created() {},
  mounted() {},
  methods: {}
}
</script>

<style lang="scss" scoped>
.dv-collapse {
  padding: 10px;

  border-bottom: 1px solid #efefef;

  .collapse-item {
    font-size: 14px;
    line-height: 30px;

    display: flex;

    .t1,
    .t2,
    .t3 {
      flex: 1;
    }

    .t2 {
      font-weight: bold;

      display: flex;
      justify-content: center;
    }

    .t3 {
      display: flex;
      justify-content: flex-end;
    }

    .t4 {
      width: 30px;
    }
  }

.collapse-more-item{
  margin-top: 10px;
  font-size: 18px
}

}

.dv-collapse:last-child{
    border-bottom: 0px solid #efefef;
}

</style>
