using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonElement]
        public Guid Id { get; set; }
        
        [BsonElement]
        public Guid GameId { get; set; }
        
        [BsonElement]
        public int TurnNo { get; set; }
        
        [BsonElement]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
        public readonly Dictionary<string, PlayerDecision> PlayerDecisions;
        
        [BsonElement]
        public Guid WinnerId { get; set; }

        public GameTurnEntity(Guid gameId, Guid winnerId, Dictionary<string, PlayerDecision> playerDecisions, int turnNo)
            : this(new Guid(), gameId, winnerId, playerDecisions, turnNo)
        {

        }
        
        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, Guid winnerId, Dictionary<string, PlayerDecision> playerDecisions, int turnNo)
        {
            Id = id;
            GameId = gameId;
            PlayerDecisions = playerDecisions;
            TurnNo = turnNo;
            WinnerId = winnerId;
        }
    }
}