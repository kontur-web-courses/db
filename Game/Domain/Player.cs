using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class Player
    {
        [BsonConstructor]
        public Player(Guid userId, string name)
        {
            UserId = userId;
            Name = name;
        }

        [BsonElement]
        public Guid UserId { get; }

        [BsonElement]
        public string Name { get; }

        public PlayerDecision? Decision { get; set; }

        public int Score { get; set; }
    }
}