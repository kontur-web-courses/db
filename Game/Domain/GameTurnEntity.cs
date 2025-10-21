using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public GameTurnEntity()
        {
            Id = Guid.Empty;
        }

        public GameTurnEntity(Guid id)
        {
            Id = id;
        }

        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int tourIndex, Guid winnerId, Dictionary<string, PlayerDecision> playerDecisions)
        {
            Id = id;
            GameId = gameId;
            TourIndex = tourIndex;
            WinnerId = winnerId;
            PlayerDecisions = playerDecisions;
        }
        public Guid Id
        {
            get;
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local For MongoDB
            private set;
        }
        
        public required Guid GameId { get; set; }
        public required int TourIndex { get; set; }
        public required Guid WinnerId { get; set; }
        public required Dictionary<string, PlayerDecision> PlayerDecisions { get; set; }
    }
}