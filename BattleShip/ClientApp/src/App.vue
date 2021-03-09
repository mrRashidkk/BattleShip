<template>
  <div id="app">
    <div id="nav">
      <router-link to="/">Главная</router-link>
    </div>
    <main>
      <router-view />
    </main>
  </div>
</template>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

#nav {
  padding: 15px;
  height: 55px;
  box-shadow: 0 2px 2px 1px rgba(0, 0, 0, 0.2);
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  background-color: rgb(250, 250, 250);

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}

main {
  height: 100vh;
  padding-top: 55px;  
}

.h-100 {
  height: 100%;
}
</style>

<script>
export default {  
  methods: {
    onConnectionStarted(connectionId) {
      this.$store.commit('setMyId', connectionId);
    },
    onError(message) {
      this.$bvToast.toast(message, {
        title: 'Произошла ошибка',
        variant: "danger",
        solid: true
      });
    }    
  },
  created() {
    this.$gameHub.$on("connection-started", this.onConnectionStarted);
    this.$gameHub.$on("error", this.onError);
  },
  beforeDestroy() {
    this.$gameHub.$off("connection-started", this.onConnectionStarted);
    this.$gameHub.$off("error", this.onError);
  }
}
</script>