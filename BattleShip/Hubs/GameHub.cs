using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using BattleShip.Entities;
using BattleShip.Models;
using BattleShip.Interfaces;
using BattleShip.Common;

namespace BattleShip.Hubs
{
    public class GameHub : Hub
    {
        private readonly IPlayerManager _playerManager;
        private readonly IMatchManager _matchManager;
        public GameHub(IPlayerManager playerManager, IMatchManager matchManager)
        {
            _playerManager = playerManager;
            _matchManager = matchManager;
        }
        public override async Task OnConnectedAsync()
        {
            _playerManager.Add(Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = _playerManager.Get(Context.ConnectionId);

            var match = _matchManager.GetMatchForPlayer(player.Id);
            if (match != null)
            {
                match.RemovePlayer(player);
                _playerManager.Delete(player);

                if (!match.Players.Any())
                {
                    _matchManager.Remove(match);                    
                }
                else
                {
                    match.State = MatchState.OnePlayerDisconnected;
                    await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToDto(match));
                }
            }
        }

        public async Task LeaveMatch(string matchId)
        {
            try
            {
                matchId = matchId.Trim();

                var player = _playerManager.Get(Context.ConnectionId);
                player.Reset();

                var match = _matchManager.GetById(matchId);
                match.RemovePlayer(player);

                if (!match.Players.Any())
                {
                    _matchManager.Remove(match);
                }
                else
                {
                    match.State = MatchState.OnePlayerDisconnected;
                    await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToDto(match));
                }                
            }
            catch (GameException e)
            {
                await Clients.Caller.SendAsync("Error", e.Message);
                throw;
            }
        }

        public async Task<MatchModel> CreateMatch()
        {
            try
            {
                var player = _playerManager.Get(Context.ConnectionId);
                player.Reset();

                Match match = _matchManager.Add(Guid.NewGuid().ToString());
                match.AddPlayer(player);

                await Groups.AddToGroupAsync(player.Id, match.Id);

                return _matchManager.MapToDto(match);
            }
            catch(GameException e)
            {
                await Clients.Caller.SendAsync("Error", e.Message);
                throw;
            }            
        }

        public async Task<MatchModel> JoinMatch(string matchId)
        {
            try
            {
                matchId = matchId.Trim();

                var player = _playerManager.Get(Context.ConnectionId);
                player.Reset();

                Match match = _matchManager.GetById(matchId);
                match.AddPlayer(player);
                match.State = MatchState.LaunchingShips;

                await Groups.AddToGroupAsync(player.Id, matchId);

                await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToDto(match));

                return _matchManager.MapToDto(match);
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
                var player = _playerManager.Get(Context.ConnectionId);
                player.Ready = true;
                player.SetBoard(board);

                Match match = _matchManager.GetMatchForPlayer(player.Id);
                var enemy = match.Players.FirstOrDefault(x => x.Id != player.Id);

                if (enemy?.Ready == true)
                {
                    match.State = MatchState.InProgress;
                    match.SetWhoseTurn();
                }

                await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToDto(match));
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
                var player = _playerManager.Get(Context.ConnectionId);

                Match match = _matchManager.GetMatchForPlayer(player.Id);
                var enemy = match.Players.FirstOrDefault(x => x.Id != player.Id);
                bool hit = enemy.GetFire(coords.Row, coords.Col);

                await Clients.Client(enemy.Id).SendAsync("GetFire", coords);

                match.WhoseTurn = enemy.Id;
                if (enemy.HP == 0)
                {
                    match.State = MatchState.Finished;
                    match.Winner = player.Id;
                };

                await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToDto(match));

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
