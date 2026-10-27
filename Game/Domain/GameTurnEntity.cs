using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    /// <summary>
    /// Снимок завершенного тура. В отличие от <see cref="GameEntity"/>,
    /// решения игроков здесь не очищаются после завершения хода.
    /// </summary>
    public class GameTurnEntity
    {
        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, List<GameTurnPlayer> players, Guid? winnerId)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            Players = players ?? throw new ArgumentNullException(nameof(players));
            WinnerId = winnerId;
        }

        [BsonId]
        public Guid Id { get; }

        [BsonElement]
        public Guid GameId { get; }

        /// <summary>
        /// Номер тура в игре, начиная с единицы.
        /// </summary>
        [BsonElement]
        public int TurnIndex { get; }

        [BsonElement]
        public List<GameTurnPlayer> Players { get; private set; }

        /// <summary>
        /// Идентификатор победителя; <c>null</c>, если была ничья.
        /// </summary>
        [BsonElement]
        public Guid? WinnerId { get; }
    }

    /// <summary>
    /// Решение игрока в одном завершенном туре.
    /// </summary>
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
