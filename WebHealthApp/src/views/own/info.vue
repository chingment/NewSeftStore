<template>
  <div
    id="own_info"
    class="own-info"
    style="display: flex;
    flex-direction: column;
    height: 100%;"
  >
    <div class="page-header">
      <span>客户信息</span>
    </div>
    <div class="page-bodyer" style="flex:1;overflow: auto;">
      <mt-field v-model="form.fullName" label="姓名" placeholder="请输入姓名" />
      <mt-field v-model="form.age" label="年龄" placeholder="请输入年龄" type="number" />
      <mt-cell title="孕周" class="mint-field" @click.native="openPickerByGesWeek">
        <span>{{ form.gesWeek }}周+{{ form.gesDay }}</span>
        <i class="mint-cell-allow-right" />
      </mt-cell>
      <mt-cell title="分娩时间" class="mint-field" @click.native="openPickerByDeliveryTime">
        <span>{{ form.deliveryTime }}</span>
        <i class="mint-cell-allow-right" />
      </mt-cell>
      <mt-cell title="分娩后异常" class="mint-field">
        <div class="c_radio clear">
          <mt-checklist
            v-model="form.deliveryExState"
            title=""
            :options="[
              {label: '出血量多',value: 1},
              {label: '伤口发炎',value: 2},
              {label: '发烧',value: 3},
              {label: '其它',value: 99}]"
          />
        </div>
      </mt-cell>
      <mt-field v-model="form.chiefComplaint" label="主诉症状" placeholder="请输入主诉症状" />
      <mt-field v-model="form.complication" label="孕期合并症" placeholder="请输入孕期合并症" />
      <mt-cell title="孕期用药" class="mint-field">
        <div class="c_radio clear">
          <mt-radio
            v-model="form.isMedicine"
            title=""
            :options="[ {label: '无',value: '0'},
                        {label: '有',value: '1'},]"
          />
        </div>
      </mt-cell>
      <mt-field v-if="form.isMedicine==='1'" v-model="form.fullName" label="用药情况" placeholder="请输入用药情况" />
      <mt-field v-model="form.medicalhistory" label="病史" placeholder="请输入病史" />
      <mt-cell title="分娩方式" class="mint-field">
        <div class="c_radio clear">
          <mt-radio
            v-model="form.deliveryWay"
            title=""
            :options="[ {label: '自然顺产',value: '1'},
                        {label: '剖腹产',value: '2'},]"
          />
        </div>
      </mt-cell>
      <mt-field v-if="form.deliveryWay==='2'" v-model="form.cesareanReason" label="剖腹产原因" placeholder="请输入剖腹产原因" />
      <mt-cell title="产钳" class="mint-field">
        <div class="c_radio clear">
          <mt-radio
            v-model="form.isForcep"
            title=""
            :options="[ {label: '使用',value: '1'},
                        {label: '无',value: '0'},]"
          />
        </div>
      </mt-cell>
      <mt-field v-model="form.fmsSituation" label="产时情况" placeholder="请输入产时情况" />
      <mt-cell title="产时尿管" class="mint-field">
        <div class="c_radio clear">
          <mt-radio
            v-model="form.isUreter"
            title=""
            :options="[ {label: '插',value: '1'},
                        {label: '未插',value: '0'},]"
          />
        </div>
      </mt-cell>
      <mt-cell title="产时胎膜" class="mint-field">
        <div class="c_radio clear">
          <mt-radio
            v-model="form.fmsMembrane"
            title=""
            :options="[ {label: '自然',value: '1'},
                        {label: '早破',value: '2'},
                        {label: '人工',value: '3'},
                        {label: '残留',value: '4'}]"
          />
        </div>
      </mt-cell>
      <mt-field v-model="form.foodProhibited" label="食物禁忌" placeholder="请输入食物禁忌况" />
      <mt-field v-model="form.fullName" label="备注" placeholder="请输入备注" />
    </div>

    <div
      class="page-footer"
      style="width: 100%;text-align: center; padding: 10px 0px;border-top: solid 1px #eaeaea;"
    >
      <mt-button type="primary" style="width:80%">保存 </mt-button>
    </div>

    <mt-datetime-picker
      ref="pickerByDeliveryTime"
      v-model="selectedValueByDeliveryTime"
      type="date"
      @confirm="onConfirmByDeliveryTime()"
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
      <mt-picker :slots="slots" @change="onValuesChangeByGesWeek" />
    </mt-popup>
  </div>
</template>

<script>
export default {
  name: 'OwnInfo',
  data() {
    return {
      loading: false,
      popupVisibleByGesWeek: false,
      selectedValueByDeliveryTime: '2021-12-29',
      form: {
        a: '',
        fullName: '',
        age: '',
        sex: '1',
        gesWeek: 1,
        gesDay: 1,
        deliveryTime: '2021-12-29',
        chiefComplaint: '',
        complication: '',
        isMedicine: '0',
        medicalhistory: '',
        deliveryWay: '1',
        deliveryExState: [1],
        isForcep: '0',
        fmsSituation: '',
        isUreter: '0',
        fmsMembrane: '1',
        foodProhibited: '',
        cesareanReason: ''
      },
      slots: [
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
      ]
    }
  },
  created() {
    for (let index = 1; index < 46; index++) {
      this.slots[0].values.push(index)
    }
  },
  methods: {
    openPickerByDeliveryTime() {
      this.$refs.pickerByDeliveryTime.open()
    },
    openPickerByGesWeek() {
      this.popupVisibleByGesWeek = true
      this.slots[0].defaultIndex = this.form.gesWeek - 1
      this.slots[2].defaultIndex = this.form.gesDay - 1
    },
    onConfirmByDeliveryTime() {
      this.form.deliveryTime = this.formatDate(this.selectedValueByDeliveryTime)
    },
    onConfirmByGesWeek() {
      this.popupVisibleByGesWeek = false
    },
    onValuesChangeByGesWeek(picker, values) {
      this.form.gesWeek = values[0]
      this.form.gesDay = values[1]
      // console.log(picker)
      // console.log(values)
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

<style lang="scss" scope>

.page-header {
  font-size: 24px;

  display: flex;
  align-items: center;
  justify-content: flex-start;
  // color: $pr;

  height: 50px;
  margin-bottom: 10px;
  padding: 10px;
}

.mint-cell-title {
  width: 105px;
}

.c_radio  .mint-cell-title {
  -webkit-box-flex: 0;
  -ms-flex: none;
      flex: none;

  width: auto;
}

.c_radio  .mint-cell {
  float: left;

  padding-right: 0;
}

.c_radio  .mint-cell:last-child {
  background-size: 120% 0;
}

.c_radio     .mint-radio-core {
  width: 20px;
  height: 20px;
}

.c_radio  .mint-radio-core:after {
  width: 8px;
  height: 8px;
}

.c_radio .mint-cell-wrapper {
  background-size: 120% 0;
}

.c_radio .mint-radiolist-label,
.c_radio .mint-checklist-label {
  font-weight: normal;

  padding: 0;
}

</style>
