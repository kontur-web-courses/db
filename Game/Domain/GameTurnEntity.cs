using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        //TODO: Придумать какие свойства должны быть в этом классе, чтобы сохранять всю информацию о закончившемся туре.
        [BsonElement] 
        public Guid Id
        {
            get;
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local For MongoDB
            private set;
        }
        [BsonElement]
        public Player player1 { get; }
        [BsonElement]

        public PlayerDecision? decision1 { get; }

        [BsonElement]
        public Player player2 { get; }
        [BsonElement]
        public PlayerDecision? decision2 { get; }
        [BsonElement]
        public Guid gameId { get; }
        [BsonElement]
        public Guid winnerId { get; }
        [BsonElement]
        public DateTime time { get; }

        [BsonConstructor] 
        public GameTurnEntity(
            Guid gameId, Guid winnerId,
            Player player1, PlayerDecision? decision1,
            Player player2, PlayerDecision? decision2,
            DateTime time)
        {
            this.gameId = gameId;
            this.winnerId = winnerId;
            this.player1 = player1;
            this.decision1 = decision1;
            this.player2 = player2;
            this.decision2 = decision2;
            this.time = time;
        }
    }
}