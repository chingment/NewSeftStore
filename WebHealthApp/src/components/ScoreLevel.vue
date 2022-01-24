<template>
  <div class="i-sign">
    <template v-for="(item, index) in range">
      <div class="col" v-bind="index">
        <div class="topnum" :style="(index>=0&&index<range.length-1?'text-align:left;':'text-align:right')+(index>=1&&index<range.length-1?'visibility: hidden;':'')">
          {{ (index>=0&&index<=(range.length-1))?item.min:item.max }}
        </div>
        <div :class="'mt-range jd1 '+((score>=item.min&&score<item.max)?'':'no-ative') ">
          <div class="mt-range-content">
            <div class="mt-range-runway" :style="'border-top-width: 8px;border-top-color:'+item.color+';'" />
            <div class="mt-range-progress" style="width: 0%; height: 8px;" />
            <div class="mt-range-thumb" :style="'left: '+score+'%;background-color:'+item.color+';'" />
          </div>
        </div>
        <div class="bottomtips">{{ item.tips }}</div>
      </div>
      <div v-if="index<range.length-1" class="col-split" v-bind="index">
        <div class="topnum">{{ item.max }}</div>
        <div class="border-split" />
      </div>
    </template>
  </div>
</template>

<script>

export default {
  name: 'ScoreLevel',
  props: {
    value: {
      type: Object,
      default: null
    },
    sign: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      score: 60,
      range: [
        {
          min: 0,
          max: 30,
          color: '#e68a8b',
          tips: '差'
        },
        {
          min: 30,
          max: 50,
          color: '#f1b46d',
          tips: '较差'
        }, {
          min: 50,
          max: 70,
          color: '#e16d6d',
          tips: '中等'
        }, {
          min: 70,
          max: 90,
          color: '#96a2dc',
          tips: '较好'
        }, {
          min: 90,
          max: 100,
          color: '#628DF2',
          tips: '好'
        }
      ],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  }
}
</script>

<style lang="scss" scoped>

    .i-sign {
      display: flex;

      padding: 10px 0;

      .col {
        flex: 1;

        .topnum {
          font-size: 12px;

          padding: 5px 0;

          text-align: left;

          color: gray;
        }

        .bottomtips {
          font-size: 12px;

          text-align: center;

          color: gray;
        }

        /deep/ .mt-range-runway {
          border-radius: 20px;
        }

        /deep/ .mt-range-thumb {
          top: 4px;

          width: 20px;
          height: 20px;
        }

        /deep/ .no-ative {
          .mt-range-thumb {
            display: none  !important;
          }
        }
      }

      .col-split {
        display: flex;
        align-items: center;
        flex-direction: column;
        justify-content: flex-start;

        .topnum {
          font-size: 12px;

          padding: 5px 0;

          color: gray;
        }

        .border-split {
          display: inline-block;

          width: 1px;
          height: 44px;

          border-left: 1px dashed #c5c5c5;
        }
      }
    }

</style>
