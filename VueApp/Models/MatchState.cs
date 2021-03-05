using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueApp.Models
{
    public enum MatchState
    {
        AwaitingOpponent,
        LaunchingShips,
        InProgress,
        OnePlayerDisconnected,
        Finished
    }
}
