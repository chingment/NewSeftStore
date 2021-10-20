import Vue from 'vue'
import SkeletonHomeIndex from './skeleton/SkeletonHomeIndex'

export default new Vue({
  components: {
    SkeletonHomeIndex
  },
  template: `
        <div>
            <skeletonHomeIndex id="skeletonHomeIndex" style="display:none"/>
        </div>
    `
})
