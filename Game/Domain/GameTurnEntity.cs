using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonElement]
        private readonly List<GameTurnPlayer> players;

        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, List<GameTurnPlayer> players, Guid? winnerId)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            this.players = players;
            WinnerId = winnerId;
        }

        public Guid Id { get; }

        [BsonElement]
        public Guid GameId { get; }

        [BsonElement]
        public int TurnIndex { get; }

        public IReadOnlyList<GameTurnPlayer> Players => players.AsReadOnly();

        [BsonElement]
        public Guid? WinnerId { get; }
    }

    public class GameTurnPlayer
    {
        [BsonConstructor]
        public GameTurnPlayer(Guid userId, string name, PlayerDecision decision)
        {
            UserId = userId;
            Name = name;
            Decision = decision;
        }

        [BsonElement]
        public Guid UserId { get; }

        [BsonElement]
        public string Name { get; }

        [BsonElement]
        public PlayerDecision Decision { get; }
    }
}
