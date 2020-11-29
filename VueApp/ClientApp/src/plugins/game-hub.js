import { HubConnectionBuilder } from '@microsoft/signalr';

export default {
    install (Vue) {
        const connection = new HubConnectionBuilder()
            .withUrl("/game-hub")
            .build();

        const gameHub = new Vue(); 
        Vue.prototype.$gameHub = gameHub;
        
        gameHub.connectionId = connection.connectionId;
        
        gameHub.createMatch = () => {
            return connection.invoke('CreateMatch');
        }

        gameHub.joinMatch = (matchId) => {
            return connection.invoke('JoinMatch', matchId);
        }

        gameHub.playerReady = (board) => {
            return connection.invoke('PlayerReady', board);
        }

        gameHub.fire = (coords) => {
            return connection.invoke('Fire', coords);
        }       

        connection.on('GetFire', (coords) => {
            gameHub.$emit('get-fire', coords);
        })

        connection.on('UpdateState', (match) => {
            gameHub.$emit('update-state', match);
        })

        connection.on('Error', (message) => {
            console.log(`Server error: ${message}`);
            gameHub.$emit('error', message);
        })

        let startedPromise = null;

        function start () {
            startedPromise = connection.start()
            .then(() => {
                gameHub.$emit('connection-started', connection.connectionId);
            })
            .catch(err => {
                console.error('Failed to connect with hub', err);
                return new Promise((resolve, reject) => 
                    setTimeout(() => start().then(resolve).catch(reject), 5000));
            })
            return startedPromise;
        }

        connection.onclose(() => start());

        start();
    }
}