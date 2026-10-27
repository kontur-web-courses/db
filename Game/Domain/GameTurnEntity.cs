using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        //TODO: Придумать какие свойства должны быть в этом классе, чтобы сохранять всю информацию о закончившемся туре.
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public int TurnNumber { get; set; }
        public List<Player> Players { get; set; }
        public Guid WinnerId { get; set; }
    }
}