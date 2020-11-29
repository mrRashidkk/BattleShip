<template>
  <div class="main-bg">
    <b-card class="home">
      <b-container fluid >
        <b-row align-v="center" >
          <b-col cols="12" class="py-2">
            <b-button @click="createMatch" variant="primary" size="lg">Создать игру</b-button>
          </b-col>
          <b-col cols="12" class="py-2">
            <hr class="h-divider">
          </b-col>
        </b-row>
        <b-row align-v="center" class="py-2">
          <b-col cols="8">
            <b-form-input :state="isValidMatchId" v-model="matchIdToJoin" autocomplete="new"></b-form-input>
          </b-col>
          <b-col cols="4">
            <b-button @click="joinMatch" variant="success" size="lg">Присоединиться</b-button>
          </b-col>
        </b-row>
      </b-container>
    </b-card>
  </div>
</template>

<style>
  .main-bg {
    height: 100%;
    background: url("../assets/cruiser.jpg") center no-repeat;
  }
  .home {
    position: absolute;
    width: 800px;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background-color: rgba(255, 255, 255, 0.7) !important;
  }
  .h-divider {
    border-top: 1px solid darkslategrey;
  }
</style>

<script>
export default {
  data() { 
    return {
      matchIdToJoin: ""
    }
  },
  computed: {
    isValidMatchId() {
      return this.matchIdToJoin != null && this.matchIdToJoin !== "";
    }
  },
  methods: {
    createMatch() {
      this.$gameHub.createMatch().then(match => {
        this.$router.push({ name: "Game", params: { match } });
      })
      .catch(err => {
        console.log(`Failed to create a match: ${err}`);
      });
    },
    joinMatch() {
      if (this.isValidMatchId) {
        this.$gameHub.joinMatch(this.matchIdToJoin).then(match => {
          this.$router.push({ name: "Game", params: { match } });
        })
        .catch(err => {
          console.log(`Failed to join the match: ${err}`);
        });
      }      
    }
  }
};
</script>
