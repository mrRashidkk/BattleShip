<template>
    <div 
        :draggable="shipInfo.disabled ? 'false' : 'true'" 
        :class="classList"
        :style="styleObject"       
    >
        <div v-for="n in length" :key="n" :data-index="n - 1"></div>
    </div>
</template>

<style scoped>
    .ship {
        display: flex;
        flex-wrap: wrap;
        left: 0;
        top: 0;
        z-index: 2;
        background-color: rgba(10, 100, 250, 0.4);
    }
    .ship div {
        width: 2.5rem;
        height: 2.5rem;
    }
    .ship.warning {
        animation: shake 0.82s cubic-bezier(.36,.07,.19,.97) both;
        background-color: rgba(255, 0, 0, 0.4);
    }
    @keyframes shake {
        10%, 90% {
            transform: translate3d(-1px, 0, 0);
        }
        
        20%, 80% {
            transform: translate3d(2px, 0, 0);
        }

        30%, 50%, 70% {
            transform: translate3d(-4px, 0, 0);
        }

        40%, 60% {
            transform: translate3d(4px, 0, 0);
        }
    }
</style>

<script>
export default {
    props: {
        shipInfo: Object,
    },
    data() {
        return {
            length: this.shipInfo.length
        }
    },    
    computed: {
        classList() {
            return this.shipInfo.warning ? "ship warning" : "ship";
        },
        styleObject() {
            return {                
                width: (this.shipInfo.horizontal ? this.length * 2.5 + "rem" : "2.5rem"),
                height: (this.shipInfo.horizontal ? "2.5rem" : this.length * 2.5 + "rem"),
                zIndex: this.shipInfo.zIndex,
                position: this.shipInfo.launched ? "absolute" : "unset",
                pointerEvents: this.shipInfo.disabled ? "none" : "auto",
                cursor: this.shipInfo.disabled ? "auto" : "move",
            }
        }
    }         
}
</script>