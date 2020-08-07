
<template>
  <!-- eslint-disable vue/require-component-is -->
  <component v-bind="linkProps(to)">
    <slot />
  </component>
</template>

<script>
import { isExternal } from '@/utils/validate'

export default {
  props: {
    to: {
      type: String,
      required: true
    }
  },
  methods: {
    linkProps(url) {
      if (isExternal(url)) {
        return {
          is: 'a',
          href: url,
          target: '_blank',
          rel: 'noopener'
        }
      }

      var path = url
      var query = {}
      if (url.indexOf('?') > -1) {
        path = url.split('?')[0]
        // var params = url.split('?')[1]
        query = this.queryString(url)
        // console.log('path:' + path)
        // console.log('params:' + params)
        // console.log('query:' + JSON.stringify(query))
      }

      return {
        is: 'router-link',
        to: { path: path, query: query }
      }
    },
    queryString(url) {
      let arr = [] // 存储参数的数组
      const res = {} // 存储最终JSON结果对象
      arr = url.split('?')[1].split('&') // arr=["a=1", "b=2", "c=test", "d"]

      for (let i = 0, len = arr.length; i < len; i++) {
        // 如果有等号，则执行赋值操作
        if (arr[i].indexOf('=') != -1) {
          const str = arr[i].split('=')
          // str=[a,1];
          res[str[0]] = str[1]
        } else { // 没有等号，则赋予空值
          res[arr[i]] = ''
        }
      }
      // res = JSON.stringify(res)// 转化为JSON字符串
      return res // {"a": "1", "b": "2", "c": "test", "d": ""}
    }
  }
}
</script>
