using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        //TODO: Придумать какие свойства должны быть в этом классе, чтобы сохранять всю информацию о закончившемся туре.
        private Guid Id;
        private Dictionary<Player, PlayerDecision?> decisions;
        private Guid winnerId;
        private int turnNumber;
        private Guid gameId;
        public GameTurnEntity(Guid id, List<Player> players, Guid winnerId, int turnNumber, Guid gameId)
        {
            this.winnerId = winnerId;
            this.turnNumber = turnNumber;
            this.gameId = gameId;
            Id = id;
            foreach (var player in players)
            {
                decisions[player] = player.Decision;
            }
        }
    }
}