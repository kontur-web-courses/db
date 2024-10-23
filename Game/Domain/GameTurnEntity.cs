using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public GameTurnEntity(Guid gameId, Guid winnerId, PlayerDecision firstPlayerDecision, PlayerDecision secondPlayerDecision, int turnIndex)
            : this(Guid.Empty, gameId, winnerId, firstPlayerDecision, secondPlayerDecision, turnIndex) {}

        [BsonConstructor]
        private GameTurnEntity(Guid id, Guid gameId, Guid winnerId, PlayerDecision firstPlayerDecision, PlayerDecision secondPlayerDecision, int turnIndex)
        {
            Id = id;
            GameId = gameId;
            WinnerId = winnerId;
            FirstPlayerDecision = firstPlayerDecision;
            SecondPlayerDecision = secondPlayerDecision;
            TurnIndex = turnIndex;
        }

        [BsonElement]
        public Guid Id { get; private set; }

        [BsonElement]
        public Guid GameId { get; }

        [BsonElement]
        public Guid WinnerId { get; }

        [BsonElement]
        public PlayerDecision FirstPlayerDecision { get; }

        [BsonElement]
        public PlayerDecision SecondPlayerDecision { get; }
        
        [BsonElement]
        public int TurnIndex { get; }
    }
}
