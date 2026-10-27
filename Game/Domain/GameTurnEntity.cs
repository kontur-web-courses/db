using System;
using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid winnerId, [CanBeNull] string winnerName, PlayerDecision? winnerDecision, int turnNumber)
        {
            Id = id;
            WinnerId = winnerId;
            WinnerName = winnerName;
            WinnerDecision = winnerDecision;
            TurnNumber = turnNumber;
            FinishedAt = DateTime.UtcNow;
            IsDraw = winnerId == Guid.Empty;
        }

        [BsonId]
        public Guid Id { get; }
        public Guid WinnerId { get; set; }
        [CanBeNull]
        public string WinnerName { get; set; }
        public PlayerDecision? WinnerDecision { get; set; }
        public PlayerDecision? OpponentDecision => WinnerDecision.HasValue ? OpponentDecision : null;
        public bool IsDraw { get; set; }
        public int TurnNumber { get; set; }
        public DateTime FinishedAt { get; set; }
    }
}