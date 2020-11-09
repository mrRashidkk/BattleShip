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
            MatchManager.Matches.Add(new Match(matchId));

            await Groups.AddToGroupAsync(userName, matchId);

            return matchId;
        }

        public async Task JoinMatch(string matchId)
        {
            string userName = Context.User.Identity.Name;

            Match match = MatchManager.Matches.FirstOrDefault(x => x.Id == matchId);

            match.AddPlayer(userName);
        }
    }
}
