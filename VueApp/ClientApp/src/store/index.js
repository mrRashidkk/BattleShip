import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    myId: ""
  },
  mutations: {
    setMyId(state, id) {
      state.myId = id;
    }
  },
  actions: {},
  modules: {}
});
