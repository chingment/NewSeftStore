<template>
  <div class="mb5" >
 
 <div class="radio-wrap">  
      <div class="radio-group" v-model="tabView">   
        <span    v-for="(tab ,index) in tabs"  :class="{cur:iscur==index}"   @click="iscur=index,tabChange('select' + (index + 1))">
             {{tab.name}}
        </span>  
      </div>  
      <div style="margin:10px 0;"></div>  
      <keep-alive>   
        <component v-bind:is="tabView"></component>  
      </keep-alive>   
    </div>  


 <button  @click="goAsInsOffer"  class="lumos-button lumos-button-positive lumos-button-bottom" >下一步</button>

  </div>
</template>

<script>


import select1 from '@/components/inscarkind.vue'; 
import select2 from '@/components/inscarkind.vue'; 
import select3 from '@/components/inscarkind.vue'; 

export default {
  data() {
    return {
          tabView: 'select1',
          iscur: 0,   
          tabs: [{name: "选项一"}, {name: "选项二"} ,{name: "选项三"}]
    };
  },
  components: {  
    select1,  
    select2,  
    select3  
  },  
  methods: {
    tabChange:function(tab){  
      this.tabView = tab;  
    },
    goAsInsOffer() {
       console.log("goAsInsOffer");
         this.$router.push({
         path: "/InsCar/As/InsOffer",
       });
    }
  }
  
};
</script>
