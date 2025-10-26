using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex, DateTime finishedAt, 
            Dictionary<Guid, PlayerDecision> decisions, Guid? winnerId)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            FinishedAt = finishedAt;
            Decisions = decisions;
            WinnerId = winnerId;
        }

        public GameTurnEntity()
        {
            Id = Guid.NewGuid();
            FinishedAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }
        public Guid GameId { get; set; }
        public int TurnIndex { get; set; }
        public DateTime FinishedAt { get; private set; }
        
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<Guid, PlayerDecision> Decisions { get; set; }
        
        public Guid? WinnerId { get; set; }
    }
}
