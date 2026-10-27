using System;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        //TODO: Придумать какие свойства должны быть в этом классе, чтобы сохранять всю информацию о закончившемся туре.
        
        public Guid Id { get; init; }
        public PlayerDecision FirstPlayerDecision { get; init; }
        public PlayerDecision SecondPlayerDecision { get; init; }
        public Guid WinnerId { get; init; }
        public Guid GameId { get; init; }
        public int TurnNumber { get; init; }
        
        public GameTurnEntity(PlayerDecision firstPlayerDecision, PlayerDecision secondPlayerDecision, Guid gameId, int turnNumber, Guid winnerId)
        {
            Id = Guid.NewGuid();
            FirstPlayerDecision = firstPlayerDecision;
            SecondPlayerDecision = secondPlayerDecision;
            GameId = gameId;
            TurnNumber = turnNumber;
            WinnerId = winnerId;
        }
    }
}