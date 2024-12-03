using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, Guid winnerId, Dictionary<string, PlayerDecision> decisions)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            WinnerId = winnerId;
            Decisions = decisions;
        }
        
        [BsonElement]
        public Guid Id { get; set; }
        
        [BsonElement]
        public Guid GameId { get; set; }
        
        [BsonElement]
        public int TurnIndex { get; set; }
        
        [BsonElement]
        public Guid WinnerId { get; set; }
        
        [BsonElement]
        public Dictionary<string, PlayerDecision> Decisions { get; set; }
    }
}