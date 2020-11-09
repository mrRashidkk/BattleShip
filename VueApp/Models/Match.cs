﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueApp.Models
{
    public class Match
    {
        private PlayerInfo[] players = new PlayerInfo[2];

        public readonly string Id;
        public string WhoseTurn { get; set; }
        
        public bool Started { get; set; }
        public bool GameOver { get; set; }
        public string Winner { get; set; }

        public Match(string id)
        {
            Id = id;
        }

        public void AddPlayer(string userName)
        {
            if (Started)
                throw new Exception("Cannot add player to started match");

            if (GameOver)
                throw new Exception("Cannot add player to finished match");

            int count = players.Count();

            switch(count)
            {
                case 0:
                    players[0] = new PlayerInfo(userName);
                    break;
                case 1:
                    if (players[0].Name == userName)
                        throw new ArgumentException("Cannot add the same player twice");
                    players[1] = new PlayerInfo(userName);
                    break;
                case 2:
                    throw new Exception("Only 2 players can play!");
            }            
        }
    }
}
