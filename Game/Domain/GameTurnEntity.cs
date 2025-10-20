using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public GameTurnEntity()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }
        
        [BsonElement]
        public Guid Id
        {
            get; 
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local For MongoDB
            private set;
        }

        [BsonElement]
        public DateTime Timestamp { get; private set; }
        
        [BsonElement]
        public Guid GameId {get; set;}
        
        [BsonElement]
        public int TurnIndex {get; set;}

        [BsonElement]
        public List<PlayerTurnInfo> PlayerTurns { get; set; } = new();
        
        [BsonElement]
        public Guid? WinnerId {get; set;}
        
        [BsonElement]
        public bool IsDraw { get; set; }
        
        public PlayerDecision? GetDecision(Guid playerId)
        {
            return PlayerTurns.FirstOrDefault(p => p.PlayerId == playerId)?.Decision;
        }

        public string GetPlayerName(Guid playerId)
        {
            return PlayerTurns.FirstOrDefault(p => p.PlayerId == playerId)?.PlayerName;
        }
    }
    
    public class PlayerTurnInfo
    {
        [BsonElement]
        public Guid PlayerId { get; set; }

        [BsonElement]
        public string PlayerName { get; set; }

        [BsonElement]
        public PlayerDecision Decision { get; set; }
    }
}
