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
            <mt-cell class="mint-field" style="width:300px" @click.native="openPickerNumber(index,'height')">
              <span style="margin-right:10px">  {{ item.value }} </span> <span>{{ item.append }}</span>
              <i class="mint-cell-allow-right" />
            </mt-cell>
            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>
          </div>
          <div v-if="item.type==='weight'" class="">
            <mt-cell class="mint-field" style="width:300px" @click.native="openPickerNumber(index,'weight')">
              <span style="margin-right:10px">  {{ item.value }} </span> <span>{{ item.append }}</span>
              <i class="mint-cell-allow-right" />
            </mt-cell>
            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>
          </div>
          <div v-if="item.type==='gmperiod'" class="">
            <mt-cell title="末次月经时间" class="mint-field" style="width:300px" @click.native="openPickerDate(index)">
              <span style="margin-right:10px">  {{ item.value[0] }} </span>
              <i class="mint-cell-allow-right" />
            </mt-cell>
            <mt-cell title="经期天数" class="mint-field" style="width:300px" @click.native="openPickerNumber(index,'gm_day')">
              <span style="margin-right:10px">  {{ item.value[1] }} </span>
              <i class="mint-cell-allow-right" />
            </mt-cell>
            <mt-cell title="月经周期" class="mint-field" style="width:300px" @click.native="openPickerNumber(index,'gm_period')">
              <span style="margin-right:10px">  {{ item.value[2] }} </span>
              <i class="mint-cell-allow-right" />
            </mt-cell>
            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>
          </div>
          <div v-if="item.type==='date'">
            <mt-cell class="mint-field" style="width:300px" @click.native="openPickerDate(index)">
              <span>  {{ item.value }} </span>
              <i class="mint-cell-allow-right" />
            </mt-cell>
            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>
          </div>
          <div v-if="item.type==='gesweek'">
            <mt-cell title="" class="mint-field" style="width:300px" @click.native="openPickerGesWeek(index)">
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
      ref="pickerDate"
      type="date"
      :start-date="new Date('1900-01-01')"
      @confirm="onConfirmPickerDate"
      @touchmove.native.stop.prevent
    />

    <mt-popup
      v-model="popupVisiblePickerGesWeek"
      position="bottom"
      style="width:100%"
    >
      <div class="picker-toolbar" style="border-bottom: solid 1px #eaeaea;">
        <span class="mint-datetime-action mint-datetime-cancel" @click="popupVisiblePickerGesWeek=false">取消</span>
        <span class="mint-datetime-action mint-datetime-confirm" @click="onConfirmPickerGesWeek">确定</span>
      </div>
      <mt-picker :slots="slotsPickerGesWeek" @change="onValuesChangePickerGesWeek" @touchmove.native.stop.prevent />
    </mt-popup>

    <mt-popup
      v-model="popupVisiblePickerNumber"
      position="bottom"
      style="width:100%"
    >
      <div class="picker-toolbar" style="border-bottom: solid 1px #eaeaea;">
        <span class="mint-datetime-action mint-datetime-cancel" @click="popupVisiblePickerNumber=false">取消</span>
        <span class="mint-datetime-action mint-datetime-confirm" @click="onConfirmPickerNumber">确定</span>
      </div>

      <mt-picker :slots="slotsPickerNumber" @change="onValuesChangePickerNumber" @touchmove.native.stop.prevent />

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
      idxPickerDate: 0,
      idxPickerGesweek: -1,
      popupVisiblePickerGesWeek: false,
      slotsPickerGesWeek: [
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
      idxPickerNumber: -1,
      idxPickerNumberType: '',
      popupVisiblePickerNumber: false,
      slotsPickerNumber: [{
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
      this.slotsPickerGesWeek[0].values.push(index)
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
      } else if (question.type === 'gmperiod') {
        if (isEmpty(val[0])) {
          this.$toast('请选择末次月经时间')
          return
        } else if (isEmpty(val[1])) {
          this.$toast('经期天数')
          return
        } else if (isEmpty(val[2])) {
          this.$toast('月经周期')
          return
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
    openPickerDate(index) {
      this.idxPickerDate = index
      var question = this.questions[this.idxPickerDate]

      var value = ''
      if (question.type === 'gmperiod') {
        value = question.value[0]
      } else {
        value = question.value
      }

      this.$picker.show({
        type: 'datePicker',
        date: value, // 初始化时间
        endTime: '2050-12-31', // 截至时间
        startTime: '1900-01-01', // 开始时间
        onOk: (e) => {
          this.onConfirmPickerDate(e)
        }

      })

      // this.$refs.pickerDate.open()
    },
    onConfirmPickerDate(val) {
      console.log(val)
      var question = this.questions[this.idxPickerDate]
      console.log(question.type)
      if (question.type === 'gmperiod') {
        var value = question.value
        this.questions[this.idxPickerDate].value = [this.formatDate(val), value[1], value[2]]
      } else {
        this.questions[this.idxPickerDate].value = this.formatDate(val)
      }
    },
    openPickerGesWeek(index) {
      this.idxPickerGesweek = index
      this.popupVisiblePickerGesWeek = true
      this.slotsPickerGesWeek[0].defaultIndex = this.questions[index].value[0] - 1
      this.slotsPickerGesWeek[2].defaultIndex = this.questions[index].value[1] - 1
    },
    onConfirmPickerGesWeek() {
      this.popupVisiblePickerGesWeek = false
    },
    onValuesChangePickerGesWeek(picker, values) {
      console.log(values)
      if (this.idxPickerGesweek > -1) {
        this.questions[this.idxPickerGesweek].value[0] = values[0]
        this.questions[this.idxPickerGesweek].value[1] = values[1]
      }
    },
    openPickerNumber(index, type) {
      this.idxPickerNumber = index
      this.idxPickerNumberType = type
      var question = this.questions[index]
      var defaultIndex = 0
      this.slotsPickerNumber[0].values = []
      if (type === 'weight') {
        for (let i = 50; i < 200; i++) {
          this.slotsPickerNumber[0].values.push(i)
          if (i.toString() === question.value.toString()) {
            defaultIndex = i - 50
          }
        }

        this.slotsPickerNumber[0].defaultIndex = defaultIndex
      } else if (type === 'height') {
        for (let i = 10; i < 200; i++) {
          this.slotsPickerNumber[0].values.push(i)
          if (i.toString() === question.value.toString()) {
            defaultIndex = i - 10
          }
        }
        this.slotsPickerNumber[0].defaultIndex = defaultIndex
      } else if (type === 'gm_day') {
        for (let i = 1; i < 10; i++) {
          this.slotsPickerNumber[0].values.push(i)
          if (i.toString() === question.value[1].toString()) {
            defaultIndex = i - 1
          }
        }
        this.slotsPickerNumber[0].defaultIndex = defaultIndex
      } else if (type === 'gm_period') {
        for (let i = 1; i < 30; i++) {
          this.slotsPickerNumber[0].values.push(i)
          if (i.toString() === question.value[2].toString()) {
            defaultIndex = i - 1
          }
        }
        this.slotsPickerNumber[0].defaultIndex = defaultIndex
      }
      //  if (question.value !== '') {
      //    var defaultIndex = 0
      //    var j = 0
      // //   for (let i = 50; i < 200; i++) {
      // //     j++
      // //     if (i.toString() === question.value.toString()) {
      // //       defaultIndex = j - 1
      // //       break
      // //     }
      // //   }

      // //   this.slotsByHeight[0].defaultIndex = defaultIndex
      // // }

      this.popupVisiblePickerNumber = true
    },
    onConfirmPickerNumber() {
      this.popupVisiblePickerNumber = false
    },
    onValuesChangePickerNumber(picker, values) {
      if (this.idxPickerNumber > -1) {
        var question = this.questions[this.idxPickerNumber]
        if (this.idxPickerNumberType === 'gm_day') {
          var value = question.value
          this.questions[this.idxPickerNumber].value = [value[0], values[0], value[2]]
        } else if (this.idxPickerNumberType === 'gm_period') {
          var value = question.value
          this.questions[this.idxPickerNumber].value = [value[0], value[1], values[0]]
        } else {
          this.questions[this.idxPickerNumber].value = values[0]
        }
      }
    },
    jump(q_idx, q_val) {
      var questions = this.questions
      var q_item = questions[q_idx]

      var jump_to = -1
      if (typeof q_item.jump !== 'undefined') {
        var jump_type = Object.prototype.toString.call(q_item.jump)
        if (jump_type === '[object Object]') {
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
          for (let i = 0; i < questions.length; i++) {
            if (questions[i].id === q_item.jump) {
              jump_to = i
              break
            }
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

<style lang="scss" scoped>

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

/deep/  .picker{
  width: 100%;
  height: 100%;
}

/deep/  .picker-items{
width: 100%;
height: 100%;
}

</style>
