using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        private readonly List<Player> players;

        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, List<Player> players, Guid winnerId) 
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            this.players = players;
            WinnerId = winnerId; 
        }
    
        [BsonId]
        public Guid Id { get; private set; }

        [BsonElement]
        public Guid GameId { get; private set; }

        [BsonElement]
        public int TurnIndex { get; private set; }

        [BsonElement]
        public Guid WinnerId { get; private set; }

        [BsonElement]
        public IReadOnlyList<Player> Players => players.AsReadOnly();
    }
}
