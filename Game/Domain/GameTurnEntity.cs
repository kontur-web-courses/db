using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public Guid Id
        {
            get;
            private set;
        }
        [BsonElement]
        public Guid GameId { get; }
        [BsonElement]
        public int TurnNumber { get; }
        [BsonElement]
        public PlayerDecision Player1Decision { get; }
        [BsonElement]
        public PlayerDecision Player2Decision { get; }
        [BsonElement]
        public Guid Player1 { get; }
        [BsonElement]
        public Guid Player2 { get; }
        [BsonElement]
        public int Winner { get; }

        [BsonConstructor]
        public GameTurnEntity(Guid gameId, int turnNumber, PlayerDecision player1Decision, PlayerDecision player2Decision, Guid player2, Guid player1)
        {
            Player1Decision = player1Decision;
            Player2Decision = player2Decision;
            Player2 = player2;
            Player1 = player1;
            TurnNumber = turnNumber;
            GameId = gameId;
            if (player1Decision.Beats(player2Decision))
                Winner = 1;
            else
                Winner = player2Decision.Beats(player1Decision) ? 2 : 0;
        }
    }
}