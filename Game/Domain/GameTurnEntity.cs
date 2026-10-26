using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public int TurnNumber { get; set; }
        public List<Player> Players { get; set; }
        public Guid WinnerId { get; set; }
    }
}
