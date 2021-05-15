<template>
  <div id="coupon_add">
    <page-header />
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="100px">
      <el-form-item label="优惠券类型" prop="category">
        <el-select v-model="form.category" style="width: 100%" @change="handleCategoryChange">
          <el-option
            v-for="item in temp.options_category"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="优惠券名称" prop="name">
        <el-input v-model="form.name" clearable />
      </el-form-item>
      <el-form-item label="发行总量" prop="issueQuantity" :show-message="rules.issueQuantity[0].isShow">
        <template v-if="form.category==2||form.category==3">
          <span>不限制</span>
        </template>
        <template v-else>
          <el-input v-model="form.issueQuantity" placeholder clearable style="max-width:250px">
            <template slot="append">张</template>
          </el-input>
        </template>
      </el-form-item>

      <el-form-item label="券种" prop="faceType">
        <el-radio-group v-model="form.faceType">
          <el-radio-button label="1">购物代金券</el-radio-button>
          <el-radio-button label="2">购物折扣券</el-radio-button>
          <el-radio-button label="3">租金代金券</el-radio-button>
          <el-radio-button label="4">押金代金券</el-radio-button>
          <el-radio-button label="5">入场券</el-radio-button>
        </el-radio-group>
      </el-form-item>

      <el-form-item label="券值" prop="faceValue">
        <el-input v-model="form.faceValue" placeholder clearable style="max-width:250px">
          <template slot="append">{{ form.faceType==2?"折":"元" }}</template>
        </el-input>
      </el-form-item>
      <el-form-item label="使用门槛" prop="atLeastAmount">
        <el-input v-model="form.atLeastAmount" placeholder clearable style="max-width:250px">
          <template slot="prepend">满</template>
          <template slot="append">元可使用</template>
        </el-input>
      </el-form-item>
      <el-form-item label="有效期" prop="validDate">
        <el-date-picker
          v-model="form.validDate"
          type="daterange"
          range-separator="至"
          start-placeholder="开始日期"
          end-placeholder="结束日期"
          value-format="yyyy-MM-dd"
          style="width: 400px"
        />
      </el-form-item>
      <el-form-item label="可使用时间" prop="useTimeValue">
        <div>
          <el-radio-group v-model="form.useTimeType" @change="handleUseTimeTypeChange">
            <el-radio-button label="1">按领取时间计算有效期</el-radio-button>
            <el-radio-button label="2">按时间段</el-radio-button>
          </el-radio-group>
        </div>
        <div style="margin-top:10px">
          <div v-if="form.useTimeType==1">
            <el-input v-model="form.useTimeValue" clearable placeholder style="max-width:250px">
              <template slot="append">日后无效</template>
            </el-input>
          </div>
          <div v-else-if="form.useTimeType==2">
            <el-date-picker
              v-model="form.useTimeValue"
              type="daterange"
              range-separator="至"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
              value-format="yyyy-MM-dd"
              style="width: 400px"
            />
          </div>
        </div>
      </el-form-item>
      <el-form-item
        label="可使用范围"
        prop="useAreaValue"
        :show-message="rules.useAreaValue[0].isShow"
        :error="rules.useAreaValue[0].message"
      >
        <div>
          <el-radio-group v-model="form.useAreaType" @change="handleUseAreaTypeChange">
            <el-radio-button label="1">全场通用</el-radio-button>
            <el-radio-button label="2">指定店铺</el-radio-button>
            <el-radio-button label="3">指定商品分类</el-radio-button>
            <el-radio-button label="4">指定具体商品</el-radio-button>
          </el-radio-group>
        </div>
        <div style="margin-top:10px">
          <div v-if="form.useAreaType==2">
            <div>
              <el-select
                v-model="temp.cur_sel_usearea_store.id"
                placeholder="选择店铺"
                style="width: 75%"
                size="medium"
                clearable
                @change="handleUseAreaSelStore"
              >
                <el-option
                  v-for="item in temp.options_stores"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
              <el-button size="medium" style="width: 20%" @click="handleUseAreaAddStore">添加</el-button>
            </div>

            <div>
              <el-table
                key="list_usearea_stores"
                :data="temp.list_usearea_stores"
                fit
                highlight-current-row
                style="width: 100%;"
              >
                <el-table-column label="店铺名称" prop="userName" align="left" min-width="30%">
                  <template slot-scope="scope">
                    <span>{{ scope.row.name }}</span>
                  </template>
                </el-table-column>
                <el-table-column
                  label="操作"
                  align="right"
                  width="180"
                  class-name="small-padding fixed-width"
                >
                  <template slot-scope="scope">
                    <el-button
                      type="text"
                      size="mini"
                      @click="handleUseAreaDelStore(scope.$index)"
                    >删除</el-button>
                  </template>
                </el-table-column>
              </el-table>
            </div>
          </div>
          <div v-if="form.useAreaType==3">
            <div>
              <el-cascader
                v-model="temp.kindIds"
                :options="temp.options_kinds"
                placeholder="请选择商品分类"
                style="width: 75%"
                size="medium"
                clearable
                @change="handleUseAreaSelKind"
              />
              <el-button size="medium" style="width: 20%" @click="handleUseAreaAddKind">添加</el-button>
            </div>

            <div>
              <el-table
                key="list_usearea_kinds"
                :data="temp.list_usearea_kinds"
                fit
                highlight-current-row
                style="width: 100%;"
              >
                <el-table-column label="分类名称" prop="userName" align="left" min-width="30%">
                  <template slot-scope="scope">
                    <span>{{ scope.row.name }}</span>
                  </template>
                </el-table-column>
                <el-table-column
                  label="操作"
                  align="right"
                  width="180"
                  class-name="small-padding fixed-width"
                >
                  <template slot-scope="scope">
                    <el-button
                      type="text"
                      size="mini"
                      @click="handleUseAreaDelKind(scope.$index)"
                    >删除</el-button>
                  </template>
                </el-table-column>
              </el-table>
            </div>
          </div>
          <div v-if="form.useAreaType==4">
            <div>
              <el-autocomplete
                v-model="temp.searchKey"
                :fetch-suggestions="handleUseAreaSrhSpu"
                placeholder="商品名称/编码/条形码/首拼音母"
                clearable
                style="width: 75%"
                size="medium"
                @select="handleUseAreaSelSpu"
              >
                <template slot-scope="{ item }">
                  <div class="spu-search">
                    <div class="name">{{ item.name }}</div>
                    <div class="desc">[{{ item.spuCode }}]</div>
                  </div>
                </template>
              </el-autocomplete>

              <el-button size="medium" style="width: 20%" @click="handleUseAreaAddSpu">添加</el-button>
            </div>

            <div>
              <el-table
                key="list_usearea_spus"
                :data="temp.list_usearea_spus"
                fit
                highlight-current-row
                style="width: 100%;"
              >
                <el-table-column label="商品名称" prop="userName" align="left" min-width="50%">
                  <template slot-scope="scope">
                    <span>{{ scope.row.name }}</span>
                  </template>
                </el-table-column>
                <el-table-column label="货号" prop="userName" align="left" min-width="50%">
                  <template slot-scope="scope">
                    <span>{{ scope.row.spuCode }}</span>
                  </template>
                </el-table-column>
                <el-table-column
                  label="操作"
                  align="right"
                  width="180"
                  class-name="small-padding fixed-width"
                >
                  <template slot-scope="scope">
                    <el-button
                      type="text"
                      size="mini"
                      @click="handleUseAreaDelSpu(scope.$index)"
                    >删除</el-button>
                  </template>
                </el-table-column>
              </el-table>
            </div>
          </div>
        </div>
      </el-form-item>
      <el-form-item label="描述" prop="description">
        <el-input
          v-model="form.description"
          type="textarea"
          placeholder="请输入内容"
          maxlength="500"
          show-word-limit
          clearable
        />
      </el-form-item>

      <el-form-item>
        <el-button type="primary" @click="onSubmit">立即创建</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { add, initAdd } from '@/api/coupon'
import { searchSpu } from '@/api/product'
import fromReg from '@/utils/formReg'
import { goBack } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'OperationCenterCouponAdd',
  components: {
    PageHeader
  },
  data() {
    return {
      loading: false,
      form: {
        category: 1,
        name: '',
        useMode: 1,
        issueQuantity: '',
        faceType: 1,
        faceValue: '',
        validDate: [],
        useAreaType: 1,
        useAreaValue: [],
        useTimeType: 1,
        useTimeValue: '',
        description: ''
      },
      temp: {
        options_stores: [],
        options_kinds: [],
        cur_sel_usearea_store: { id: '', name: '' },
        cur_sel_usearea_kind: { id: '', name: '' },
        cur_sel_usearea_spu: { id: '', name: '', spuCode: '' },
        list_usearea_stores: [],
        list_usearea_kinds: [],
        list_usearea_spus: [],
        kindIds: [],
        searchKey: '',
        options_category: [
          {
            value: 1,
            label: '全场赠券'
          },
          {
            value: 2,
            label: '新用户赠券'
          },
          {
            value: 3,
            label: '会员赠券'
          },
          {
            value: 4,
            label: '购物赠券'
          }
        ]
      },
      rules: {
        name: [
          {
            required: true,
            min: 1,
            max: 200,
            message: '必填,且不能超过200个字符',
            trigger: 'change'
          }
        ],
        issueQuantity: [
          {
            required: true,
            message: '只能输入正整数',
            pattern: fromReg.intege1,
            isShow: true
          }
        ],
        faceValue: [
          { required: true, message: '格式,eg:88.88', pattern: fromReg.money }
        ],
        atLeastAmount: [
          { required: true, message: '格式,eg:88.88', pattern: fromReg.money1 }
        ],
        validDate: [{ type: 'array', required: true, message: '请选择有效期' }],
        useTimeValue: [
          {
            required: true,
            message: '请输入正整数',
            isShow: true,
            pattern: fromReg.intege1
          }
        ],
        useAreaValue: [
          { type: 'array', required: false, message: '请选择', isShow: false }
        ]
      }
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initAdd().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.temp.options_stores = d.optionsStores
          this.temp.options_kinds = d.optionsKinds
        }
        this.loading = false
      })
    },
    onSubmit() {
      this.$refs['form'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要创建', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          })
            .then(() => {
              add(this.form).then(res => {
                if (res.result === 1) {
                  this.$message({
                    message: res.message,
                    type: 'success'
                  })
                  goBack(this)
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
    handleCategoryChange(value) {
      value = parseInt(value)
      this.form.category = value
      if (value === 1 || value === 4) {
        this.rules.issueQuantity[0].required = true
        this.rules.issueQuantity[0].isShow = true
        this.rules.issueQuantity[0].pattern = fromReg.intege1
      } else {
        this.rules.issueQuantity[0].required = false
        this.rules.issueQuantity[0].isShow = false
        this.rules.issueQuantity[0].pattern = null
      }
    },
    handleUseTimeTypeChange(value) {
      this.form.useTimeValue = ''
      if (parseInt(value) === 1) {
        this.form.useTimeType = 1
        this.rules.useTimeValue[0].pattern = fromReg.intege1
        this.rules.useTimeValue[0].type = 'string'
        this.rules.useTimeValue[0].required = true
        this.rules.useTimeValue[0].message = '只能输入正整数'
        this.rules.useTimeValue[0].isShow = true
      } else {
        this.form.useTimeType = 2
        this.rules.useTimeValue[0].pattern = null
        this.rules.useTimeValue[0].type = 'array'
        this.rules.useTimeValue[0].required = true
        this.rules.useTimeValue[0].message = '请选择日期'
        this.rules.useTimeValue[0].isShow = true
      }
    },
    handleUseAreaTypeChange(value) {
      value = parseInt(value)
      if (value === 1) {
        this.rules.useAreaValue[0].required = false
        this.rules.useAreaValue[0].isShow = false
        this.rules.useAreaValue[0].message = ''
      } else {
        if (value === 2) {
          this.form.useAreaValue = this.temp.list_usearea_stores
        } else if (value === 3) {
          this.form.useAreaValue = this.temp.list_usearea_kinds
        } else if (value === 4) {
          this.form.useAreaValue = this.temp.list_usearea_spus
        }

        if (
          this.form.useAreaValue == null ||
          this.form.useAreaValue.length === 0
        ) {
          this.rules.useAreaValue[0].required = true
          this.rules.useAreaValue[0].isShow = true
          this.rules.useAreaValue[0].message = '请选择.'
        } else {
          this.rules.useAreaValue[0].required = false
          this.rules.useAreaValue[0].isShow = false
          this.rules.useAreaValue[0].message = ''
        }
      }
    },
    handleUseAreaSrhSpu(queryString, cb) {
      searchSpu({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({
              value: d[j].name,
              mainImgUrl: d[j].mainImgUrl,
              name: d[j].name,
              spuId: d[j].spuId,
              spuCode: d[j].spuCode
            })
          }

          cb(restaurants)
        }
      })
    },
    handleUseAreaSelSpu(item) {
      this.temp.cur_sel_usearea_spu.id = item.spuId
      this.temp.cur_sel_usearea_spu.name = item.name
      this.temp.cur_sel_usearea_spu.spuCode = item.spuCode
    },
    handleUseAreaAddSpu(item) {
      var list = this.temp.list_usearea_spus
      var id = this.temp.cur_sel_usearea_spu.id
      var name = this.temp.cur_sel_usearea_spu.name
      var spuCode = this.temp.cur_sel_usearea_spu.spuCode
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
      list.push({ id: id, name: name, type: 'spu', spuCode: spuCode })

      this.handleUseAreaCheckSel(list)
    },
    handleUseAreaDelSpu(index) {
      var list = this.temp.list_usearea_spus
      list.splice(index, 1)

      this.handleUseAreaCheckSel(list)
    },
    handleUseAreaSelStore(val) {
      const sel_obj = this.temp.options_stores.find(item => {
        return item.value === val
      })

      this.temp.cur_sel_usearea_store.id = sel_obj.value
      this.temp.cur_sel_usearea_store.name = sel_obj.label
    },
    handleUseAreaAddStore(e) {
      var list = this.temp.list_usearea_stores
      var id = this.temp.cur_sel_usearea_store.id
      var name = this.temp.cur_sel_usearea_store.name

      if (id === '' || typeof id === 'undefined') {
        this.$message('请选择店铺')
        return
      }
      const is_has = list.find(item => {
        return item.id === id
      })

      if (is_has != null) {
        this.$message('店铺已存在')
        return
      }
      list.push({ id: id, name: name, type: 'store' })

      this.handleUseAreaCheckSel(list)
    },
    handleUseAreaDelStore(index) {
      var list = this.temp.list_usearea_stores
      list.splice(index, 1)
      this.handleUseAreaCheckSel(list)
    },
    handleUseAreaSelKind(val) {
      let name1
      let name2
      let name3
      var kinds = this.temp.options_kinds
      for (let i = 0; i < kinds.length; i++) {
        if (kinds[i].value === val[0]) {
          name1 = kinds[i].label
          for (let j = 0; j < kinds[i].children.length; j++) {
            if (kinds[i].children[j].value === val[1]) {
              name2 = kinds[i].children[j].label

              for (
                let x = 0;
                x < kinds[i].children[j].children.length;
                x++
              ) {
                if (kinds[i].children[j].children[x].value === val[2]) {
                  name3 = kinds[i].children[j].children[x].label
                }
              }
            }
          }
        }
      }

      // const resultArr = this.options_kinds.find((item) => {
      //   item.chhi
      //   return item.value === val
      // })
      this.temp.cur_sel_usearea_kind.id = val[2]
      this.temp.cur_sel_usearea_kind.name =
        name1 + '/' + name2 + '/' + name3
    },
    handleUseAreaAddKind(e) {
      var list = this.temp.list_usearea_kinds
      var id = this.temp.cur_sel_usearea_kind.id
      var name = this.temp.cur_sel_usearea_kind.name

      if (id === '' || typeof id === 'undefined') {
        this.$message('请选择分类')
        return
      }
      const is_has = list.find(item => {
        return item.id === id
      })

      if (is_has != null) {
        this.$message('分类已存在')
        return
      }
      list.push({ id: id, name: name, type: 'kind' })

      this.handleUseAreaCheckSel(list)
    },
    handleUseAreaDelKind(index) {
      var list = this.temp.list_usearea_kinds
      list.splice(index, 1)

      this.handleUseAreaCheckSel(list)
    },
    handleUseAreaCheckSel(list) {
      if (list == null || list.length === 0) {
        this.form.useAreaValue = null
        this.rules.useAreaValue[0].required = true
        this.rules.useAreaValue[0].isShow = true
        this.rules.useAreaValue[0].message = '请选择'
      } else {
        this.form.useAreaValue = list
        this.rules.useAreaValue[0].required = false
        this.rules.useAreaValue[0].isShow = false
        this.rules.useAreaValue[0].message = ''
      }
    }
  }
}
</script>

<style  lang="scss"  scoped>
#coupon_add {
  max-width: 800px;
  .line {
    text-align: center;
  }

  .is-leaf {
    display: none !important;
    width: 0px !important;
  }
}
</style>

