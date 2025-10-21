using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonElement]
        public Guid Id { get; set; }
        
        [BsonElement]
        public Guid GameId { get; private set; }
        
        [BsonElement]
        public int TurnIndex { get; private set; }
        
        [BsonElement]
        public Guid WinnerId { get; private set; }
        
        [BsonElement]
        public Dictionary<string, PlayerDecision> Decisions { get; private set; }
        
        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, Guid winnerId, Dictionary<string, PlayerDecision> decisions)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            WinnerId = winnerId;
            Decisions = decisions;
        }
    }
}