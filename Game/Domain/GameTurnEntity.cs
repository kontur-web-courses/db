using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonElement]
        public Guid Id { get; init; }

        [BsonElement]
        public Guid GameId { get; init; }

        [BsonElement]
        public int TurnNumber { get; init; }


        [BsonElement]
        public PlayerDecision[] PlayerDescisions { get; init; }

        [BsonElement]
        public Guid? Winner { get; set; }
        
    }
}