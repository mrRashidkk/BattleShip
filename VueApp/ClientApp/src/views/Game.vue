<template>
    <b-container fluid class="game">
        <b-row align-v="start">
            <b-col>
                <b-alert v-if="enemy == null" variant="primary" show>
                    <strong>Чтобы позвать друга, сообщите ему ID матча:</strong> {{matchInfo.id}}
                </b-alert>
                <b-alert v-else-if="!enemy.connected" variant="danger" show>
                    Соперник отключился от игры
                </b-alert>                
                <b-alert v-else-if="!matchInfo.started" variant="primary" show>
                    <span v-if="!enemy.ready">
                        Соперник подключен и расставляет корабли. 
                    </span>
                    <span v-else>
                        Соперник готов начать игру.
                    </span>
                    <span v-if="!player.ready">
                        Расставьте корабли и нажмите "Готов к игре"
                    </span>
                </b-alert>
                <b-alert v-else-if="matchInfo.gameOver && matchInfo.winner != myId" variant="danger" show>
                    Вы проиграли
                </b-alert>
                <b-alert v-else-if="matchInfo.gameOver && matchInfo.winner == myId" variant="success" show>
                    Вы выиграли
                </b-alert>
                <b-alert v-else-if="myTurn" variant="primary" show>
                    Ваш ход
                </b-alert>
                <b-alert v-else variant="primary" show>
                    Ход соперника
                </b-alert>                
            </b-col>            
        </b-row>
        <b-row>        
            <b-col v-if="notLaunchedShips.length > 0" id="dock">
                <p>Перетащите корабли на свое поле.<br>Чтобы повернуть корабль, кликните по нему дважды.</p>
                <ship v-for="ship in notLaunchedShips"
                    :key="ship.id"
                    :shipInfo="ship"
                    @mousedown.native="selectIndex" 
                    @dragstart.native="dragStart($event, ship)" 
                    @dragend.native.prevent="dragEnd"
                    @dblclick.native="rotate($event, ship)"
                ></ship>
            </b-col>

            <b-col>
                <p class="text-center">Ваше поле</p>
                <table class="board">
                    <tr v-for="(squaresRow, rowIndex) in squares" :key="rowIndex">
                        <td v-for="(square, colIndex) in squaresRow"
                            :key="`square-${rowIndex}-${colIndex}`"
                            @drop="drop($event, rowIndex, colIndex)"
                            @dragover.prevent
                            @dragenter.prevent
                            :class="getClassListForSquare(square)"
                        >
                            <ship v-if="isShipInSquare(rowIndex, colIndex)"
                                :shipInfo="getShipFromSquare(rowIndex, colIndex)"
                                @mousedown.native="selectIndex" 
                                @dragstart.native="dragStart($event, getShipFromSquare(rowIndex, colIndex))"
                                @dragend.native.prevent="dragEnd"
                                @dblclick.native="rotate($event, getShipFromSquare(rowIndex, colIndex))"
                            ></ship>
                        </td>
                    </tr>
                </table>
                <div v-if="!player.ready">
                    <b-button @click="ready" variant="success">Готов к игре</b-button>
                </div>                
            </b-col>

            <b-col>
                <p class="text-center">Поле соперника</p>
                <table :class="enemyBoardClassList">
                    <tr v-for="(squaresRow, rowIndex) in enemySquares" :key="rowIndex">
                        <td v-for="(square, colIndex) in squaresRow"
                            :key="`square-${rowIndex}-${colIndex}`"
                            @click="fire(rowIndex, colIndex)"
                            :class="getClassListForSquare(square)"
                        ></td>
                    </tr>
                </table>
            </b-col>
        </b-row>
        
    </b-container>        
</template>

<style scoped>
    .game {
        height: 100%;
        padding-top: 10px;
    }
    #dock .ship {
        margin: 0.8em;
    }

    .board {
        border-collapse: collapse;
        table-layout: fixed;
        display: inline-block;
    }
    .board td {
        width: 2.5rem;
        height: 2.5rem;
        border: 1px solid gray;
        position: relative;
        padding: 0;
        box-sizing: border-box;
    }
    .board.active td {
        cursor: pointer;
    }
    .board.disabled td {
        cursor: not-allowed;
        background-color: gainsboro;
    }
    .board.active td.hit,
    .board.active td.miss {
        cursor: not-allowed;
    }
    td.hit:after {
        content: "\274C";
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        font-size: 2rem;
    }    
    td.miss:after {
        content: "\2022";
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        font-size: 2rem;
    }    
</style>

<script>
import Ship from "@/components/Ship";

export default {
    props: [ "match" ],
    components: {
        "ship": Ship
    },
    data() {
        return {
            squares: [],
            enemySquares: [],
            ships: [
                { id: "ship-0", length: 1, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false },
                { id: "ship-1", length: 1, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false },
                { id: "ship-2", length: 1, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false },
                { id: "ship-3", length: 1, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false },
                { id: "ship-4", length: 2, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false },
                { id: "ship-5", length: 2, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false },
                { id: "ship-6", length: 2, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false },
                { id: "ship-7", length: 3, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false },
                { id: "ship-8", length: 3, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false },
                { id: "ship-9", length: 4, horizontal: true, launched: false, zIndex: 2, warning: false, row: 0, col: 0, disabled: false }
            ],
            draggedShip: null,
            selectedIndex: 0,
            matchInfo: this.match
        }
    },    
    computed: {
        launchedShips() {
            return this.ships.filter(x => x.launched);
        },
        notLaunchedShips() {
            return this.ships.filter(x => !x.launched);
        },
        enemyBoardClassList() {
            if (this.enemy == null || !this.enemy.connected || !this.myTurn) {
                return "board disabled"
            }
            return "board active";
        },
        myId() {
            return this.$store.state.myId;
        },
        myTurn() {
            return this.matchInfo.whoseTurn === this.myId;
        },
        enemy() {
            return this.matchInfo.players.find(x => x.id !== this.myId);
        },
        player() {
            return this.matchInfo.players.find(x => x.id === this.myId);
        }
    },   
    methods: {
        showMessage(message, positive) {
            this.$bvToast.toast(message, {
                noCloseButton: true,
                variant: positive ? "success" : "danger",
                solid: true,
            });
        },
        onUpdateState(match) {
            this.matchInfo = match;
        },
        fire(row, col) {
            if (this.myTurn) {
                const targetSquare = this.enemySquares[row][col];
                if (targetSquare.boom) return;

                targetSquare.boom = true;
                this.$gameHub.fire({ row, col }).then(hit => {
                    if (hit) {
                        targetSquare.taken = true;
                        this.showMessage("Попадание", true);
                    }
                    else {
                        this.showMessage("Промах", false);
                    }
                })
                .catch(err => {
                    console.error(`Failed to fire enemy: ${err}`);
                })
            } 
        },
        onGetFire(coords) {
            const targetSquare = this.squares[coords.row][coords.col];
            targetSquare.boom = true;
            if (targetSquare.taken) {
                this.showMessage("Ваш корабль подбит", false);
            }
            else {
                this.showMessage("Соперник промахнулся", true);
            }
        },
        ready() {
            if (this.notLaunchedShips.length > 0)
                this.showMessage("Сначала расставьте корабли", false);                
            else {
                this.ships.forEach(x => x.disabled = true);
                this.$gameHub.playerReady(this.squares);
            }                
        },        
        getClassListForSquare(square) {
            if (square.boom) {
                return square.taken ? "hit" : "miss";
            }
            return "";
        },

        getShipFromSquare(row, col) {
            return this.launchedShips.find(x => x.row === row && x.col === col);
        },
        isShipInSquare(row, col) {
            return this.launchedShips.some(x => x.row === row && x.col === col);
        },
        selectIndex(e) {
            this.selectedIndex = parseInt(e.target.dataset.index);
        },
        dragStart(e, ship) {
            this.draggedShip = ship;
            if (ship.launched) {
                this.revealSquares(ship);
                // chrome bug workaround
                setTimeout(() => { ship.zIndex = 0; }, 10);
            }            
        },
        revealSquares(ship) {
            const shipSquares = this.getShipSquares(ship);
            shipSquares.forEach(square => {
                square.taken = false;
            });
        },
        getShipSquares(ship) {
            let shipSquares = [];
            const shipLength = ship.length;

            if (ship.horizontal) {
                const start = ship.col;
                const end = start + shipLength - 1;

                for(let i = start; i <= end; i++) {
                    const square = this.getSquare(ship.row, i);
                    shipSquares.push(square);
                }
            }
            else {
                const start = ship.row;
                const end = start + shipLength - 1;

                for(let i = start; i <= end; i++) {
                    const square = this.getSquare(i, ship.col);
                    shipSquares.push(square);
                }
            }

            return shipSquares;
        },
        getSquare(row, col) {
            const squaresRow = this.squares[row];
            if (squaresRow) {
                return squaresRow[col];
            }
            return null;
        },
        dragEnd() {
            this.draggedShip.zIndex = 2;
            if (this.draggedShip.launched) {
                this.takeSquares(this.draggedShip);
            }
            this.draggedShip = null;
        },
        takeSquares(ship) {
            const shipSquares = this.getShipSquares(ship);
            shipSquares.forEach(square => {
                square.taken = true;
            });
        },
        rotate(e, ship) {
            if (ship.launched) {
                const shipLength = ship.length;
                let isHorizontal = ship.horizontal;
                const isHorizontalNew = !isHorizontal;              

                const start = isHorizontalNew === true ? ship.col : ship.row;

                this.revealSquares(ship);
                if (this.isShipOnBoard(start, shipLength) && this.squaresAvailable(ship.row, ship.col, isHorizontalNew, shipLength)) {
                    
                    ship.horizontal = isHorizontalNew;
                }
                else {
                    ship.warning = true;
                    setTimeout(() => { ship.warning = false }, 1000);
                }
                this.takeSquares(ship);
            }
        },
        drop(e, overRow, overColumn) {
            if (this.matchInfo.started) return false;
            
            let start;
            if (this.draggedShip.horizontal) {
                start = overColumn - this.selectedIndex;
            }
            else {
                start = overRow - this.selectedIndex;
            }           

            if (this.isShipOnBoard(start, this.draggedShip.length)) {
                let rowStart;
                let colStart;
                if (this.draggedShip.horizontal) {
                    rowStart = overRow;
                    colStart = start;
                }
                else {
                    rowStart = start;
                    colStart = overColumn;
                }                

                if (this.squaresAvailable(rowStart, colStart, this.draggedShip.horizontal, this.draggedShip.length)) {
                    this.draggedShip.row = rowStart;
                    this.draggedShip.col = colStart;
                    this.draggedShip.launched = true;
                }            
            }
        },
        isShipOnBoard(start, shipLength) {
            const end = start + shipLength - 1;
            return (start >= 0 && end <= 9);
        },
        squaresAvailable(rowStart, colStart, isShipHorizontal, shipLength) {
            let squaresToCheck = [];

            if (isShipHorizontal) {
                const row = rowStart;
                const left = colStart - 1;
                const right = left + shipLength + 1;           

                for(let col = left; col <= right; col++) {
                    this.addSquareIfExists(squaresToCheck, row + 1, col);
                    this.addSquareIfExists(squaresToCheck, row, col);
                    this.addSquareIfExists(squaresToCheck, row - 1, col);                
                }
            }
            else {
                const col = colStart;
                const top = rowStart - 1;
                const bottom = top + shipLength + 1;

                for(let row = top; row <= bottom; row++) {
                    this.addSquareIfExists(squaresToCheck, row , col - 1);
                    this.addSquareIfExists(squaresToCheck, row, col);
                    this.addSquareIfExists(squaresToCheck, row, col + 1);
                }
            }
            
            return squaresToCheck.every((square) => !square.taken);
        },
        addSquareIfExists(squaresArray, row, col) {
            const square = this.getSquare(row, col);
            if (square) squaresArray.push(square);
        },
        createBoard() {
            let board = [];
            for (let row = 0; row < 10; row++) {
                let squaresRow = [];
                for (let col = 0; col < 10; col++) {
                    squaresRow.push({ taken: false, boom: false });
                }
                board.push(squaresRow);
            }
            return board;
        }
    },
    created() {
        this.squares = this.createBoard();
        this.enemySquares = this.createBoard();

        this.$gameHub.$on("get-fire", this.onGetFire);
        this.$gameHub.$on("update-state", this.onUpdateState);
    },
    beforeDestroy() {
        this.$gameHub.$off("get-fire", this.onGetFire);
        this.$gameHub.$off("update-state", this.onUpdateState);
    }
}
</script>