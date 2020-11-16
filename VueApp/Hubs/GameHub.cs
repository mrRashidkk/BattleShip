using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueApp.Models;

namespace VueApp.Hubs
{
    public class GameHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var id = Context.ConnectionId;
        }

        public async Task<string> CreateMatch()
        {
            string clientId = Context.ConnectionId;
            string matchId = Guid.NewGuid().ToString();

            Match match = new Match(matchId);
            match.AddPlayer(clientId);
            MatchManager.Matches.Add(match);

            await Groups.AddToGroupAsync(clientId, matchId);

            return matchId;
        }

        public async Task<bool> JoinMatch(string matchId)
        {
            string clientId = Context.ConnectionId;
            Match match = MatchManager.Matches.FirstOrDefault(x => x.Id == matchId);
            match.AddPlayer(clientId);
            await Groups.AddToGroupAsync(clientId, matchId);
            return true;
        }

        public async Task PlayerReady(Square[][] board)
        {
            string clientId = Context.ConnectionId;
            Match match = MatchManager.GetMatch(clientId);
            var currentPlayer = match.Players.FirstOrDefault(x => x.Id == clientId);
            var enemy = match.Players.FirstOrDefault(x => x.Id != clientId);
            currentPlayer.Ready = true;
            currentPlayer.SetBoard(board);
            if (enemy.Ready)
            {
                int rnd = new Random().Next(2);
                string whoseTurn = match.Players[rnd].Id;
                await Clients.Group(match.Id).SendAsync("GameStarted", whoseTurn);
            }
                
        }

        public async Task<bool> Fire(Coords coords)
        {
            string clientId = Context.ConnectionId;
            Match match = MatchManager.GetMatch(clientId);
            var enemy = match.Players.FirstOrDefault(x => x.Id != clientId);
            bool hit = enemy.GetFire(coords.Row, coords.Col);

            await Clients.Client(enemy.Id).SendAsync("GetFire", coords);

            return hit;
        }

        public class Coords
        {
            public int Row { get; set; }
            public int Col { get; set; }
        }
    }
}
