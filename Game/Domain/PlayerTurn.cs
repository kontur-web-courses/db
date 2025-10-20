using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Numerics;

namespace Game.Domain
{
    public class PlayerTurn
    {
        [BsonConstructor]
        public PlayerTurn(Guid id, Guid userId, string name, PlayerDecision? decision) 
        { 
            Id = id;
            UserId = userId;
            Name = name;
            Decision = decision;
        }

        public PlayerTurn(Player player) : this(Guid.NewGuid(), player.UserId, player.Name, player.Decision)
        {
        }

        [BsonElement]
        public Guid Id { get; private set; }
        [BsonElement]
        public Guid UserId { get; private set; }
        [BsonElement]
        public string Name { get; private set; }
        [BsonElement]
        public PlayerDecision? Decision { get; private set; }
    }
}
