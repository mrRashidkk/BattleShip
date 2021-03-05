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
            PlayerManager.Add(Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = PlayerManager.Get(Context.ConnectionId);
            player.Connected = false;

            var match = MatchManager.GetMatchForPlayer(player.Id);
            if (match != null)
            {
                if (match.Players.All(x => !x.Connected))
                {
                    var players = match.Players.ToList();

                    MatchManager.Delete(match);

                    foreach (var p in players)
                    {
                        if (!MatchManager.PlayerInAnyMatch(p.Id))
                            PlayerManager.Delete(p);
                    }
                }
                else
                {
                    await Clients.Group(match.Id).SendAsync("UpdateState", MatchManager.MapToDto(match));
                }
            }
        }

        public async Task<MatchDto> CreateMatch()
        {
            try
            {
                var player = PlayerManager.Get(Context.ConnectionId);
                player.Reset();
                player.Connected = true;

                Match match = MatchManager.Add(Guid.NewGuid().ToString());
                match.AddPlayer(player);

                await Groups.AddToGroupAsync(player.Id, match.Id);

                return MatchManager.MapToDto(match);
            }
            catch(GameException e)
            {
                await Clients.Caller.SendAsync("Error", e.Message);
                throw;
            }            
        }

        public async Task<MatchDto> JoinMatch(string matchId)
        {
            try
            {
                matchId = matchId.Trim();

                var player = PlayerManager.Get(Context.ConnectionId);
                player.Reset();
                player.Connected = true;

                Match match = MatchManager.GetById(matchId);
                match.AddPlayer(player);

                await Groups.AddToGroupAsync(player.Id, matchId);

                await Clients.Group(match.Id).SendAsync("UpdateState", MatchManager.MapToDto(match));

                return MatchManager.MapToDto(match);
            }
            catch(GameException e)
            {
                await Clients.Caller.SendAsync("Error", e.Message);
                throw;
            }            
        }

        public async Task LeaveMatch(string matchId)
        {
            try
            {
                matchId = matchId.Trim();

                var player = PlayerManager.Get(Context.ConnectionId);
                player.Reset();
                player.Connected = false;

                var match = MatchManager.GetById(matchId);

                await Clients.Group(matchId).SendAsync("UpdateState", MatchManager.MapToDto(match));
            }
            catch (GameException e)
            {
                await Clients.Caller.SendAsync("Error", e.Message);
                throw;
            }
        }

        public async Task PlayerReady(Square[][] board)
        {
            try
            {
                var player = PlayerManager.Get(Context.ConnectionId);
                player.Ready = true;
                player.SetBoard(board);

                Match match = MatchManager.GetMatchForPlayer(player.Id);
                var enemy = match.Players.FirstOrDefault(x => x.Id != player.Id);

                if (enemy?.Ready == true)
                {
                    match.Started = true;
                    int index = new Random().Next(2);
                    match.WhoseTurn = match.Players[index].Id;
                }

                await Clients.Group(match.Id).SendAsync("UpdateState", MatchManager.MapToDto(match));
            }
            catch(GameException e)
            {
                await Clients.Caller.SendAsync("Error", e.Message);
                throw;
            }
        }

        public async Task<bool> Fire(Coords coords)
        {
            try
            {
                var player = PlayerManager.Get(Context.ConnectionId);

                Match match = MatchManager.GetMatchForPlayer(player.Id);
                var enemy = match.Players.FirstOrDefault(x => x.Id != player.Id);
                bool hit = enemy.GetFire(coords.Row, coords.Col);

                await Clients.Client(enemy.Id).SendAsync("GetFire", coords);

                match.WhoseTurn = enemy.Id;
                if (enemy.HP == 0)
                {
                    match.GameOver = true;
                    match.Winner = player.Id;
                };

                await Clients.Group(match.Id).SendAsync("UpdateState", MatchManager.MapToDto(match));

                return hit;
            }
            catch(GameException e)
            {
                await Clients.Caller.SendAsync("Error", e.Message);
                throw;
            }            
        }                
    }
}
