using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement]
        public DateTime Timestamp { get; set; }

        [BsonElement]
        public PlayerDecision FirstPlayerDecision { get; set; }

        [BsonElement]
        public PlayerDecision SecondPlayerDecision { get; set; }

        [BsonElement]
        public Guid Winner { get; set; }
    }
}