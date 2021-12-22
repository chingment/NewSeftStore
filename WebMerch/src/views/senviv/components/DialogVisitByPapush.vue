<template>
  <el-dialog v-if="visible" title="公众告知" :visible.sync="visible" width="400" custom-class="dialog_visit_by_telephone" append-to-body :before-close="onBeforeClose">
    <div v-loading="loading">
      <el-row>
        <el-col :span="12">

          <div style="font-size: 18px;color: #303133;line-height:50px ">
            最新记录
          </div>
          <el-form ref="form" :model="form" :rules="rules" label-width="80px">
            <el-form-item label="模板" prop="visitTemplate">
              <el-select v-model="form.visitTemplate" placeholder="请选择">
                <el-option
                  v-for="item in optionsByVisitTemplate"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
            <el-form-item label="异常结果" prop="keyword1">
              <el-input
                v-model="form.keyword1"
                type="textarea"
                :rows="3"
                placeholder="请输入内容"
              />
            </el-form-item>
            <el-form-item label="风险因素" prop="keyword2">
              <el-input
                v-model="form.keyword2"
                type="textarea"
                :rows="3"
                placeholder="请输入内容"
              />
            </el-form-item>
            <el-form-item label="健康建议" prop="keyword3">
              <el-input
                v-model="form.keyword3"
                type="textarea"
                :rows="3"
                placeholder="请输入内容"
              />
            </el-form-item>
            <el-form-item label="备注" prop="remark">
              <el-input
                v-model="form.remark"
                type="textarea"
                :rows="3"
                placeholder="请输入内容"
              />
            </el-form-item>
          </el-form>

        </el-col>
        <el-col :span="12">
          <div style="font-size: 18px;color: #303133;line-height:50px ">
            最近记录
          </div>
          <div class="block">
            <el-timeline>

              <el-timeline-item
                v-for="(record, index) in recordsData"
                :key="index"
                :timestamp="record.visitTime"
                placement="top"
              >

                <el-card class="box-card">
                  <div slot="header" class="clearfix">
                    <span>{{ record.visitType }}</span>
                  </div>
                  <div v-for="item in record.visitContent" :key="item" class="text item" style=" margin-bottom: 18px;font-size: 14px;">
                    {{ item.key +' ' + item.value }}
                  </div>
                  <p>{{ record.operater }} 提交于 {{ record.visitTime }}</p>
                </el-card>

              </el-timeline-item>

            </el-timeline>
          </div>

        </el-col>
      </el-row>
    </div>
    <div slot="footer" class="dialog-footer">
      <el-button size="small" type="primary" @click="onSave">保存</el-button>
      <el-button size="small" @click="onClose">关闭</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { MessageBox } from 'element-ui'
import { saveVisitRecordByPapush, getVisitRecords } from '@/api/senviv'
export default {
  name: 'PaneVisitRecord',
  props: {
    userId: {
      type: String,
      default: ''
    },
    visible: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      loading: false,
      form: {
        visitTemplate: '',
        nextTime: '',
        content: '',
        userId: ''
      },
      optionsByVisitTemplate: [{
        value: '2',
        label: '监测异常提醒'
      }],
      rules: {
        visitTemplate: [{ required: true, message: '请选择模板', trigger: 'change' }],
        keyword1: [{ required: true, message: '必填', trigger: 'change' }],
        keyword2: [{ required: true, message: '必填', trigger: 'change' }],
        keyword3: [{ required: true, message: '必填', trigger: 'change' }]
      },
      recordsKey: 0,
      recordsData: null,
      recordsTotal: 0,
      recordsQuery: {
        page: 1,
        limit: 10,
        userId: undefined
      },
      pickerOptions: {
        shortcuts: [{
          text: '今天',
          onClick(picker) {
            picker.$emit('pick', new Date())
          }
        }, {
          text: '昨天',
          onClick(picker) {
            const date = new Date()
            date.setTime(date.getTime() - 3600 * 1000 * 24)
            picker.$emit('pick', date)
          }
        }, {
          text: '一周前',
          onClick(picker) {
            const date = new Date()
            date.setTime(date.getTime() - 3600 * 1000 * 24 * 7)
            picker.$emit('pick', date)
          }
        }]
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this.form.userId = this.userId
    this.onGetVisitRecords()
  },
  methods: {
    onGetVisitRecords() {
      this.loading = true
      this.recordsQuery.userId = this.userId
      getVisitRecords(this.recordsQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.recordsData = d.items
          this.recordsTotal = d.total
        }
        this.loading = false
      })
    },
    onBeforeClose() {
      this.$emit('update:visible', false)
    },
    onSave() {
      this.$refs['form'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          })
            .then(() => {
              var _from = {
                userId: this.form.userId,
                visitTemplate: this.form.visitTemplate,
                visitContent: {
                  keyword1: this.form.keyword1,
                  keyword2: this.form.keyword2,
                  keyword3: this.form.keyword3,
                  remark: this.form.remark
                }
              }
              saveVisitRecordByPapush(_from).then(res => {
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
        }
      })
    },
    onClose() {
      this.$emit('update:visible', false)
    }
  }
}
</script>
<style lang="scss" scoped>

.dialog_visit_by_telephone{

     height: 400px !important;

}
</style>
