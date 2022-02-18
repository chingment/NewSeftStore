<template>
  <div class="flow-form">

    <div v-if="questions!==null&&questions.length>0" class="questions">
      <div v-for="(item, index) in questions" v-show="active==index" :key="index" class="question">
        <div class="question-title">{{ item.title }}</div>
        <div class="question-content">
          <div v-if="item.type==='input'" class="">
            <mt-field v-model="item.value" class="qt-input" label="" placeholder="" style="width:300px">
              <span>{{ item.append }}</span>
            </mt-field>
            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>
          </div>
          <div v-if="item.type==='height'" class="">
            <mt-cell class="mint-field" style="width:300px" @click.native="openPickerByHeight(index)">
              <span style="margin-right:10px">  {{ item.value }} </span> <span>{{ item.append }}</span>
              <i class="mint-cell-allow-right" />
            </mt-cell>
            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>
          </div>
          <div v-if="item.type==='weight'" class="">
            <mt-cell class="mint-field" style="width:300px" @click.native="openPickerByWeight(index)">
              <span style="margin-right:10px">  {{ item.value }} </span> <span>{{ item.append }}</span>
              <i class="mint-cell-allow-right" />
            </mt-cell>
            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>
          </div>
          <div v-if="item.type==='date'">

            <mt-cell class="mint-field" style="width:300px" @click.native="openPickerByDate(index)">
              <span>  {{ item.value }} </span>
              <i class="mint-cell-allow-right" />
            </mt-cell>

            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>
          </div>
          <div v-if="item.type==='gesweek'">
            <mt-cell title="" class="mint-field" style="width:300px" @click.native="openPickerByGesWeek(index)">
              <span>{{ item.value[0] }}周+{{ item.value[1] }}</span>
              <i class="mint-cell-allow-right" />
            </mt-cell>
            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>
          </div>
          <div v-if="item.type==='radio'">
            <div class="qt-radio">
              <div v-for="(option,j) in item.options" :key="option.id" :class="isRadioVal(option.value,item.value)===false?'qt-radio-item ' :'qt-radio-item on'" @click="onRadioSure(index,option.value)">
                <span class="label">{{ option.label }}</span>
              </div>
            </div>
          </div>
          <div v-if="item.type==='checklist'">
            <div class="qt-checklist">

              <div :class="onGetCheckNullStyle(index)" @click="onCheckSetNull(index)">
                <span class="label">均无</span>
              </div>

              <div v-for="(option,j) in item.options" :key="option.id" :class="isChecklistVal(item.value,option.value)===false?'qt-checklist-item ' :'qt-checklist-item on'" @click="onChecklist(index,option.value)">
                <span class="label">{{ option.label }}</span>
              </div>
              <!-- // {{ item.otherIsVisiable }} -->
              <!-- <div class="qt-checklist-item on">
                <input placeholder="dd" type="text" class="mint-field-core">
              </div> -->
              <!--
              <div v-else class="qt-checklist-item" @click="onChecklistAddOther(index)">
                <span class="label">其他</span>
              </div> -->

            </div>
            <mt-button type="primary" class="btn-sure" @click="onChecklistSure(index)">确定</mt-button>
          </div>

        </div>
      </div>

      <mt-button v-show="active>0" type="primary" class="btn-previous" plain @click="onPrevious">上一题</mt-button>
    </div>

    <mt-datetime-picker
      ref="pickerByDate"
      type="date"
      :start-date="new Date('1949-10-01')"
      @confirm="onConfirmByDate"
      @touchmove.native.stop.prevent
    />

    <mt-popup
      v-model="popupVisibleByGesWeek"
      position="bottom"
      style="width:100%"
    >
      <div class="picker-toolbar" style="border-bottom: solid 1px #eaeaea;">
        <span class="mint-datetime-action mint-datetime-cancel" @click="popupVisibleByGesWeek=false">取消</span>
        <span class="mint-datetime-action mint-datetime-confirm" @click="onConfirmByGesWeek">确定</span>
      </div>
      <mt-picker :slots="slotsByGesWeek" @change="onValuesChangeByGesWeek" @touchmove.native.stop.prevent />
    </mt-popup>

    <mt-popup
      v-model="popupVisibleByHeight"
      position="bottom"
      style="width:100%"
    >
      <div class="picker-toolbar" style="border-bottom: solid 1px #eaeaea;">
        <span class="mint-datetime-action mint-datetime-cancel" @click="popupVisibleByHeight=false">取消</span>
        <span class="mint-datetime-action mint-datetime-confirm" @click="onConfirmByHeight">确定</span>
      </div>

      <mt-picker :slots="slotsByHeight" @change="onValuesChangeByHeight" @touchmove.native.stop.prevent />

    </mt-popup>

    <mt-popup
      v-model="popupVisibleByWeight"
      position="bottom"
      style="width:100%"
    >
      <div class="picker-toolbar" style="border-bottom: solid 1px #eaeaea;">
        <span class="mint-datetime-action mint-datetime-cancel" @click="popupVisibleByWeight=false">取消</span>
        <span class="mint-datetime-action mint-datetime-confirm" @click="onConfirmByWeight">确定</span>
      </div>

      <mt-picker :slots="slotsByWeight" @change="onValuesChangeByWeight" @touchmove.native.stop.prevent />

    </mt-popup>

  </div>
</template>

<script>
import { isEmpty } from '@/utils/commonUtil'
export default {
  name: 'FlowForm',
  props: {
    questions: {
      type: Array,
      default() {
        return []
      }
    }
  },
  data() {
    return {
      active: 0,
      nextSkip: null,
      previous: null,
      idx_date: 0,
      idx_gesweek: -1,
      popupVisibleByGesWeek: false,
      slotsByGesWeek: [
        {
          flex: 1,
          values: [],
          className: 'slot1',
          textAlign: 'right',
          defaultIndex: 3
        }, {
          divider: true,
          content: '周 +',
          className: 'slot2'
        }, {
          flex: 1,
          values: [1, 2, 3, 4, 5, 6],
          className: 'slot3',
          textAlign: 'left',
          defaultIndex: 3
        }
      ],
      idx_height: -1,
      popupVisibleByHeight: false,
      slotsByHeight: [{
        flex: 1,
        values: [],
        className: 'slot1',
        textAlign: 'center',
        defaultIndex: 0
      }],
      idx_weight: -1,
      popupVisibleByWeight: false,
      slotsByWeight: [{
        flex: 1,
        values: [],
        className: 'slot1',
        textAlign: 'center',
        defaultIndex: 0
      }]
    }
  },
  computed: {

  },
  created() {
    for (let index = 1; index < 46; index++) {
      this.slotsByGesWeek[0].values.push(index)
    }
    for (let index = 50; index < 200; index++) {
      this.slotsByHeight[0].values.push(index)
    }

    for (let index = 10; index < 200; index++) {
      this.slotsByWeight[0].values.push(index)
    }
  },
  methods: {
    onPrevious() {
      var question = this.questions[this.active]
      // console.log(question)
      this.active = question.previous
      this.previous = question.previous
    },
    onInputSure(q_idx) {
      var question = this.questions[q_idx]
      var val = question.value
      if (typeof question.value === 'string') {
        if (typeof question.rule !== 'undefined') {
          var rule = question.rule
          if (rule.required) {
            if (isEmpty(val)) {
              this.$toast(rule.message)
              return
            } else if (val.length > rule.max) {
              this.$toast(rule.message)
              return
            }
          }

          if (typeof rule.pattern !== 'undefined') {
            if (!rule.pattern.test(val)) {
              this.$toast(rule.message)
              return
            }
          }
        }
      }

      this.jump(q_idx)
    },
    onRadioSure(q_idx, val) {
      // console.log(val)
      this.questions[q_idx].value = val

      // this.questions = this.questions
      this.jump(q_idx, val)
    },
    onRadio(q_idx, val) {
      // console.log(val)
      this.questions[q_idx].value = val

      // this.questions = this.questions
      this.jump(q_idx, val)
    },
    isRadioVal(item, val) {
      // console.log('item:' + item)
      // console.log('val:' + val)
      if (typeof val === 'undefined') { return false }

      if (item === val) { return true }

      return false
    },
    onChecklistSure(q_idx) {
      var question = this.questions[q_idx]
      if (question.required) {
        var val = question.value
        if (val == null || val.length <= 0) {
          this.$toast('至少选择一个')
          return
        }
      }
      this.jump(q_idx)
    },
    onChecklist(q_idx, val) {
      var _val = this.questions[q_idx].value

      // console.log(q_idx)
      // console.log(val)
      // console.log(_val)

      if (_val.length === 0) {
        _val.push(val)
      } else {
        var isDel = false
        for (var i = 0; i < _val.length; i++) {
          if (_val[i] === val) {
            _val.splice(i, 1)
            isDel = true
            break
          }
        }

        if (!isDel) {
          _val.push(val)
        }
      }

      if (_val <= 0) {
        this.questions[q_idx].value = ['0']
      } else {
        _val.some((item, i) => {
          if (item === '0') {
            _val.splice(i, 1)
            return true
          }
        })
      }

      this.questions[q_idx].value = _val
    },
    onCheckSetNull(q_idx) {
      this.questions[q_idx].value = ['0']
    },
    onChecklistAddOther(q_idx) {
      var questions = this.questions[q_idx]
      questions.otherIsVisiable = true
      this.questions[q_idx] = questions
      console.log(this.questions[q_idx].otherIsVisiable)
    },
    isChecklistVal(item, val) {
      var isFlag = false
      if (item != null) {
        for (var i = 0; i < item.length; i++) {
          if (item[i] === val) {
            isFlag = true
            break
          }
        }
      }

      // console.log(isFlag)

      return isFlag
    },
    onGetCheckNullStyle(q_idx) {
      var val = this.questions[q_idx].value
      if (val == null) {
        return 'qt-checklist-item'
      } else if (val.length === 0) {
        return 'qt-checklist-item'
      } else if (val.length === 1 && val[0] === '0') {
        return 'qt-checklist-item on'
      } else {
        return 'qt-checklist-item'
      }
    },
    openPickerByDate(index) {
      this.idx_date = index
      this.$refs.pickerByDate.open()
    },
    onConfirmByDate(val) {
      this.questions[this.idx_date].value = this.formatDate(val)
      // console.log(val)
    },
    openPickerByGesWeek(index) {
      this.idx_gesweek = index
      this.popupVisibleByGesWeek = true
      this.slotsByGesWeek[0].defaultIndex = this.questions[index].value[0] - 1
      this.slotsByGesWeek[2].defaultIndex = this.questions[index].value[1] - 1
    },
    onConfirmByGesWeek() {
      this.popupVisibleByGesWeek = false
    },
    onValuesChangeByGesWeek(picker, values) {
      console.log(values)
      if (this.idx_gesweek > -1) {
        this.questions[this.idx_gesweek].value[0] = values[0]
        this.questions[this.idx_gesweek].value[1] = values[1]
      }
    },
    openPickerByHeight(index) {
      this.idx_height = index

      var question = this.questions[this.idx_height]
      if (question.value !== '') {
        var defaultIndex = 0
        var j = 0
        for (let i = 50; i < 200; i++) {
          j++
          if (i.toString() === question.value.toString()) {
            defaultIndex = j - 1
            break
          }
        }

        this.slotsByHeight[0].defaultIndex = defaultIndex
      }

      this.popupVisibleByHeight = true
    },
    onConfirmByHeight() {
      this.popupVisibleByHeight = false
    },
    onValuesChangeByHeight(picker, values) {
      if (this.idx_height > -1) {
        this.questions[this.idx_height].value = values[0]
      }
    },
    openPickerByWeight(index) {
      this.idx_weight = index

      var question = this.questions[this.idx_weight]
      if (question.value !== '') {
        var defaultIndex = 0
        var j = 0
        for (let i = 10; i < 200; i++) {
          j++
          if (i.toString() === question.value.toString()) {
            defaultIndex = j - 1
            break
          }
        }

        this.slotsByWeight[0].defaultIndex = defaultIndex
      }

      this.popupVisibleByWeight = true
    },
    onConfirmByWeight() {
      this.popupVisibleByWeight = false
    },
    onValuesChangeByWeight(picker, values) {
      if (this.idx_weight > -1) {
        this.questions[this.idx_weight].value = values[0]
      }
    },
    jump(q_idx, q_val) {
      var questions = this.questions
      var q_item = questions[q_idx]

      var jump_to = -1
      if (typeof q_item.jump !== 'undefined') {
        var skip_ids = []
        for (var ob in q_item.jump) {
          var skip_id = q_item.jump[ob]

          var k1 = skip_ids.filter(function(item) {
            return item === skip_id
          })

          if (k1 == null) {
            skip_ids.push(skip_id)
          }
        }

        for (let x = 0; x < skip_ids.length; x++) {
          for (let i = 0; i < questions.length; i++) {
            console.log(skip_ids[x])
            if (questions[i].id === skip_ids[x]) {
            // console.log('nextSkip:' + i + 'q_item.id:' + q_item.id + 'skip_id:' + skip_id)
              this.nextSkip = i
            }
          }
        }

        console.log('this.nextSkip:' + this.nextSkip)
        var jump_id = q_item.jump[q_val]
        for (let i = 0; i < questions.length; i++) {
          if (questions[i].id === jump_id) {
            jump_to = i
            break
          }
        }
      } else {
        console.log('q_idx:' + q_idx + ',nextSkip:' + this.nextSkip)
        if (this.nextSkip == null) {
          jump_to = q_idx + 1
        } else {
          // console.log('this.nextSkip:' + this.nextSkip + ',val:' + q_idx)
          if (q_idx > this.nextSkip) {
            // console.log('a')
            jump_to = q_idx + 1

            // jump_to = this.nextSkip + 1
          } else {
            // console.log('b')
            jump_to = this.nextSkip + 1
          }
          this.nextSkip = null
        }
      }

      if (jump_to <= this.questions.length - 1) {
        var _this = this
        setTimeout(function() {
          _this.active = jump_to
          _this.questions[jump_to].previous = q_idx
          _this.previous = q_idx
        }, 500)
      } else {
        this.$emit('on-complete')
      }
    },
    formatDate(secs, type = 0) {
      var t = new Date(secs)
      var year = t.getFullYear()
      var month = t.getMonth() + 1
      if (month < 10) { month = '0' + month }
      var date = t.getDate()
      if (date < 10) { date = '0' + date }
      var hour = t.getHours()
      if (hour < 10) { hour = '0' + hour }
      var minute = t.getMinutes()
      if (minute < 10) { minute = '0' + minute }
      var second = t.getSeconds()
      if (second < 10) { second = '0' + second }
      if (type === 0) {
        return year + '-' + month + '-' + date
      } else {
        return year + '-' + month + '-' + date
      }
    }
  }
}
</script>

<style scoped>

.btn-sure {
  width: 100%;
  margin-top: 25px;
}

.btn-previous {
  margin-top: 20px;

  border: 0;
}

.qt-input {
  border-radius: 5px;
}

.mint-cell:last-child {
  background-size: 100% 0;
}

.question-title {
  font-size: 28px;

  display: flex;
  justify-content: center;

  padding: 30px 0;

  color: #474749;
}

.question-content {
  display: flex;
  justify-content: center;
}

.qt-radio {
  display: flex;
  flex-direction: column;
}

.qt-radio .on {
  color: #fff;
  border: 1px solid #2a2a2a;
  background-color: #2a2a2a;
}

.qt-radio-item {
  font-weight: 600;

  min-width: 200px;
  margin: 10px;
  padding: 10px 20px;

  text-align: center;

  border: 1px solid #fff;
  border-radius: 10px;
  background-color: #fff;
  background-color: #f6f6f8;
}

.qt-checklist .on {
  color: #fff;
  border: 1px solid #2a2a2a;
  background-color: #2a2a2a;
}

.qt-checklist-item {
  font-weight: 600;

  float: left;

  min-width: 80px;
  margin: 10px;
  padding: 10px 20px;

  text-align: center;

  border: 1px solid #fff;
  border-radius: 10px;
  background-color: #fff;
  background-color: #f6f6f8;
}

</style>
