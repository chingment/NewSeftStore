


import confirmTemp from './src/confirm.vue'

let Confirm = {};  // 定义插件对象

Confirm.install = (Vue, options) => { //Vue的install方法，用于定义vue插件
    // 如果存在loading 不重复创建DOM
    if (document.getElementsByClassName('.lumos-confirm').length) return

    // 创建一个VUE构造器
    let lTemp = Vue.extend(confirmTemp);

    // 提供一个在页面上已存在的DOM元素作为Vue实例的挂载目标。
    // 在实例挂载之后，可以通过$vm.$el访问。
    // 如果这个选项在实例化时有用到，实例将立即进入编译过程。否则，需要显示调用vm.$mount()手动开启编译(如下)
    // 提供的元素只能作为挂载点。所有的挂载元素会被vue生成的dom替换。因此不能挂载在顶级元素(html, body)上
    // let $vm = new toastTpl({
    //  el: document.createElement('div')
    // })

    // 实例化VUE实例
    let $vm = new lTemp();

    // 此处使用$mount来手动开启编译。用$el来访问元素，并插入到body中
    let tpl = $vm.$mount().$el;
    document.body.appendChild(tpl);

    let defaultOptions = {
        title:'提示',
        yesBtnText: '确定',
        noBtnText: '取消',
        isShow:true
    };

    const confirm = function (options) {
        Object.assign($vm, defaultOptions, options, {
            type: 'confirm'
        });
    };

    Vue.prototype.$confirm=confirm;

    // Vue.prototype.$loading = { //在Vue的原型上添加实例方法，以全局调用
    //     show() {
    //       $vm.isShow = true;
    //     },
    //     hide() {
    //         $vm.isShow = false;
    //     }
    // }
}
// //导出Load
export default Confirm;




// import Vue from 'vue'
// import message from './src/confirm.vue'

// const VueComponent = Vue.extend(message);
// const vm = new VueComponent().$mount();
// let init = false;
// let defaultOptions = {
// yesBtnText: '确定',
// noBtnText: '取消',
// shhow:false
// };

// const confirm = function (options) {
// Object.assign(vm,defaultOptions , options,{
//     type:'confirm'
// });

// if (!init) {
//     document.body.appendChild(vm.$el);
//     init = true;
// }

// return vm.confirm();
// };

// Vue.prototype.$confirm=confirm;

// export default confirm;