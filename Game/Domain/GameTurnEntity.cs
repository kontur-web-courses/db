using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        //TODO: Придумать какие свойства должны быть в этом классе, чтобы сохранять всю информацию о закончившемся туре.
        [BsonConstructor]
        public GameTurnEntity(Guid id, int turn, PlayerDecision firstPlayerDecision,
            PlayerDecision secondPlayerDecision, Guid firstPlayer, Guid secondPlayer)
        {
            FirstPlayerDecision = firstPlayerDecision;
            SecondPlayerDecision = secondPlayerDecision;
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            Turn = turn;
            GameId = id;
            Winner = firstPlayerDecision.Beats(secondPlayerDecision) 
                ? 1 : secondPlayerDecision.Beats(firstPlayerDecision) 
                    ? 2 : 0;
        }
        public Guid Id
        {
            get;
            private set;
        }
        [BsonElement]
        public PlayerDecision FirstPlayerDecision { get; }
        [BsonElement]
        public PlayerDecision SecondPlayerDecision { get; }
        [BsonElement]
        public Guid FirstPlayer { get; }
        [BsonElement]
        public Guid SecondPlayer { get; }
        [BsonElement]
        public Guid GameId { get; }
        [BsonElement]
        public int Turn { get; }
        [BsonElement]
        public int Winner { get; }
    }
}