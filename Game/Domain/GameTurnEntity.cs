using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public GameTurnEntity() { }

        [BsonConstructor]
        public GameTurnEntity(long turnIndex, PlayerDecision player1Decision, PlayerDecision player2Decision, Guid winnerId)
        {
            TurnIndex = turnIndex;
            WinnerId = winnerId;
            Player1Decision = player1Decision;
            Player2Decision = player2Decision;
        }

        [BsonElement]
        public long TurnIndex { get; set; }

        [BsonElement]
        public PlayerDecision Player1Decision { get; set; }

        [BsonElement]
        public PlayerDecision Player2Decision { get; set; }

        [BsonElement]
        public Guid WinnerId { get; set; }
        //TODO: Придумать какие свойства должны быть в этом классе, чтобы сохранять всю информацию о закончившемся туре.
    }
}