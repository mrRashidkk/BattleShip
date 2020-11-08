import "./style.css";

document.addEventListener("DOMContentLoaded", () => {
    const board = document.querySelector("#board");
    const ships = document.querySelectorAll(".ship");    

    function createBoard() {
        let squares = []
        for(let i = 0; i < 10; i++) {
            let row = document.createElement("tr");
            for(let j = 0; j < 10; j++) {
                let square = document.createElement("td");
                square.dataset.row = i;
                square.dataset.col = j;
                square.dataset.taken = false;
                row.append(square);
                squares.push(square);
            }
            board.append(row);
        }
        return squares;
    }

    const userSquares = createBoard();

    ships.forEach(ship => ship.addEventListener("dragstart", dragStart));
    ships.forEach(square => square.addEventListener("dragend", dragEnd));
    ships.forEach(ship => ship.addEventListener("dblclick", rotate));
    userSquares.forEach(square => square.addEventListener("dragover", dragOver));
    userSquares.forEach(square => square.addEventListener("dragenter", dragEnter));
    userSquares.forEach(square => square.addEventListener("drop", dragDrop));    
    
    let draggedShip;
    let draggedShipLength;
    let isDraggedShipHorizontal;
    let isDraggedShipLaunched;
    let selectedIndex;

    ships.forEach(ship => ship.addEventListener("mousedown", (e) => {
        selectedIndex = parseInt(e.target.dataset.index);
    }));   

    function rotate() {
        if (this.dataset.launched === "true") {
            const shipLength = parseInt(this.dataset.length);
            let isHorizontal = (this.dataset.horizontal === "true");
            const isHorizontalNew = !isHorizontal;
            const startSquare = this.parentElement;
            const start = isHorizontalNew === true ? parseInt(startSquare.dataset.col) : parseInt(startSquare.dataset.row);

            revealSquares(this);
            if (isShipOnBoard(start, shipLength) && squaresAvailable(startSquare, isHorizontalNew, shipLength)) {
                this.dataset.horizontal = isHorizontalNew;
                this.classList.toggle("horizontal");                
            }
            takeSquares(this);
        }
    }

    function dragStart() { 
        draggedShip = this;
        draggedShipLength = parseInt(draggedShip.dataset.length);
        isDraggedShipHorizontal = (draggedShip.dataset.horizontal === "true");
        isDraggedShipLaunched = (draggedShip.dataset.launched === "true");
        if (isDraggedShipLaunched) {
            // chrome bug workaround
            setTimeout(() => { draggedShip.style.zIndex = 0; }, 10);
        }
        revealSquares(draggedShip);
    }

    function dragOver(e) {
        e.preventDefault();
    }

    function dragEnter(e) {
        e.preventDefault();
    }    

    function dragDrop(e) {
        const overSquare = e.target;
        const overColumn = parseInt(overSquare.dataset.col);
        const overRow = parseInt(overSquare.dataset.row);

        let start;
        if (isDraggedShipHorizontal) {
            start = overColumn - selectedIndex;
        }
        else {
            start = overRow - selectedIndex;
        }

        if (isShipOnBoard(start, draggedShipLength)) {
            const startSquare = isDraggedShipHorizontal ? document.querySelector(`[data-row='${overRow}'][data-col='${start}']`) : 
                document.querySelector(`[data-row='${start}'][data-col='${overColumn}']`);

            if (squaresAvailable(startSquare, isDraggedShipHorizontal, draggedShipLength)) {
                startSquare.append(draggedShip);
                draggedShip.style.position = "absolute";            
                draggedShip.dataset.launched = true;
            }            
        }        
    }

    function dragEnd(e) {
        e.preventDefault();
        draggedShip.style.zIndex = 2;
        takeSquares(draggedShip);
    }
    
    function isShipOnBoard(start, shipLength) {
        const end = start + shipLength - 1;
        return (start >= 0 && end <= 9);
    }

    function squaresAvailable(startSquare, isShipHorizontal, shipLength) {
        let squaresToCheck = [];

        if (isShipHorizontal) {
            const row = parseInt(startSquare.dataset.row);
            const left = parseInt(startSquare.dataset.col) - 1;
            const right = left + shipLength + 1;           

            for(let col = left; col <= right; col++) {
                addSquareIfExists(squaresToCheck, row + 1, col);
                addSquareIfExists(squaresToCheck, row, col);
                addSquareIfExists(squaresToCheck, row - 1, col);                
            }
        }
        else {
            const col = parseInt(startSquare.dataset.col);
            const top = parseInt(startSquare.dataset.row) - 1;
            const bottom = top + shipLength + 1;

            for(let row = top; row <= bottom; row++) {
                addSquareIfExists(squaresToCheck, row , col - 1);
                addSquareIfExists(squaresToCheck, row, col);
                addSquareIfExists(squaresToCheck, row, col + 1);
            }
        }
        
        return squaresToCheck.every((square) => square.dataset.taken === "false");
    }

    function revealSquares(ship) {
        const shipSquares = getShipSquares(ship);
        shipSquares.forEach(square => {
            square.dataset.taken = false;
        });
    }

    function takeSquares(ship) {
        const shipSquares = getShipSquares(ship);
        shipSquares.forEach(square => {
            square.dataset.taken = true;
        });        
    }

    function getShipSquares(ship) {
        let shipSquares = [];
        const shipLength = parseInt(ship.dataset.length);
        const startSquare = ship.parentElement;

        if (ship.dataset.horizontal === "true") {
            const start = parseInt(startSquare.dataset.col);
            const end = start + shipLength - 1;

            for(let i = start; i <= end; i++) {
                let square = document.querySelector(`[data-row='${startSquare.dataset.row}'][data-col='${i}']`);
                shipSquares.push(square);
            }
        }
        else {
            const start = parseInt(startSquare.dataset.row);
            const end = start + shipLength - 1;

            for(let i = start; i <= end; i++) {
                let square = document.querySelector(`[data-row='${i}'][data-col='${startSquare.dataset.col}']`);
                shipSquares.push(square);
            }
        }

        return shipSquares;
    }    

    function getShipNearestSquares(ship) {
        let shipNearestSquares = [];
        const shipLength = parseInt(ship.dataset.length);
        const startSquare = ship.parentElement;

        if (ship.dataset.horizontal === "true") {
            const row = parseInt(startSquare.dataset.row);
            const firstCol = parseInt(startSquare.dataset.col);
            const lastCol = firstCol + shipLength - 1;
            const left = firstCol - 1;
            const right = lastCol + 1;

            addSquareIfExists(shipNearestSquares, row, left);
            addSquareIfExists(shipNearestSquares, row, right);

            for(let col = firstCol - 1; col <= lastCol + 1; col++) {
                addSquareIfExists(shipNearestSquares, row - 1, col);
                addSquareIfExists(shipNearestSquares, row + 1, col);
            }
        }
        else {
            const col = parseInt(startSquare.dataset.col);
            const firstRow = parseInt(startSquare.dataset.row);
            const lastRow = firstRow + shipLength - 1;
            const top = firstRow - 1;
            const bottom = lastRow + 1;

            addSquareIfExists(shipNearestSquares, top, col);
            addSquareIfExists(shipNearestSquares, bottom, col);

            for(let row = firstRow - 1; row <= lastRow + 1; row++) {
                addSquareIfExists(shipNearestSquares, row, col - 1);
                addSquareIfExists(shipNearestSquares, row, col + 1);
            }
        }

        return shipNearestSquares;
    }

    function addSquareIfExists(squaresArray, row, col) {
        const square = document.querySelector(`[data-row='${row}'][data-col='${col}']`);
        if (square) squaresArray.push(square);
    }
     
});