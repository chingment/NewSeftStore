<template>

  <el-dialog v-if="visible" title="健康报告（月）" :visible.sync="visible" width="1600px" custom-class="senviv-stage-detail" append-to-body :before-close="onBeforeClose">
    <el-container class="brech-work">
      <el-aside class="brech-work-aside">

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>数据标签</h5>
          </div>
          <div class="pull-right" />
        </div>

        <div v-if="rd!=null">
          <template
            v-for="tag in rd.smTags"
          >
            <el-badge :key="tag.name" :value="tag.count" class="item" style="margin-right:20px;margin-bottom:10px">
              <el-tag>
                {{ tag.name }}
              </el-tag>
            </el-badge>
          </template>
        </div>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>时间段数据</h5>
          </div>
          <div class="pull-right" />
        </div>
        <table v-if="rd!=null&&rd.timeFrameStaPt!=null" class="clz" cellspacing="0" cellpadding="0" style="width:100%;">
          <thead>
            <tr>
              <th style="width:10%" />
              <th style="width:5%">上床</th>
              <th style="width:5%">入睡</th>
              <th style="width:5%">清醒</th>
              <th style="width:5%">离床</th>
              <th style="width:10%">呼吸暂停</th>
              <th style="width:10%">体动</th>
              <th style="width:10%">平均呼吸</th>
              <th style="width:10%">平均心率</th>
              <th style="width:10%">深睡</th>
              <th style="width:10%">浅睡</th>
              <th style="width:10%">REM</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>21:00~23:00</td>
              <td>{{ rd.timeFrameStaPt.t1.sccs }}</td>
              <td>{{ rd.timeFrameStaPt.t1.rscs }}</td>
              <td>{{ rd.timeFrameStaPt.t1.qxcs }}</td>
              <td>{{ rd.timeFrameStaPt.t1.lccs }}</td>
              <td>{{ rd.timeFrameStaPt.t1.hxZtcs }}</td>
              <td>{{ rd.timeFrameStaPt.t1.tdcs }}</td>
              <td>{{ rd.timeFrameStaPt.t1.pjHx }}</td>
              <td>{{ rd.timeFrameStaPt.t1.pjXl }}</td>
              <td>{{ rd.timeFrameStaPt.t1.sd }}</td>
              <td>{{ rd.timeFrameStaPt.t1.qd }}</td>
              <td>{{ rd.timeFrameStaPt.t1.rem }}</td>
            </tr>
            <tr>
              <td>23:00~01:00</td>
              <td>{{ rd.timeFrameStaPt.t2.sccs }}</td>
              <td>{{ rd.timeFrameStaPt.t2.rscs }}</td>
              <td>{{ rd.timeFrameStaPt.t2.qxcs }}</td>
              <td>{{ rd.timeFrameStaPt.t2.lccs }}</td>
              <td>{{ rd.timeFrameStaPt.t2.hxZtcs }}</td>
              <td>{{ rd.timeFrameStaPt.t2.tdcs }}</td>
              <td>{{ rd.timeFrameStaPt.t2.pjHx }}</td>
              <td>{{ rd.timeFrameStaPt.t2.pjXl }}</td>
              <td>{{ rd.timeFrameStaPt.t2.sd }}</td>
              <td>{{ rd.timeFrameStaPt.t2.qd }}</td>
              <td>{{ rd.timeFrameStaPt.t2.rem }}</td>
            </tr>
            <tr>
              <td>01:00~03:00</td>
              <td>{{ rd.timeFrameStaPt.t3.sccs }}</td>
              <td>{{ rd.timeFrameStaPt.t3.rscs }}</td>
              <td>{{ rd.timeFrameStaPt.t3.qxcs }}</td>
              <td>{{ rd.timeFrameStaPt.t3.lccs }}</td>
              <td>{{ rd.timeFrameStaPt.t3.hxZtcs }}</td>
              <td>{{ rd.timeFrameStaPt.t3.tdcs }}</td>
              <td>{{ rd.timeFrameStaPt.t3.pjHx }}</td>
              <td>{{ rd.timeFrameStaPt.t3.pjXl }}</td>
              <td>{{ rd.timeFrameStaPt.t3.sd }}</td>
              <td>{{ rd.timeFrameStaPt.t3.qd }}</td>
              <td>{{ rd.timeFrameStaPt.t3.rem }}</td>
            </tr>
            <tr>
              <td>03:00~05:00</td>
              <td>{{ rd.timeFrameStaPt.t4.sccs }}</td>
              <td>{{ rd.timeFrameStaPt.t4.rscs }}</td>
              <td>{{ rd.timeFrameStaPt.t4.qxcs }}</td>
              <td>{{ rd.timeFrameStaPt.t4.lccs }}</td>
              <td>{{ rd.timeFrameStaPt.t4.hxZtcs }}</td>
              <td>{{ rd.timeFrameStaPt.t4.tdcs }}</td>
              <td>{{ rd.timeFrameStaPt.t4.pjHx }}</td>
              <td>{{ rd.timeFrameStaPt.t4.pjXl }}</td>
              <td>{{ rd.timeFrameStaPt.t4.sd }}</td>
              <td>{{ rd.timeFrameStaPt.t4.qd }}</td>
              <td>{{ rd.timeFrameStaPt.t4.rem }}</td>
            </tr>
            <tr>
              <td>05:00~07:00</td>
              <td>{{ rd.timeFrameStaPt.t5.sccs }}</td>
              <td>{{ rd.timeFrameStaPt.t5.rscs }}</td>
              <td>{{ rd.timeFrameStaPt.t5.qxcs }}</td>
              <td>{{ rd.timeFrameStaPt.t5.lccs }}</td>
              <td>{{ rd.timeFrameStaPt.t5.hxZtcs }}</td>
              <td>{{ rd.timeFrameStaPt.t5.tdcs }}</td>
              <td>{{ rd.timeFrameStaPt.t5.pjHx }}</td>
              <td>{{ rd.timeFrameStaPt.t5.pjXl }}</td>
              <td>{{ rd.timeFrameStaPt.t5.sd }}</td>
              <td>{{ rd.timeFrameStaPt.t5.qd }}</td>
              <td>{{ rd.timeFrameStaPt.t5.rem }}</td>
            </tr>
            <tr>
              <td>07:00~09:00</td>
              <td>{{ rd.timeFrameStaPt.t6.sccs }}</td>
              <td>{{ rd.timeFrameStaPt.t6.rscs }}</td>
              <td>{{ rd.timeFrameStaPt.t6.qxcs }}</td>
              <td>{{ rd.timeFrameStaPt.t6.lccs }}</td>
              <td>{{ rd.timeFrameStaPt.t6.hxZtcs }}</td>
              <td>{{ rd.timeFrameStaPt.t6.tdcs }}</td>
              <td>{{ rd.timeFrameStaPt.t6.pjHx }}</td>
              <td>{{ rd.timeFrameStaPt.t6.pjXl }}</td>
              <td>{{ rd.timeFrameStaPt.t6.sd }}</td>
              <td>{{ rd.timeFrameStaPt.t6.qd }}</td>
              <td>{{ rd.timeFrameStaPt.t6.rem }}</td>
            </tr>
          </tbody>
        </table>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>心率-呼吸变化趋势</h5>
          </div>
          <div class="pull-right" />
        </div>
        <div ref="echart_xl" style="width: 960px;height: 400px;margin:auto" />
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>心率变异性(HRV)</h5>
          </div>
          <div class="pull-right" />
        </div>
        <div ref="echart_hrv" style="width: 960px;height: 400px;margin:auto" />
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>数据指标</h5>
          </div>
          <div class="pull-right" />
        </div>
        <table v-if="rd!=null&&rd.smSmsc!=null" class="clz" cellspacing="0" cellpadding="0" style="width:100%;margin-bottom:80px;">
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

      </el-aside>
      <el-container class="brech-work-container">
        <div class="header">
          <el-button size="small" type="primary" @click="onOpenByDrawer('sug')">评 价</el-button>
          <!-- <el-button size="small" type="primary" @click="onOpenByDrawerBySug">电话回访</el-button>
        <el-button size="small" type="primary" @click="onOpenByDrawerBySug">公众号告知</el-button> -->
        </div>
        <div v-show="drawer.active==='sug'" class="drawer">
          <div class="drawer_content">
            <div v-loading="loadingBySug">
              <div style="margin:10px 0px">
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
              <el-card class="box-card" style="margin-bottom:10px">
                <div slot="header" class="clearfix">
                  <span>推荐商品</span>
                </div>
                <div>

                  <div v-if="!formBySug.isSend">
                    <el-autocomplete
                      v-model="temp.searchSkuKey"
                      :fetch-suggestions="onSearchSku"
                      placeholder="商品名称/编码/条形码/首拼音母"
                      clearable
                      style="width: 75%"
                      size="medium"
                      @select="onSearchSkuSelect"
                    >
                      <template slot-scope="{ item }">
                        <div class="spu-search">
                          <div class="name">{{ item.name }}</div>
                          <div class="desc">[{{ item.cumCode }}]</div>
                        </div>
                      </template>
                    </el-autocomplete>

                    <el-button size="medium" style="width: 20%" @click="onAddSugSku">添加</el-button>
                  </div>

                  <div>
                    <el-table
                      key="list_sugskus"
                      :data="formBySug.sugSkus"
                      fit
                      highlight-current-row
                      style="width: 100%;"
                    >
                      <el-table-column label="商品名称" align="left" min-width="50%">
                        <template slot-scope="scope">
                          <span>{{ scope.row.name }}</span>
                        </template>
                      </el-table-column>
                      <el-table-column label="编码" align="left" min-width="50%">
                        <template slot-scope="scope">
                          <span>{{ scope.row.cumCode }}</span>
                        </template>
                      </el-table-column>
                      <el-table-column
                        v-if="!formBySug.isSend"
                        label="操作"
                        align="right"
                        width="180"
                        class-name="small-padding fixed-width"
                      >
                        <template slot-scope="scope">
                          <el-button
                            type="text"
                            size="mini"
                            @click="onDeleteSugSku(scope.$index)"
                          >删除</el-button>
                        </template>
                      </el-table-column>
                    </el-table>
                  </div>

                </div>
              </el-card>
            </div>
          </div>
          <div class="drawer_footer">
            <el-button size="small" @click="onCloseByDrawer">关 闭</el-button>
            <el-button v-if="!formBySug.isSend" size="small" type="primary" @click="onSaveSug(false)">暂 存</el-button>
            <el-button v-if="!formBySug.isSend" size="small" type="success" @click="onSaveSug(true)">保存并发送</el-button>
          </div>
        </div>

      </el-container>
    </el-container>
  </el-dialog>
</template>

<script>

import { MessageBox } from 'element-ui'
import echarts from 'echarts'
import { getMonthReportDetail, saveMonthReportSug, getMonthReportSug } from '@/api/senviv'
import { searchSku } from '@/api/product'
var myChart1
var myChart2

export default {
  name: 'PaneStageReportDetail',
  props: {
    reportId: {
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
        width: '1000px'
      },
      loading: false,
      loadingBySug: false,
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
      },
      drawer: {
        active: ''
      },
      formBySug: {
        reportId: '',
        rptSummary: '',
        rptSuggest: '',
        isSend: false,
        sugSkus: []
      },
      temp: {
        searchSkuKey: '',
        cur_search_sel_sku: { id: '', name: '', cumCode: '' }
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {
    // if (!myChart1) {
    //   myChart1 = echarts.init(this.$refs.echart_xl, null, { renderer: 'svg' })
    // }

    // if (!myChart2) {
    //   myChart2 = echarts.init(this.$refs.echart_hrv, null, { renderer: 'svg' })
    // }

    window.addEventListener('beforeunload', this.clearChart)
  },
  created() {
    this.onGetDayReportDetail()
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
    onGetDayReportDetail() {
      this.loading = true
      getMonthReportDetail({ reportId: this.reportId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.userInfo = d.userInfo
          this.rd = d.reportData

          this.$nextTick(function() {
            this.onGetChartByXl()
            this.onGetChartByHrv()
          }, 2000)
        }
        this.loading = false
      })
    },
    onGetChartByXl() {
      var rd = this.rd
      if (rd.datePt === null) { return }
      var datePt = rd.datePt

      var xlDcjzxlPt = rd.xlDcjzxlPt
      var xlCqjzxlPt = rd.xlCqjzxlPt
      var hxDcjzhxPt = rd.hxDcjzhxPt
      var hxCqjzhxPt = rd.hxCqjzhxPt
      var HxZtcsPt = rd.hxCqjzhxPt
      var hxZtahizsPt = rd.hxZtahizsPt

      if (!myChart1) {
        myChart1 = echarts.init(this.$refs.echart_xl, null, { renderer: 'svg' })
      }

      var option = {
        grid: [{
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
          data: ['当次基准心率', '长期基准心率', '当次基准呼吸频率', '长期基准呼吸频率', '当夜呼吸暂停次数', 'AHI指数'],
          y: 'bottom'
        },
        tooltip: {
          trigger: 'axis'
        },
        xAxis: [{
          data: datePt,
          show: true
        }],
        yAxis: {
          type: 'value'
        },
        series: [{
          type: 'line',
          name: '当次基准心率',
          showSymbol: true,
          data: xlDcjzxlPt
        },
        {
          type: 'line',
          name: '长期基准心率',
          showSymbol: true,
          data: xlCqjzxlPt
        },
        {
          type: 'line',
          name: '当次基准呼吸频率',
          showSymbol: true,
          data: hxDcjzhxPt
        }, {
          type: 'line',
          name: '长期基准呼吸频率',
          showSymbol: true,
          data: hxCqjzhxPt
        },
        {
          type: 'line',
          name: '当夜呼吸暂停次数',
          showSymbol: true,
          data: HxZtcsPt
        }, {
          type: 'line',
          name: 'AHI指数',
          showSymbol: true,
          data: hxZtahizsPt
        }
        ]
      }

      myChart1.setOption(option, null)
    },
    onGetChartByHrv() {
      var rd = this.rd
      if (rd.datePt === null) { return }
      var datePt = rd.datePt

      var hrvXzznlPt = rd.hrvXzznlPt
      var hrvJgsjzlzsPt = rd.hrvJgsjzlzsPt
      var hrvMzsjzlzsPt = rd.hrvMzsjzlzsPt
      var hrvZzsjzlzsPt = rd.hrvZzsjzlzsPt
      var jbfxXlscfxPt = rd.jbfxXlscfxPt
      var jbfxXljslPt = rd.jbfxXljslPt
      // console.log(JSON.stringify(xlCqjzxlPt))

      if (!myChart2) {
        myChart2 = echarts.init(this.$refs.echart_hrv, null, { renderer: 'svg' })
      }

      var option = {
        grid: [{
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
          data: ['心脏总能量', '交感神经张力指数', '迷走神经张力指数', '自主神经张力指数', '心率失常风险指数', '猝死风险指数'],
          y: 'bottom'
        },
        tooltip: {
          trigger: 'axis'
        },
        xAxis: [{
          data: datePt,
          show: true
        }],
        yAxis: {
          type: 'value'
        },
        series: [{
          type: 'line',
          name: '心脏总能量',
          showSymbol: true,
          data: hrvXzznlPt
        },
        {
          type: 'line',
          name: '交感神经张力指数',
          showSymbol: true,
          data: hrvJgsjzlzsPt
        },
        {
          type: 'line',
          name: '迷走神经张力指数',
          showSymbol: true,
          data: hrvMzsjzlzsPt
        }, {
          type: 'line',
          name: '自主神经张力指数',
          showSymbol: true,
          data: hrvZzsjzlzsPt
        },
        {
          type: 'line',
          name: '心率失常风险指数',
          showSymbol: true,
          data: jbfxXlscfxPt
        }, {
          type: 'line',
          name: '猝死风险指数',
          showSymbol: true,
          data: jbfxXljslPt
        }
        ]
      }

      myChart2.setOption(option, null)
    },
    onCloseByDrawer() {
      this.drawer.active = ''
    },
    onOpenByDrawer(active) {
      this.drawer.active = active
      this.onGetMonthReportSug()
    },
    onGetMonthReportSug() {
      this.loadingBySug = true
      getMonthReportSug({ reportId: this.reportId }).then(res => {
        this.loadingBySug = false
        if (res.result === 1) {
          var d = res.data

          this.formBySug.rptSummary = d.rptSummary
          this.formBySug.rptSuggest = d.rptSuggest
          this.formBySug.isSend = d.isSend
          this.formBySug.sugSkus = d.sugSkus
        }
      })
    },
    onSaveSug(isSend) {
      var tips = '确定要暂存'
      if (isSend) {
        tips = '确定要保存并发送'
      }

      var form = {
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
        saveMonthReportSug(form).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.onGetMonthReportSug()

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
    onSearchSku(queryString, cb) {
      searchSku({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({
              value: d[j].name,
              mainImgUrl: d[j].mainImgUrl,
              name: d[j].name,
              skuId: d[j].skuId,
              cumCode: d[j].cumCode
            })
          }

          cb(restaurants)
        }
      })
    },
    onSearchSkuSelect(item) {
      this.temp.cur_search_sel_sku.id = item.skuId
      this.temp.cur_search_sel_sku.name = item.name
      this.temp.cur_search_sel_sku.cumCode = item.cumCode
    },
    onAddSugSku() {
      var list = this.formBySug.sugSkus
      var id = this.temp.cur_search_sel_sku.id
      var name = this.temp.cur_search_sel_sku.name
      var cumCode = this.temp.cur_search_sel_sku.cumCode
      if (id === '' || typeof id === 'undefined') {
        this.$message('请选择商品')
        return
      }
      const is_has = list.find(item => {
        return item.id === id
      })

      if (is_has != null) {
        this.$message('商品已存在')
        return
      }
      list.push({ id: id, name: name, cumCode: cumCode })
      this.temp.searchSkuKey = ''
      this.temp.cur_search_sel_sku.id = ''
      this.temp.cur_search_sel_sku.name = ''
      this.temp.cur_search_sel_sku.cumCode = ''
    },
    onDeleteSugSku(index) {
      var list = this.formBySug.sugSkus
      list.splice(index, 1)
    },
    onBeforeClose() {
      this.$emit('update:visible', false)
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

// .el-drawer__body{
//       overflow: auto;
// }
</style>
