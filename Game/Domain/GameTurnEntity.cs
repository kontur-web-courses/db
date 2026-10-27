using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public int TurnIndex { get; set; }
        public List<PlayerTurnInfo> PlayerTurns { get; set; } = new();
        public Guid? WinnerId { get; set; }
        public string WinnerName { get; set; }

        public GameTurnEntity()
        {
        }

        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, List<PlayerTurnInfo> playerTurns, Guid? winnerId, string winnerName)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            PlayerTurns = playerTurns;
            WinnerId = winnerId;
            WinnerName = winnerName;
        }
    }
}
