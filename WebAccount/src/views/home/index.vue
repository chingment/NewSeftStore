<template>
  <div class="home-container">
    <el-row :gutter="20">

      <el-col v-for="appcaltion in appcaltions" :key="appcaltion.url" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="header-item clearfix">
            <span>{{ appcaltion.name }}</span>
            <el-button style="float: right; padding: 3px 0" type="text" @click="goAppcaltion(appcaltion)">进入</el-button>
          </div>
          <div class="component-item">
            <div class="it-img"> <img :src="appcaltion.imgUrl" alt=""> </div>
            <div class="it-describe"> {{ appcaltion.describe }} </div>
          </div>
        </el-card>
      </el-col>

    </el-row>
  </div>
</template>

<script>
import { getIndexPageData } from '@/api/home'
import { mapGetters } from 'vuex'

export default {
  name: 'Home',
  data() {
    return {
      appcaltions: []
    }
  },
  computed: {
    ...mapGetters([
      'name'
    ])
  },
  created() {
    this.getPageData()
  },
  methods: {
    getPageData() {
      getIndexPageData().then(response => {
        this.appcaltions = response.data.appcaltions
      })
    },
    goAppcaltion(appcaltion) {
      window.location.href = appcaltion.url + '?token=' + this.$store.getters.token
    }
  }
}
</script>

<style lang="scss" scoped>
.dashboard {
  &-container {
    margin: 30px;
  }
  &-text {
    font-size: 30px;
    line-height: 46px;
  }
}

.home-container{
  padding: 30px;

  .header-item{
    .it-login{
      float: right;
    }
  }
  .component-item{
    min-height: 100px;
    display: flex;
    .it-img{
      width: 120px;
      height: 120px;

      img{
        width: 100%;
        height: 100%;
      }
    }

    .it-describe{
      flex: 1;
      padding: 5px;
      font-size: 12px;
    }
  }
}
</style>
