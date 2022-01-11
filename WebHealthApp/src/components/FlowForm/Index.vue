<template>
  <div class="flow-form">

    <div v-if="questions!==null&&questions.length>0" class="questions">
      <div v-for="(item, index) in questions" v-show="active==index" :key="index" class="question">
        <div class="question-title">{{ item.title }}</div>
        <div class="question-content">
          <div v-if="item.type==='input'">

            <mt-field v-model="item.value" class="qt-input" label="" placeholder="" style="width:300px" />

            <mt-button type="primary" class="btn-sure" @click="onInputSure(index)">确定</mt-button>

          </div>
          <div v-if="item.type==='date'">
            <mt-field v-model="item.value" class="qt-input" label="" placeholder="" style="width:300px" @click.native="openPickerByDate(index)" />

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
              <div v-for="(option,j) in item.options" :key="option.id" :class="isChecklistVal(item.value,option.value)===false?'qt-checklist-item ' :'qt-checklist-item on'" @click="onChecklist(index,option.value)">
                <span class="label">{{ option.label }}</span>
              </div>

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
    />

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
      idx_date: 0
    }
  },
  computed: {

  },
  methods: {
    onPrevious() {
      var question = this.questions[this.active]
      console.log(question)
      this.active = question.previous
      this.previous = question.previous
    },
    onInputSure(q_idx) {
      var val = this.questions[q_idx].value
      if (isEmpty(val)) {
        this.$toast('不能为空')
        return
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
      var val = this.questions[q_idx].value
      if (val.length <= 0) {
        this.$toast('至少选择一个')
        return
      }
      this.jump(q_idx)
    },
    onChecklist(q_idx, val) {
      var _val = this.questions[q_idx].value

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

      console.log(_val)
      this.questions[q_idx].value = _val
    },
    isChecklistVal(item, val) {
      var isFlag = false
      for (var i = 0; i < item.length; i++) {
        if (item[i] === val) {
          isFlag = true
          break
        }
      }

      // console.log(isFlag)

      return isFlag
    },
    openPickerByDate(index) {
      this.idx_date = index
      this.$refs.pickerByDate.open()
    },
    onConfirmByDate(val) {
      this.questions[this.idx_date].value = this.formatDate(val)
      console.log(val)
    },
    jump(q_idx, q_val) {
      var questions = this.questions
      var q_item = questions[q_idx]

      var jump_to = -1
      if (typeof q_item.jump !== 'undefined') {
        for (var ob in q_item.jump) {
          var skip_id = q_item.jump[ob]
          // console.log(skip_id)
          for (let i = 0; i < questions.length; i++) {
            if (questions[i].id === skip_id) {
              // console.log('nextSkip:' + i)
              this.nextSkip = i
            }
          }
        }

        var jump_id = q_item.jump[q_val]
        for (let i = 0; i < questions.length; i++) {
          if (questions[i].id === jump_id) {
            jump_to = i
            break
          }
        }
      } else {
        console.log(this.nextSkip)
        if (this.nextSkip == null) {
          jump_to = q_idx + 1
        } else {
          jump_to = this.nextSkip + 1
          this.nextSkip = null
        }
      }

      var _this = this
      setTimeout(function() {
        _this.active = jump_to
        _this.questions[jump_to].previous = q_idx
        _this.previous = q_idx
      }, 500)
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

.btn-sure{
  width:100%;
  margin-top: 25px;
}

.btn-previous{
  border: 0px;
  margin-top: 20px;
}

.qt-input{
  border-radius: 5px;
}

.mint-cell:last-child{
  background-size: 100% 0px;
}

.question{

}

.question-title{
    color:#474749;
    font-size: 28px;
    display: flex;
    justify-content: center;
    padding: 30px 0px;
}

.question-content{
    display: flex;
    justify-content: center;
}

.qt-radio{
  display: flex;
  flex-direction: column;
}

.qt-radio .on{
     background-color: #2a2a2a;
     border: 1px solid #2a2a2a;
     color: #fff;
}

.qt-radio-item{
    padding: 10px 20px;
    border: 1px solid #fff;
    border-radius: 10px;
    background-color: #fff;
    min-width: 200px;
    text-align: center;
    font-weight: 600;
    margin: 10px;
}

.qt-checklist{

}

.qt-checklist .on{
     background-color: #2a2a2a;
     border: 1px solid #2a2a2a;
     color: #fff;
}

.qt-checklist-item{
    float: left;
    padding: 10px 20px;
    border: 1px solid #fff;
    border-radius: 10px;
    background-color: #fff;
    min-width: 80px;
    text-align: center;
    font-weight: 600;
    margin: 10px;
}

</style>
