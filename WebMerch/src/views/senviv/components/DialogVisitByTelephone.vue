<template>
  <div id="dialog_visit_telephone">
    <el-dialog v-if="visible" title="电话回访" :visible.sync="visible" width="400" append-to-body :before-close="onBeforeClose">

      <el-row>
        <el-col :span="12">
          <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
            <el-form-item label="时间" prop="userName">
              <el-date-picker
                v-model="form.visitTime"
                type="datetime"
                placeholder="选择日期时间"
                align="right"
                :picker-options="pickerOptions"
              />
            </el-form-item>
            <el-form-item label="记录" prop="userName">
              <el-input
                v-model="form.content"
                type="textarea"
                :rows="5"
                placeholder="请输入内容"
              />
            </el-form-item>
            <el-form-item label="下次预约" prop="userName">
              <el-date-picker
                v-model="form.nextTime"
                type="datetime"
                placeholder="选择日期时间"
                align="right"
                :picker-options="pickerOptions"
              />
            </el-form-item>
          </el-form>
        </el-col>
        <el-col :span="12">
          <el-timeline :reverse="reverse">
            <el-timeline-item
              v-for="(activity, index) in activities"
              :key="index"
              :timestamp="activity.timestamp"
            >
              {{ activity.content }}
            </el-timeline-item>
          </el-timeline>
        </el-col>
      </el-row>

      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="onSave">保存</el-button>
        <el-button size="small" @click="onClose">关闭</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>

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
        visitTime: '',
        nextTime: '',
        content: ''
      },
      rules: [],
      reverse: true,
      activities: [{
        content: '活动按期开始',
        timestamp: '2018-04-15'
      }, {
        content: '通过审核',
        timestamp: '2018-04-13'
      }, {
        content: '创建成功',
        timestamp: '2018-04-11'
      }],
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

  },
  methods: {
    onBeforeClose() {
      this.$emit('update:visible', false)
    },
    onSave() {

    },
    onClose() {
      this.$emit('update:visible', false)
    }
  }
}
</script>
