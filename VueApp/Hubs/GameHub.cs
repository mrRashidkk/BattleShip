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
            string userName = Context.User.Identity.Name;
            string matchId = Guid.NewGuid().ToString();           

            Match match = new Match(matchId);
            match.AddPlayer(userName);
            MatchManager.Matches.Add(match);

            await Groups.AddToGroupAsync(userName, matchId);

            return matchId;
        }

        public async Task<bool> JoinMatch(string matchId)
        {
            string userName = Context.User.Identity.Name;

            await Groups.AddToGroupAsync(userName, matchId);

            Match match = MatchManager.Matches.FirstOrDefault(x => x.Id == matchId);

            match.AddPlayer(userName);

            return true;
        }

        public async Task PlayerReady()
        {
            string userName = Context.User.Identity.Name;
            Match match = MatchManager.GetMatch(userName);
            var currentPlayer = match.Players.FirstOrDefault(x => x.Name == userName);
            var enemy = match.Players.FirstOrDefault(x => x.Name != userName);
            currentPlayer.Ready = true;
            if (enemy.Ready)
            {
                var test = Clients.Group(match.Id);
                await Clients.Group(match.Id).SendAsync("GameStarted");
            }
                
        }

        public async Task<bool> Fire(Coords coords)
        {
            string userName = Context.User.Identity.Name;

            Match match = MatchManager.GetMatch(userName);
            var enemy = match.Players.FirstOrDefault(x => x.Name != userName);
            bool hit = enemy.GetFire(coords.Row, coords.Col);

            await Clients.User(enemy.Name).SendAsync("GetFire", coords);

            return hit;
        }

        public class Coords
        {
            public int Row { get; set; }
            public int Col { get; set; }
        }
    }
}
