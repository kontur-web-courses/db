using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public Guid Id { get; set; }

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

        [BsonConstructor]
        public GameTurnEntity(Guid gameId, Guid winnerId, PlayerDecision firstPlayerDecision, PlayerDecision secondPlayerDecision, int turnIndex)
        {
            GameId = gameId;
            WinnerId = winnerId;
            FirstPlayerDecision = firstPlayerDecision;
            SecondPlayerDecision = secondPlayerDecision;
            TurnIndex = turnIndex;
        }
    }
}