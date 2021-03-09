import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import { BootstrapVue } from "bootstrap-vue";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-vue/dist/bootstrap-vue.css'
import GameHub from "@/plugins/game-hub";

Vue.config.productionTip = false;

Vue.use(BootstrapVue);
Vue.use(GameHub);

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount("#app");
