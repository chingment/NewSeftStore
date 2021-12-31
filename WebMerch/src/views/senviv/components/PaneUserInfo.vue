<template>
  <div id="day_report_detail" v-loading="loading">
    <div class="row-title clearfix">
      <div class="pull-left"> <h5>基本信息</h5>
      </div>
      <div class="pull-right">
        <el-button v-show="!isEditBaseInfo" class="btn_in" type="text" @click="onOpenEditBaseInfo">修改</el-button>
        <el-button v-show="isEditBaseInfo" class="btn_in" type="text" @click="onCancleEditBaseInfo">取消</el-button>
        <el-button v-show="isEditBaseInfo" class="btn_in" type="text" @click="onSaveEditBaseInfo">保存</el-button>
      </div>
    </div>
    <div>
      <el-form ref="fromByTemp" style="display:flex" :model="fromByTemp" :rules="rulesByBaseInfo" size="small">
        <el-col :span="24">
          <el-row>
            <el-col :span="12">
              <el-form-item label-width="100px" label="档案号" :show-message="isEditBaseInfo" prop="docNum">
                <span v-show="!isEditBaseInfo">{{ fromByTemp.docNum }}</span>
                <el-input v-show="isEditBaseInfo" v-model="fromByTemp.docNum" clearable style="width:200px" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="6">
              <el-form-item label-width="100px" label="姓名" :show-message="isEditBaseInfo" prop="fullName">
                <span v-show="!isEditBaseInfo">{{ fromByTemp.fullName }}</span>
                <el-input v-show="isEditBaseInfo" v-model="fromByTemp.fullName" clearable style="width:200px" />
              </el-form-item>
            </el-col>
            <el-col :span="18">
              <el-form-item label-width="100px" label="昵称">
                {{ fromByTemp.nickName }}
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="6">

              <el-form-item label-width="100px" label="性别" :show-message="isEditBaseInfo" prop="sex">
                <span v-show="!isEditBaseInfo">{{ fromByTemp.sex.text }}</span>

                <el-radio-group v-show="isEditBaseInfo" v-model="fromByTemp.sex.value" @change="onChangeBySex">
                  <el-radio label="1">男</el-radio>
                  <el-radio label="2">女</el-radio>
                </el-radio-group>

              </el-form-item>

            </el-col>
            <el-col :span="18">

              <el-form-item label-width="100px" label="生日" :show-message="isEditBaseInfo" prop="age">
                <span v-show="!isEditBaseInfo">{{ fromByTemp.birthday }}</span>
                <el-date-picker
                  v-show="isEditBaseInfo"
                  v-model="fromByTemp.birthday"
                  type="date"
                  placeholder="选择日期"
                />
              </el-form-item>

            </el-col>
          </el-row>
          <el-row>
            <el-col :span="6">
              <el-form-item label-width="100px" label="身高" :show-message="isEditBaseInfo" prop="height">
                <span v-show="!isEditBaseInfo">{{ fromByTemp.height }}cm</span>
                <el-input v-show="isEditBaseInfo" v-model="fromByTemp.height" placeholder="" style="width:120px">
                  <template slot="append">cm</template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="18">
              <el-form-item label-width="100px" label="体重" :show-message="isEditBaseInfo" prop="weight">
                <span v-show="!isEditBaseInfo">{{ fromByTemp.weight }}kg</span>
                <el-input v-show="isEditBaseInfo" v-model="fromByTemp.weight" placeholder="" style="width:120px">
                  <template slot="append">kg</template>
                </el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="12">

              <el-form-item label-width="100px" label="身份证号码" :show-message="isEditBaseInfo" prop="idNumber">
                <span v-show="!isEditBaseInfo">{{ fromByTemp.idNumber }}</span>
                <el-input v-show="isEditBaseInfo" v-model="fromByTemp.idNumber" clearable style="width:200px" />
              </el-form-item>

            </el-col>
          </el-row>
          <el-row>
            <el-col :span="12">
              <el-form-item label-width="100px" label="手机号码" :show-message="isEditBaseInfo" prop="phoneNumber">
                <span v-show="!isEditBaseInfo">{{ fromByTemp.phoneNumber }}</span>
                <el-input v-show="isEditBaseInfo" v-model="fromByTemp.phoneNumber" clearable style="width:200px" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="12">
              <el-form-item label-width="100px" label="联系地址" :show-message="isEditBaseInfo" prop="contactAddress">
                <span v-show="!isEditBaseInfo">{{ fromByTemp.contactAddress }}</span>
                <el-input v-show="isEditBaseInfo" v-model="fromByTemp.contactAddress" clearable style="width:600px" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-col>
      </el-form>
    </div>

    <div>
      <div class="row-title clearfix">
        <div class="pull-left"> <h5>扩充信息</h5>
        </div>
        <div class="pull-right" />
      </div>
      <div>
        <el-form ref="fromByTemp" style="display:flex" :model="fromByTemp" :rules="rulesByBaseInfo" size="small">
          <el-col :span="24">
            <el-row>
              <el-col :span="12">
                <el-form-item label-width="100px" label="关怀模式" :show-message="isEditBaseInfo">
                  <span v-show="!isEditBaseInfo">{{ fromByTemp.careMode.text }}</span>

                  <el-radio-group v-show="isEditBaseInfo" v-model="fromByTemp.careMode.value" @change="onChangeByCareMode">
                    <el-radio
                      v-for="item in options_caremode"
                      :key="item.value"
                      :label="item.value"
                      :value="item.label"
                    >{{ item.label }}</el-radio>

                  </el-radio-group>

                </el-form-item>
              </el-col>
            </el-row>
            <el-row v-if="fromByTemp.careMode.value===25&&fromByTemp.pregnancy!=null">
              <el-col :span="12">
                <el-form-item label-width="100px" label="孕期" :show-message="isEditBaseInfo">
                  <span v-show="!isEditBaseInfo">{{ fromByTemp.pregnancy.gesWeek }}周+{{ fromByTemp.pregnancy.gesDay }}</span>

                  <el-input v-show="isEditBaseInfo" v-model="fromByTemp.pregnancy.gesWeek" placeholder="" style="width:120px">
                    <template slot="append">周</template>
                  </el-input>
                  <el-input v-show="isEditBaseInfo" v-model="fromByTemp.pregnancy.gesDay" placeholder="" style="width:120px">
                    <template slot="append">天</template>
                  </el-input>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row v-if="fromByTemp.careMode.value===25&&fromByTemp.pregnancy!=null">
              <el-col :span="12">
                <el-form-item label-width="100px" label="预产期" :show-message="isEditBaseInfo">
                  <span v-show="!isEditBaseInfo">{{ fromByTemp.pregnancy.deliveryTime }}</span>

                  <el-date-picker
                    v-show="isEditBaseInfo"
                    v-model="fromByTemp.pregnancy.deliveryTime"
                    type="date"
                    placeholder="选择日期"
                  />

                </el-form-item>
              </el-col>
            </el-row>
          </el-col>
        </el-form>
      </div>

    </div>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { saveUserDetail } from '@/api/senviv'

export default {
  name: 'PaneUserInfo',
  props: {
    initData: {
      type: Object,
      default: null
    }
  },
  data() {
    return {
      loading: false,
      fromByTemp: {
        userId: '',
        docNum: '',
        fullName: '',
        nickName: '',
        sex: { text: '', value: '' },
        birthday: '-',
        height: '-',
        weight: '-',
        idNumber: '',
        careMode: { text: '', value: '' },
        contactPhoneNumber: '',
        contactAddress: ''
      },
      isEditBaseInfo: false,
      rulesByBaseInfo: {
        docNum: [{ required: false, min: 1, max: 20, message: '必填,且不能超过20个字符', trigger: 'change' }],
        fullName: [{ required: false, min: 1, max: 20, message: '必填,且不能超过20个字符', trigger: 'change' }]
      },
      options_caremode: [],
      options_caremode_man: [{
        value: 1,
        label: '正常模式'
      }],
      options_caremode_wowen: [{
        value: 1,
        label: '正常模式'
      }, {
        value: 24,
        label: '备孕中'
      }, {
        value: 25,
        label: '怀孕中'
      }, {
        value: 26,
        label: '产后'
      }],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  watch: {
    initData: function(val, oldval) {
      this.fromByTemp = val
      this.options_caremode = this.getCareMode(val.sex.value)
      // console.log(this.initData)
    }
  },
  created() {
    // console.log(this.initData)
    this.fromByTemp = this.initData
  },
  methods: {
    onOpenEditBaseInfo() {
      this.isEditBaseInfo = true
      this.rulesByBaseInfo.docNum[0].required = true
    },
    onSaveEditBaseInfo() {
      MessageBox.confirm('确定要保存', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      })
        .then(() => {
          var _from = {
            userId: this.fromByTemp.userId,
            fullName: this.fromByTemp.fullName,
            sex: this.fromByTemp.sex.value,
            birthday: this.fromByTemp.birthday,
            weight: this.fromByTemp.weight,
            height: this.fromByTemp.height,
            careMode: this.fromByTemp.careMode.value
          }
          saveUserDetail(_from).then(res => {
            if (res.result === 1) {
              this.$message({
                message: res.message,
                type: 'success'
              })
              this.onClose()
              this.$emit('aftersave')
            } else {
              this.$message({
                message: res.message,
                type: 'error'
              })
            }
          })
        })
        .catch(() => {})
    },
    onCancleEditBaseInfo() {
      this.isEditBaseInfo = false
    },
    onChangeBySex(value) {
      this.options_caremode = this.getCareMode(value)
    },
    getCareMode(value) {
      value = parseInt(value)
      var mode
      if (value === 1) {
        mode = this.options_caremode_man
      } else {
        mode = this.options_caremode_wowen
      }
      return mode
    },
    onChangeByCareMode(value) {
      value = parseInt(value)

      if (value === 25) {
        if (this.fromByTemp.pregnancy == null) {
          this.fromByTemp.pregnancy = { gesWeek: 0, gesDay: 0, deliveryTime: '' }
        }
      }
    }
  }
}
</script>

<style lang="scss" scoped>

.btn_in{
    padding: 0px;
}
</style>
