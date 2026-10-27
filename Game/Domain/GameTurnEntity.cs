using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnId
    {
        
        [BsonElement]
        public Guid GameId;
        
        [BsonElement]
        public int TurnIndex;
    }
    
    public class GameTurnEntity
    {
        [BsonElement]
        public GameTurnId Id = new GameTurnId();
        
        [BsonElement]
        public IReadOnlyList<Player> Players;
        
        [BsonElement]
        public Guid WinnerId;
        
        public GameTurnEntity(IReadOnlyList<Player> players, int turnIndex, Guid gameId)
        {
            this.Players = players;
            Id.TurnIndex = turnIndex;
            this.Id.GameId = gameId;
        }
    }
}