using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnNumber, PlayerDecision player1Decision, PlayerDecision player2Decision, Guid winnerId)
        {
            Id = id;
            GameId = gameId;
            TurnNumber = turnNumber;
            Player1Decision = player1Decision;
            Player2Decision = player2Decision;
            WinnerId = winnerId;
        }

        [BsonElement]
        public Guid Id { get; }

        [BsonElement]
        public Guid GameId { get; }

        [BsonElement]
        public int TurnNumber { get; }

        [BsonElement]
        public PlayerDecision Player1Decision { get; }

        [BsonElement]
        public PlayerDecision Player2Decision { get; }

        [BsonElement]
        public Guid WinnerId { get; }
    }
}