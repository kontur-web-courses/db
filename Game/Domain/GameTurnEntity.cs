using System;
using System.Collections.Generic;


namespace Game.Domain
{
    public class GameTurnEntity
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public int TurnIndex { get; set; }
        public Guid WinnerId { get; set; }

        public List<PlayerTurnInfo> Players { get; set; }
    }
    
    public class PlayerTurnInfo
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public PlayerDecision Decision { get; set; }
    }
}