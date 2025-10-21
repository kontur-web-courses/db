using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonId]
        public Guid Id { get; private set; }

        [BsonElement]
        public Guid GameId { get; private set; }

        [BsonElement]
        public int TurnIndex { get; private set; }

        [BsonElement]
        public DateTime FinishedAt { get; private set; }

        [BsonElement]
        public Guid WinnerId { get; private set; }

        [BsonElement]
        public IReadOnlyList<PlayerTurnDecision> PlayerDecisions { get; private set; }

        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, DateTime finishedAt, Guid winnerId, IReadOnlyList<PlayerTurnDecision> playerDecisions)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            FinishedAt = finishedAt;
            WinnerId = winnerId;
            PlayerDecisions = playerDecisions;
        }

        public GameTurnEntity(Guid gameId, int turnIndex, Guid winnerId, IReadOnlyList<PlayerTurnDecision> playerDecisions)
        {
            Id = Guid.NewGuid();
            GameId = gameId;
            TurnIndex = turnIndex;
            FinishedAt = DateTime.UtcNow;
            WinnerId = winnerId;
            PlayerDecisions = playerDecisions;
        }
    }

    public class PlayerTurnDecision
    {
        [BsonElement]
        public Guid PlayerId { get; private set; }

        [BsonElement]
        public string PlayerName { get; private set; }

        [BsonElement]
        public PlayerDecision Decision { get; private set; }

        public PlayerTurnDecision(Guid playerId, string playerName, PlayerDecision decision)
        {
            PlayerId = playerId;
            Decision = decision;
            PlayerName = playerName;
        }
    }
}