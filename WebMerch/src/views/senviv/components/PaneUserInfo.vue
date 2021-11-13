<template>
  <div id="day_report_detail" v-loading="loading">
    <div class="row-title clearfix">
      <div class="pull-left"> <h5>基本信息</h5>
      </div>
      <div class="pull-right">
        <el-button v-show="!isEditBaseInfo" class="btn_in" type="text" @click="onOpenEditBaseInfo">修改</el-button>
        <el-button v-show="isEditBaseInfo" class="btn_in" type="text" @click="onSaveEditBaseInfo">保存</el-button>
      </div>
    </div>
    <div>

      <el-form ref="formByBaseInfo" style="display:flex" :model="formByBaseInfo" :rules="rulesByBaseInfo" size="small">
        <el-col :span="24">
          <el-row>
            <el-col :span="12">
              <el-form-item label-width="100px" label="档案号" :show-message="isEditBaseInfo" prop="docNum">
                <span v-show="!isEditBaseInfo">{{ formByBaseInfo.docNum }}</span>
                <el-input v-show="isEditBaseInfo" v-model="formByBaseInfo.docNum" clearable style="width:200px" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="6">
              <el-form-item label-width="100px" label="姓名" :show-message="isEditBaseInfo" prop="fullName">
                <span v-show="!isEditBaseInfo">{{ formByBaseInfo.fullName }}</span>
                <el-input v-show="isEditBaseInfo" v-model="formByBaseInfo.fullName" clearable style="width:200px" />
              </el-form-item>
            </el-col>
            <el-col :span="18">
              <el-form-item label-width="100px" label="昵称">
                B
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="6">

              <el-form-item label-width="100px" label="性别" :show-message="isEditBaseInfo" prop="sex">
                <span v-show="!isEditBaseInfo">{{ formByBaseInfo.sex }}</span>

                <el-radio-group v-show="isEditBaseInfo" v-model="formByBaseInfo.sex">
                  <el-radio label="1">男</el-radio>
                  <el-radio label="2">女</el-radio>
                </el-radio-group>

              </el-form-item>

            </el-col>
            <el-col :span="18">

              <el-form-item label-width="100px" label="年龄" :show-message="isEditBaseInfo" prop="age">
                <span v-show="!isEditBaseInfo">{{ formByBaseInfo.age }}岁</span>
                <el-input v-show="isEditBaseInfo" v-model="formByBaseInfo.age" placeholder="" style="width:120px">
                  <template slot="append">岁</template>
                </el-input>
              </el-form-item>

            </el-col>
          </el-row>
          <el-row>
            <el-col :span="6">
              <el-form-item label-width="100px" label="身高" :show-message="isEditBaseInfo" prop="height">
                <span v-show="!isEditBaseInfo">{{ formByBaseInfo.height }}cm</span>
                <el-input v-show="isEditBaseInfo" v-model="formByBaseInfo.height" placeholder="" style="width:120px">
                  <template slot="append">cm</template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="18">
              <el-form-item label-width="100px" label="体重" :show-message="isEditBaseInfo" prop="weight">
                <span v-show="!isEditBaseInfo">{{ formByBaseInfo.weight }}kg</span>
                <el-input v-show="isEditBaseInfo" v-model="formByBaseInfo.weight" placeholder="" style="width:120px">
                  <template slot="append">kg</template>
                </el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="12">

              <el-form-item label-width="100px" label="身份证号码" :show-message="isEditBaseInfo" prop="idNumber">
                <span v-show="!isEditBaseInfo">{{ formByBaseInfo.idNumber }}</span>
                <el-input v-show="isEditBaseInfo" v-model="formByBaseInfo.idNumber" clearable style="width:200px" />
              </el-form-item>

            </el-col>
          </el-row>
          <el-row>
            <el-col :span="12">
              <el-form-item label-width="100px" label="手机号码" :show-message="isEditBaseInfo" prop="phoneNumber">
                <span v-show="!isEditBaseInfo">{{ formByBaseInfo.phoneNumber }}</span>
                <el-input v-show="isEditBaseInfo" v-model="formByBaseInfo.phoneNumber" clearable style="width:200px" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="12">
              <el-form-item label-width="100px" label="联系地址" :show-message="isEditBaseInfo" prop="contactAddress">
                <span v-show="!isEditBaseInfo">{{ formByBaseInfo.contactAddress }}</span>
                <el-input v-show="isEditBaseInfo" v-model="formByBaseInfo.contactAddress" clearable style="width:600px" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-col>
      </el-form>
    </div>

  </div>
</template>

<script>

export default {
  name: 'PaneUserInfo',
  props: {
    reportId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
      formByBaseInfo: {
        docNum: '',
        fullName: '',
        sex: '',
        age: '-',
        height: '-',
        weight: '-',
        idNumber: '',
        contactPhoneNumber: '',
        contactAddress: ''
      },
      isEditBaseInfo: false,
      rulesByBaseInfo: {
        docNum: [{ required: false, min: 1, max: 20, message: '必填,且不能超过20个字符', trigger: 'change' }],
        fullName: [{ required: false, min: 1, max: 20, message: '必填,且不能超过20个字符', trigger: 'change' }]
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  },
  methods: {
    onOpenEditBaseInfo() {
      this.isEditBaseInfo = true
      this.rulesByBaseInfo.docNum[0].required = true
    },
    onSaveEditBaseInfo() {
      // this.isEditBaseInfo = false
      // this.rulesByBaseInfo.docNum[0].required = false

      this.$refs['formByBaseInfo'].validate((valid) => {
        if (valid) {

        }
      })
    }
  }
}
</script>

<style lang="scss" scoped>

.btn_in{
    padding: 0px;
}
</style>
