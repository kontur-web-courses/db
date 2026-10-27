using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {

        public GameTurnEntity(Guid gameId, int turnIndex, Guid winnerId, IReadOnlyList<PlayerTurnDecision> decisions)
            : this(Guid.NewGuid(), gameId, turnIndex, winnerId, decisions)
        {
        }

        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, Guid winnerId, IReadOnlyList<PlayerTurnDecision> decisions)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            WinnerId = winnerId;
            Decisions = decisions;
        }

        public Guid Id { get; private set; }

        [BsonElement]
        public Guid GameId { get; }

        [BsonElement]
        public int TurnIndex { get; }

        [BsonElement]
        public Guid WinnerId { get; }

        [BsonElement]
        public IReadOnlyList<PlayerTurnDecision> Decisions { get; }
    }

    public class PlayerTurnDecision
    {
        [BsonConstructor]
        public PlayerTurnDecision(Guid userId, string userName, PlayerDecision decision)
        {
            UserId = userId;
            UserName = userName;
            Decision = decision;
        }

        [BsonElement]
        public Guid UserId { get; }

        [BsonElement]
        public string UserName { get; }

        [BsonElement]
        public PlayerDecision Decision { get; }
    }
}