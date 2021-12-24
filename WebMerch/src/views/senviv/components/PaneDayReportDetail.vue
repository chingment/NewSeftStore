<template>
  <el-dialog v-if="visible" title="健康报告（日）" :visible.sync="visible" width="1600px" custom-class="senviv-stage-detail" append-to-body :before-close="onBeforeClose">
    <el-container class="brech-work">
      <el-aside class="brech-work-aside">
        <div id="day_report_detail" v-loading="loading">
          <div class="row-title clearfix">
            <div class="pull-left"> <h5>客户信息</h5>
            </div>
            <div class="pull-right" />
          </div>
          <el-descriptions title="">
            <el-descriptions-item label="姓名">{{ userInfo.signName }}</el-descriptions-item>
            <el-descriptions-item label="性别">{{ userInfo.sex }}</el-descriptions-item>
            <el-descriptions-item label="年龄">{{ userInfo.age }}</el-descriptions-item>
          </el-descriptions>
          <div class="row-title clearfix">
            <div class="pull-left"> <h5>数据标签</h5>
            </div>
            <div class="pull-right" />
          </div>
          <div v-if="rd!=null">
            <el-tag
              v-for="tag in rd.dsTags"
              :key="tag"
              style="margin-right: 10px;margin-bottom: 10px"
            >
              {{ tag }}
            </el-tag>
          </div>
          <div class="row-title clearfix">
            <div class="pull-left"> <h5>监测数据图</h5>
            </div>
            <div class="pull-right" />
          </div>
          <div ref="echart_sm_zx" style="width: 960px;height: 400px;margin:auto" />
          <div class="row-title clearfix">
            <div class="pull-left"> <h5>睡眠结构</h5>
            </div>
            <div class="pull-right" />
          </div>
          <div ref="echart_sm_bi" style="width: 960px;height: 400px;margin:auto" />
          <div class="row-title clearfix">
            <div class="pull-left"> <h5>数据指标</h5>
            </div>
            <div class="pull-right" />
          </div>
          <table v-if="rd!=null&&rd.smSmsc!=null" class="clz" cellspacing="0" cellpadding="0" style="width:100%">
            <thead>
              <tr>
                <th>类别</th>
                <th>指标</th>
                <th>测量值</th>
                <th>判断</th>
                <th>参考范围</th>
              </tr>
            </thead>
            <tbody>
              <tr><td rowspan="7">睡眠</td></tr>
              <tr>
                <td>实际睡眠时长</td>
                <td><span :style="{'color': rd.smSmsc.color}">{{ rd.smSmsc.value }}</span> </td>
                <td><span :style="{'color': rd.smSmsc.color}">{{ rd.smSmsc.sign }}</span></td>
                <td><span>{{ rd.smSmsc.refRange }}</span></td>
              </tr>
              <tr>
                <td>浅度睡眠</td>
                <td><span :style="{'color': rd.smQdsmsc.color}">{{ rd.smQdsmsc.value }}</span> </td>
                <td><span :style="{'color': rd.smQdsmsc.color}">{{ rd.smQdsmsc.sign }}</span></td>
                <td><span>{{ rd.smQdsmsc.refRange }}</span></td>
              </tr>
              <tr>
                <td>深度睡眠</td>
                <td><span :style="{'color': rd.smSdsmsc.color}">{{ rd.smSdsmsc.value }}</span> </td>
                <td><span :style="{'color': rd.smSdsmsc.color}">{{ rd.smSdsmsc.sign }}</span></td>
                <td><span>{{ rd.smSdsmsc.refRange }}</span></td>
              </tr>
              <tr>
                <td>REM睡眠</td>
                <td><span :style="{'color': rd.smRemsmsc.color}">{{ rd.smRemsmsc.value }}</span> </td>
                <td><span :style="{'color': rd.smRemsmsc.color}">{{ rd.smRemsmsc.sign }}</span></td>
                <td><span>{{ rd.smRemsmsc.refRange }}</span></td>
              </tr>
              <tr>
                <td>睡眠周期</td>
                <td><span :style="{'color': rd.smSmzq.color}">{{ rd.smSmzq.value }}</span> </td>
                <td><span :style="{'color': rd.smSmzq.color}">{{ rd.smSmzq.sign }}</span></td>
                <td><span>{{ rd.smSmzq.refRange }}</span></td>
              </tr>
              <tr>
                <td>体动次数</td>
                <td><span :style="{'color': rd.smTdcs.color}">{{ rd.smTdcs.value }}</span> </td>
                <td><span :style="{'color': rd.smTdcs.color}">{{ rd.smTdcs.sign }}</span></td>
                <td><span>{{ rd.smTdcs.refRange }}</span></td>
              </tr>

              <tr><td rowspan="4">心率</td></tr>
              <tr>
                <td>当次基准心率</td>
                <td><span :style="{'color': rd.xlDcjzxl.color}">{{ rd.xlDcjzxl.value }}</span> </td>
                <td><span :style="{'color': rd.xlDcjzxl.color}">{{ rd.xlDcjzxl.sign }}</span></td>
                <td><span>{{ rd.xlDcjzxl.refRange }}</span></td>
              </tr>
              <tr>
                <td>长期基准心率</td>
                <td><span :style="{'color': rd.xlCqjzxl.color}">{{ rd.xlCqjzxl.value }}</span> </td>
                <td><span :style="{'color': rd.xlCqjzxl.color}">{{ rd.xlCqjzxl.sign }}</span></td>
                <td><span>{{ rd.xlCqjzxl.refRange }}</span></td>
              </tr>
              <tr>
                <td>当次平均心率</td>
                <td><span :style="{'color': rd.xlDcpjxl.color}">{{ rd.xlDcpjxl.value }}</span> </td>
                <td><span :style="{'color': rd.xlDcpjxl.color}">{{ rd.xlDcpjxl.sign }}</span></td>
                <td><span>{{ rd.xlDcpjxl.refRange }}</span></td>
              </tr>

              <tr><td rowspan="6">呼吸</td></tr>
              <tr>
                <td>当次基准呼吸</td>
                <td><span :style="{'color': rd.hxDcjzhx.color}">{{ rd.hxDcjzhx.value }}</span> </td>
                <td><span :style="{'color': rd.hxDcjzhx.color}">{{ rd.hxDcjzhx.sign }}</span></td>
                <td><span>{{ rd.hxDcjzhx.refRange }}</span></td>
              </tr>
              <tr>
                <td>长期基准呼吸</td>
                <td><span :style="{'color': rd.hxCqjzhx.color}">{{ rd.hxCqjzhx.value }}</span> </td>
                <td><span :style="{'color': rd.hxCqjzhx.color}">{{ rd.hxCqjzhx.sign }}</span></td>
                <td><span>{{ rd.hxCqjzhx.refRange }}</span></td>
              </tr>
              <tr>
                <td>平均呼吸</td>
                <td><span :style="{'color': rd.hxDcpjhx.color}">{{ rd.hxDcpjhx.value }}</span> </td>
                <td><span :style="{'color': rd.hxDcpjhx.color}">{{ rd.hxDcpjhx.sign }}</span></td>
                <td><span>{{ rd.hxDcpjhx.refRange }}</span></td>
              </tr>
              <tr>
                <td>呼吸暂停次数</td>
                <td><span :style="{'color': rd.hxZtcs.color}">{{ rd.hxZtcs.value }}</span> </td>
                <td><span :style="{'color': rd.hxZtcs.color}">{{ rd.hxZtcs.sign }}</span></td>
                <td><span>{{ rd.hxZtcs.refRange }}</span></td>
              </tr>
              <tr>
                <td>AHI指数</td>
                <td><span :style="{'color': rd.hxZtahizs.color}">{{ rd.hxZtahizs.value }}</span> </td>
                <td><span :style="{'color': rd.hxZtahizs.color}">{{ rd.hxZtahizs.sign }}</span></td>
                <td><span>{{ rd.hxZtahizs.refRange }}</span></td>
              </tr>

              <tr><td rowspan="9">HRV</td></tr>
              <tr>
                <td>心率失常风险</td>
                <td><span :style="{'color': rd.jbfxXlscfx.color}">{{ rd.jbfxXlscfx.value }}</span> </td>
                <td><span :style="{'color': rd.jbfxXlscfx.color}">{{ rd.jbfxXlscfx.sign }}</span></td>
                <td><span>{{ rd.jbfxXlscfx.refRange }}</span></td>
              </tr>
              <tr>
                <td>心率减力</td>
                <td><span :style="{'color': rd.jbfxXljsl.color}">{{ rd.jbfxXljsl.value }}</span> </td>
                <td><span :style="{'color': rd.jbfxXljsl.color}">{{ rd.jbfxXljsl.sign }}</span></td>
                <td><span>{{ rd.jbfxXljsl.refRange }}</span></td>
              </tr>
              <tr>
                <td>心脏总能量</td>
                <td><span :style="{'color': rd.hrvXzznl.color}">{{ rd.hrvXzznl.value }}</span> </td>
                <td><span :style="{'color': rd.hrvXzznl.color}">{{ rd.hrvXzznl.sign }}</span></td>
                <td><span>{{ rd.hrvXzznl.refRange }}</span></td>
              </tr>
              <tr>
                <td>交感神经张力指数</td>
                <td><span :style="{'color': rd.hrvJgsjzlzs.color}">{{ rd.hrvJgsjzlzs.value }}</span> </td>
                <td><span :style="{'color': rd.hrvJgsjzlzs.color}">{{ rd.hrvJgsjzlzs.sign }}</span></td>
                <td><span>{{ rd.hrvJgsjzlzs.refRange }}</span></td>
              </tr>
              <tr>
                <td>迷走神经张力指数</td>
                <td><span :style="{'color': rd.hrvMzsjzlzs.color}">{{ rd.hrvMzsjzlzs.value }}</span> </td>
                <td><span :style="{'color': rd.hrvMzsjzlzs.color}">{{ rd.hrvMzsjzlzs.sign }}</span></td>
                <td><span>{{ rd.hrvMzsjzlzs.refRange }}</span></td>
              </tr>
              <tr>
                <td>自主神经平衡指数</td>
                <td><span :style="{'color': rd.hrvZzsjzlzs.color}">{{ rd.hrvZzsjzlzs.value }}</span> </td>
                <td><span :style="{'color': rd.hrvZzsjzlzs.color}">{{ rd.hrvZzsjzlzs.sign }}</span></td>
                <td><span>{{ rd.hrvZzsjzlzs.refRange }}</span></td>
              </tr>
              <tr>
                <td>荷尔蒙指数</td>
                <td><span :style="{'color': rd.hrvHermzs.color}">{{ rd.hrvHermzs.value }}</span> </td>
                <td><span :style="{'color': rd.hrvHermzs.color}">{{ rd.hrvHermzs.sign }}</span></td>
                <td><span>{{ rd.hrvHermzs.refRange }}</span></td>
              </tr>
              <tr>
                <td>体温及血管舒缩指数</td>
                <td><span :style="{'color': rd.hrvTwjxgsszs.color}">{{ rd.hrvTwjxgsszs.value }}</span> </td>
                <td><span :style="{'color': rd.hrvTwjxgsszs.color}">{{ rd.hrvTwjxgsszs.sign }}</span></td>
                <td><span>{{ rd.hrvTwjxgsszs.refRange }}</span></td>
              </tr>
            </tbody>
          </table>
        </div>
      </el-aside>
      <el-container class="brech-work-container">
        <el-tabs class="brech-work-tabs" @tab-click="onBrechWorkTabs">
          <el-tab-pane v-if="brechWorkTabs.isShowByHealthSug" label="健康评价">
            <div v-loading="loadingBySug" class="drawer">
              <div class="drawer_content">
                <div style="margin-bottom:10px">
                  <el-tag v-if="formBySug.isSend" type="success">
                    已发送
                  </el-tag>
                  <el-tag v-if="!formBySug.isSend" type="warning">
                    未发送
                  </el-tag>
                </div>
                <el-card class="box-card" style="margin-bottom:10px">
                  <div slot="header" class="clearfix">
                    <span>健康总结</span>
                  </div>
                  <div>
                    <el-input v-if="!formBySug.isSend" v-model="formBySug.rptSummary" rows="5" type="textarea" show-word-limit />
                    <pre v-if="formBySug.isSend" style="white-space: pre-line;line-height: 23px;">{{ formBySug.rptSummary }}</pre>
                  </div>
                </el-card>
                <el-card class="box-card" style="margin-bottom:10px">
                  <div slot="header" class="clearfix">
                    <span>健康建议</span>
                  </div>
                  <div>
                    <el-input v-if="!formBySug.isSend" v-model="formBySug.rptSuggest" rows="5" type="textarea" show-word-limit />
                    <pre v-if="formBySug.isSend" style="white-space: pre-line;line-height: 23px;">{{ formBySug.rptSuggest }}</pre>
                  </div>
                </el-card>
              </div>
              <div class="drawer_footer">
                <el-button v-if="!formBySug.isSend" size="small" type="primary" @click="onSaveReportSug(false)">暂 存</el-button>
                <el-button v-if="!formBySug.isSend" size="small" type="success" @click="onSaveReportSug(true)">保存并发送</el-button>
              </div>
            </div>
          </el-tab-pane>
          <el-tab-pane v-if="brechWorkTabs.isShowByVisitTelephone" label="电话回访">
            <div class="drawer">
              <div class="drawer_content">
                <el-form ref="formByVisitTelephone" :model="formByVisitTelephone" :rules="rulesByVisitTelephone" label-width="80px">
                  <el-form-item label="回访时间" prop="visitTime">
                    <el-date-picker
                      v-model="formByVisitTelephone.visitTime"
                      type="datetime"
                      placeholder="选择日期时间"
                      align="right"
                      :picker-options="pickerOptions"
                    />
                  </el-form-item>
                  <el-form-item label="回访记录" prop="remark">
                    <el-input
                      v-model="formByVisitTelephone.remark"
                      type="textarea"
                      :rows="5"
                      placeholder="请输入内容"
                    />
                  </el-form-item>
                  <el-form-item label="下次预约" prop="nextTime">
                    <el-date-picker
                      v-model="formByVisitTelephone.nextTime"
                      type="datetime"
                      placeholder="选择日期时间"
                      align="right"
                      :picker-options="pickerOptions"
                    />
                  </el-form-item>
                </el-form>
              </div>
              <div class="drawer_footer">
                <el-button size="small" type="primary" @click="onSaveVisitTelephone(false)">保 存</el-button>
              </div>
            </div>
          </el-tab-pane>
          <el-tab-pane v-if="brechWorkTabs.isShowByVisitWaPush" label="微信告知">
            <div class="drawer">
              <div class="drawer_content">
                <el-form ref="formByVisitPapush" :model="formByVisitPapush" :rules="rulesByVisitPapush" label-width="80px">
                  <el-form-item label="模板" prop="visitTemplate">
                    <el-select v-model="formByVisitPapush.visitTemplate" placeholder="请选择">
                      <el-option
                        v-for="item in visitTemplateOptions"
                        :key="item.value"
                        :label="item.label"
                        :value="item.value"
                      />
                    </el-select>
                  </el-form-item>
                  <el-form-item label="异常结果" prop="keyword1">
                    <el-input
                      v-model="formByVisitPapush.keyword1"
                      type="textarea"
                      :rows="3"
                      placeholder="请输入内容"
                    />
                  </el-form-item>
                  <el-form-item label="风险因素" prop="keyword2">
                    <el-input
                      v-model="formByVisitPapush.keyword2"
                      type="textarea"
                      :rows="3"
                      placeholder="请输入内容"
                    />
                  </el-form-item>
                  <el-form-item label="健康建议" prop="keyword3">
                    <el-input
                      v-model="formByVisitPapush.keyword3"
                      type="textarea"
                      :rows="3"
                      placeholder="请输入内容"
                    />
                  </el-form-item>
                  <el-form-item label="备注" prop="remark">
                    <el-input
                      v-model="formByVisitPapush.remark"
                      type="textarea"
                      :rows="3"
                      placeholder="请输入内容"
                    />
                  </el-form-item>
                </el-form>
              </div>
              <div class="drawer_footer">
                <el-button size="small" type="primary" @click="onSaveVisitWapush(false)">保 存</el-button>
              </div>
            </div>
          </el-tab-pane>
          <el-tab-pane v-if="brechWorkTabs.isShowHandleRecord" label="历史记录">
            <div class="drawer">
              <div v-loading="loadingByHandleRecord" class="drawer_content">
                <el-timeline style="padding:0px">

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
                      <div v-for="(item, index2) in record.visitContent" :key="index2" class="text item" style=" margin-bottom: 18px;font-size: 14px;">
                        {{ item.key +' ' + item.value }}
                      </div>
                      <p>{{ record.operater }} 提交于 {{ record.visitTime }}</p>
                    </el-card>
                  </el-timeline-item>

                </el-timeline>
              </div>
            </div>
          </el-tab-pane>
        </el-tabs>
      </el-container>
    </el-container>
  </el-dialog>
</template>
<script>

import { MessageBox } from 'element-ui'
import echarts from 'echarts'
import { getDayReportDetail, getDayReportSug, saveDayReportSug, saveVisitRecordByTelePhone, saveVisitRecordByPapush, getHandleRecords } from '@/api/senviv'

var myChart1
var myChart2

export default {
  name: 'PaneDayReportDetail',
  props: {
    reportId: {
      type: String,
      default: ''
    },
    taskId: {
      type: String,
      default: ''
    },
    workType: {
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
      dialog: {
        width: '1000px',
        title: ''
      },
      loading: false,
      loadingBySug: false,
      loadingByHandleRecord: false,
      userInfo: {
        avatar: '',
        signName: '',
        sex: '',
        age: '',
        height: '',
        weight: '',
        signTags: []
      },
      rd: {
        hxPoint: [],
        xlPoint: [],
        smPoint: []
      },
      brechWorkTabs: {
        isShowByHealthSug: false,
        isShowByVisitTelephone: true,
        isShowByVisitWaPush: true,
        isShowHandleRecord: true
      },
      formBySug: {
        reportId: '',
        rptSummary: '',
        rptSuggest: '',
        isSend: false,
        sugSkus: []
      },
      formByVisitTelephone: {
        visitTime: '',
        nextTime: '',
        remark: ''
      },
      rulesByVisitTelephone: {
        visitTime: [{ required: true, message: '必选', trigger: 'change' }],
        remark: [{ required: true, message: '必填', trigger: 'change' }]
      },
      formByVisitPapush: {
        visitTemplate: '',
        nextTime: '',
        content: '',
        userId: ''
      },
      rulesByVisitPapush: {
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
        userId: undefined,
        reportId: undefined
      },
      visitTemplateOptions: [{
        value: '2',
        label: '监测异常提醒'
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
  mounted() {
    window.addEventListener('beforeunload', this.clearChart)
  },
  created() {
    if (this.workType === 'task_saw') {
      this.brechWorkTabs.isShowByHealthSug = false
      this.brechWorkTabs.isShowByVisitTelephone = false
      this.brechWorkTabs.isShowByVisitWaPush = false
      this.brechWorkTabs.isShowHandleRecord = true
    } else if (this.workType === 'task_handle') {
      this.brechWorkTabs.isShowByHealthSug = false
      this.brechWorkTabs.isShowByVisitTelephone = true
      this.brechWorkTabs.isShowByVisitWaPush = true
      this.brechWorkTabs.isShowHandleRecord = true
    } else if (this.workType === 'day_saw') {
      this.brechWorkTabs.isShowByHealthSug = false
      this.brechWorkTabs.isShowByVisitTelephone = false
      this.brechWorkTabs.isShowByVisitWaPush = false
      this.brechWorkTabs.isShowHandleRecord = true
    }

    this.onGetReportDetail()
    this.onGetHandleRecords()
  },
  beforeDestroy() {
    if (myChart1) {
      myChart1.clear()
      myChart1.dispose()
      myChart1 = null
    }
    if (myChart2) {
      myChart2.clear()
      myChart2.dispose()
      myChart2 = null
    }

    // 清空 resize 事件处理函数
    window.removeEventListener('resize', this.resize)
    // 清空 beforeunload 事件处理函数
    window.removeEventListener('beforeunload', this.clearChart)
  },
  methods: {
    clearChart() {
      this.$destroy()
    },
    onGetReportDetail() {
      this.loading = true
      getDayReportDetail({ reportId: this.reportId, taskId: this.taskId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.userInfo = d.userInfo
          this.rd = d.reportData

          this.$nextTick(function() {
            this.onGetPieBySmjg()
            this.onGetChartByJcsj()
          }, 2000)
        }

        this.loading = false
      })
    },
    onGetPieBySmjg() {
      // 绘制图表
      if (!myChart1) {
        myChart1 = echarts.init(this.$refs.echart_sm_bi, null, { renderer: 'svg' })
      }
      // 指定图表的配置项和数据
      var option = {
        // 标题
        title: {
          text: '',
          x: 'left' // 标题位置
          // textStyle: { //标题内容的样式
          //   color: '#000',
          //   fontStyle: 'normal',
          //   fontWeight: 100,
          //   fontSize: 16 //主题文字字体大小，默认为18px
          // },
        },
        // stillShowZeroSum: true,
        // 鼠标划过时饼状图上显示的数据
        tooltip: {
          trigger: 'item',
          formatter: '{a}<br/>{b}:{c} ({d}%)'
        },
        // 图例
        legend: {// 图例  标注各种颜色代表的模块
          // orient: 'vertical',//图例的显示方式  默认横向显示
          bottom: 10, // 控制图例出现的距离  默认左上角
          left: 'center', // 控制图例的位置
          // itemWidth: 16,//图例颜色块的宽度和高度
          // itemHeight: 12,
          textStyle: {// 图例中文字的样式
            color: '#000',
            fontSize: 16
          }
        },
        // 饼图中各模块的颜色
        color: ['#32dadd', '#b6a2de', '#5ab1ef', '#59f5ae', '#1d0fdc'],
        // 饼图数据
        series: {
          // name: 'bug分布',
          type: 'pie', // echarts图的类型   pie代表饼图
          radius: '70%', // 饼图中饼状部分的大小所占整个父元素的百分比
          center: ['50%', '50%'], // 整个饼图在整个父元素中的位置
          // data:''               //饼图数据
          data: this.rd.smPie.data,
          itemStyle: {
            normal: {
              label: {
                show: true // 饼图上是否出现标注文字 标注各模块代表什么  默认是true
                // position: 'inner',//控制饼图上标注文字相对于饼图的位置  默认位置在饼图外
              },
              labelLine: {
                show: true// 官网demo里外部标注上的小细线的显示隐藏    默认显示
              }
            }
          }
        }

      }
      // 使用刚指定的配置项和数据显示图表。
      myChart1.setOption(option)
    },
    onGetChartByJcsj() {
      if (!myChart2) {
        myChart2 = echarts.init(this.$refs.echart_sm_zx, null, { renderer: 'svg' })
      }
      // var hx_point_d= [20, 18, 16, 19, 16, 19, 19, 18, 18, 16, 17, 14, 15, 15, 15, 15, 16, 9, 12, 16, 15, 18, 18, 19, 10, 18, 11, 17, 13, 18, 18, 18, 19, 19, 20, 19, 21, 19, 19, 19, 19, 17, 19, 18, 19, 19, 17, 18, 17, 14, 16, 9, 17, 17, 18, 17, 17, 17, 17, 17, 17, 18, 19, 18, 16, 18, 17, 18, 18, 18, 18, 17, 18, 18, 17, 16, 17, 18, 17, 17, 18, 18, 17, 18, 17, 18, 17, 18, 18, 18, 17, 18, 17, 17, 18, 17, 17, 17, 18, 17, 17, 20, 21, 18, 20, 20, 17, 17, 18, 17, 18, 13, 10, 14, 18, 18, 18, 18, 17, 18, 17, 17, 17, 9, 17, 15, 16, 18, 17, 16, 17, 17, 17, 17, 17, 15, 17, 10, 17, 16, 16, 17, 17, 16, 17, 16, 17, 17, 17, 18, 16, 16, 17, 16, 16, 17, 17, 17, 16, 16, 17, 16, 17, 17, 17, 19, 17, 17, 17, 17, 20, 15, 16, 17, 18, 17, 18, 21, 18, 18, 16, 17, 18, 16, 17, 17, 17, 17, 16, 16, 16, 16, 16, 8, 10, 17, 17, 15, 15, 16, 16, 15, 15, 16, 15, 17, 15, 16, 16, 16, 16, 15, 16, 16, 15, 16, 18, 17, 16, 17, 17, 15, 16, 15, 15, 17, 16, 14, 15, 16, 16, 17, 16, 16, 15, 16, 15, 16, 16, 16, 17, 17, 16, 16, 16, 16, 15, 16, 16, 15, 15, 16, 16, 15, 16, 11, 16, 10, 17, 16, 16, 15, 16, 12, 13, 16, 17, 16, 14, 17, 18, 16, 16, 16, 16, 16, 15, 12, 15, 16, 16, 16, 16, 16, 13, 11, 17, 16, 16, 15, 15, 14, 15, 15, 16, 16, 15, 15, 15, 16, 16, 10, 16, 14, 15, 11, 16, 17, 16, 17, 17, 17, 18, 18, 17, 16, 16, 17, 17, 17, 16, 17, 17, 17, 17, 18, 17, 18, 18, 17, 16, 17, 16, 18, 19, 19, 18, 18, 18, 19, 20, 19, 19, 21, 16, 17, 15, 15, 15, 15, 15, 15, 15, 15, 16, 16, 16, 15, 15, 15, 16, 16, 16, 16, 16, 15, 16, 16, 15, 15, 15, 16, 16, 16, 15, 15, 16, 15, 15, 15, 15, 15, 17, 17, 16, 16, 16, 16, 16, 16, 16, 17, 19, 18, 18, 18, 16, 10, 14, 16, 15, 14, 15, 15, 15, 16, 10, 12, 11, 12, 14]

      if (this.rd.hxPoint === null) { return }

      var hx_point_d = this.rd.hxPoint.dataValue

      if (hx_point_d === null) { return }

      var xl_point_d = this.rd.xlPoint.dataValue
      // var xl_point_d = [76, 76, 76, 74, 71, 68, 60, 58, 59, 59, 59, 59, 60, 60, 60, 60, 60, 60, 60, 61, 61, 60, 60, 61, 61, 61, 62, 62, 62, 60, 60, 60, 61, 63, 63, 64, 64, 64, 64, 64, 64, 62, 62, 63, 63, 63, 62, 63, 63, 63, 63, 62, 61, 62, 63, 63, 61, 62, 61, 62, 61, 60, 59, 58, 57, 59, 57, 58, 57, 57, 57, 57, 57, 58, 58, 58, 58, 58, 58, 59, 59, 59, 58, 58, 58, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 56, 56, 57, 56, 57, 59, 63, 60, 59, 61, 62, 61, 55, 59, 58, 60, 61, 62, 62, 62, 60, 59, 58, 58, 58, 57, 57, 56, 55, 55, 55, 57, 58, 58, 58, 57, 57, 57, 56, 56, 55, 54, 54, 53, 53, 53, 53, 53, 53, 53, 53, 52, 54, 55, 55, 55, 55, 54, 54, 59, 58, 57, 55, 53, 54, 52, 51, 53, 53, 52, 52, 52, 52, 51, 55, 57, 58, 53, 50, 51, 51, 56, 54, 53, 52, 51, 51, 53, 51, 52, 52, 52, 51, 52, 51, 52, 52, 55, 57, 60, 59, 59, 56, 55, 55, 54, 53, 53, 53, 54, 54, 54, 54, 52, 54, 53, 53, 53, 53, 52, 53, 55, 54, 54, 53, 52, 52, 52, 52, 52, 51, 53, 52, 52, 52, 52, 52, 52, 52, 52, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 52, 52, 53, 53, 52, 51, 50, 51, 52, 55, 54, 50, 51, 52, 52, 52, 56, 52, 50, 51, 50, 51, 52, 53, 52, 54, 55, 56, 55, 55, 57, 57, 56, 55, 55, 55, 56, 55, 55, 55, 55, 54, 54, 54, 54, 54, 54, 53, 51, 52, 52, 52, 55, 56, 56, 58, 59, 61, 58, 58, 56, 56, 55, 56, 55, 55, 54, 55, 53, 54, 52, 56, 56, 54, 55, 57, 56, 56, 55, 55, 56, 57, 55, 56, 56, 52, 54, 55, 54, 57, 55, 56, 56, 56, 56, 56, 55, 56, 55, 55, 55, 55, 55, 55, 54, 55, 54, 54, 55, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 55, 56, 56, 56, 56, 56, 55, 55, 53, 54, 54, 57, 56, 53, 56, 58, 61, 54, 54, 53, 53, 53, 53, 55, 53, 53, 53, 53, 52]

      // "XDataTime": [1615914256, 1615914316, 1615914376, 1615914436, 1615914496, 1615914556, 1615914616, 1615914676, 1615914736, 1615914796, 1615914856, 1615914916, 1615914976, 1615915036, 1615915096, 1615915156, 1615915216, 1615915276, 1615915336, 1615915396, 1615915456, 1615915516, 1615915576, 1615915636, 1615915696, 1615915756, 1615915816, 1615915876, 1615915936, 1615915996, 1615916056, 1615916116, 1615916176, 1615916236, 1615916296, 1615916356, 1615916416, 1615916476, 1615916536, 1615916596, 1615916656, 1615916716, 1615916776, 1615916836, 1615916896, 1615916956, 1615917016, 1615917076, 1615917136, 1615917196, 1615917256, 1615917316, 1615917376, 1615917436, 1615917496, 1615917556, 1615917616, 1615917676, 1615917736, 1615917796, 1615917856, 1615917916, 1615917976, 1615918036, 1615918096, 1615918156, 1615918216, 1615918276, 1615918336, 1615918396, 1615918456, 1615918516, 1615918576, 1615918636, 1615918696, 1615918756, 1615918816, 1615918876, 1615918936, 1615918996, 1615919056, 1615919116, 1615919176, 1615919236, 1615919296, 1615919356, 1615919416, 1615919476, 1615919536, 1615919596, 1615919656, 1615919716, 1615919776, 1615919836, 1615919896, 1615919956, 1615920016, 1615920076, 1615920136, 1615920196, 1615920256, 1615920316, 1615920376, 1615920436, 1615920496, 1615920556, 1615920616, 1615920676, 1615920736, 1615920796, 1615920856, 1615920916, 1615920976, 1615921036, 1615921096, 1615921156, 1615921216, 1615921276, 1615921336, 1615921396, 1615921456, 1615921516, 1615921576, 1615921636, 1615921696, 1615921756, 1615921816, 1615921876, 1615921936, 1615921996, 1615922056, 1615922116, 1615922176, 1615922236, 1615922296, 1615922356, 1615922416, 1615922476, 1615922536, 1615922596, 1615922656, 1615922716, 1615922776, 1615922836, 1615922896, 1615922956, 1615923016, 1615923076, 1615923136, 1615923196, 1615923256, 1615923316, 1615923376, 1615923436, 1615923496, 1615923556, 1615923616, 1615923676, 1615923736, 1615923796, 1615923856, 1615923916, 1615923976, 1615924036, 1615924096, 1615924156, 1615924216, 1615924276, 1615924336, 1615924396, 1615924456, 1615924516, 1615924576, 1615924636, 1615924696, 1615924756, 1615924816, 1615924876, 1615924936, 1615924996, 1615925056, 1615925116, 1615925176, 1615925236, 1615925296, 1615925356, 1615925416, 1615925476, 1615925536, 1615925596, 1615925656, 1615925716, 1615925776, 1615925836, 1615925896, 1615925956, 1615926016, 1615926076, 1615926136, 1615926196, 1615926256, 1615926316, 1615926376, 1615926436, 1615926496, 1615926556, 1615926616, 1615926676, 1615926736, 1615926796, 1615926856, 1615926916, 1615926976, 1615927036, 1615927096, 1615927156, 1615927216, 1615927276, 1615927336, 1615927396, 1615927456, 1615927516, 1615927576, 1615927636, 1615927696, 1615927756, 1615927816, 1615927876, 1615927936, 1615927996, 1615928056, 1615928116, 1615928176, 1615928236, 1615928296, 1615928356, 1615928416, 1615928476, 1615928536, 1615928596, 1615928656, 1615928716, 1615928776, 1615928836, 1615928896, 1615928956, 1615929016, 1615929076, 1615929136, 1615929196, 1615929256, 1615929316, 1615929376, 1615929436, 1615929496, 1615929556, 1615929616, 1615929676, 1615929736, 1615929796, 1615929856, 1615929916, 1615929976, 1615930036, 1615930096, 1615930156, 1615930216, 1615930276, 1615930336, 1615930396, 1615930456, 1615930516, 1615930576, 1615930636, 1615930696, 1615930756, 1615930816, 1615930876, 1615930936, 1615930996, 1615931056, 1615931116, 1615931176, 1615931236, 1615931296, 1615931356, 1615931416, 1615931476, 1615931536, 1615931596, 1615931656, 1615931716, 1615931776, 1615931836, 1615931896, 1615931956, 1615932016, 1615932076, 1615932136, 1615932196, 1615932256, 1615932316, 1615932376, 1615932436, 1615932496, 1615932556, 1615932616, 1615932676, 1615932736, 1615932796, 1615932856, 1615932916, 1615932976, 1615933036, 1615933096, 1615933156, 1615933216, 1615933276, 1615933336, 1615933396, 1615933456, 1615933516, 1615933576, 1615933636, 1615933696, 1615933756, 1615933816, 1615933876, 1615933936, 1615933996, 1615934056, 1615934116, 1615934176, 1615934236, 1615934296, 1615934356, 1615934416, 1615934476, 1615934536, 1615934596, 1615934656, 1615934716, 1615934776, 1615934836, 1615934896, 1615934956, 1615935016, 1615935076, 1615935136, 1615935196, 1615935256, 1615935316, 1615935376, 1615935436, 1615935496, 1615935556, 1615935616, 1615935676, 1615935736, 1615935796, 1615935856, 1615935916, 1615935976, 1615936036, 1615936096, 1615936156, 1615936216, 1615936276, 1615936336, 1615936396, 1615936456, 1615936516, 1615936576, 1615936636, 1615936696, 1615936756, 1615936816, 1615936876, 1615936936, 1615936996, 1615937056, 1615937116, 1615937176, 1615937236, 1615937296, 1615937356, 1615937416, 1615937476, 1615937536, 1615937596, 1615937656, 1615937716, 1615937776, 1615937836, 1615937896, 1615937956, 1615938016, 1615938076, 1615938136, 1615938196, 1615938256, 1615938316, 1615938376, 1615938436, 1615938496, 1615938556, 1615938616, 1615938676, 1615938736, 1615938796, 1615938856, 1615938916, 1615938976, 1615939036, 1615939096, 1615939156, 1615939216, 1615939276, 1615939336, 1615939396, 1615939456, 1615939516, 1615939576, 1615939636, 1615939696, 1615939756, 1615939816, 1615939876, 1615939936, 1615939996, 1615940056, 1615940116, 1615940176, 1615940236, 1615940296, 1615940356, 1615940416, 1615940476, 1615940536, 1615940596, 1615940656, 1615940716, 1615940776, 1615940836, 1615940896, 1615940956]

      var sm_point_d = this.rd.smPoint.dataValue
      var sm_point_x_min_t = this.datetimeFormat2(this.rd.smPoint.startTime, -1000 * 60)
      var sm_point_x_max_t = this.datetimeFormat2(this.rd.smPoint.endTime, +1000 * 60)
      var sc_time = this.rd.smScsj
      var rs_time = this.rd.smRssj
      var qx_time = this.rd.smQxsj
      var lc_time = this.rd.smLcsj

      var sm_point_xAxis = []
      var sm_point_xAxis_data = []
      for (let index = 0; index < sm_point_d.length; index++) {
        const var1 = sm_point_d[index].starttime
        const var2 = sm_point_d[index].endtime
        var type = sm_point_d[index].type

        var var3
        var color = ''

        if (type === 0) {
          var3 = 'W'
          color = '#d8edff'
        } else if (type === 1) {
          var3 = 'N1'
          color = '#a9ddfd'
        } else if (type === 5) {
          var3 = 'N3'
          color = '#4c6ce5'
        } else if (type === 4) {
          var3 = 'R'
          color = '#e6affd'
        } else if (type === 3) {
          var3 = 'O'
          color = '#ffdf24'
        } else if (type === 6) {
          var3 = 'N2'
          color = '#a9ddfd'
        }

        sm_point_xAxis.push(this.datetimeFormat(var1))
        sm_point_xAxis.push(this.datetimeFormat(var2))

        sm_point_xAxis_data.push({ itemStyle: { normal: { color: color }}, value: [var3, this.datetimeFormat(var1), this.datetimeFormat(var2)] })
      }

      var option = {
        grid: [{
          bottom: '50%',
          x: 50,
          y: 50,
          x2: 50,
          y2: 50
        }, {
          top: '50%',
          x: 50,
          y: 50,
          x2: 50,
          y2: 50

        }, {
          show: false,
          borderWidth: 1,
          borderColor: '#FF0000',
          x: 50,
          y: 50,
          x2: 50,
          y2: 50
        }],
        title: [{
          left: 'center',
          text: ''
        }],
        legend: {
          data: ['呼吸', '心率', 'N3', 'N2', 'N1', 'R', 'W', 'O']
        },
        tooltip: {
          trigger: 'axis',
          formatter: function(params) {
            if (params[0].componentSubType === 'line') {
              return ' 心率：' + params[1].data + '<br/>' + ' 呼吸：' + params[0].data
            } else if (params[0].componentSubType === 'custom') {
              var html = ''
              for (let index = 0; index < params.length; index++) {
                html += params[index].value[1] + '-' + params[index].value[2] + ',' + params[index].value[0] + '<BR/>'
              }
              // var param = params[params.length - 1]
              // console.log(JSON.stringify(param))
              return html
            }
          }
        },
        xAxis: [{
          data: hx_point_d,
          show: false
        }, {
          gridIndex: 1,
          axisTick: {
            show: false,
            height: 0
          },
          type: 'time',
          rotate: 0,
          splitLine: { show: false },
          interval: 60 * 1000,
          min: sm_point_x_min_t,
          max: sm_point_x_max_t,
          axisLabel: {
            textStyle: {
              align: 'center'
              // verticalAlign: 'bottom'
            },
            interval: 0,
            padding: [-8, 0, 0, 0],
            formatter: function(value) {
              function datetimeFormat(longTypeDate) {
                const date = new Date(longTypeDate)
                const y = date.getFullYear()
                let MM = date.getMonth() + 1
                MM = MM < 10 ? ('0' + MM) : MM
                let d = date.getDate()
                d = d < 10 ? ('0' + d) : d
                let h = date.getHours()
                h = h < 10 ? ('0' + h) : h
                let m = date.getMinutes()
                m = m < 10 ? ('0' + m) : m
                let s = date.getSeconds()
                s = s < 10 ? ('0' + s) : s
                return y + '/' + MM + '/' + d + ' ' + h + ':' + m
              }

              function getMin(longTypeDate) {
                const date = new Date(longTypeDate)
                const y = date.getFullYear()
                let MM = date.getMonth() + 1
                MM = MM < 10 ? ('0' + MM) : MM
                let d = date.getDate()
                d = d < 10 ? ('0' + d) : d
                let h = date.getHours()
                h = h < 10 ? ('0' + h) : h
                let m = date.getMinutes()
                m = m < 10 ? ('0' + m) : m
                let s = date.getSeconds()
                s = s < 10 ? ('0' + s) : s
                return h + ':' + m
              }

              if (datetimeFormat(value) === sc_time) {
                // console.log('上床')
                return '|\n|\n|\n上床 ' + getMin(value)
              } else if (datetimeFormat(value) === rs_time) {
                // console.log('入睡')
                return '|\n入睡 ' + getMin(value)
              } else if (datetimeFormat(value) === qx_time) {
                // console.log('清醒')
                return '|\n清醒 ' + getMin(value)
              } else if (datetimeFormat(value) === lc_time) {
                // console.log('离床')
                return '|\n|\n|\n离床 ' + getMin(value)
              }
              return ''
            },
            rich: {
              // 这里的rich，下面有解释
              'SC': {
                width: 1,
                height: 100,
                borderColor: '#000',
                verticalAlign: 'bottom',
                borderWidth: 0.5
              },
              'RS': {
                verticalAlign: 'bottom',
                width: 0,
                height: 30,
                borderColor: '#000',
                borderWidth: 0.5
              },
              'QX': {
                align: 'left',
                width: 0,
                height: 30,
                borderColor: '#000',
                borderWidth: 0.5
              },
              'QC': {
                align: 'right',
                verticalAlign: 'right',
                width: 100,
                height: 100,
                borderColor: '#000',
                borderWidth: 0.5
              }
            }

          }
        }],
        yAxis: [{
          nameTextStyle: {
            color: '#63B8FF',
            fontWeight: 'bold',
            fontSize: 15
          },
          name: '',
          splitLine: {
            show: true
          }
        }, {
          splitLine: { show: false },
          gridIndex: 1,
          data: ['N3', 'N2', 'N1', 'R', 'W', 'O']
        }],
        series: [
          { name: 'N3', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#4c6ce5',
              lineStyle: {
                color: '#4c6ce5'
              }
            }
          }},
          { name: 'N2', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#a9ddfd',
              lineStyle: {
                color: '#a9ddfd'
              }
            }
          }},
          { name: 'N1', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#a9ddfd',
              lineStyle: {
                color: '#a9ddfd'
              }
            }
          }},
          { name: 'R', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#e6affd',
              lineStyle: {
                color: '#e6affd'
              }
            }
          }},
          { name: 'W', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#d8edff',
              lineStyle: {
                color: '#d8edff'
              }
            }
          }},
          { name: 'O', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#ffdf24',
              lineStyle: {
                color: '#ffdf24'
              }
            }
          }},
          {
            type: 'line',
            name: '呼吸',
            showSymbol: false,
            data: hx_point_d,
            itemStyle: {
              normal: {
                color: '#2f2ffa',
                lineStyle: {
                  color: '#2f2ffa'
                }
              }
            },
            markPoint: {
              data: [
                {
                  type: 'max',
                  name: '最高呼吸',
                  label: {
                    color: '#2f2ffa',
                    show: true
                  }
                },
                {
                  type: 'min',
                  name: '最低呼吸',
                  label: {
                    color: '#2f2ffa',
                    show: true
                  }
                }
              ],
              label: {
                show: false
              },
              itemStyle: {
                color: '#6a6afc',
                opacity: 1
              }
            }
          }, {
            name: '心率',
            type: 'line',
            showSymbol: false,
            data: xl_point_d,
            itemStyle: {
              normal: {
                color: '#ffa500',
                lineStyle: {
                  color: '#ffa500'
                }
              }
            },
            markPoint: {
              data: [
                {
                  type: 'max',
                  name: '最大心率',
                  label: {
                    color: '#ffa500',
                    show: true
                  }
                },
                {
                  type: 'min',
                  name: '最小心率',
                  label: {
                    color: '#ffa500',
                    show: true
                  }
                }
              ],
              label: {
                show: false
              },
              itemStyle: {
                color: '#081944',
                opacity: 1
              }
            }
          },
          {
            xAxisIndex: 1,
            yAxisIndex: 1,
            type: 'custom',
            renderItem: function(params, api) {
              var categoryIndex = api.value(0)
              var start = api.coord([api.value(1), categoryIndex])
              var end = api.coord([api.value(2), categoryIndex])
              var height = 12
              return {
                type: 'rect',
                shape: echarts.graphic.clipRectByRect({
                  x: start[0],
                  y: start[1] - height / 2,
                  width: end[0] - start[0],
                  height: height
                }, {
                  x: params.coordSys.x,
                  y: params.coordSys.y,
                  width: params.coordSys.width,
                  height: params.coordSys.height
                }),
                style: api.style()
              }
            },
            encode: {
              x: [1, 2],
              y: 0
            },
            data: sm_point_xAxis_data
          }]
      }

      myChart2.setOption(option, null)
    },
    onGetReportSug() {
      this.loadingBySug = true
      getDayReportSug({ reportId: this.reportId }).then(res => {
        if (res.result === 1) {
          var d = res.data

          this.formBySug.rptSummary = d.rptSummary
          this.formBySug.rptSuggest = d.rptSuggest
          this.formBySug.isSend = d.isSend
          this.formBySug.sugSkus = d.sugSkus
        }
        this.loadingBySug = false
      })
    },
    onGetHandleRecords() {
      this.loadingByHandleRecord = true
      this.recordsQuery.userId = this.userInfo.userId
      this.recordsQuery.reportId = this.reportId
      this.recordsQuery.taskId = this.taskId
      getHandleRecords(this.recordsQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.recordsData = d.items
          this.recordsTotal = d.total
        }
        this.loadingByHandleRecord = false
      })
    },
    onSaveReportSug(isSend) {
      var tips = '确定要暂存'
      if (isSend) {
        tips = '确定要保存并发送'
      }

      var form = {
        taskId: this.taskId,
        reportId: this.reportId,
        isSend: isSend,
        rptSuggest: this.formBySug.rptSuggest,
        rptSummary: this.formBySug.rptSummary,
        sugSkus: this.formBySug.sugSkus
      }
      MessageBox.confirm(tips, '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        saveDayReportSug(form).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.onGetReportSug()

            if (isSend) {
              this.$emit('aftersave')
            }
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }
        })
      }).catch(() => {
      })
    },
    onSaveVisitTelephone() {
      this.$refs['formByVisitTelephone'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          })
            .then(() => {
              var _from = {
                taskId: this.taskId,
                userId: this.userInfo.userId,
                reportId: this.reportId,
                visitTime: this.formByVisitTelephone.visitTime,
                nextTime: this.formByVisitTelephone.nextTime,
                visitContent: { remark: this.formByVisitTelephone.remark }
              }
              saveVisitRecordByTelePhone(_from).then(res => {
                if (res.result === 1) {
                  this.$message({
                    message: res.message,
                    type: 'success'
                  })
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
    onSaveVisitWapush() {
      this.$refs['formByVisitPapush'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          })
            .then(() => {
              var _from = {
                taskId: this.taskId,
                userId: this.userInfo.userId,
                reportId: this.reportId,
                visitTemplate: this.formByVisitPapush.visitTemplate,
                visitContent: {
                  keyword1: this.formByVisitPapush.keyword1,
                  keyword2: this.formByVisitPapush.keyword2,
                  keyword3: this.formByVisitPapush.keyword3,
                  remark: this.formByVisitPapush.remark
                }
              }
              saveVisitRecordByPapush(_from).then(res => {
                if (res.result === 1) {
                  this.$message({
                    message: res.message,
                    type: 'success'
                  })
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
    onBrechWorkTabs(tab, event) {
      var tabInnerText = event.target.innerText

      if (tabInnerText === '电话回访') {

      } else if (tabInnerText === '微信告知') {

      } else if (tabInnerText === '历史记录') {
        this.onGetHandleRecords()
      }
    },
    onBeforeClose() {
      this.$emit('update:visible', false)
    },
    datetimeFormat(longTypeDate) {
      const date = new Date(longTypeDate * 1000)
      const y = date.getFullYear()
      let MM = date.getMonth() + 1
      MM = MM < 10 ? ('0' + MM) : MM
      let d = date.getDate()
      d = d < 10 ? ('0' + d) : d
      let h = date.getHours()
      h = h < 10 ? ('0' + h) : h
      let m = date.getMinutes()
      m = m < 10 ? ('0' + m) : m
      let s = date.getSeconds()
      s = s < 10 ? ('0' + s) : s
      return y + '/' + MM + '/' + d + ' ' + h + ':' + m
    },
    datetimeFormat2(longTypeDate, ts) {
      const date = new Date(longTypeDate * 1000 + ts)
      const y = date.getFullYear()
      let MM = date.getMonth() + 1
      MM = MM < 10 ? ('0' + MM) : MM
      let d = date.getDate()
      d = d < 10 ? ('0' + d) : d
      let h = date.getHours()
      h = h < 10 ? ('0' + h) : h
      let m = date.getMinutes()
      m = m < 10 ? ('0' + m) : m
      let s = date.getSeconds()
      s = s < 10 ? ('0' + s) : s
      return y + '/' + MM + '/' + d + ' ' + h + ':' + m
    }
  }
}
</script>

<style lang="scss" scoped>

.clz{
    font-size:11px;
            color:#333333;
                   border-top: 1px solid #666666;
        border-left: 1px solid #666666;
    th{
        text-align: center;
            padding: 8px;
            background-color: #dedede;
                    border-right: 1px solid #666666;
        border-bottom: 1px solid #666666;
    }

    td{
        text-align: center;
        padding: 8px;
        background-color: #ffffff;
        border-right: 1px solid #666666;
        border-bottom: 1px solid #666666;
    }
 }

  #main{
 width: 100%;
 }

</style>
