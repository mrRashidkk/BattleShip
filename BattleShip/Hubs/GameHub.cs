using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using BattleShip.Entities;
using BattleShip.Models;
using BattleShip.Interfaces;

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
            _playerManager.Add(new Player(Context.ConnectionId));
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = _playerManager.Get(Context.ConnectionId);

            var match = _matchManager.GetMatchForPlayer(player.Id);
            if (match != null)
            {
                match.RemovePlayer(player.Id);

                if (!match.Players.Any())
                    _matchManager.Remove(match.Id);
                else
                    await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToModel(match));
            }

            _playerManager.Remove(player.Id);
        }

        public async Task LeaveMatch(string matchId)
        {
            try
            {
                var player = _playerManager.Get(Context.ConnectionId);
                player.Reset();

                matchId = matchId.Trim();
                var match = _matchManager.GetById(matchId);
                match.RemovePlayer(player.Id);

                if (!match.Players.Any())
                    _matchManager.Remove(match.Id);
                else
                    await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToModel(match));
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

                Match match = new Match(Guid.NewGuid().ToString());
                _matchManager.Add(match);
                match.AddPlayer(player);

                await Groups.AddToGroupAsync(player.Id, match.Id);

                return _matchManager.MapToModel(match);
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

                await Groups.AddToGroupAsync(player.Id, matchId);

                await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToModel(match));

                return _matchManager.MapToModel(match);
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
                player.SetBoard(board);

                Match match = _matchManager.GetMatchForPlayer(player.Id);
                var enemy = match.Players.FirstOrDefault(x => x.Id != player.Id);

                if (enemy?.Ready == true)
                    match.Start();

                await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToModel(match));
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

                if (enemy.HP == 0)
                    match.Finish(player.Id);
                else
                    match.SwitchTurn();

                await Clients.Group(match.Id).SendAsync("UpdateState", _matchManager.MapToModel(match));

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
