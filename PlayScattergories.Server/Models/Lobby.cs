﻿using PlayScattergories.Server.Models.Game;

namespace PlayScattergories.Server.Models
{
    public class Lobby
    {
        public Lobby(string id)
        { 
            Id = id;
            GameState = new GameState();
            Players = new List<Player.Player>();
            IsActive = true;
            IsWaitingToStart = true;
            FailedToStart = false;
        }

        public string Id { get; set; }
        public GameState? GameState { get; set; }
        public List<Player.Player> Players { get; set; }
        public bool IsActive { get; set; }
        public bool IsWaitingToStart { get; set; }
        public bool FailedToStart { get; set; }
    }
}
