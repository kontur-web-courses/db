using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public GameTurnEntity()
        {
            Id = Guid.Empty;
            Timestamp = DateTime.UtcNow;
        }

        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, Guid player1Id, PlayerDecision player1Decision, Guid player2Id, PlayerDecision player2Decision, Guid? winnerId, DateTime timestamp)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            Player1Id = player1Id;
            Player1Decision = player1Decision;
            Player2Id = player2Id;
            Player2Decision = player2Decision;
            WinnerId = winnerId;
            Timestamp = timestamp;
        }

        public Guid Id { get; set; }

        [BsonElement]
        public Guid GameId { get; set; }

        [BsonElement]
        public int TurnIndex { get; set; }

        [BsonElement]
        public Guid Player1Id { get; set; }

        [BsonElement]
        public PlayerDecision Player1Decision { get; set; }

        [BsonElement]
        public Guid Player2Id { get; set; }

        [BsonElement]
        public PlayerDecision Player2Decision { get; set; }

        [BsonElement]
        public Guid? WinnerId { get; set; }

        [BsonElement]
        public DateTime Timestamp { get; set; }
    }
}