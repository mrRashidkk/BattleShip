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

            var match = MatchManager.GetMatchForPlayer(player.Id);
            if (match != null)
            {
                match.RemovePlayer(player);
                PlayerManager.Delete(player);

                if (!match.Players.Any())
                {
                    MatchManager.Remove(match);                    
                }
                else
                {
                    match.State = MatchState.OnePlayerDisconnected;
                    await Clients.Group(match.Id).SendAsync("UpdateState", MatchManager.MapToDto(match));
                }
            }
        }

        public async Task LeaveMatch(string matchId)
        {
            try
            {
                matchId = matchId.Trim();

                var player = PlayerManager.Get(Context.ConnectionId);
                player.Reset();

                var match = MatchManager.GetById(matchId);
                match.RemovePlayer(player);

                if (!match.Players.Any())
                {
                    MatchManager.Remove(match);
                }
                else
                {
                    match.State = MatchState.OnePlayerDisconnected;
                    await Clients.Group(match.Id).SendAsync("UpdateState", MatchManager.MapToDto(match));
                }                
            }
            catch (GameException e)
            {
                await Clients.Caller.SendAsync("Error", e.Message);
                throw;
            }
        }

        public async Task<MatchDto> CreateMatch()
        {
            try
            {
                var player = PlayerManager.Get(Context.ConnectionId);
                player.Reset();

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

                Match match = MatchManager.GetById(matchId);
                match.AddPlayer(player);
                match.State = MatchState.LaunchingShips;

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
                    match.State = MatchState.InProgress;
                    match.SetWhoseTurn();
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
                    match.State = MatchState.Finished;
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
